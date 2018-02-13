using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOnline
{
    class NetworkedInput : Input, INetworkable
    {
        private SnakeOnlineServer.ServerOutput ServerOutHandle;

        // Never Initialize a NetworkedInput with a Window Instance.
        public override bool Initialize(SnakeOnlineServer.ServerInput ServerIn, AppWindow WindowInst)
        {
            return false;
        }

        public override bool Initialize(SnakeOnlineServer.ServerOutput ServerOut)
        {
            return true;
        }

        public void NetworkUpdate()
        {
            int NewInput;

            ServerOutHandle.GetMovement(out NewInput);

            LastInput = (MovementDirection)NewInput;
        }
    }
}
