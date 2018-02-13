using System;
using System.Collections;
using System.Collections.Generic;

namespace SnakeOnline
{
    struct Point
    {
        public int Row, Column;

        public static bool operator==(Point TargetA, Point TargetB)
        {
            return (TargetA.Row == TargetB.Row && TargetA.Column == TargetB.Column);
        }

        public static bool operator!=(Point TargetA, Point TargetB)
        {
            return !(TargetA == TargetB);
        }
    }

    class World
    {
        private int Rows;
        private int Columns;

        private object[,] ItemMatrix;

        public bool Initialize(int Rows, int Columns)
        {
            ItemMatrix = new object[Rows, Columns];

            this.Rows = Rows;
            this.Columns = Columns;

            for (int Row = 0; Row < Rows; ++Row)
            {
                for (int Column = 0; Column < Columns; ++Column)
                {
                    ItemMatrix[Row, Column] = 0;
                }
            }

            return true;
        }

        public int GetRows()
        {
            return Rows;
        }

        public int GetColumns()
        {
            return Columns;
        }

        public bool IsValidIndex(int Row, int Column)
        {
            return ((Row >= 0 && Row < Rows) && (Column >= 0 && Column < Columns));
        }

        public object Get(int Row, int Column)
        {
            if (!IsValidIndex(Row, Column))
                return default(object);

            return ItemMatrix[Row, Column];
        }

        public bool Set(object Object, int Row, int Column)
        {
            if (!IsValidIndex(Row, Column))
                return false;

            ItemMatrix[Row, Column] = Object;

            return true;
        }
    }
}
