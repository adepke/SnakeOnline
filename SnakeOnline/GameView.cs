using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeOnline
{
    class GameView
    {
        public World WorldInst;
        public Snake SnakeInst;

        public bool Initialize(int WorldSizeX, int WorldSizeY)
        {
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

            return true;
        }
    }
}
