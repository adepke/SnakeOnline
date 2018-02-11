using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class ItemSpawner
    {
        public static int ItemWorth = 4;

        private World WorldInst;
        private Random RandomHandler;

        public bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            RandomHandler = new Random();

            return true;
        }

        public void SpawnNew()
        {
            int Row = RandomHandler.Next(0, WorldInst.GetRows());
            int Column = RandomHandler.Next(0, WorldInst.GetColumns());

            WorldInst.Set(2, Row, Column);
        }
    }
}
