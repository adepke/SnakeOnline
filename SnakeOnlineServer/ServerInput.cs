using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SnakeOnlineServer
{
    public class ServerInput
    {
        // Copy Entire Game World over Network
        public void SendWorld(int Rows, int Columns, int[,] World)
        {

        }

        public void SendMovement(int Direction)
        {

        }

        public void SendItemSpawn(int Row, int Column)
        {

        }
    }
}
