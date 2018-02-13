using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class LocalItemSpawner : ItemSpawner
    {
        private SnakeOnlineServer.ServerInput ServerInHandler;

        private Random RandomHandler;

        Point LastSpawn;

        public override bool Initialize(SnakeOnlineServer.ServerInput ServerIn, World WorldInst)
        {
            ServerInHandler = ServerIn;

            this.WorldInst = WorldInst;

            RandomHandler = new Random();

            return true;
        }

        public override bool Initialize(SnakeOnlineServer.ServerOutput ServerOut, World WorldInst)
        {
            return false;
        }

        public override void SpawnNew()
        {
            int Row = RandomHandler.Next(0, WorldInst.GetRows());
            int Column = RandomHandler.Next(0, WorldInst.GetColumns());

            if ((int)WorldInst.Get(Row, Column) == 1)
            {
                // Find Closet Open Cell

                int Iter = 1;

                while (Iter != -1)
                {
                    List<Point> Shell = SurroundingCells(Row, Column, Iter);

                    // No Open Cells in the World
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

                            Point Last;
                            Last.Row = Row;
                            Last.Column = Column;

                            LastSpawn = Last;

                            break;
                        }
                    }

                    ++Iter;
                }
            }

            else
            {
                WorldInst.Set(2, Row, Column);

                Point Last;
                Last.Row = Row;
                Last.Column = Column;

                LastSpawn = Last;
            }
        }

        private List<Point> SurroundingCells(int Row, int Column, int Gap)
        {
            List<Point> Result = new List<Point>();

            Point TopLeft;
            TopLeft.Row = Row - Gap;
            TopLeft.Column = Column - Gap;

            Point BottomRight;
            BottomRight.Row = Row + Gap;
            BottomRight.Column = Column + Gap;

            // Top
            for (int Iter = 0; Iter < BottomRight.Column - TopLeft.Column; ++Iter)
            {
                if (WorldInst.IsValidIndex(TopLeft.Row, TopLeft.Column + Iter))
                {
                    Point NewPoint;
                    NewPoint.Row = TopLeft.Row;
                    NewPoint.Column = TopLeft.Column + Iter;

                    Result.Add(NewPoint);
                }
            }

            // Right
            for (int Iter = 0; Iter < BottomRight.Row - TopLeft.Row; ++Iter)
            {
                if (WorldInst.IsValidIndex(TopLeft.Row + Iter, BottomRight.Column))
                {
                    Point NewPoint;
                    NewPoint.Row = TopLeft.Row + Iter;
                    NewPoint.Column = BottomRight.Column;

                    Result.Add(NewPoint);
                }
            }

            // Bottom
            for (int Iter = 0; Iter < BottomRight.Column - TopLeft.Column; ++Iter)
            {
                if (WorldInst.IsValidIndex(BottomRight.Row, TopLeft.Column + Iter))
                {
                    Point NewPoint;
                    NewPoint.Row = BottomRight.Row;
                    NewPoint.Column = TopLeft.Column + Iter;

                    Result.Add(NewPoint);
                }
            }

            // Left
            for (int Iter = 0; Iter < BottomRight.Row - TopLeft.Row; ++Iter)
            {
                if (WorldInst.IsValidIndex(TopLeft.Row + Iter, TopLeft.Column))
                {
                    Point NewPoint;
                    NewPoint.Row = TopLeft.Row + Iter;
                    NewPoint.Column = TopLeft.Column;

                    Result.Add(NewPoint);
                }
            }

            return Result;
        }

        public override void NetworkUpdate()
        {
            base.NetworkUpdate();

            ServerInHandler.SendItemSpawn(LastSpawn.Row, LastSpawn.Column);
        }
    }
}
