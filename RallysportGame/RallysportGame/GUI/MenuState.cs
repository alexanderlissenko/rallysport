using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using QuickFont;
using RallysportGame.GUI;
using OpenTK.Input;

namespace RallysportGame.GUI
{
    /// <summary>
    /// Class representing the menu. This class is responisble for graphics and logic for the menu
    /// 
    /// TODO currently has some magic numbers
    /// </summary>
    public class MenuState : IState
    {
        private String shaderDir = @"..\..\..\..\Shaders\";
        private int texture;
        private Window window;
        private TextRowMenu textMenu;
        private int shader;
        private Entity plane;

        private const int MAX_WIDTH = 500; //determines the max allowed width for the textRowMenu
        private const int TEXT_SIZE = 80;
        private const float LINE_SPACE = 1.3f;
        private const int VERTICAL_OFFSET = 0;

        public MenuState(Window window)
        {
            this.window = window;
        }

        public int LoadTexture(string file)
        {
            TextureTarget Target = TextureTarget.Texture2D;
            int texture = GL.GenTexture();
            GL.BindTexture(Target, texture);
            //GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Modulate);

            Bitmap bitmap = new Bitmap(file);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            
            bitmap.UnlockBits(data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.BindTexture(Target, 0);
            return texture;
        }

        public void Load(GameWindow gameWindow)
        {

            plane = new Entity("plane");
            shader = GameState.loadShaderProgram(shaderDir + "Menu_VS.glsl", shaderDir + "Menu_FS.glsl");
            GL.BindAttribLocation(shader, 0, "positionIn");
            GL.BindFragDataLocation(shader, 0, "diffuseOutput");
            GL.LinkProgram(shader);

            texture = LoadTexture(@"..\\..\\..\\..\\Models\\2d\\temp.jpg");//vegitatio,n_bana_berg.jpg");//

            textMenu = new TextRowMenu((SettingsParser.GetInt(Settings.WINDOW_WIDTH) / 11), VERTICAL_OFFSET, TEXT_SIZE, MAX_WIDTH, LINE_SPACE, gameWindow.Mouse); //needs to be rerun in case of resize call not sure what'll happen
            textMenu.AddTextButton("Singleplayer", test);
            textMenu.AddTextButton("Multiplayer", test);
            textMenu.AddTextButton("Options", test);
            textMenu.AddTextButton("Exit", test);
        }
        public void test()
        {
            System.Console.WriteLine("Click!");
        }

        public void Render(GameWindow gameWindow)
        {
            
            GL.ClearColor(1.0f, 0f, 0f, 0f);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
           
            GL.UseProgram(shader);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(GL.GetUniformLocation(shader, "diffuseTex"), 0);
                
            GL.BindVertexArray(plane.vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, plane.numOfTri * 3);
            GL.UseProgram(0);

            GL.End();

            textMenu.Render();

            gameWindow.SwapBuffers();
            GL.UseProgram(0);
        }

        public void Update(GameWindow gameWindow)
        {
            textMenu.Update();

            //InputHandler input = InputHandler.Instance;
            //if (input.isKeyPressed(Key.Up))
            //{
            //    textMenu.SelectUp();
            //}
        }
    }
}