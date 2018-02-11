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

        public void Spawn(int Row, int Column, int Size)
        {
            Point Head;
            Head.Row = Row;
            Head.Column = Column;

            Coords.Add(Head);

            for (int Iter = 0; Iter < Size - 1; ++Iter)
            {
                Move(MovementDirection.Left, true);
            }

            Alive = true;
        }

        public void Move(MovementDirection Direction, bool OverrideGrow = false)
        {
            Point NewPosition = Coords[0];

            switch (Direction)
            {
                case MovementDirection.Up:
                    NewPosition.Row -= 1;
                    break;
                case MovementDirection.Down:
                    NewPosition.Row += 1;
                    break;
                case MovementDirection.Left:
                    NewPosition.Column -= 1;
                    break;
                case MovementDirection.Right:
                    NewPosition.Column += 1;
                    break;
            }

            if (!WorldInst.IsValidIndex(NewPosition.Row, NewPosition.Column))
            {
                Alive = false;

                return;
            }

            // Hit Self
            if ((int)WorldInst.Get(NewPosition.Row, NewPosition.Column) == 1)
            {
                Alive = false;

                return;
            }

            bool Grow = OverrideGrow;

            // Hit an Item
            if ((int)WorldInst.Get(NewPosition.Row, NewPosition.Column) == 2)
            {
                Grow = true;
            }

            Coords.Insert(0, NewPosition);

            WorldInst.Set(1, NewPosition.Row, NewPosition.Column);

            /*
            if (!Grow)
            {
                Coords.RemoveAt(Coords.Count - 2);

                WorldInst.Set(0, Coords[Coords.Count - 2].X, Coords[Coords.Count - 2].Y);
            }
            */
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
