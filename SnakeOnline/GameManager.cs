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
        private Session GameSession;

        private GameView LocalView;
        private GameView RemoteView;
        
        private AppWindow Window;

        private Timer ClientGameLoop;
        private Timer NetworkUpdateLoop;

        public void Initialize(AppWindow Window, string WindowTitle, Size WindowSize)
        {
            this.Window = Window;
            Window.Title = WindowTitle;
            Window.Size = WindowSize;

            if (!Window.Initialize(LocalView.WorldInst, LocalView.SnakeInst))
            {
                throw new Exception("Failed to Initialize Window");
            }

        }

        public void StartSession(SessionType Type)
        {
            GameSession = new Session();
            GameSession.Type = Type;

            LocalView = new GameView();

            if (!LocalView.Initialize(Window, 15, 15))
            {
                throw new Exception("Failed to Create Local Game");
            }

            if (GameSession.Type == SessionType.Multiplayer)
            {
                RemoteView = new GameView();
            }
        }

        public void Run(double UpdateRate)
        {
            LocalView.Run(UpdateRate);

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
            // Send and Receive World.

            GameSession.SendWorld(LocalView.WorldInst);
            GameSession.ReceiveWorld(RemoteView.WorldInst);
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
