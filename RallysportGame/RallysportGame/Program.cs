using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        //	Global variables
        //*****************************************************************************
        static int basicShaderProgram;
        static Vector3 lightPosition;

        //*****************************************************************************
        //	Camera state variables
        //*****************************************************************************
        static float camera_theta = pi / 6.0f;
        static float camera_phi = pi / 4.0f;
        static float camera_r = 30.0f;
        static float camera_target_altitude = 5.2f;
        static float camera_horizontal_delta = 0.1f;
        static float camera_vertical_delta = 0.1f;
        static Vector4 camera_lookAt = new Vector4(0.0f, camera_target_altitude, 0.0f, 1.0f);
        static Matrix4 camera_rotation_matrix = Matrix4.Identity;

        static Entity myDog;



        // Helper function to turn spherical coordinates into cartesian (x,y,z)
        static Vector3 sphericalToCartesian(float theta, float phi, float r)
        {
            return new Vector3( (float)(r * Math.Sin(theta) * Math.Sin(phi)),
                                (float)(r * Math.Cos(phi)),
                                (float)(r * Math.Cos(theta) * Math.Sin(phi)));
        }

        static void loadShaderProgram(int shaderProgram, String vShaderPath, String fShaderPath)
        {
            int vShader = GL.CreateShader(ShaderType.VertexShader);
            int fShader = GL.CreateShader(ShaderType.FragmentShader);
            using (StreamReader vertReader = new StreamReader(vShaderPath), 
                                fragReader = new StreamReader(fShaderPath))
            {
                GL.ShaderSource(vShader, vertReader.ReadToEnd());
                GL.ShaderSource(fShader, fragReader.ReadToEnd());
            }
            GL.CompileShader(vShader);
            GL.CompileShader(fShader);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vShader);
            GL.DeleteShader(vShader);
            GL.AttachShader(shaderProgram, fShader);
            GL.DeleteShader(fShader);
            ErrorCode error = GL.GetError();
            if (error != 0)
                Console.WriteLine(error);
        }
        

        static void Main(string[] args)
        {
            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                    myDog = new Entity(new Meshomatic.ObjLoader().LoadFile("dog.obj"));

                    //Set up shaders
                    loadShaderProgram(basicShaderProgram, "vertexShader.vert", "fragmentShader.frag");
                    GL.BindAttribLocation(basicShaderProgram, 0, "position");
                    GL.BindFragDataLocation(basicShaderProgram, 0, "fragmentColor");
                    GL.LinkProgram(basicShaderProgram);

                    lightPosition = new Vector3(up);
                    
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    camera_rotation_matrix = Matrix4.Identity;
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }else if(game.Keyboard[Key.A]){
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
                    }
                    else if (game.Keyboard[Key.Up])
                    {
                        Matrix4.CreateRotationX(0.1f, out camera_rotation_matrix);
                    }
                    else if (game.Keyboard[Key.Down])
                    {
                        Matrix4.CreateRotationX(-0.1f, out camera_rotation_matrix);
                    }
                    else if (game.Keyboard[Key.Left])
                    {
                        Matrix4.CreateRotationY(0.1f, out camera_rotation_matrix);
                    }
                    else if (game.Keyboard[Key.Right])
                    {
                        Matrix4.CreateRotationY(-0.1f, out camera_rotation_matrix);
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    GL.ClearColor(0.2f, 0.2f, 0.8f, 1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    int w = game.Width;
                    int h = game.Height;

                    GL.UseProgram(basicShaderProgram);

                    Vector3 camera_position = sphericalToCartesian(camera_theta, camera_phi, camera_r);
                    //camera_lookAt = new Vector3(0.0f, camera_target_altitude, 0.0f);
                    camera_lookAt = Vector4.Transform(camera_lookAt, camera_rotation_matrix);
                    Matrix4 viewMatrix = Matrix4.LookAt(camera_position, new Vector3(camera_lookAt),up);
                    Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi/4, w/h, 0.1f, 1000f);
                    // Here we start getting into the lighting model
                    //GL.ProgramUniformMatrix3


                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadMatrix(ref viewMatrix);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref projectionMatrix);

                    myDog.render();
                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
