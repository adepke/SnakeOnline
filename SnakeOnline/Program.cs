using System;

using OpenTK;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            GameWindow AppWindow = new GameWindow(800, 600, OpenTK.Graphics.GraphicsMode.Default, "Snake Online", 0, DisplayDevice.Default);

            GameView LocalView = new GameView();
            if (!LocalView.Initialize(5, 5))
            {
                throw new Exception("Failed to Create Local Game View");
            }

            AppWindow.Run(30.0d);
        }
    }
}
