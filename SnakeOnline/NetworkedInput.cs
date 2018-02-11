using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class NetworkedInput : Input
    {
        public override bool Initialize()
        {
            return true;
        }

        public override bool Initialize(AppWindow WindowInst)
        {
            return false;
        }

        public void KeyPress(char Key)
        {
            switch (Key)
            {
                case 'w':
                    LastInput = MovementDirection.Up;
                    break;
                case 'a':
                    LastInput = MovementDirection.Left;
                    break;
                case 's':
                    LastInput = MovementDirection.Down;
                    break;
                case 'd':
                    LastInput = MovementDirection.Right;
                    break;
            }
        }
    }
}
