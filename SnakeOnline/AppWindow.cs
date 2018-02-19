using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Net;

namespace SnakeOnline
{
    enum Screen
    {
        Game,
        Menu,
        NetworkedSessionMenu,
        Highscores,
        UsernameMenu,
        Connecting,
    }

    class AppWindow : GameWindow
    {
        public bool Ready = false;

        public bool IsInSession = false;

        private SessionType ActiveSessionType;

        internal Screen ActiveScreen = Screen.UsernameMenu;
        private Screen LastActiveScreen = Screen.Game;  // Set this to Anything Other Than ActiveScreen.

        private Gwen.Renderer.OpenTK RenderHandler;
        private Gwen.Input.OpenTK InputHandler;
        private Gwen.Skin.Base BaseSkin;
        private Gwen.Control.Canvas BaseCanvas;

        // Game Screen.
        private Gwen.Control.TextBox LocalSizeBox;
        private Gwen.Control.TextBox RemoteSizeBox;
        private Gwen.Control.Button ReturnToMenuButton;

        // Menu Screen.
        private Gwen.Control.Button SingleplayerButton;
        private Gwen.Control.Button MultiplayerButton;
        private Gwen.Control.Button HighscoresButton;

        // Networked Session Menu Screen.
        private Gwen.Control.Label ServerSelectorLabel;
        private Gwen.Control.ComboBox ServerSelector;
        private Gwen.Control.Label CustomServerLabel;
        private Gwen.Control.TextBox CustomServerAddress;
        private Gwen.Control.TextBox CustomServerPort;
        private Gwen.Control.Button SessionConnect;
        private Gwen.Control.Button BackButton;

        // Highscores Screen.
        private Gwen.Control.Label NoDataFailure;
        private Gwen.Control.Label TopScoresLabel;
        private Gwen.Control.ListBox TopScoresList;
        private Gwen.Control.Button TopScoresBackButton;

        // Username Menu.
        private Gwen.Control.Label UsernameLabel;
        private Gwen.Control.TextBox UsernameBox;
        private Gwen.Control.Button UsernameContinue;

        public delegate void EndCallback();
        public delegate void RetrieveHighscoresCallback();

        private EndCallback SessionLeaveCallback;
        private RetrieveHighscoresCallback GetHighscoresCallback;

        private int Rows;
        private int Columns;

        private int ColumnWidth = 25;
        private int RowHeight = 25;

        private World LocalWorldInst;
        private World RemoteWorldInst;
        private Snake LocalSnakeInst;

        private int GridCellTexture;
        private int SnakeCellTexture;
        private int ItemCellTexture;

        public bool Initialize(int Rows, int Columns)
        {
            this.Rows = Rows;
            this.Columns = Columns;

            RenderHandler = new Gwen.Renderer.OpenTK();

#if DEBUG
            BaseSkin = new Gwen.Skin.TexturedBase(RenderHandler, @"../../../Assets/DefaultSkin.png");
#else
            BaseSkin = new Gwen.Skin.TexturedBase(RenderHandler, @"Assets/DefaultSkin.png");
#endif
            BaseSkin.DefaultFont = new Gwen.Font(RenderHandler, "Arial", 10);

            BaseCanvas = new Gwen.Control.Canvas(BaseSkin);
            BaseCanvas.SetSize(Width, Height);
            BaseCanvas.ShouldDrawBackground = true;
            BaseCanvas.BackgroundColor = Color.White;

            InputHandler = new Gwen.Input.OpenTK(this);
            InputHandler.Initialize(BaseCanvas);
            
            // Setup Generic Window Callbacks
            Keyboard.KeyDown += Keyboard_KeyDown;
            Keyboard.KeyUp += Keyboard_KeyUp;
            Mouse.ButtonDown += Mouse_ButtonDown;
            Mouse.ButtonUp += Mouse_ButtonUp;
            Mouse.Move += Mouse_Move;
            Mouse.WheelChanged += Mouse_Wheel;

            SetupMenu();
            SetupNetworkedSessionMenu();
            SetupHighscores();
            SetupUsernameMenu();

            Ready = true;

            return true;
        }
        
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            InputHandler.ProcessKeyDown(e);
        }
        
        void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            InputHandler.ProcessKeyUp(e);
        }
        
        void Mouse_ButtonDown(object sender, MouseButtonEventArgs args)
        {
            InputHandler.ProcessMouseMessage(args);
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs args)
        {
            InputHandler.ProcessMouseMessage(args);
        }

        void Mouse_Move(object sender, MouseMoveEventArgs args)
        {
            InputHandler.ProcessMouseMessage(args);
        }

        void Mouse_Wheel(object sender, MouseWheelEventArgs args)
        {
            InputHandler.ProcessMouseMessage(args);
        }

        // Used to Interrupt Logic Flow of GameManager When an Event That Causes and End of Session in the GUI is Triggered.
        public void SessionEndCallback(EndCallback Callback)
        {
            SessionLeaveCallback = Callback;
        }

        // Used to Fill Highscores.
        public void GetScoresCallback(RetrieveHighscoresCallback Callback)
        {
            GetHighscoresCallback = Callback;
        }

        public void SessionInterface(out SessionType RequestedSessionType, out IPEndPoint RequestedEndPoint)
        {
            ActiveScreen = Screen.Menu;

            // Defaults.
            RequestedSessionType = SessionType.Singleplayer;
            RequestedEndPoint = null;

            // Stall Calling Thread Until the Interface is Closed.
            while (ActiveScreen == Screen.Menu || ActiveScreen == Screen.NetworkedSessionMenu || ActiveScreen == Screen.Highscores)
            {
                System.Threading.Thread.Yield();
            }

            if (ActiveSessionType == SessionType.Multiplayer)
            {
                RequestedSessionType = SessionType.Multiplayer;

                switch (ServerSelector.SelectedItem.Text)
                {
                    case "Server A":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6700);
                        break;
                    case "Server B":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6701);
                        break;
                    case "Server C":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6702);
                        break;
                    case "Server D":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6703);
                        break;
                    case "Server E":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6704);
                        break;
                    case "Server F":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6705);
                        break;
                    case "Server G":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6706);
                        break;
                    case "Server H":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6707);
                        break;
                    case "Server I":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6708);
                        break;
                    case "Server J":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse("10.16.1.100"), 6709);
                        break;
                    case "Custom Server...":
                        RequestedEndPoint = new IPEndPoint(IPAddress.Parse(CustomServerAddress.Text), Convert.ToInt32(CustomServerPort.Text));
                        break;
                    default:
                        RequestedSessionType = SessionType.Singleplayer;
                        RequestedEndPoint = null;
                        break;
                }
            }

            else
            {
                RequestedSessionType = SessionType.Singleplayer;
                RequestedEndPoint = null;
            }
        }

        public string GetUsername()
        {
            return UsernameBox.Text;
        }

        public void SetupMenu()
        {
            SingleplayerButton = new Gwen.Control.Button(BaseCanvas);
            SingleplayerButton.SetText("Singleplayer");
            SingleplayerButton.SetSize(300, 100);
            SingleplayerButton.SetPosition(200, 100);
            SingleplayerButton.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.Game;
                ActiveSessionType = SessionType.Singleplayer;
            };

            MultiplayerButton = new Gwen.Control.Button(BaseCanvas);
            MultiplayerButton.SetText("Multiplayer");
            MultiplayerButton.SetSize(300, 100);
            MultiplayerButton.SetPosition(200, 240);
            MultiplayerButton.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.NetworkedSessionMenu;
                ActiveSessionType = SessionType.Multiplayer;
            };

            HighscoresButton = new Gwen.Control.Button(BaseCanvas);
            HighscoresButton.SetText("Highscores");
            HighscoresButton.SetSize(300, 100);
            HighscoresButton.SetPosition(200, 380);
            HighscoresButton.Clicked += (B, Args) =>
            {
                GetHighscoresCallback();
                ActiveScreen = Screen.Highscores;
            };
        }

        public void SetupNetworkedSessionMenu()
        {
            ServerSelectorLabel = new Gwen.Control.Label(BaseCanvas);
            ServerSelectorLabel.SetText("Server");
            ServerSelectorLabel.AutoSizeToContents = true;
            ServerSelectorLabel.SetPosition(50, 0);

            ServerSelector = new Gwen.Control.ComboBox(BaseCanvas);
            ServerSelector.AddItem("Server A");
            ServerSelector.AddItem("Server B");
            ServerSelector.AddItem("Server C");
            ServerSelector.AddItem("Server D");
            ServerSelector.AddItem("Server E");
            ServerSelector.AddItem("Server F");
            ServerSelector.AddItem("Server G");
            ServerSelector.AddItem("Server H");
            ServerSelector.AddItem("Server I");
            ServerSelector.AddItem("Server J");
            ServerSelector.AddItem("Custom Server...");
            ServerSelector.SetSize(200, 50);
            ServerSelector.SetPosition(50, 20);
            ServerSelector.TextColor = Color.Black;
            ServerSelector.UpdateColors();

            CustomServerLabel = new Gwen.Control.Label(BaseCanvas);
            CustomServerLabel.SetText("Address                                           Port");
            CustomServerLabel.AutoSizeToContents = true;
            CustomServerLabel.SetPosition(50, 180);

            CustomServerAddress = new Gwen.Control.TextBox(BaseCanvas);
            CustomServerAddress.SetSize(200, 30);
            CustomServerAddress.SetPosition(50, 200);

            CustomServerPort = new Gwen.Control.TextBox(BaseCanvas);
            CustomServerPort.SetSize(70, 30);
            CustomServerPort.SetPosition(254, 200);

            SessionConnect = new Gwen.Control.Button(BaseCanvas);
            SessionConnect.SetText("Connect");
            SessionConnect.SetSize(200, 50);
            SessionConnect.SetPosition(260, 380);
            SessionConnect.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.Connecting;
            };

            BackButton = new Gwen.Control.Button(BaseCanvas);
            BackButton.SetText("Back");
            BackButton.AutoSizeToContents = true;
            BackButton.SetPosition(50, 450);
            BackButton.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.Menu;
            };
        }

        public void SetupHighscores()
        {
            NoDataFailure = new Gwen.Control.Label(BaseCanvas);
            NoDataFailure.SetText("No Highscore Data Retrieved");
            NoDataFailure.AutoSizeToContents = true;
            NoDataFailure.SetPosition(100, 150);

            TopScoresLabel = new Gwen.Control.Label(BaseCanvas);
            TopScoresLabel.SetText("Highscores");
            TopScoresLabel.SetSize(100, 50);
            TopScoresLabel.SetPosition(200, 50);

            TopScoresList = new Gwen.Control.ListBox(BaseCanvas);
            TopScoresList.SetSize(250, 300);
            TopScoresList.SetPosition(200, 70);

            TopScoresBackButton = new Gwen.Control.Button(BaseCanvas);
            TopScoresBackButton.SetText("Back");
            TopScoresBackButton.AutoSizeToContents = true;
            TopScoresBackButton.SetPosition(50, 350);
            TopScoresBackButton.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.Menu;
            };
        }

        private void SetupUsernameMenu()
        {
            UsernameLabel = new Gwen.Control.Label(BaseCanvas);
            UsernameLabel.SetText("Enter Desired Username:" + "\n(This is how your score will be represented in the Highscores page.)");
            UsernameLabel.AutoSizeToContents = true;
            UsernameLabel.SetPosition(300, 150);

            UsernameBox = new Gwen.Control.TextBox(BaseCanvas);
            UsernameBox.SetSize(180, 35);
            UsernameBox.SetPosition(300, 200);

            UsernameContinue = new Gwen.Control.Button(BaseCanvas);
            UsernameContinue.SetText("Continue");
            UsernameContinue.SetSize(150, 30);
            UsernameContinue.SetPosition(300, 260);
            UsernameContinue.Clicked += (B, Args) =>
            {
                if (UsernameBox.Text != "")
                {
                    ActiveScreen = Screen.Menu;
                }
            };
        }

        public void SetupLocal(World LocalWorldInst, Snake LocalSnakeInst)
        {
            this.LocalWorldInst = LocalWorldInst;
            this.LocalSnakeInst = LocalSnakeInst;

            LocalSizeBox = new Gwen.Control.TextBox(BaseCanvas);
            LocalSizeBox.TextColor = Color.Black;
            LocalSizeBox.SetPosition(0, Rows * RowHeight);

            ReturnToMenuButton = new Gwen.Control.Button(BaseCanvas);
            ReturnToMenuButton.SetText("Return to Menu");
            ReturnToMenuButton.AutoSizeToContents = true;
            ReturnToMenuButton.SetPosition(0, Rows * RowHeight + 60);
            ReturnToMenuButton.Clicked += (B, Args) =>
            {
                ActiveScreen = Screen.Menu;

                SessionLeaveCallback();
            };
        }

        public void SetupRemote(World RemoteWorldInst)
        {
            this.RemoteWorldInst = RemoteWorldInst;
            
            RemoteSizeBox = new Gwen.Control.TextBox(BaseCanvas);
            RemoteSizeBox.TextColor = Color.Black;
            RemoteSizeBox.SetPosition(Columns * ColumnWidth + 50, Rows * RowHeight);
        }

        public void FillHighscores(List<Highscore> Highscores)
        {
            if (Highscores != null)
            {
                foreach (Highscore Score in Highscores)
                {
                    TopScoresList.AddRow(Score.Name + "      -      " + Score.Score);
                }
            }
        }

        protected void ShowGame(bool Show = true)
        {
            if (Show)
            {
                if (LocalSizeBox != null) LocalSizeBox.Show();
                if (RemoteSizeBox != null) RemoteSizeBox.Show();
                if (ReturnToMenuButton != null) ReturnToMenuButton.Show();
            }

            else
            {
                if (LocalSizeBox != null) LocalSizeBox.Hide();
                if (RemoteSizeBox != null) RemoteSizeBox.Hide();
                if (ReturnToMenuButton != null) ReturnToMenuButton.Hide();
            }
        }

        protected void ShowMenu(bool Show = true)
        {
            if (Show)
            {
                SingleplayerButton.Show();
                MultiplayerButton.Show();
                HighscoresButton.Show();
            }

            else
            {
                SingleplayerButton.Hide();
                MultiplayerButton.Hide();
                HighscoresButton.Hide();
            }
        }

        protected void ShowNetworkedSessionMenu(bool Show = true)
        {
            if (Show)
            {
                ServerSelectorLabel.Show();
                ServerSelector.Show();
                CustomServerLabel.Show();
                CustomServerAddress.Show();
                CustomServerPort.Show();
                SessionConnect.Show();
                BackButton.Show();
            }

            else
            {
                ServerSelectorLabel.Hide();
                ServerSelector.Hide();
                CustomServerLabel.Hide();
                CustomServerAddress.Hide();
                CustomServerPort.Hide();
                SessionConnect.Hide();
                BackButton.Hide();
            }
        }

        protected void ShowHighscores(bool Show = true)
        {
            if (Show)
            {
                if (TopScoresList.RowCount > 0)
                {
                    TopScoresLabel.Show();
                    TopScoresList.Show();
                    TopScoresBackButton.Show();
                }

                else
                {
                    NoDataFailure.Show();
                    TopScoresBackButton.Show();
                }
            }

            else
            {
                NoDataFailure.Hide();
                TopScoresLabel.Hide();
                TopScoresList.Hide();
                TopScoresBackButton.Hide();
            }
        }

        protected void ShowUsernameMenu(bool Show = true)
        {
            if (Show)
            {
                UsernameLabel.Show();
                UsernameBox.Show();
                UsernameContinue.Show();
            }

            else
            {
                UsernameLabel.Hide();
                UsernameBox.Hide();
                UsernameContinue.Hide();
            }
        }

        protected void ShowConnecting(bool Show = true)
        {
            if (Show)
            {

            }

            else
            {

            }
        }

        protected void ShowScreen(Screen NewScreen)
        {
            switch (NewScreen)
            {
                case Screen.Game:
                    ShowGame(true);
                    ShowMenu(false);
                    ShowNetworkedSessionMenu(false);
                    ShowHighscores(false);
                    ShowUsernameMenu(false);
                    ShowConnecting(false);
                    break;
                case Screen.Menu:
                    ShowGame(false);
                    ShowMenu(true);
                    ShowNetworkedSessionMenu(false);
                    ShowHighscores(false);
                    ShowUsernameMenu(false);
                    ShowConnecting(false);
                    break;
                case Screen.NetworkedSessionMenu:
                    ShowGame(false);
                    ShowMenu(false);
                    ShowNetworkedSessionMenu(true);
                    ShowHighscores(false);
                    ShowUsernameMenu(false);
                    ShowConnecting(false);
                    break;
                case Screen.Highscores:
                    ShowGame(false);
                    ShowMenu(false);
                    ShowNetworkedSessionMenu(false);
                    ShowHighscores(true);
                    ShowUsernameMenu(false);
                    ShowConnecting(false);
                    break;
                case Screen.UsernameMenu:
                    ShowGame(false);
                    ShowMenu(false);
                    ShowNetworkedSessionMenu(false);
                    ShowHighscores(false);
                    ShowUsernameMenu(true);
                    ShowConnecting(false);
                    break;
                case Screen.Connecting:
                    ShowGame(false);
                    ShowMenu(false);
                    ShowNetworkedSessionMenu(false);
                    ShowHighscores(false);
                    ShowUsernameMenu(false);
                    ShowConnecting(true);
                    break;
            }
        }

        public void DrawLocalUI()
        {
            if (LocalSizeBox != null)
            {
                LocalSizeBox.Text = "Size: " + LocalSnakeInst.GetSize();
                LocalSizeBox.SizeToContents();
            }
        }

        public void DrawLocalGame()
        {
            if (LocalWorldInst != null)
            {
                //GL.MatrixMode(MatrixMode.Projection);
                //GL.LoadIdentity();
                //GL.PushMatrix();

                GL.Color4(Color.White);

                for (int Row = 0; Row < Rows; ++Row)
                {
                    for (int Column = 0; Column < Columns; ++Column)
                    {
                        if ((int)LocalWorldInst.Get(Row, Column) == 0)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, GridCellTexture);
                        }

                        else if ((int)LocalWorldInst.Get(Row, Column) == 1)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, SnakeCellTexture);
                        }

                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, ItemCellTexture);
                        }

                        GL.Begin(PrimitiveType.Quads);

                        // Bottom Left
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex2(Column * ColumnWidth, Row * RowHeight);

                        // Bottom Right
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex2(Column * ColumnWidth + ColumnWidth, Row * RowHeight);

                        // Top Right
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex2(Column * ColumnWidth + ColumnWidth, Row * RowHeight + RowHeight);

                        // Top Left
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex2(Column * ColumnWidth, Row * RowHeight + RowHeight);

                        GL.End();
                    }
                }

                GL.BindTexture(TextureTarget.Texture2D, 0);

                //GL.PopMatrix();
            }
        }

        public void DrawRemoteUI()
        {
            if (RemoteWorldInst != null)
            {
                int RemoteSize = 0;

                for (int Row = 0; Row < Rows; ++Row)
                {
                    for (int Column = 0; Column < Columns; ++Column)
                    {
                        if (RemoteWorldInst.Get(Row, Column) == 1)
                        {
                            ++RemoteSize;
                        }
                    }
                }

                RemoteSizeBox.Text = "Size: " + RemoteSize;
                RemoteSizeBox.SizeToContents();
            }
        }

        public void DrawRemoteGame()
        {
            if (RemoteWorldInst != null)
            {
                GL.Color4(Color.White);

                for (int Row = 0; Row < Rows; ++Row)
                {
                    for (int Column = 0; Column < Columns; ++Column)
                    {
                        if ((int)RemoteWorldInst.Get(Row, Column) == 0)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, GridCellTexture);
                        }

                        else if ((int)RemoteWorldInst.Get(Row, Column) == 1)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, SnakeCellTexture);
                        }

                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, ItemCellTexture);
                        }

                        GL.Begin(PrimitiveType.Quads);

                        // Bottom Left
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex2(Column * ColumnWidth + (Columns * ColumnWidth + 50), Row * RowHeight);

                        // Bottom Right
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex2(Column * ColumnWidth + ColumnWidth + (Columns * ColumnWidth + 50), Row * RowHeight);

                        // Top Right
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex2(Column * ColumnWidth + ColumnWidth + (Columns * ColumnWidth + 50), Row * RowHeight + RowHeight);

                        // Top Left
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex2(Column * ColumnWidth + (Columns * ColumnWidth + 50), Row * RowHeight + RowHeight);

                        GL.End();
                    }
                }

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.White);

#if DEBUG
            GridCellTexture = LoadTexture(@"../../../Assets/GridCell.png");
            SnakeCellTexture = LoadTexture(@"../../../Assets/SnakeCell.png");
            ItemCellTexture = LoadTexture(@"../../../Assets/ItemCell.png");
#else
            GridCellTexture = LoadTexture(@"Assets/GridCell.png");
            SnakeCellTexture = LoadTexture(@"Assets/SnakeCell.png");
            ItemCellTexture = LoadTexture(@"Assets/ItemCell.png");
#endif

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);  // Enable Alpha Blending
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);
        }

        protected int LoadTexture(string File)
        {
            int Texture = 0;

            try
            {
                using (Bitmap TextureBitmap = new Bitmap(File))
                {
                    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                    GL.GenTextures(1, out Texture);
                    GL.BindTexture(TextureTarget.Texture2D, Texture);

                    BitmapData TextureBitmapData = TextureBitmap.LockBits(new System.Drawing.Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureBitmapData.Width, TextureBitmapData.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureBitmapData.Scan0);

                    TextureBitmap.UnlockBits(TextureBitmapData);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                }
            }

            catch (System.IO.FileNotFoundException)
            {
                return -1;
            }

            return Texture;
        }
        
        protected int LoadTexture2(string File)
        {
            int Texture;

            using (Bitmap TextureBitmap = new Bitmap(File))
            {
                Texture = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, Texture);

                // Prevent Averaging, Maintaining Image Quality.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

                // Clamp.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

                // Create Texture Object Definition.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureBitmap.Width, TextureBitmap.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

                // Load Texture into Bitmap Data.
                BitmapData TextureBitmapData = TextureBitmap.LockBits(new Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Write Bitmap to Texture Object.
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, TextureBitmapData.Width, TextureBitmapData.Height,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureBitmapData.Scan0);

                TextureBitmap.UnlockBits(TextureBitmapData);
            }

            // Reset Texture Binding.
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return Texture;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Add Slight Delay to Screen Changes to Give Slower Machines Time to Prepare Everything.
            if (ActiveScreen != LastActiveScreen)
            {
                System.Threading.Thread.Sleep(100);

                LastActiveScreen = ActiveScreen;

                ShowScreen(ActiveScreen);
            }

            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (ActiveScreen == Screen.Game)
            {
                DrawLocalUI();

                if (ActiveSessionType == SessionType.Multiplayer)
                {
                    DrawRemoteUI();
                }

                BaseCanvas.RenderCanvas();

                DrawLocalGame();

                if (ActiveSessionType == SessionType.Multiplayer)
                {
                    DrawRemoteGame();
                }
            }

            else
            {
                BaseCanvas.RenderCanvas();
            }

            GL.Flush();

            SwapBuffers();
        }

        public override void Dispose()
        {
            base.Dispose();

            BaseCanvas.Dispose();
            BaseSkin.Dispose();
            RenderHandler.Dispose();
        }
    }
}
