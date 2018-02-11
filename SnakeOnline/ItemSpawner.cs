using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    abstract class ItemSpawner
    {
        public static int ItemWorth = 4;

        protected World WorldInst;

        public abstract bool Initialize(World WorldInst);

        public abstract void SpawnNew();
    }
}
