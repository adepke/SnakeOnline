using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SnakeOnline
{
    class AppWindow : GameWindow
    {
        private World WorldInst;

        private int GridCellTexture;
        private int SnakeCellTexture;
        private int ItemCellTexture;

        public bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

            GridCellTexture = LoadTexture2("../Assets/GridCell.png");
            SnakeCellTexture = LoadTexture2("../Assets/SnakeCell.png");
            ItemCellTexture = LoadTexture2("../Assets/ItemCell.png");

            if (GridCellTexture <= 0 || SnakeCellTexture <= 0 || ItemCellTexture <= 0)
            {
                return false;
            }

            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.Black);
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

            catch (System.IO.FileNotFoundException e)
            {
                return -1;
            }

            return Texture;
        }
        
        protected int LoadTexture2(string File)
        {
            using (Bitmap TextureBitmap = new Bitmap(File)
            {
                int Texture = GL.GenTexture();
                
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

        protected void DrawGrid(int Rows, int Columns)
        {
            for (int Row = 0; Row < Rows; ++Row)
            {
                for (int Column = 0; Column < Columns; ++Column)
                {
                    GL.Begin(PrimitiveType.Quads);
                    //GL.Color3((Row / (Rows * 2)) + (Column / (Columns * 2)), (Row / (Rows * 2)) + (Column / (Columns * 2)), (Row / (Rows * 2)) + (Column / (Columns * 2)));
                    GL.Color3(0.2 + Row / Rows, 0.2 + Row / Rows, 0.2 + Row / Rows);
                    GL.Vertex2(Column * 25, Row * 25);
                    GL.Vertex2(Column * 25 + 23.5, Row * 25);
                    GL.Vertex2(Column * 25 + 23.5, Row * 25 + 23.5);
                    GL.Vertex2(Column * 25, Row * 25 + 23.5);
                    GL.End();
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawGrid(WorldInst.GetRows(), WorldInst.GetColumns());

            GL.Flush();

            SwapBuffers();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
