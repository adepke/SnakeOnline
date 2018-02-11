using System;
using System.Drawing;

using OpenTK;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            AppWindow Window = new AppWindow();

            GameManager Manager = new GameManager();

            Manager.Initialize(Window, "Snake Online", new Size(100, 100));

            Manager.Run(0.2d);
            Window.Run(30.0d);

            Manager.Shutdown();
        }
    }
}
