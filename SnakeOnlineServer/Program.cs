using System;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnlineServer
{
    class Program
    {
        private static Socket[] ClientSockets;

        static void Main(string[] args)
        {
            Console.WriteLine("SnakeOnline Dedicated Server\n");

            Console.WriteLine("Host Name: " + Dns.GetHostName() + "\n");

            ClientSockets[0] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ClientSockets[1] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ClientSockets[0].Blocking = true;
            ClientSockets[1].Blocking = true;

            // Listen Until Client 0 Connects.
            ClientSockets[0].Listen(256);

            // Listen Until Client 1 Connects.
            ClientSockets[1].Listen(256);

            const int WorldRows = 15;
            const int WorldColumns = 15;

            byte[] DataBuffer = new byte[WorldRows * WorldColumns];

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
        }
    }
}
