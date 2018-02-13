using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using OpenTK;

namespace SnakeOnline
{
    class GameManager
    {
        private GameView LocalView;
        private AppWindow Window;

        private Timer ClientGameLoop;
        private Timer NetworkUpdateLoop;

        public void Initialize(AppWindow Window, string WindowTitle, Size WindowSize)
        {
            this.Window = Window;
            Window.Title = WindowTitle;
            Window.Size = WindowSize;

            LocalView = new GameView();
        }

        public void Run(double UpdateRate)
        {
            if (!LocalView.Initialize(Window, 15, 15))
            {
                throw new Exception("Failed to Create Local Game");
            }

            if (!Window.Initialize(LocalView.WorldInst, LocalView.SnakeInst))
            {
                throw new Exception("Failed to Initialize Window");
            }

            LocalView.Run(UpdateRate);

            // Exchange Worlds
            LocalView.ServerIn.SendWorld(LocalView.WorldInst.GetRows(), LocalView.WorldInst.GetColumns(), LocalView.WorldInst.ItemMatrix);
            LocalView.WorldInst.ConstructFromNetwork(LocalView.ServerOut);

            NetworkUpdateLoop = new Timer(500.0d);
            NetworkUpdateLoop.AutoReset = true;
            NetworkUpdateLoop.Elapsed += new ElapsedEventHandler(NetworkUpdate);
            NetworkUpdateLoop.Enabled = true;

            ClientGameLoop = new Timer(UpdateRate * 1000d);

            ClientGameLoop.AutoReset = true;
            ClientGameLoop.Elapsed += new ElapsedEventHandler(GameLoop);
            ClientGameLoop.Enabled = true;
        }

        protected void NetworkUpdate(object Sender, ElapsedEventArgs e)
        {
            LocalView.InputInst.NetworkUpdate();
            LocalView.ItemSpawnerInst.NetworkUpdate();
        }

        protected void GameLoop(object Sender, ElapsedEventArgs e)
        {
            if (LocalView.GameOver)
            {
                Console.WriteLine("Local Game Over");

                ClientGameLoop.Stop();
            }

            else
            {
                LocalView.Tick();
            }
        }

        public void Shutdown()
        {
            Window.Dispose();
        }
    }
}
