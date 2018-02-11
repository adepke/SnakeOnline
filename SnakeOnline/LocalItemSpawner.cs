using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class LocalItemSpawner : ItemSpawner
    {
        private Random RandomHandler;

        public override bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            RandomHandler = new Random();

            return true;
        }

        public override void SpawnNew()
        {
            int Row = RandomHandler.Next(0, WorldInst.GetRows());
            int Column = RandomHandler.Next(0, WorldInst.GetColumns());

            if ((int)WorldInst.Get(Row, Column) == 1)
            {
                // Find Closet Open Cell

                int Iter = 0;

                while (Iter != -1)
                {
                    List<Point> Shell = SurroundingCells(Row, Column, Iter);

                    // No Open Cells in the World, Don't Spawn.
                    if (Shell.Count == 0)
                    {
                        break;
                    }

                    foreach (Point NewPoint in Shell)
                    {
                        // If this Point is Open
                        if ((int)WorldInst.Get(NewPoint.Row, NewPoint.Column) == 0)
                        {
                            Iter = -2;

                            WorldInst.Set(2, NewPoint.Row, NewPoint.Column);

                            break;
                        }
                    }

                    ++Iter;
                }
            }

            else
            {
                WorldInst.Set(2, Row, Column);
            }
        }

        private List<Point> SurroundingCells(int Row, int Column, int Gap)
        {
            List<Point> Result = new List<Point>();

            return Result;
        }
    }
}
