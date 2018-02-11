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

            WorldInst.Set(2, Row, Column);
        }
    }
}
