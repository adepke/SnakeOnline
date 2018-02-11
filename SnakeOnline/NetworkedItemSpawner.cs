using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class NetworkedItemSpawner : ItemSpawner
    {
        public override bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            return true;
        }

        // Disabled
        public override void SpawnNew()
        {
            return;
        }

        public void NetworkedSpawnNew(Point Position)
        {
            
        }
    }
}
