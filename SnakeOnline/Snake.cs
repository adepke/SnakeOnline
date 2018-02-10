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

        private bool Alive = false;

        public bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            Coords = new List<Point>();

            return true;
        }

        public void Spawn(int X, int Y, int Size)
        {
            Point Head;
            Head.X = X;
            Head.Y = Y;

            Coords.Add(Head);

            for (int Iter = 0; Iter < Size - 1; ++Iter)
            {
                Move(MovementDirection.Left);
            }

            Alive = true;
        }

        public void Move(MovementDirection Direction)
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

            // Hit Self
            if ((int)WorldInst.Get(NewPosition.X, NewPosition.Y) == 1)
            {
                Alive = false;

                return;
            }

            bool Grow = false;

            // Hit an Item
            if ((int)WorldInst.Get(NewPosition.X, NewPosition.Y) == 2)
            {
                Grow = true;
            }

            if (WorldInst.IsValidIndex(NewPosition.X, NewPosition.Y))
            {
                Coords.Insert(0, NewPosition);

                WorldInst.Set(1, NewPosition.X, NewPosition.Y);
            }

            // Hit World Border
            else
            {
                Alive = false;
            }

            if (!Grow)
            {
                Coords.RemoveAt(Coords.Count - 2);

                WorldInst.Set(0, Coords[Coords.Count - 2].X, Coords[Coords.Count - 2].Y);
            }
        }

        public Point GetHead()
        {
            return Coords[0];
        }

        public int GetSize()
        {
            return Coords.Count;
        }

        public bool IsAlive()
        {
            return Alive;
        }
    }
}
