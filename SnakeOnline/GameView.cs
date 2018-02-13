using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using OpenTK;

namespace SnakeOnline
{
    class GameView
    {
        public World WorldInst;
        public ItemSpawner ItemSpawnerInst;
        public Snake SnakeInst;
        public Input InputInst;

        public bool GameOver = false;

        public bool Initialize(AppWindow Window, int WorldSizeX, int WorldSizeY)
        {
            WorldInst = new World();

            if (!WorldInst.Initialize(WorldSizeX, WorldSizeY))
            {
                return false;
            }

            ItemSpawnerInst = new LocalItemSpawner();

            if (!ItemSpawnerInst.Initialize(WorldInst))
            {
                return false;
            }

            SnakeInst = new Snake();

            if (!SnakeInst.Initialize(WorldInst, ItemSpawnerInst))
            {
                return false;
            }

            InputInst = new LocalInput();

            if (!InputInst.Initialize(Window))
            {
                return false;
            }

            return true;
        }

        public bool InitializeNetworked(int WorldSizeX, int WorldSizeY)
        {
            WorldInst = new World();

            if (!WorldInst.Initialize(WorldSizeX, WorldSizeY))
            {
                return false;
            }

            ItemSpawnerInst = new NetworkedItemSpawner();

            if (!ItemSpawnerInst.Initialize(WorldInst))
            {
                return false;
            }

            SnakeInst = new Snake();

            if (!SnakeInst.Initialize(WorldInst, ItemSpawnerInst))
            {
                return false;
            }

            InputInst = new NetworkedInput();

            if (!InputInst.Initialize())
            {
                return false;
            }

            return true;
        }

        public void Run(double UpdateRate)
        {
            ItemSpawnerInst.SpawnNew();

            SnakeInst.Spawn(0, 0, 1);
        }

        internal void Tick()
        {
            if (SnakeInst.IsAlive())
            {
                SnakeInst.Move(InputInst.LastInput);

                if (!SnakeInst.IsAlive())
                {
                    GameOver = true;

                    return;
                }
            }

            else
            {
                GameOver = true;

                return;
            }

            //DebugDraw();
        }

        private void DebugDraw()
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
        }
    }
}
