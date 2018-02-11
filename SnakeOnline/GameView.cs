using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using OpenTK;

namespace SnakeOnline
{
    class GameView
    {
        public GameWindow WindowInst;
        public World WorldInst;
        public Snake SnakeInst;
        public Input InputInst;

        public bool Initialize(GameWindow Window, int WorldSizeX, int WorldSizeY)
        {
            WindowInst = Window;

            WorldInst = new World();

            if (!WorldInst.Initialize(WorldSizeX, WorldSizeY))
            {
                return false;
            }

            SnakeInst = new Snake();

            if (!SnakeInst.Initialize(WorldInst))
            {
                return false;
            }

            InputInst = new Input();

            if (!InputInst.Initialize(SnakeInst, WindowInst))
            {
                return false;
            }

            return true;
        }

        public void Run(double TickRate)
        {
            SnakeInst.Spawn(4, 2, 1);

            Timer GameLoopTimer = new Timer(TickRate * 1000d);

            GameLoopTimer.AutoReset = true;
            GameLoopTimer.Elapsed += new ElapsedEventHandler(Tick);
            GameLoopTimer.Enabled = true;
        }

        private void Tick(object Sender, ElapsedEventArgs e)
        {
            Console.WriteLine("X: {0}   Y: {1}", SnakeInst.GetHead().X, SnakeInst.GetHead().Y);

            if (SnakeInst.IsAlive())
            {
                SnakeInst.Move(InputInst.LastInput);
            }

            else
            {
                Console.WriteLine("Game Over");
            }
        }
    }
}
