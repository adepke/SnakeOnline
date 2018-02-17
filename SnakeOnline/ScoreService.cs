using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SnakeOnline
{
    internal class ScoreService : IDisposable
    {
        private Socket ServerSocket;

        internal bool Initialize()
        {
            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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

            return false;
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

        public void Dispose()
        {
            ServerSocket.Disconnect(false);
            ServerSocket.Shutdown(SocketShutdown.Both);
            ServerSocket.Dispose();
        }
    }
}
