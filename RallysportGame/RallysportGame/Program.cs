using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        static String shaderDir = @"..\..\..\..\Shaders\";

        //*****************************************************************************
        //	Global variables
        //*****************************************************************************
        static int basicShaderProgram;
        //static Vector3 lightPosition;

        //*****************************************************************************
        //	Camera state variables
        //*****************************************************************************
        static float camera_theta = pi / 6.0f;
        static float camera_phi = pi / 4.0f;
        static float camera_r = 300.0f;
        static float camera_target_altitude = 5.2f;
        static float camera_horizontal_delta = 0.1f;
        static float camera_vertical_delta = 1.0f;
        //static Vector4 camera_lookAt = new Vector4(0.0f, camera_target_altitude, 0.0f, 1.0f);
        static Matrix4 camera_rotation_matrix = Matrix4.Identity;
        
        //ShadowMap constants
        static int shadowMapRes = 1024;
        static int shadowMapTexture, shadowMapFBO;
        //

        static float light_theta = pi / 6.0f;
        static float light_phi = pi / 4.0f;
        static float light_r = 600.0f;

<<<<<<< HEAD
        static Entity myCar,myCar2;

        static Car playerCar;

=======
        static Entity myCar;
        static Car myCar2;
>>>>>>> origin/collision


        static ArrayList keyList = new ArrayList();

        static int source = 0;
        static bool musicPaused;
        static bool keyHandled = false;
<<<<<<< HEAD
        static MouseState current;
        static MouseState previous;
=======
        private static CollisionHandler collisionHandler;
>>>>>>> origin/collision

        // Helper function to turn spherical coordinates into cartesian (x,y,z)
        static Vector3 sphericalToCartesian(float theta, float phi, float r)
        {
            return new Vector3( (float)(r * Math.Sin(theta) * Math.Sin(phi)),
                                (float)(r * Math.Cos(phi)),
                                (float)(r * Math.Cos(theta) * Math.Sin(phi)));
        }

        static int loadShaderProgram(String vShaderPath, String fShaderPath)
        {
            int shaderProgram;
            int vShader = GL.CreateShader(ShaderType.VertexShader);
            int fShader = GL.CreateShader(ShaderType.FragmentShader);
            using (StreamReader vertReader = new StreamReader(vShaderPath), 
                                fragReader = new StreamReader(fShaderPath))
            {
                GL.ShaderSource(vShader, vertReader.ReadToEnd());
                GL.ShaderSource(fShader, fragReader.ReadToEnd());
            }
            GL.CompileShader(vShader);
            Console.WriteLine("Vertex Shader: "+GL.GetShaderInfoLog(vShader));
            GL.CompileShader(fShader);
            Console.WriteLine("Fragment Shader: "+GL.GetShaderInfoLog(fShader));

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vShader);
            GL.AttachShader(shaderProgram, fShader);
            GL.DeleteShader(vShader);
            GL.DeleteShader(fShader);
            ErrorCode error = GL.GetError();
            if (error != 0)
                Console.WriteLine(error);
            return shaderProgram;
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
                        playerCar.Turn(pi / 32);
                        break;
                    case Key.D:
                        playerCar.Turn(-pi / 32);
                        break;
                    case Key.W:
                        playerCar.accelerate(0.1f);
                        break;
                    case Key.S:
                        playerCar.accelerate(-0.1f);
                        break;
                    case Key.Z:
                        camera_phi -= camera_horizontal_delta*0.5f;
                        break;
                    case Key.X:
                        camera_phi += camera_horizontal_delta*0.5f;
                        break;
                    default:
                        break;
                }
            }
        }

        // Move camera with mouse
        static void UpdateMouse()
        {
            current = Mouse.GetState();
            if (current != previous)
            {
                // Mouse state has changed
                int xdelta = current.X - previous.X;
                int ydelta = current.Y - previous.Y;
                int zdelta = current.Wheel - previous.Wheel;

                camera_theta += xdelta > 0 ? 0.01f : -0.01f;
                camera_phi += ydelta > 0 ? 0.01f : -0.01f;
                //camera_r += zdelta > 0 ? 1 : -1;

            }
            previous = current;
        }


        static void Main(string[] args)
        { 
          
            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                    myCar = new Entity("map\\uggly_test_track_Triangulate");//"TeapotCar\\Teapot car\\Teapot-no-materials-tri");//"Cube\\3ds-cube");//
<<<<<<< HEAD
                    myCar2 = new Entity("Cube\\testCube");//"Cube\\megu_koob");//"TeapotCar\\Teapot car\\Teapot-no-materials-tri");//
                    playerCar = new Car("TeapotCar\\Teapot car\\Teapot-no-materials-tri");
                    playerCar.setUp3DSModel();
                    myCar.setUpBlenderModel();
=======

                    myCar2 = new Car(new Vector3(0,4,0));
                    // myCar2 = new Entity("Cube\\testCube");//"Cube\\megu_koob");//"TeapotCar\\Teapot car\\Teapot-no-materials-tri");//
>>>>>>> origin/collision
                    //Set up shaders
                    basicShaderProgram = loadShaderProgram(shaderDir+"Simple_VS.glsl",shaderDir+"Simple_FS.glsl");
                    GL.BindAttribLocation(basicShaderProgram, 0, "position");
                    GL.BindAttribLocation(basicShaderProgram, 1, "normalIn");
                    GL.BindAttribLocation(basicShaderProgram, 2, "textCoordIn");
                    GL.BindFragDataLocation(basicShaderProgram, 0, "fragmentColor");
                    GL.LinkProgram(basicShaderProgram);

                    Console.WriteLine(GL.GetProgramInfoLog(basicShaderProgram));

                    //Load uniforms and texture
                    GL.UseProgram(basicShaderProgram);
                    myCar.setUpMtl();
                    myCar.loadTexture();
                    myCar.setUpBlenderModel();
                    myCar2.setUpBlenderModel();
                    GL.UseProgram(0);
                    
                    //Set up Uniforms
                    
                    
                    //Shadowmaps
                    shadowMapRes = 1024;
                    shadowMapTexture = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, shadowMapTexture);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, shadowMapRes, shadowMapRes, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)All.CompareRefToTexture);

                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    //Generate FBO

                    shadowMapFBO = GL.GenFramebuffer();
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFBO);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, shadowMapTexture, 0);
                    
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    
                    

                    //lightPosition = new Vector3(up);
           
                    game.KeyDown += handleKeyDown;
                    game.KeyUp += handleKeyUp;


                    //Music
                    source = Audio.initSound();


                    //enable depthtest and face culling
                    GL.Enable(EnableCap.DepthTest);
                    GL.Enable(EnableCap.CullFace);
                    //GL.DepthMask(true);
                    //GL.DepthFunc(DepthFunction.Lequal);
                    //GL.DepthRange(0.0f, 5.0f);
                    collisionHandler = new CollisionHandler();
                    collisionHandler.addCar(myCar2);
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
                        Audio.deleteBS(source);
                        game.Exit();
                    }
                    else if (game.Keyboard[Key.Number9])
                    {
                        if (!keyHandled)
                        {
                            Audio.increaseGain(source);
                            keyHandled = !keyHandled;
                        }
                    }
                    else if (game.Keyboard[Key.Number0])
                    {
                        if (!keyHandled)
                        {
                            Audio.decreaseGain(source);
                            keyHandled = !keyHandled;
                        }
                    }
                    else if (game.Keyboard[Key.Space])
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
                    else if (game.Keyboard[Key.O])
                    {
                        if (!keyHandled)
                        {
                            source = Audio.nextTrack(source);
                            keyHandled = !keyHandled;
                        }
                    }
                    collisionHandler.update();

                    updateCamera();
                    UpdateMouse();
                    playerCar.Update();
                    //////////////////////////////////////////////////////ÄNDRA TILLBAKA!!!
                    //Audio management
                    if (Audio.audioStatus(source) == 1)
                        Audio.playSound(source);
                    else if (Audio.audioStatus(source) == 3)
                        source = Audio.nextTrack(source);

                    //move light

                    //light_phi += camera_horizontal_delta*0.1f;
                    
                };

                game.RenderFrame += (sender, e) =>
                {   
                    GL.ClearColor(0.2f, 0.2f, 0.8f, 1.0f);
                    GL.ClearDepth(1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    
                    
                    GL.UseProgram(basicShaderProgram);
                    Vector3 lightPosition = new Vector3(sphericalToCartesian(light_theta, light_phi, light_r));

                    
                    //Render Shadowmap
                    Matrix4 lightViewMatrix = Matrix4.LookAt(lightPosition, new Vector3(0.0f, 0.0f, 0.0f), up);
                    //Matrix4 lightProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi / 4, 1.0f, 520f, 850f);
                    Matrix4 lightProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi / 4, 1.0f, 0.1f, 1000f);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFBO);
                    GL.Viewport(0, 0, shadowMapRes, shadowMapRes);

                    myCar2.render(basicShaderProgram, lightProjectionMatrix, lightViewMatrix,lightPosition,lightViewMatrix,lightProjectionMatrix);
                    myCar.render(basicShaderProgram, lightProjectionMatrix, lightViewMatrix,lightPosition,lightViewMatrix,lightProjectionMatrix);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    
                    //


                    GL.ClearDepth(1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    
                    int w = game.Width;
                    int h = game.Height;

                    GL.Viewport(0, 0, w, h);
                    //GL.UseProgram(basicShaderProgram);

                    Vector3 camera_position = sphericalToCartesian(camera_theta, camera_phi, camera_r);
                    //camera_lookAt = new Vector3(0.0f, camera_target_altitude, 0.0f);
                    Vector3 camera_lookAt = new Vector3(0.0f, 0.0f, 0.0f);//Vector4.Transform(camera_lookAt, camera_rotation_matrix);
                    Matrix4 viewMatrix = Matrix4.LookAt(camera_position, camera_lookAt,up);
                    Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi/4, (float)w/(float)h, 0.1f, 1000f);
                    // Here we start getting into the lighting model
                    
                    
                    
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, myCar.getTextureId());
                    GL.Uniform1(GL.GetUniformLocation(basicShaderProgram, "diffuse_texture"), 0);

                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, shadowMapTexture);
                    GL.Uniform1(GL.GetUniformLocation(basicShaderProgram, "shadowMapTex"), 1);
                    
                    myCar.render(basicShaderProgram, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    
                    myCar2.render(basicShaderProgram, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);

                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, 0);

                   



                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, playerCar.getTextureId());
                    GL.Uniform1(GL.GetUniformLocation(basicShaderProgram, "diffuse_texture"), 0);

                    playerCar.render(basicShaderProgram, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);



                    GL.End();



                    game.SwapBuffers();
                    GL.UseProgram(0);

                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
