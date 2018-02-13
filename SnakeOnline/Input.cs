using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    public enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    abstract class Input
    {
        public static MovementDirection DefaultInput = MovementDirection.Right;

        public MovementDirection LastInput = DefaultInput;

        public abstract bool Initialize(SnakeOnlineServer.ServerInput ServerIn, AppWindow Window);
        public abstract bool Initialize(SnakeOnlineServer.ServerOutput ServerOut);
    }
}
