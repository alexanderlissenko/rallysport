using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using QuickFont;

namespace RallysportGame
{
    /// <summary>
    /// Class representing the menu. This class is responisble for graphics and logic for the menu
    /// 
    /// TODO currently has some magic numbers
    /// </summary>
    public class MenuState : IState
    {
        private QFont font;
        private int texture;
        private Window window;

        //: base(resolution[0], resolution[1], GraphicsMode.Default, "Hoard of Upgrades")
        public MenuState(Window window)
        {
            this.window = window;
        }

        public int LoadTexture(string file)
        {
            Bitmap bitmap = new Bitmap(file);

            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }

        public static void DrawImage(int image, int resolutionX, int resolutionY)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(0, 800, 0, 600, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Disable(EnableCap.Lighting);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, image);

            GL.Begin(BeginMode.Quads);
            /*
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(resolutionX, 0, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(resolutionX, resolutionY, 0);

            GL.TexCoord2(0, 1);
            GL.Vertex3(0, resolutionY, 0);
            */
            GL.TexCoord2(1, 1);
            GL.Vertex3(0, 0, 0);

            GL.TexCoord2(0, 1);
            GL.Vertex3(resolutionX, 0, 0);

            GL.TexCoord2(0, 0);
            GL.Vertex3(resolutionX, resolutionY, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(0, resolutionY, 0);
            
            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Load(GameWindow gameWindow)
        {
            GL.ClearColor(0, 0.1f, 0.4f, 1);
            texture = LoadTexture(@"..\..\..\..\Models\2d\temp.jpg");
            font = new QFont("Fonts/Calibri.ttf", 72, new QFontBuilderConfiguration(true));
        }

        public void Render(GameWindow gameWindow)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawImage(texture, 800, 600);
            QFont.Begin();
            font.Print("hi everyone");
            QFont.End();
            gameWindow.SwapBuffers();
        }

        public void Update(GameWindow gameWindow)
        {
           
        }
    }
}