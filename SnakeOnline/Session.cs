using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnline
{
    public enum SessionType
    {
        Singleplayer,
        Multiplayer
    }

    internal class Session : IDisposable
    {
        public SessionType Type;

        private Socket SessionSocket;

        private IPEndPoint Remote;

        private byte[] WorldInstSerialized;

        private int Rows;
        private int Columns;

        public bool Create(IPEndPoint EndPoint, int WorldRows, int WorldColumns)
        {
            try
            {
                SessionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 306);

                SessionSocket.Bind(LocalEndPoint);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Session Creation Error: " + e.Message);

                return false;
            }

            SessionSocket.ReceiveTimeout = 60000;

            Remote = EndPoint;

            WorldInstSerialized = new byte[WorldRows * WorldColumns];

            Rows = WorldRows;
            Columns = WorldColumns;

            return true;
        }

        public bool Connect()
        {
            string ConnectMessage = "CONNECT";
            
            // Char Type is 2 Bytes Wide, P2PPresider Takes 32 Byte Request.
            char[] ConnectMessageBuffer = new char[16];

            ConnectMessageBuffer = ConnectMessage.ToCharArray();

            try
            {
                SessionSocket.SendTo(Encoding.ASCII.GetBytes(ConnectMessageBuffer), Remote);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Connection Failure: " + e.Message);

                return false;
            }

            byte[] PartnerSerialized = new byte[32];

            try
            {
                SessionSocket.Receive(PartnerSerialized, 32, SocketFlags.None);
            }

            catch (SocketException e)
            {
                Console.WriteLine("Connection Failure: " + e.Message);

                return false;
            }

            string Partner = Encoding.ASCII.GetString(PartnerSerialized);

            if (Partner.Substring(0, 4) != "PEER")
            {
                return false;
            }

            string PartnerAddress = Partner.Substring(4);

            for (int Iter = 0; Iter < PartnerAddress.Length; ++Iter)
            {
                if (PartnerAddress[Iter] == ':')
                {
                    Remote = new IPEndPoint(IPAddress.Parse(PartnerAddress.Substring(0, Iter)), Int32.Parse(PartnerAddress.Substring(Iter + 1)));

                    return true;
                }
            }

            return false;
        }

        internal void SendWorld(World WorldInst)
        {
            Buffer.BlockCopy(WorldInst.ItemMatrix, 0, WorldInstSerialized, 0, Rows * Columns);

            SessionSocket.SendTo(WorldInstSerialized, Rows * Columns, SocketFlags.None, Remote);
        }

        // Blocking Call
        internal void ReceiveWorld(World WorldInst)
        {
            SessionSocket.Receive(WorldInstSerialized, Rows * Columns, SocketFlags.None);

            for (int Iter = 0; Iter < WorldInstSerialized.Length; ++Iter)
            {
                WorldInst.ItemMatrix[(int)Math.Floor((double)(Iter / Columns)), Iter % Columns] = WorldInstSerialized[Iter];
            }
        }

        public void Dispose()
        {
            SessionSocket.Dispose();
        }
    }
}
