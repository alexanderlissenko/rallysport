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
using System.Collections.Generic;

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
        private TextRowMenu currentTextMenu;
        private TextRowMenu mainMenu;
        private SettingsMenu settingsMenu;

        private int shader;
        private Entity plane;

        private const int MAX_WIDTH = 600; //determines the max allowed width for the textRowMenu
        private const int TEXT_SIZE = 70; //80
        private const float LINE_SPACE = 1.3f;
        private const int VERTICAL_OFFSET = 0;
        private KeyboardDevice keyBoard;

        public MenuState()
        {

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

        public override void Load(GameWindow gameWindow)
        {
            keyBoard = gameWindow.Keyboard;
            plane = new Entity("plane");
            shader = GameState.loadShaderProgram(shaderDir + "Menu_VS.glsl", shaderDir + "Menu_FS.glsl");
            GL.BindAttribLocation(shader, 0, "positionIn");
            GL.BindFragDataLocation(shader, 0, "diffuseOutput");
            GL.LinkProgram(shader);
            
            texture = LoadTexture(@"..\\..\\..\\..\\Models\\2d\\temp.jpg");//vegitatio,n_bana_berg.jpg");//
            #region mainMenu
            QFont.ForceViewportRefresh();
            mainMenu = new TextRowMenu((SettingsParser.GetInt(Settings.WINDOW_WIDTH) / 11), VERTICAL_OFFSET, TEXT_SIZE, MAX_WIDTH, LINE_SPACE, gameWindow.Mouse); //needs to be rerun in case of resize call not sure what'll happen

            Action startGame = delegate { StateHandler.Instance.changeStateToGame(); };
            mainMenu.AddTextButton("Singleplayer", startGame);

            mainMenu.AddTextButton("Multiplayer", test);

            mainMenu.AddTextButton("Options", swapToSettings);

            Action exitAction = delegate { gameWindow.Exit(); };

            mainMenu.AddTextButton("Exit", exitAction);

            currentTextMenu = mainMenu;
            #endregion
            settingsMenu = new SettingsMenu(gameWindow, swapToMainMenu);
        }

        private void swapToMainMenu()
        {
            currentTextMenu = mainMenu;
        }
        private void swapToSettings()
        {
            settingsMenu.prepareEntryToSettings();
            currentTextMenu = settingsMenu.toTextMenu();
        }

        private void test()
        {
            System.Console.WriteLine("Click!");
        }

        public override void Render(GameWindow gameWindow)
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
            GL.Disable(EnableCap.DepthTest);
            currentTextMenu.Render();
            GL.Enable(EnableCap.DepthTest);

            gameWindow.SwapBuffers();
            GL.UseProgram(0);
        }

        public override void Update(GameWindow gameWindow)
        {
            currentTextMenu.Update();
            //
        }

        bool awaitingDownKey = true;
        public override void HandleKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key.Equals(Key.Down) && awaitingDownKey)
            {
                currentTextMenu.SelectDown();
                awaitingDownKey=false;
            }
            else if (e.Key.Equals(Key.Up) && awaitingDownKey)
            {
                currentTextMenu.SelectUp();
                awaitingDownKey = false;
            }
            else if (e.Key.Equals(Key.Enter) && awaitingDownKey) {
                currentTextMenu.ClickSelected();
                awaitingDownKey = false;
            }
        }
        public override void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button.Equals(MouseButton.Left))
            {
                currentTextMenu.ClickSelected();
            }
        }

        public override void HandleKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            //if (e.Key.Equals(Key.Down) || e.Key.Equals(Key.Up))
            //{
                awaitingDownKey = true;
            //}
        }
    }
}