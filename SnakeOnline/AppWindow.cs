using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Net;

namespace SnakeOnline
{
    class AppWindow : GameWindow
    {
        public bool Ready = false;

        public bool IsInSession = false;

        public SessionType ActiveSessionType;

        private Gwen.Renderer.OpenTK RenderHandler;
        private Gwen.Input.OpenTK InputHandler;
        private Gwen.Skin.Base BaseSkin;
        private Gwen.Control.Canvas BaseCanvas;

        private Gwen.Control.Canvas MenuCanvas;

        private Gwen.Control.Canvas NetworkedSessionMenuCanvas;

        private Gwen.Control.ComboBox ServerSelector;
        private Gwen.Control.TextBox CustomServerAddress;
        private Gwen.Control.TextBox CustomServerPort;

        private bool SessionInterfaceIsOpen = false;

        private Gwen.Control.TextBox LocalSizeBox;
        private Gwen.Control.TextBox RemoteSizeBox;

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
            
            MenuCanvas = new Gwen.Control.Canvas(BaseSkin);
            MenuCanvas.SetSize(Width, Height);
            MenuCanvas.SetPosition(0, 0);
            MenuCanvas.ShouldDrawBackground = false;

            NetworkedSessionMenuCanvas = new Gwen.Control.Canvas(BaseSkin);
            NetworkedSessionMenuCanvas.SetSize((int)Math.Floor(Width * 0.8d), (int)Math.Floor(Height * 0.8d));
            NetworkedSessionMenuCanvas.SetPosition(50, 50);
            NetworkedSessionMenuCanvas.ShouldDrawBackground = false;
            NetworkedSessionMenuCanvas.BackgroundColor = Color.Green;
            
            InputHandler = new Gwen.Input.OpenTK(this);
            InputHandler.Initialize(BaseCanvas);
            
            // Setup Generic Window Callbacks
            Keyboard.KeyDown += Keyboard_KeyDown;
            Keyboard.KeyUp += Keyboard_KeyUp;
            Mouse.ButtonDown += Mouse_ButtonDown;
            Mouse.ButtonUp += Mouse_ButtonUp;
            Mouse.Move += Mouse_Move;
            Mouse.WheelChanged += Mouse_Wheel;

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

        public void SessionInterface(out SessionType RequestedSessionType, out IPEndPoint RequestedEndPoint)
        {
            SessionInterfaceIsOpen = true;

            RequestedSessionType = SessionType.Multiplayer;

            //RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 735);

            // Stall Calling Thread Until the Interface is Closed.
            while (SessionInterfaceIsOpen)
            {
                System.Threading.Thread.Yield();
            }
            
            // Pseudo Code
            /*
            switch (ServerSelector.ActiveElement)
            {
                case "Server A":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6700);
                    break;
                case "Server B":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6701);
                    break;
                case "Server C":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6702);
                    break;
                case "Server D":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6703);
                    break;
                case "Server E":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6704);
                    break;
                case "Server F":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6705);
                    break;
                case "Server G":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6706);
                    break;
                case "Server H":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6707);
                    break;
                case "Server I":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6708);
                    break;
                case "Server J":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.24"), 6709);
                    break;
                case "Custom Server...":
                    RequestedEndPoint = new IPEndPoint(IPAddress.Parse(CustomServerAddress.Content), Convert.ToInt32(CustomServerPort.Content));
                    break;
                default:
                    RequestedSessionType = SessionType.Singleplayer;
                    RequestedEndPoint = null;
                    break;
            }
            */
        }

        public void SetupNetworkedSessionMenu()
        {
            Gwen.Control.Label ServerSelectorLabel = new Gwen.Control.Label(NetworkedSessionMenuCanvas);
            ServerSelectorLabel.SetText("Server");
            ServerSelectorLabel.AutoSizeToContents = true;
            ServerSelectorLabel.SetPosition(50, 0);

            ServerSelector = new Gwen.Control.ComboBox(NetworkedSessionMenuCanvas);
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

            Gwen.Control.Label CustomServerLabel = new Gwen.Control.Label(NetworkedSessionMenuCanvas);
            CustomServerLabel.SetText("Address                                           Port");
            CustomServerLabel.AutoSizeToContents = true;
            CustomServerLabel.SetPosition(50, 180);

            CustomServerAddress = new Gwen.Control.TextBox(NetworkedSessionMenuCanvas);
            CustomServerAddress.SetSize(200, 30);
            CustomServerAddress.SetPosition(50, 200);

            CustomServerPort = new Gwen.Control.TextBox(NetworkedSessionMenuCanvas);
            CustomServerPort.SetSize(70, 30);
            CustomServerPort.SetPosition(254, 200);

            Gwen.Control.Button SessionConnect = new Gwen.Control.Button(NetworkedSessionMenuCanvas);
            SessionConnect.SetText("Connect");
            SessionConnect.SetSize(200, 50);
            SessionConnect.SetPosition(260, 400);
            SessionConnect.Clicked += (B, Args) => { SessionInterfaceIsOpen = false; };
        }

        public void SetupLocal(World LocalWorldInst, Snake LocalSnakeInst)
        {
            this.LocalWorldInst = LocalWorldInst;
            this.LocalSnakeInst = LocalSnakeInst;

            LocalSizeBox = new Gwen.Control.TextBox(BaseCanvas);
            LocalSizeBox.TextColor = Color.Black;
            LocalSizeBox.SetPosition(0, Rows * RowHeight);
        }

        public void SetupRemote(World RemoteWorldInst)
        {
            this.RemoteWorldInst = RemoteWorldInst;
            
            RemoteSizeBox = new Gwen.Control.TextBox(BaseCanvas);
            RemoteSizeBox.TextColor = Color.Black;
            RemoteSizeBox.SetPosition(Columns * ColumnWidth + 50, Rows * RowHeight);
        }

        public void DrawLocalUI()
        {
            LocalSizeBox.Text = "Size: " + LocalSnakeInst.GetSize();
            LocalSizeBox.SizeToContents();
        }

        public void DrawLocalGame()
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

        public void DrawRemoteUI()
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

        public void DrawRemoteGame()
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
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (IsInSession)
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

            else if (SessionInterfaceIsOpen)
            {
                NetworkedSessionMenuCanvas.RenderCanvas();
            }

            GL.Flush();

            SwapBuffers();
        }

        public override void Dispose()
        {
            base.Dispose();

            NetworkedSessionMenuCanvas.Dispose();
            MenuCanvas.Dispose();
            BaseCanvas.Dispose();
            BaseSkin.Dispose();
            RenderHandler.Dispose();
        }
    }
}
