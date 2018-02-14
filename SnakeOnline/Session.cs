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

        bool Create(IPEndPoint EndPoint)
        {
            try
            {
                SessionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                SessionSocket.Connect(EndPoint);
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal void SendWorld(World WorldInst)
        {
            byte[] WorldInstSerialized = new byte[WorldInst.GetRows() * WorldInst.GetColumns()];

            Buffer.BlockCopy(WorldInst.ItemMatrix, 0, WorldInstSerialized, 0, WorldInst.GetRows() * WorldInst.GetColumns());

            SessionSocket.Send(WorldInstSerialized);
        }

        // Blocking Call
        internal void ReceiveWorld(World WorldInst)
        {
            byte[] WorldInstSerialized = new byte[WorldInst.GetRows() * WorldInst.GetColumns()];

            SessionSocket.Receive(WorldInstSerialized);

            for (int Iter = 0; Iter < WorldInstSerialized.Length; ++Iter)
            {
                WorldInst.ItemMatrix[(int)Math.Floor((double)(4 / WorldInst.GetColumns())), Iter % WorldInst.GetColumns()] = WorldInstSerialized[Iter];
            }
        }

        public void Dispose()
        {
            SessionSocket.Disconnect(false);
            SessionSocket.Dispose();
        }
    }
}
