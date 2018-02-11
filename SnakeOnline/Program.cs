using System;

using OpenTK;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            GameWindow AppWindow = new GameWindow(100, 100, OpenTK.Graphics.GraphicsMode.Default, "Snake Online", 0, DisplayDevice.Default);

            GameView LocalView = new GameView();
            if (!LocalView.Initialize(AppWindow, 25, 25))
            {
                throw new Exception("Failed to Create Local Game");
            }

            LocalView.Run(0.2d);
            AppWindow.Run(30.0d);
        }
    }
}
