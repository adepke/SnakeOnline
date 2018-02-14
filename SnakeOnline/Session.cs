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
            }

            catch (Exception)
            {
                return false;
            }

            Remote = EndPoint;

            WorldInstSerialized = new byte[WorldRows * WorldColumns];

            Rows = WorldRows;
            Columns = WorldColumns;

            return true;
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
