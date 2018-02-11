using System;

using OpenTK;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            AppWindow Window = new AppWindow();

            GameManager Manager = new GameManager();

            Manager.Initialize(Window);

            Manager.Run(0.2d);
            Window.Run(30.0d);
        }
    }
}
