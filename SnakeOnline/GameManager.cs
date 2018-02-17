using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using OpenTK;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SnakeOnline
{
    class GameManager
    {
        private bool Running = false;

        private Session GameSession;

        private bool SessionReady = false;
        private bool InSession = false;

        private ScoreService Scoring;
        private bool ScoreServiceOnline;

        private SessionType RequestedSessionType;
        private IPEndPoint RequestedEndPoint;

        private GameView LocalView;
        private GameView RemoteView;

        private double UpdateRate;
        private int Rows;
        private int Columns;

        internal AppWindow Window;

        private System.Timers.Timer ClientGameLoop;
        private System.Timers.Timer NetworkUpdateLoop;

        public void Initialize(double GameUpdateRate, int WorldRows, int WorldColumns, string WindowTitle, Size WindowSize)
        {
            UpdateRate = GameUpdateRate;
            Rows = WorldRows;
            Columns = WorldColumns;

            Thread WindowThread = new Thread(() =>
            {
                Window = new AppWindow();
                Window.Title = WindowTitle;
                Window.Size = WindowSize;
                Window.TargetUpdateFrequency = 1.0d;

                if (!Window.Initialize(Rows, Columns))
                {
                    throw new Exception("Failed to Initialize Window");
                }

                Window.Run(1.0d, 60.0d);
            });

            WindowThread.Start();

            // Wait Until Window's Memory is Initialized.
            while (Window == null) { Thread.Yield(); }

            // Wait Until Window is Ready for Usage.
            while (!Window.Ready) { Thread.Yield(); }

            Window.SessionEndCallback(
                () =>
                {
                    InSession = false;
                    SessionReady = false;
                });

            ScoreServiceOnline = true;

            Scoring = new ScoreService();
            if (!Scoring.Initialize())
            {
                ScoreServiceOnline = false;
            }

            IPEndPoint ScoringServiceEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6710);

            if (!ScoreServiceOnline || !Scoring.Connect(ScoringServiceEndPoint))
            {
                ScoreServiceOnline = false;

                Scoring.Dispose();
            }
        }

        public void RequestNewSession()
        {
            Window.SessionInterface(out RequestedSessionType, out RequestedEndPoint);
        }

        public void StartSession()
        {
            if (GameSession != null)
            {
                GameSession.Dispose();
            }

            else
            {
                GameSession = new Session();
            }

            GameSession.Type = RequestedSessionType;

            if (GameSession.Type == SessionType.Multiplayer)
            {
                if (!GameSession.Create(RequestedEndPoint, Rows, Columns))
                {
                    throw new Exception("Failed to Create Game Session");
                }
            }

            LocalView = new GameView();

            if (!LocalView.Initialize(Window, Rows, Columns))
            {
                throw new Exception("Failed to Create Local Game");
            }

            if (GameSession.Type == SessionType.Multiplayer)
            {
                RemoteView = new GameView();

                if (!RemoteView.InitializeNetworked(Rows, Columns))
                {
                    throw new Exception("Failed to Create Remote Game");
                }
            }
        }

        public bool ConnectSession()
        {
            if (GameSession.Type == SessionType.Multiplayer)
            {
                Console.WriteLine("Connecting... (Timeout in 60 Seconds)\n");
                
                if (!GameSession.Connect())
                {
                    Console.WriteLine("\nFailed to Connect to Presider Server\n");

                    return false;
                }
                
                else
                {
                    Console.WriteLine("Connection Established\n");
                }
            }

            return true;
        }

        public void Run()
        {
            Running = true;

            while (Running)
            {
                if (InSession)
                {
                    // Divert Resources to Other Threads.
                    Thread.Yield();
                }

                else if (SessionReady)
                {
                    LocalView.Spawn();

                    Window.SetupLocal(LocalView.WorldInst, LocalView.SnakeInst);

                    if (GameSession.Type == SessionType.Multiplayer)
                    {
                        Window.SetupRemote(RemoteView.WorldInst);
                    }

                    if (GameSession.Type == SessionType.Multiplayer)
                    {
                        NetworkUpdateLoop = new System.Timers.Timer(100.0d);
                        NetworkUpdateLoop.AutoReset = true;
                        NetworkUpdateLoop.Elapsed += new ElapsedEventHandler(NetworkUpdate);
                        NetworkUpdateLoop.Enabled = true;
                    }

                    ClientGameLoop = new System.Timers.Timer(UpdateRate * 1000d);
                    ClientGameLoop.AutoReset = true;
                    ClientGameLoop.Elapsed += new ElapsedEventHandler(GameLoop);
                    ClientGameLoop.Enabled = true;

                    InSession = true;
                    Window.IsInSession = true;
                }

                else
                {
                    if (ClientGameLoop != null)
                        ClientGameLoop.Stop();

                    if (NetworkUpdateLoop != null)
                        NetworkUpdateLoop.Stop();

                    // Loop Until a Valid Session is Ready.
                    while (!SessionReady)
                    {
                        RequestNewSession();

                        StartSession();

                        SessionReady = ConnectSession();

                        Window.ActiveScreen = Screen.Game;

                        if (!SessionReady)
                        {
                            GameSession.Dispose();
                        }
                    }
                }
            }
        }

        protected void NetworkUpdate(object Sender, ElapsedEventArgs e)
        {
            GameSession.SendWorld(LocalView.WorldInst);
            GameSession.ReceiveWorld(RemoteView.WorldInst);
        }

        protected void GameLoop(object Sender, ElapsedEventArgs e)
        {
            if (LocalView.GameOver)
            {
                Console.WriteLine("Local Game Over");

                ClientGameLoop.Stop();

                if (ScoreServiceOnline)
                {
                    // @todo: Implement GUI Element for Getting Name.
                    Scoring.Submit("Test", LocalView.SnakeInst.GetSize());
                }
            }

            else
            {
                LocalView.Tick();
            }

            if (GameSession.Type == SessionType.Multiplayer)
            {
                // @todo: Implement Game Over Replication.
                if (RemoteView.GameOver)
                {
                    Console.WriteLine("Remote Game Over");
                }
            }
        }

        public void Shutdown()
        {
            Window.Dispose();
        }
    }
}
