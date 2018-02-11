using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SnakeOnline
{
    class GameManager
    {
        private GameView LocalView;
        private AppWindow Window;

        private Timer ClientGameLoop;

        public void Initialize(AppWindow Window)
        {
            this.Window = Window;

            LocalView = new GameView();
        }

        public void Run(double UpdateRate)
        {
            if (!LocalView.Initialize(Window, 25, 25))
            {
                throw new Exception("Failed to Create Local Game");
            }

            LocalView.Run(UpdateRate);

            ClientGameLoop = new Timer(UpdateRate * 1000d);

            ClientGameLoop.AutoReset = true;
            ClientGameLoop.Elapsed += new ElapsedEventHandler(GameLoop);
            ClientGameLoop.Enabled = true;
        }

        protected void GameLoop(object Sender, ElapsedEventArgs e)
        {
            if (LocalView.GameOver)
            {
                Console.WriteLine("Local Game Over");
            }

            else
            {
                LocalView.Tick();
            }
        }
    }
}
