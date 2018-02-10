using System;

using OpenTK;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            GameWindow AppWindow = new GameWindow(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Snake Online", 0, DisplayDevice.Default);

            AppWindow.Run(30.0d);
        }
    }
}
