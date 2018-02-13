using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    abstract class ItemSpawner : INetworkable
    {
        public static int ItemWorth = 3;

        protected World WorldInst;

        public abstract bool Initialize(SnakeOnlineServer.ServerInput ServerIn, World WorldInst);
        public abstract bool Initialize(SnakeOnlineServer.ServerOutput ServerOut, World WorldInst);

        public abstract void SpawnNew();

        public virtual void NetworkUpdate()
        {
        }
    }
}
