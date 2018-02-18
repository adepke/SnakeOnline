using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SnakeOnlineServer
{
    class ScoreService
    {
        private Socket ServiceSocket;

        private FileStream ScoreDatabase;
        private StreamReader ScoreReader;
        private StreamWriter ScoreWriter;

        private System.Threading.SpinLock Lock;

        private List<Socket> Clients;

        public bool Initialize(int Port)
        {
            try
            {
                ServiceSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, Port);

                ServiceSocket.Bind(LocalEndPoint);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Initialization Failure: " + e.Message);

                return false;
            }

            ServiceSocket.Blocking = true;

            try
            {
                ScoreDatabase = new FileStream("ScoreDatabase.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }

            catch (Exception e)
            {
                Console.WriteLine("Failed to Open or Create File 'ScoreDatabase.dat': " + e.Message + "\n");
            }

            ScoreReader = new StreamReader(ScoreDatabase);
            ScoreWriter = new StreamWriter(ScoreDatabase);

            Clients = new List<Socket>();

            Lock = new System.Threading.SpinLock();

            return true;
        }

        private List<string> GetHighestScores(int Count)
        {
            List<string> NameList = new List<string>();
            List<int> ScoreList = new List<int>();

            bool AquiredLock = new bool();
            Lock.Enter(ref AquiredLock);

            string CurrentLine;
            while ((CurrentLine = ScoreReader.ReadLine()) != null)
            {
                for (int Iter = 0; Iter < CurrentLine.Length; ++Iter)
                {
                    if (CurrentLine[Iter] == '|')
                    {
                        NameList.Add(CurrentLine.Substring(0, Iter));
                        ScoreList.Add(Convert.ToInt32(CurrentLine.Substring(Iter + 1)));
                    }
                }
            }

            Lock.Exit();

            List<string> Result = new List<string>();

            for (int Iter = 0; Iter < Count; ++Iter)
            {
                if (ScoreList.Count == 0)
                {
                    break;
                }

                int TopEntryScore = ScoreList.Max();
                int TopEntryIndex = ScoreList.IndexOf(TopEntryScore);
                string TopEntryName = NameList[TopEntryIndex];

                ScoreList.RemoveAt(TopEntryIndex);
                NameList.RemoveAt(TopEntryIndex);

                Result.Add(TopEntryName + '|' + TopEntryScore);
            }

            return Result;
        }

        private void Process(object Sender, SocketAsyncEventArgs Args)
        {
            SocketTrackedAsyncArgs TrackedArgs = (SocketTrackedAsyncArgs)Args;
            IPEndPoint ClientEndPoint = ((IPEndPoint)TrackedArgs.Client.RemoteEndPoint);

            string ReceivedString = Encoding.ASCII.GetString(Args.Buffer);
            ReceivedString = ReceivedString.Replace("\0", String.Empty);

            // Don't Process Disconnect Messages.
            if (ReceivedString == "")
            {
                return;
            }

            if (ReceivedString == "GETTOP10SCORES")
            {
                List<string> Entries = GetHighestScores(10);

                byte[] EntriesBuffer = new byte[128];

                int Offset = 0;

                for (int Iter = 0; Iter < Entries.Count; ++Iter)
                {
                    byte[] EntryBuffer = Encoding.ASCII.GetBytes(Entries[Iter]);

                    System.Buffer.BlockCopy(EntryBuffer, 0, EntriesBuffer, Offset, EntryBuffer.Length);

                    Offset += EntryBuffer.Length;
                }

                Console.WriteLine("Client Requested Highscores At: " + ClientEndPoint.Address + ':' + ClientEndPoint.Port);

                TrackedArgs.Client.Send(EntriesBuffer);

                return;
            }

            if (ReceivedString.Substring(0, 5) != "NAME:")
            {
                Console.WriteLine("Received Malformed Data");

                return;
            }

            string NameSearch = ReceivedString.Substring(5);
            int Position = 0;

            for (int Iter = 0; Iter < NameSearch.Length; ++Iter)
            {
                if (NameSearch[Iter] == '|')
                {
                    Position = Iter;
                }
            }

            if (Position == 0)
            {
                Console.WriteLine("Received Malformed Data");

                return;
            }

            string Name = NameSearch.Substring(0, Position);

            int Score = Convert.ToInt32(NameSearch.Substring(Position + 7));
            if (Score == 0)
            {
                return;
            }

            Console.WriteLine("Client Submitted Score At: " + ClientEndPoint.Address + ':' + ClientEndPoint.Port);

            string Entry = Name + '|' + Score;

            bool AquiredLock = new bool();
            Lock.Enter(ref AquiredLock);

            long EOF = ScoreDatabase.Length;
            ScoreDatabase.Seek(EOF, SeekOrigin.Begin);
            ScoreWriter.WriteLine(Entry);
            ScoreWriter.Flush();
            ScoreDatabase.Flush();

            Lock.Exit();

            // Reset Receiving on this Socket.
            SocketTrackedAsyncArgs AsyncArgs = new SocketTrackedAsyncArgs();
            AsyncArgs.Client = TrackedArgs.Client;
            byte[] Buffer = new byte[1024];
            AsyncArgs.SetBuffer(Buffer, 0, Buffer.Length);
            AsyncArgs.Completed += Process;
            TrackedArgs.Client.ReceiveAsync(AsyncArgs);
        }

        public bool Update()
        {
            try
            {
                ServiceSocket.Listen(256);  // Block on Listen() Until a New Client Connects.
            }

            catch (SocketException e)
            {
                Console.WriteLine("Listen Failure: " + e.Message);

                return false;
            }

            Socket NewClient;

            try
            {
                NewClient = ServiceSocket.Accept();

                IPEndPoint ClientEndPoint = ((IPEndPoint)NewClient.RemoteEndPoint);
                Console.WriteLine("Client Connected At: " + ClientEndPoint.Address + ':' + ClientEndPoint.Port);

                Clients.Add(NewClient);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Accept Failure: " + e.Message);

                return false;
            }

            SocketTrackedAsyncArgs AsyncArgs = new SocketTrackedAsyncArgs();
            AsyncArgs.Client = NewClient;
            byte[] Buffer = new byte[1024];
            AsyncArgs.SetBuffer(Buffer, 0, Buffer.Length);
            AsyncArgs.Completed += Process;
            NewClient.ReceiveAsync(AsyncArgs);

            return true;
        }

        public void Shutdown()
        {
            ScoreReader.Close();
            ScoreReader.Dispose();

            ScoreWriter.Close();
            ScoreWriter.Dispose();

            ScoreDatabase.Close();
            ScoreDatabase.Dispose();

            foreach (Socket Client in Clients)
            {
                Client.Disconnect(false);
                Client.Shutdown(SocketShutdown.Both);
                Client.Dispose();
            }

            ServiceSocket.Disconnect(false);
            ServiceSocket.Shutdown(SocketShutdown.Both);
            ServiceSocket.Dispose();
        }
    }
}
