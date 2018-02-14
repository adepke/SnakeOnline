using System;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnlineServer
{
    class Program
    {
        private static Socket ServerSocket;

        static void Main(string[] args)
        {
            Console.WriteLine("SnakeOnline P2P Presider Server\n");

            Console.WriteLine("Host Name: " + Dns.GetHostName() + "\n");

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ServerSocket.Blocking = true;

            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 735);

            ServerSocket.Bind(LocalEndPoint);

            const int WorldRows = 15;
            const int WorldColumns = 15;

            byte[] DataBuffer = new byte[WorldRows * WorldColumns];

            // Disabled While Testing P2PPresider Server
            /*

            // Swap Buffers Between Clients Endlessly.
            while (true)
            {
                ClientSockets[0].Receive(DataBuffer);

                Console.WriteLine("Received {0} Bytes from Client A at {1}:{2}", DataBuffer.Length, ((IPEndPoint)ClientSockets[0].RemoteEndPoint).Address, ((IPEndPoint)ClientSockets[0].RemoteEndPoint).Port);

                ClientSockets[1].Send(DataBuffer);

                Console.WriteLine("Sent     {0} Bytes to Client B at   {1}:{2}", DataBuffer.Length, ((IPEndPoint)ClientSockets[1].RemoteEndPoint).Address, ((IPEndPoint)ClientSockets[1].RemoteEndPoint).Port);

                ClientSockets[1].Receive(DataBuffer);

                Console.WriteLine("Received {0} Bytes from Client B at {1}:{2}", DataBuffer.Length, ((IPEndPoint)ClientSockets[1].RemoteEndPoint).Address, ((IPEndPoint)ClientSockets[1].RemoteEndPoint).Port);

                ClientSockets[0].Send(DataBuffer);

                Console.WriteLine("Sent     {0} Bytes to Client A at   {1}:{2}", DataBuffer.Length, ((IPEndPoint)ClientSockets[0].RemoteEndPoint).Address, ((IPEndPoint)ClientSockets[0].RemoteEndPoint).Port);
            }

            */
        }
    }
}
