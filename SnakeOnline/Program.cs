using System;
using System.Drawing;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            AppWindow Window = new AppWindow();

            GameManager Manager = new GameManager();

            Manager.Initialize(Window, "Snake Online", new Size(800, 600));

            Manager.Run(0.15d);
            Window.Run(1.0d, 30.0d);

            Manager.Shutdown();
        }
    }
}
