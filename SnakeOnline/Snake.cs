using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class Snake
    {
        private World WorldInst;

        public List<Point> Coords;
        public int Size { get; set; }

        private bool Alive = true;

        public bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            Coords = new List<Point>();

            return false;
        }

        public void Move(MovementDirection Direction)
        {
            GrowMove(Direction);

            if (Alive)
            {
                Coords.RemoveAt(Coords.Count - 2);
            }
        }

        public void GrowMove(MovementDirection Direction)
        {
            Point NewPosition = Coords[0];

            switch (Direction)
            {
                case MovementDirection.Up:
                    NewPosition.Y -= 1;
                    break;
                case MovementDirection.Down:
                    NewPosition.Y += 1;
                    break;
                case MovementDirection.Left:
                    NewPosition.X -= 1;
                    break;
                case MovementDirection.Right:
                    NewPosition.X += 1;
                    break;
            }

            if (WorldInst.IsValidIndex(NewPosition.X, NewPosition.Y))
            {
                Coords.Insert(0, NewPosition);
            }

            else
            {
                Alive = false;
            }
        }

        public bool IsAlive()
        {
            return Alive;
        }
    }
}
