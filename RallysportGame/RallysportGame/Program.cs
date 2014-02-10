using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using System.Drawing;
using OpenTK.Input;
using System.IO;



namespace RallysportGame
{


    class Program
    {
        //*****************************************************************************
        //	Useful constants
        //*****************************************************************************
        const float pi = MathHelper.Pi;
        static Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);


        //*****************************************************************************
        //	Camera state variables
        //*****************************************************************************
        static float camera_theta = pi / 6.0f;
        static float camera_phi = pi / 4.0f;
        static float camera_r = 30.0f;
        static float camera_target_altitude = 5.2f;
        static float camera_horizontal_delta = 0.1f;
        static float camera_vertical_delta = 0.1f;

        static ArrayList keyList = new ArrayList();

        static int source = 0;
        static bool musicPaused;
        static bool keyHandled = false;

        // Helper function to turn spherical coordinates into cartesian (x,y,z)
        static Vector3 sphericalToCartesian(float theta, float phi, float r)
        {
            return new Vector3( (float)(r * Math.Sin(theta) * Math.Sin(phi)),
                                (float)(r * Math.Cos(phi)),
                                (float)(r * Math.Cos(theta) * Math.Sin(phi)));
        }

        /// <summary>
        /// Will handle key events so multiple keys can be triggered at once
        /// 
        /// alla loopar kan säkert optimeras och borde kanske ses över detta e mest som ett snabb test 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        static void handleKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if(!keyList.Contains(e.Key)) /// FULHACK tydligen så kan den annars generera 30+ keydown events om man håller inne
                keyList.Add(e.Key);
        }
        static void handleKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            keyHandled = false;
            for(int i = 0; i < keyList.Count;i++)  
            {
                if(keyList[i].Equals(e.Key))
                {
                    keyList.RemoveAt(i);
                }
            }
        }

        static void updateCamera()
        {
            foreach(Key key in keyList)
            {
                switch(key)
                {
                    case Key.A:
                        camera_theta -= camera_horizontal_delta;
                        break;
                    case Key.D:
                        camera_theta += camera_horizontal_delta;
                        break;
                    case Key.W:
                        camera_r -= camera_vertical_delta;
                        break;
                    case Key.S:
                        camera_r += camera_vertical_delta;
                        break;
                    default:
                        break;
                }
            }
        }


        void compileShader(int shader, string source)
        {

            String text = "";
            text = File.ReadAllText(source);
            GL.ShaderSource(shader, text);
            GL.CompileShader(shader);
        }


        static void Main(string[] args)
        {

            using (var game = new GameWindow())
            {

                
                game.Load += (sender, e) =>
                {

                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                    game.KeyDown += handleKeyDown;
                    game.KeyUp += handleKeyUp;

                    try
                    {
                        AudioContext AC = new AudioContext();
                    }
                    catch (AudioException ex)
                    { // problem with Device or Context, cannot continue
                        game.Exit();
                    }

                    //Music
                    int[] bs = Audio.generateBS();
                    source = bs[1];
                    Audio.loadSound(bs[0], bs[1]);
                    Audio.playSound(source);
                    musicPaused = false;
                    
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    else if(game.Keyboard[Key.Number9])
                    {
                        if (!keyHandled)
                        {
                            Audio.increaseGain(source);
                            keyHandled = !keyHandled;
                        }
                    }
                    else if(game.Keyboard[Key.Number0])
                    {
                        if (!keyHandled)
                        {
                            Audio.decreaseGain(source);
                            keyHandled = !keyHandled;
                        }
                    }
                    else if(game.Keyboard[Key.Space])
                    {
                        if (!keyHandled)
                        {
                            if (musicPaused)
                            {
                                Audio.playSound(source);
                                musicPaused = !musicPaused;
                                keyHandled = !keyHandled;
                            }
                            else
                            {
                                Audio.pauseSound(source);
                                musicPaused = !musicPaused;
                                keyHandled = !keyHandled;
                            }
                        }
                    }
                    
                    updateCamera();

                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


                    Vector3 camera_position = sphericalToCartesian(camera_theta, camera_phi, camera_r);
                    Vector3 camera_lookAt = new Vector3(0.0f, camera_target_altitude, 0.0f);
                    Matrix4 viewMatrix = Matrix4.LookAt(camera_position, camera_lookAt, up);
                    Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi/4, game.Width/game.Height, 0.1f, 1000f);

                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadMatrix(ref viewMatrix);

                    /*            C# SUCKS, can't pass strings
                    // creates a shader object.
                    int shaderProgram = GL.CreateProgram();
                    // creates shader objects.
                    int vert = GL.CreateShader(ShaderType.VertexShader);
                    int frag = GL.CreateShader(ShaderType.FragmentShader);
                    string source = "\vertexShader.txt";
                    compileShader(vert,source);
                    string source = "\fragmentShader.txt";
                    compileShader(frag, source);                    
                    */


                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref projectionMatrix);


                    GL.Begin(PrimitiveType.Triangles);

                    GL.Color3(Color.MidnightBlue);
                    GL.Vertex3(0.0f, 3.0f, 0.0f);
                    GL.Color3(Color.SpringGreen);
                    GL.Vertex3(2.0f, 0.0f, 0.0f);
                    GL.Color3(Color.Ivory);
                    GL.Vertex3(-2.0f, 0.0f, 0.0f);

                    GL.End();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
