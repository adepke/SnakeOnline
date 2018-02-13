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
        private ItemSpawner ItemSpawnerInst;

        public List<Point> Coords;

        private int GrowthQueue = 0;

        private bool Alive = false;

        public bool Initialize(World WorldInst, ItemSpawner ItemSpawnerInst)
        {
            this.WorldInst = WorldInst;
            this.ItemSpawnerInst = ItemSpawnerInst;

            Coords = new List<Point>();

            return true;
        }

        public void Spawn(int Row, int Column, int Size)
        {
            Point Head;
            Head.Row = Row;
            Head.Column = Column;

            Coords.Add(Head);

            GrowthQueue = Size - 1;

            for (int Iter = 0; Iter < Size - 1; ++Iter)
            {
                Move(Input.DefaultInput);
            }

            Move(Input.DefaultInput);

            Alive = true;
        }

        public void Move(MovementDirection Direction)
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

            bool ShouldSpawnNew = false;

            // Hit an Item
            if ((int)WorldInst.Get(NewPosition.Row, NewPosition.Column) == 2)
            {
                GrowthQueue += ItemSpawner.ItemWorth;

                ShouldSpawnNew = true;
            }

            Coords.Insert(0, NewPosition);

            WorldInst.Set(1, NewPosition.Row, NewPosition.Column);

            if (GrowthQueue == 0)
            {
                Point Tail = Coords[Coords.Count - 1];

                Coords.RemoveAt(Coords.Count - 1);

                WorldInst.Set(0, Tail.Row, Tail.Column);
            }

            else
            {
                GrowthQueue -= 1;
            }

            // Only Spawn New Item After Snake Has Been Fully Processed.
            if (ShouldSpawnNew)
            {
                ItemSpawnerInst.SpawnNew();
            }
        }

        public Point GetHead()
        {
            return Coords[0];
        }

        public int GetSize()
        {
            return Coords.Count + GrowthQueue;
        }

        public bool IsAlive()
        {
            return Alive;
        }
    }
}
