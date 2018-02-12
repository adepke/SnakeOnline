using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SnakeOnline
{
    class AppWindow : GameWindow
    {
        private World WorldInst;

        public bool Initialize(World WorldInst)
        {
            this.WorldInst = WorldInst;

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

        protected void DrawGrid(int Rows, int Columns)
        {
            for (int Row = 0; Row < Rows; ++Row)
            {
                for (int Column = 0; Column < Columns; ++Column)
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(1f, (Row / (Rows * 2)) + (Column / (Columns * 2)), (Row / (Rows * 2)) + (Column / (Columns * 2)));
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
