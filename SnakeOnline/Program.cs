using System;
using System.Drawing;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            GameManager Manager = new GameManager();

            Manager.Initialize(0.15d, 15, 15, "Snake Online", new Size(800, 600));

            Manager.RequestNewSession();
            Manager.StartSession();

            Manager.ConnectSession();  // This is Blocking to the Game Thread, Consider Adding UI Loading Screen? Timeout after 10 Seconds.

            Manager.Run();

            Manager.Window.Run();

            Manager.Shutdown();
        }
    }
}
