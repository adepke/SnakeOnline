using System;
using System.Collections;
using System.Collections.Generic;

namespace SnakeOnline
{
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

        private bool IsValidIndex(int Row, int Column)
        {
            return (Row <= Rows && Column <= Columns);
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
