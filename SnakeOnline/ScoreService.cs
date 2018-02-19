using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnline
{
    public struct Highscore
    {
        public string Name;
        public int Score;
    }

    internal class ScoreService : IDisposable
    {
        private Socket ServerSocket;

        private List<Highscore> Highscores;

        internal bool Initialize()
        {
            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.ReceiveTimeout = 5000;

                IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 0);

                ServerSocket.Bind(LocalEndPoint);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Score Service Error: " + e.Message);

                return false;
            }

            return true;
        }

        internal bool Connect(IPEndPoint EndPoint)
        {
            try
            {
                ServerSocket.Connect(EndPoint);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Score Server Connection Failure: " + e.Message);

                return false;
            }

            return true;
        }

        internal bool Submit(string Name, int Score)
        {
            string SubmissionMessage = "NAME:" + Name + "|SCORE:" + Score;

            try
            {
                ServerSocket.Send(Encoding.ASCII.GetBytes(SubmissionMessage));
            }

            catch (SocketException e)
            {
                Console.WriteLine("Submission Failure: " + e.Message);

                return false;
            }

            return true;
        }

        private void HighscoresReceived(object Sender, SocketAsyncEventArgs Args)
        {
            Highscores = new List<Highscore>();

            string ScoreList = Encoding.ASCII.GetString(Args.Buffer);
            ScoreList = ScoreList.Replace("\0", String.Empty);

            for (int Iter = 0; Iter < ScoreList.Length; ++Iter)
            {
                string Name = "ERROR";
                string Score = "0";

                // '&' Signals New Entry
                if (ScoreList[Iter] == '&')
                {
                    for (int PipeIter = 1; PipeIter < 64; ++PipeIter)
                    {
                        if (ScoreList[Iter + PipeIter] == '|')
                        {
                            Name = ScoreList.Substring(Iter + 1, PipeIter - 1);

                            for (int NextEntryIter = 1; NextEntryIter < 32; ++NextEntryIter)
                            {
                                // Prevent Out of Bounds Exception.
                                if (Iter + PipeIter + NextEntryIter >= ScoreList.Length)
                                {
                                    Score = ScoreList.Substring(Iter + PipeIter + 1, ScoreList.Length - Iter - PipeIter - 1);
                                }

                                else if (ScoreList[Iter + PipeIter + NextEntryIter] == '&')
                                {
                                    Score = ScoreList.Substring(Iter + PipeIter + 1, NextEntryIter - 1);

                                    break;
                                }
                            }

                            break;
                        }
                    }
                }

                Highscore EntryHighscore;
                EntryHighscore.Name = Name;
                EntryHighscore.Score = Convert.ToInt32(Score);

                Highscores.Add(EntryHighscore);
            }
        }

        internal List<Highscore> GetHighscores()
        {
            // Setup Receiving Before Sending.
            SocketAsyncEventArgs AsyncArgs = new SocketAsyncEventArgs();
            byte[] ScoreBuffer = new byte[256];
            AsyncArgs.SetBuffer(ScoreBuffer, 0, ScoreBuffer.Length);
            AsyncArgs.Completed += HighscoresReceived;

            try
            {
                ServerSocket.ReceiveAsync(AsyncArgs);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Highscore Retrieval at Receive Failure: " + e.Message);

                return default(List<Highscore>);
            }

            try
            {
                ServerSocket.Send(Encoding.ASCII.GetBytes("GETTOP10SCORES"));
            }

            catch (SocketException e)
            {
                Console.WriteLine("Highscore Retrieval at Send Failure: " + e.Message);

                return default(List<Highscore>);
            }

            while (Highscores == null)
            {
                System.Threading.Thread.Yield();
            }

            return Highscores;
        }

        public void Dispose()
        {
            ServerSocket.Disconnect(false);
            ServerSocket.Shutdown(SocketShutdown.Both);
            ServerSocket.Dispose();
        }

        public void DisposeFromInitializationError()
        {
            ServerSocket.Dispose();
        }
    }
}
