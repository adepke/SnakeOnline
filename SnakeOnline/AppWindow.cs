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
        private SessionType GameType;

        private Gwen.Renderer.OpenTK RenderHandler;
        private Gwen.Skin.Base BaseSkin;
        private Gwen.Control.Canvas BaseCanvas;

        private Gwen.Control.TextBox LocalSizeBox;

        private int Rows;
        private int Columns;

        private int RowWidth = 25;
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
            BaseSkin = new Gwen.Skin.TexturedBase(RenderHandler, "DefaultSkin.png");

            BaseSkin.DefaultFont = new Gwen.Font(RenderHandler, "Arial", 10);

            BaseCanvas = new Gwen.Control.Canvas(BaseSkin);

            BaseCanvas.SetSize(Width, Height);
            BaseCanvas.ShouldDrawBackground = true;
            BaseCanvas.BackgroundColor = Color.White;

            return true;
        }

        public void SessionInterface(out SessionType RequestedSessionType, out System.Net.IPEndPoint RequestedEndPoint)
        {
            RequestedSessionType = SessionType.Singleplayer;

            RequestedEndPoint = new IPEndPoint(IPAddress.Parse("67.166.8.31"), 735);
        }

        public void SetupLocal(World LocalWorldInst, Snake LocalSnakeInst)
        {
            this.LocalWorldInst = LocalWorldInst;
            this.LocalSnakeInst = LocalSnakeInst;

            LocalSizeBox = new Gwen.Control.TextBox(BaseCanvas);
            LocalSizeBox.TextColor = Color.Black;
            LocalSizeBox.SetPosition(0, Rows * RowHeight);
        }

        // @todo: Add RemoteSnakeInst
        public void SetupRemote(World RemoteWorldInst)
        {
            this.RemoteWorldInst = RemoteWorldInst;
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
                    GL.Vertex2(Column * 25, Row * 25);

                    // Bottom Right
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex2(Column * 25 + 25, Row * 25);

                    // Top Right
                    GL.TexCoord2(1f, 0f);
                    GL.Vertex2(Column * 25 + 25, Row * 25 + 25);

                    // Top Left
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex2(Column * 25, Row * 25 + 25);

                    GL.End();
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);

            //GL.PopMatrix();
        }

        public void DrawRemoteUI()
        {

        }

        public void DrawRemoteGame()
        {

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

            DrawLocalUI();

            BaseCanvas.RenderCanvas();

            DrawLocalGame();

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
