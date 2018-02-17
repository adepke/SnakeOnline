using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnlineServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ScoreService Scoring = new ScoreService();

            if (!Scoring.Initialize(6710))
            {
                Console.Read();

                return;
            }

            Console.WriteLine("Server Online at Port: 6710");

            // Loop Until Internal Failure.
            while (Scoring.Update())
            {

            }

            Scoring.Shutdown();
        }
    }
}
