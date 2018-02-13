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
        private SnakeOnlineServer.ServerInput ServerInHandle;

        public override bool Initialize(SnakeOnlineServer.ServerInput ServerIn, AppWindow WindowInst)
        {
            ServerInHandle = ServerIn;
            WindowInst.KeyPress += KeyPress;

            return true;
        }

        // Never Initialize a LocalInput with an Output Server.
        public override bool Initialize(SnakeOnlineServer.ServerOutput ServerOut)
        {
            return false;
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

        public override void NetworkUpdate()
        {
            base.NetworkUpdate();

            ServerInHandle.SendMovement((int)LastInput);
        }
    }
}
