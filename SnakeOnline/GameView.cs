using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
