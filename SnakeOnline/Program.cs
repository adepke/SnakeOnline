﻿using System;
using System.Drawing;

namespace SnakeOnline
{
    class Program
    {
        public static void Main()
        {
            GameManager Manager = new GameManager();

            Manager.Initialize(0.15d, 15, 15, "Snake Online", new Size(800, 600));

            Manager.StartSession(Manager.RequestSessionType());

            Manager.Run();

            Manager.Window.Run();

            Manager.Shutdown();
        }
    }
}
