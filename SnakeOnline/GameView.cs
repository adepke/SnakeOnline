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
            SnakeInst.Spawn(15, 20, 8);

            Timer GameLoopTimer = new Timer(TickRate * 1000d);

            GameLoopTimer.AutoReset = true;
            GameLoopTimer.Elapsed += new ElapsedEventHandler(Tick);
            GameLoopTimer.Enabled = true;
        }

        private void Tick(object Sender, ElapsedEventArgs e)
        {
            Console.Clear();

            for (int Row = 0; Row < WorldInst.GetRows(); ++Row)
            {
                for (int Column = 0; Column < WorldInst.GetColumns(); ++Column)
                {
                    Console.Write(WorldInst.Get(Row, Column) + " ");
                }

                Console.WriteLine("");
            }

            if (SnakeInst.IsAlive())
            {
                SnakeInst.Move(InputInst.LastInput);
            }

            else
            {
                Console.WriteLine("\nGame Over");
            }
        }
    }
}
