using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeOnline
{
    class GameView
    {
        public World WorldInst;

        public bool Initialize(int WorldSizeX, int WorldSizeY)
        {
            WorldInst = new World();

            if (!WorldInst.Initialize(WorldSizeX, WorldSizeY))
            {
                return false;
            }

            return true;
        }
    }
}
