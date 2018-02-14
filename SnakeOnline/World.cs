using System;
using System.Collections;
using System.Collections.Generic;

namespace SnakeOnline
{
    struct Point
    {
        public int Row, Column;
    }

    class World
    {
        private int Rows;
        private int Columns;

        internal byte[,] ItemMatrix;

        public bool Initialize(int Rows, int Columns)
        {
            ItemMatrix = new byte[Rows, Columns];

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

        public int Get(int Row, int Column)
        {
            if (!IsValidIndex(Row, Column))
                return -1;

            return ItemMatrix[Row, Column];
        }

        public bool Set(byte Value, int Row, int Column)
        {
            if (!IsValidIndex(Row, Column))
                return false;

            ItemMatrix[Row, Column] = Value;

            return true;
        }
    }
}
