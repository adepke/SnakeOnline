using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenTK;

namespace SnakeOnline
{
    class LocalInput : Input
    {
        public override bool Initialize()
        {
            return false;
        }

        public override bool Initialize(AppWindow WindowInst)
        {
            WindowInst.KeyPress += KeyPress;

            return true;
        }

        protected void KeyPress(object Sender, KeyPressEventArgs e)
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
