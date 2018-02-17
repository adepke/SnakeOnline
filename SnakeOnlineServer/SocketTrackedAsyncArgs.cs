using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnlineServer
{
    class SocketTrackedAsyncArgs : SocketAsyncEventArgs
    {
        public Socket Client;
    }
}
