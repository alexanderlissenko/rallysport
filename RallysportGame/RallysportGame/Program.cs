using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;



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


                    updateCamera();
                    
                    /*else if(game.Keyboard[Key.A]){
                        camera_theta -= camera_horizontal_delta;
                    }
                    else if (game.Keyboard[Key.D])
                    {
                        camera_theta += camera_horizontal_delta;
                    }
                    else if (game.Keyboard[Key.W])
                    {
                        camera_r -= camera_vertical_delta;
                    }
                    else if (game.Keyboard[Key.S])
                    {
                        camera_r += camera_vertical_delta;
                    }*/
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
