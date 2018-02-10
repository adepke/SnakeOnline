using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using OpenTK;

namespace SnakeOnline
{
    enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    class Input
    {
        private Snake SnakeInst;

        public MovementDirection LastInput = MovementDirection.Left;

        public bool Initialize(Snake SnakeInst, GameWindow WindowInst)
        {
            this.SnakeInst = SnakeInst;

            WindowInst.KeyPress += KeyPress;

            return true;
        }

        public void KeyPress(object Sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
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
