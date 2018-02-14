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

            catch (Exception)
            {
                return false;
            }

            SessionSocket.ReceiveTimeout = 10000;

            Remote = EndPoint;

            WorldInstSerialized = new byte[WorldRows * WorldColumns];

            Rows = WorldRows;
            Columns = WorldColumns;

            return true;
        }

        public bool Connect()
        {
            string ConnectMessage = "CONNECT";

            SessionSocket.SendTo(Encoding.ASCII.GetBytes(ConnectMessage), Remote);

            byte[] PartnerSerialized = new byte[64];

            try
            {
                SessionSocket.Receive(PartnerSerialized, 64, SocketFlags.None);
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

            string PartnerAddress = Partner.Substring(3);

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

            SessionSocket.SendTo(WorldInstSerialized, Remote);
        }

        // Blocking Call
        internal void ReceiveWorld(World WorldInst)
        {
            SessionSocket.Receive(WorldInstSerialized, Rows * Columns, SocketFlags.None);

            for (int Iter = 0; Iter < WorldInstSerialized.Length; ++Iter)
            {
                WorldInst.ItemMatrix[(int)Math.Floor((double)(4 / Columns)), Iter % Columns] = WorldInstSerialized[Iter];
            }
        }

        public void Dispose()
        {
            SessionSocket.Disconnect(false);
            SessionSocket.Dispose();
        }
    }
}
