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
        public MovementDirection LastInput = MovementDirection.Left;

        public abstract bool Initialize();
        public abstract bool Initialize(AppWindow Window);
    }
}
