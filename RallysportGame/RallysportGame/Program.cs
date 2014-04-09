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
using BEPUphysics.Vehicle;





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
        static String iniDir = @"..\..\ini\";

        //*****************************************************************************
        //	Global variables
        //*****************************************************************************
        static int basicShaderProgram, shadowShaderProgram, firstPassShader, secondPassShader, postShader, verticalGaussianFilterShader, horizontalGaussianFilterShader;
        //static Vector3 lightPosition;
        static GaussianFilter gaussBlurr;
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
        static int shadowMapRes;
        static int shadowMapTexture, shadowMapFBO;
        //

        //Deferred Rendering
        static int deferredTex, deferredNorm, deferredDepth, deferredFBO,deferredVel,deferredSSAO;
        //

        //postProcessing
        static int postFBO, postTex,FBOtest;
        //

        static float light_theta = pi / 6.0f;
        static float light_phi = pi / 4.0f;
        static float light_r = 3000.0f;

        //test particles
        static ParticleSystem testPartSys;// = new ParticleSystem(new OpenTK.Vector3(0, 0, 0), 60f, 5, new TimeSpan(0, 0, 0, 4), new Entity());
        static Entity myCar2,skybox,unitSphere;
        static Entity plane;

        static Environment environment;
        static Car playerCar;
        static Wheel w;



        static ArrayList keyList = new ArrayList();



        
        static int source = 0;
        static bool musicPaused;
        static bool keyHandled = false;
        static MouseState current;
        static MouseState previous;
        private static CollisionHandler collisionHandler;




        // Helper function to turn spherical coordinates into cartesian (x,y,z)
        static Vector3 sphericalToCartesian(float theta, float phi, float r, Vector3 pos)
        {
            return (pos + new Vector3( (float)(r * Math.Sin(theta) * Math.Sin(phi)),
                                (float)(r * Math.Cos(phi)),
                                (float)(r * Math.Cos(theta) * Math.Sin(phi))));
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
                    if(e.Key.Equals(Key.A)||e.Key.Equals(Key.D))
                    {
                        playerCar.Turn(0);
                    }
                    if (e.Key.Equals(Key.W) || e.Key.Equals(Key.S))
                    {
                        playerCar.accelerate(0);
                    }
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
                        playerCar.accelerate(1f);
                        break;
                    case Key.S:
                        playerCar.accelerate(-1f);
                        break;
                    case Key.Left:
                        camera_theta += camera_horizontal_delta;
                        break;
                    case Key.Right:
                        camera_theta -= camera_horizontal_delta;
                        break;
                    case Key.Up:
                        camera_r -= camera_vertical_delta;
                        break;
                    case Key.Down:
                        camera_r += camera_vertical_delta;
                        break;
                    case Key.Z:
                        camera_phi -= camera_horizontal_delta*0.5f;
                        break;
                    case Key.X:
                        camera_phi += camera_horizontal_delta*0.5f;
                        break;
                    case Key.L:
                        //megaParticles.stopEmit();
                        break;
                    case Key.K:
                        //megaParticles.startEmit();
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
        /// <summary>
        /// Renders a shadowmap for a given light
        /// </summary>
        /// <param name="program"> Current Shader program</param>
        /// <param name="lightViewMatrix">viewmatrix of the light</param>
        /// <param name="lightProjectionMatrix">Projectionmatrix of the light</param>
        static Matrix4 renderSM(int program,Matrix4 viewMatrix,Matrix4 lightViewMatrix, Matrix4 lightProjectionMatrix)
        {

            //ändra till 300f
            GL.UseProgram(shadowShaderProgram);
            //SHADOW MAP FBO RENDERING
            GL.PushAttrib(AttribMask.EnableBit);
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFBO);

                GL.Viewport(0, 0, shadowMapRes, shadowMapRes);
                //GL.CullFace(CullFaceMode.Front);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.PolygonOffsetFill);
                GL.PolygonOffset(1.0f, 1.0f);

                //GL.BindTexture(TextureTarget.Texture2D, shadowMapTexture);
                playerCar.renderShadowMap(shadowShaderProgram, lightProjectionMatrix, lightViewMatrix);
                //myCar2.renderShadowMap(shadowShaderProgram, lightProjectionMatrix, lightViewMatrix);
                environment.renderShadowMap(shadowShaderProgram, lightProjectionMatrix, lightViewMatrix);

            }
            GL.PopAttrib();
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.CullFace(CullFaceMode.Back);
            //GL.Disable(EnableCap.PolygonOffsetFill);

            GL.UseProgram(program);

            Matrix4 lightMatrix;
            Matrix4 invView = Matrix4.Invert(viewMatrix);

            Matrix4 lightModelView;

            //invView = Matrix4.Mult(invProj, invView);
            Matrix4.Mult(ref invView, ref lightViewMatrix, out lightModelView);
            //lightViewMatrix.Transpose();
            Matrix4.Mult(ref lightModelView, ref  lightProjectionMatrix, out lightMatrix);
            Matrix4 test = Matrix4.Mult(viewMatrix, lightMatrix);

            lightMatrix = lightMatrix * Matrix4.CreateScale(0.5f) * Matrix4.CreateTranslation(new OpenTK.Vector3(0.5f, 0.5f, 0.5f));
            return lightMatrix;
        }

        static void Main(string[] args)
        { 
          
            using (var game = new GameWindow())
            {
                #region Load
                game.Load += (sender, e) =>
                {
                
                    SettingsParser.Init(iniDir + "default.ini");
                    //enable depthtest and face culling
                    GL.Enable(EnableCap.DepthTest);
                    GL.Enable(EnableCap.CullFace);

                    Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;

                    // Dynamic objects
                    collisionHandler = new CollisionHandler();
                    environment = new Environment("map\\uggly_test_track_Triangulate");//"plane");//
                    environment.setUpMtl();
                    environment.loadTexture();
                    //environment.setUpBlenderModel(); //Handled in constructor

                    playerCar = new Car(@"Mustang\mustang-no-wheels", @"Mustang\one-wheel-tex-scale", new Vector3(300, 40, 0), collisionHandler.space);
                    skybox = new Entity("Cube\\inside_koob");
                    unitSphere = new Entity("Cube\\unitSphere");
                    myCar2 = new Entity("Cube\\inside_koob");

                    //collisionHandler.addObject(playerCar);
                    collisionHandler.addObject(environment);
                    collisionHandler.addTestTrigger();
                    plane = new Entity("plane");

                    //Particle System
                    //testPartSys = new ParticleSystem(new OpenTK.Vector3(0, 0, 0), 60f, 1, new TimeSpan(0, 0, 0, 2), unitSphere);

                    //Set up shaders

                    shadowShaderProgram = loadShaderProgram(shaderDir + "Shadow_VS.glsl", shaderDir + "Shadow_FS.glsl");
                    GL.BindAttribLocation(shadowShaderProgram, 0, "position");
                    GL.BindFragDataLocation(shadowShaderProgram, 0, "fragmentColor");
                    GL.LinkProgram(shadowShaderProgram);

                    firstPassShader = loadShaderProgram(shaderDir + "deferredShader\\firstVertexPass", shaderDir + "deferredShader\\firstFragmentpass");
                    GL.BindAttribLocation(firstPassShader, 0, "positionIn");
                    GL.BindAttribLocation(firstPassShader, 1, "normalIn");
                    GL.BindAttribLocation(firstPassShader, 2, "texCoordIn");
                    GL.BindFragDataLocation(firstPassShader, 0, "diffuseOutput");
                    GL.BindFragDataLocation(firstPassShader, 1, "normOutput");
                    GL.BindFragDataLocation(firstPassShader, 2, "velOutput");
                    GL.BindFragDataLocation(firstPassShader, 3, "ssaoOutput");
                    
                    GL.LinkProgram(firstPassShader);


                    secondPassShader = loadShaderProgram(shaderDir + "deferredShader\\secondVertexPass", shaderDir + "deferredShader\\secondFragmentPass");
                    GL.BindAttribLocation(secondPassShader, 0, "positionIn");
                    GL.BindFragDataLocation(secondPassShader, 0, "fragColor");
                    GL.LinkProgram(secondPassShader);
                    

                    verticalGaussianFilterShader = loadShaderProgram(shaderDir + "gaussianFilter\\verticalGaussianFilterVertexShader",shaderDir + "gaussianFilter\\verticalGaussianFilterFragmentShader");
                    GL.BindAttribLocation(verticalGaussianFilterShader, 0, "vertexPos");
                    GL.BindAttribLocation(verticalGaussianFilterShader, 1, "texCoordIn");

                    GL.BindFragDataLocation(verticalGaussianFilterShader, 0, "fragColor");
                    GL.LinkProgram(verticalGaussianFilterShader );

                    horizontalGaussianFilterShader = loadShaderProgram(shaderDir + "gaussianFilter\\horizontalGaussianFilterVertexShader", shaderDir + "gaussianFilter\\horizontalGaussianFilterFragmentShader");
                    GL.BindAttribLocation(horizontalGaussianFilterShader, 0, "vertexPos");
                    GL.BindAttribLocation(horizontalGaussianFilterShader, 1, "texCoordIn");

                    GL.BindFragDataLocation(horizontalGaussianFilterShader, 0, "fragColor");
                    GL.LinkProgram(horizontalGaussianFilterShader);


                    postShader = loadShaderProgram(shaderDir + "postProcessing\\postProcessing_VS.glsl", shaderDir + "postProcessing\\postProcessing_FS.glsl");
                    GL.BindAttribLocation(postShader, 0, "positionIn");
                    GL.BindFragDataLocation(postShader, 0, "fragColor");
                    GL.LinkProgram(postShader);

                    Console.WriteLine(GL.GetProgramInfoLog(shadowShaderProgram));
                    Console.WriteLine(GL.GetProgramInfoLog(firstPassShader));
                    Console.WriteLine(GL.GetProgramInfoLog(secondPassShader));
                    Console.WriteLine(GL.GetProgramInfoLog(postShader));
                    //Load uniforms and texture
                    GL.UseProgram(firstPassShader);
                    
                    //myCar2.setUpBlenderModel();
                    //playerCar.setUpMtl();
                    //playerCar.setUp3DSModel(); Don't do this here! Scaling is done in Car class

                    //skybox.setUp3DSModel();// setUpBlenderModel();
                    GL.UseProgram(0);
                    
                    //Set up Uniforms

                    plane.loadUniformLocations(secondPassShader);

                    //Shadowmaps
                    #region ShadowMap
                    shadowMapRes = 1080;
                    shadowMapTexture = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, shadowMapTexture);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, shadowMapRes, shadowMapRes, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                    
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
                    //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 0.0f);


                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)All.CompareRefToTexture);

                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    //Generate FBO

                    shadowMapFBO = GL.GenFramebuffer();
                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowMapFBO);
                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, shadowMapTexture, 0);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    
                    #endregion

                    #region postporcessing
                    
                    postFBO = GL.GenFramebuffer();

                    postTex = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, postTex);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, postFBO);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, postTex, 0);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    #endregion
                    //Deferred Rendering 

                    #region Deferred Rendering

                    deferredFBO = GL.GenFramebuffer();

                    deferredTex = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, deferredTex);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, game.Width, game.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    deferredVel = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, deferredVel);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, game.Width, game.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    deferredNorm = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, deferredNorm);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, game.Width, game.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    deferredDepth = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, deferredDepth);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,PixelInternalFormat.Depth32fStencil8 , game.Width, game.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    deferredSSAO = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, deferredSSAO);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, game.Width, game.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, deferredFBO);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, deferredTex, 0);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, deferredNorm, 0);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, deferredVel, 0);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3, TextureTarget.Texture2D, deferredSSAO, 0);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, deferredDepth, 0);


                    //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                    
                    // GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer ,0);

                    //DrawBuffersEnum[] draw_buffs = {DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1};
                    //GL.DrawBuffers(2, draw_buffs);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                    

                    
                    #endregion
                    

                    

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
                    
                    
                    
                };
                #endregion

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                    
                };

                #region Update
                game.UpdateFrame += (sender, e) =>
                {
                    camera_rotation_matrix = Matrix4.Identity;
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        GL.DeleteTextures(1, ref shadowMapTexture);
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
                    collisionHandler.Update();

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

                    light_theta += camera_horizontal_delta*0.1f;
                    //myCar2.modelMatrix *= Matrix4.CreateRotationY(0.1f);
                };
                #endregion

                #region Render
                game.RenderFrame += (sender, e) =>
                {
                   
                    GL.ClearColor(0.2f, 0.2f, 0.8f, 1.0f);
                    GL.ClearDepth(1.0f);

                    #region Let there be light
                    Vector3 lightPosition = sphericalToCartesian(light_theta, light_phi, light_r,new Vector3(0,0,0));
                    Vector3 scaleVector = new Vector3(10, 10, 10);
                    //Vector3 scaleVector = new Vector3(1000, 1000, 1000);
                    

                    #endregion
                    
                    

                    
                    int w = game.Width;
                    int h = game.Height;


                    Vector3 camera_position = sphericalToCartesian(camera_theta, camera_phi,camera_r,playerCar.getCarPos());
                    //camera_lookAt = new Vector3(0.0f, camera_target_altitude, 0.0f);
                    Vector3 camera_lookAt = playerCar.getCarPos();//new Vector3(0.0f, 0.0f, 0.0f);//Vector4.Transform(camera_lookAt, camera_rotation_matrix);
                    Matrix4 viewMatrix = Matrix4.LookAt(camera_position, camera_lookAt,up);
                    Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi / 4, (float)w / (float)h, 0.1f, 1000f);
                    // Here we start getting into the lighting model
                    
                    //Matrix4 bias = new Matrix4(0.9f, 0.0f, 0.0f, 0.0f, 0.0f, 0.9f, 0.0f, 0.0f, 0.0f, 0.0f, 0.9f, 0.0f, 0.9f, 0.9f, 0.9f, 1.0f);
                    

                    //Render Shadowmap

                    #region shadowMapRender
                    Matrix4 lightViewMatrix = Matrix4.LookAt(lightPosition, new Vector3(0.0f, 0.0f, 0.0f), up);
                    Matrix4 lightProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi / 4, 1.0f, 2180f,3580f);

                    Matrix4 lightMatrix = renderSM(shadowShaderProgram,viewMatrix, lightViewMatrix, lightProjectionMatrix);

                    #endregion

                    ///END OF SHADOWMAP FBO RENDERING
                    
                    
                    
                    #region firstPass
                    GL.UseProgram(firstPassShader);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer ,deferredFBO);
                    GL.Viewport(0, 0, w, h);
                    DrawBuffersEnum[] draw_buffs = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2,DrawBuffersEnum.ColorAttachment3 };
                    GL.DrawBuffers(4, draw_buffs);
                    GL.ClearColor(1.0f, 0f, 0f, 0.1f);
                    GL.ClearDepth(1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.DepthMask(true);
                    GL.Enable(EnableCap.DepthTest);
                    GL.Disable(EnableCap.Blend);
                    DrawBuffersEnum[] draw_buffs2 = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 };
                    GL.DrawBuffers(4, draw_buffs2);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, environment.getTextureId());
                    GL.Uniform1(GL.GetUniformLocation(firstPassShader, "firstTexture"), 0);
                    
                    //megaParticles.firstPass(firstPassShader, projectionMatrix, viewMatrix);
                    environment.firstPass(firstPassShader,  projectionMatrix,  viewMatrix);
                    

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    //myCar2.firstPass(firstPassShader, projectionMatrix, viewMatrix);
                    playerCar.firstPass(firstPassShader, projectionMatrix, viewMatrix);
                    //skybox.firstPass(firstPassShader, projectionMatrix, viewMatrix);

                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    #endregion
                    
                    Matrix4 invProj = Matrix4.Invert(projectionMatrix);
                    
                    #region secondPass
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, postFBO);
                    GL.UseProgram(secondPassShader);

                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, w, h);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Enable(EnableCap.Blend);
                    
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);


                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, deferredTex);
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, deferredNorm);
                    GL.ActiveTexture(TextureUnit.Texture2);
                    GL.BindTexture(TextureTarget.Texture2D, deferredDepth);
                    GL.ActiveTexture(TextureUnit.Texture3);
                    GL.BindTexture(TextureTarget.Texture2D, deferredVel);
                    GL.ActiveTexture(TextureUnit.Texture4);
                    GL.BindTexture(TextureTarget.Texture2D, shadowMapTexture);
                    

                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "diffuseTex"), 0);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "normalTex"), 1);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "depthTex"), 2);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "velTex"), 3);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "shadowMapTex"), 4);

                    Vector2 size = new Vector2(game.Width,game.Height);
                    GL.Uniform2(GL.GetUniformLocation(secondPassShader, "screenSize"), ref size);
                    

                    GL.UniformMatrix4(GL.GetUniformLocation(secondPassShader, "lightMatrix"),false, ref lightMatrix);
                    int lTUniform = GL.GetUniformLocation(secondPassShader, "lightType");
                    
                    //Directional Light
                    GL.Uniform1(lTUniform, 0.0f);
                    plane.directionalLight(secondPassShader, invProj,viewMatrix, lightPosition, camera_position);
                    
                    //Point Lights
                    GL.Uniform1(lTUniform, 1.0f);


                    //Render Car Lights
                    playerCar.renderBackLight(secondPassShader, plane);


                    //for (int i = 0; i < 100; i++ )
                        //plane.pointLight(secondPassShader, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1, 0, 0), 10.0f);
                        //plane.pointLight(secondPassShader, new Vector3(-15.0f, 10.0f, 0), new Vector3(0, 1, 0), 10.0f);
                        //plane.pointLight(secondPassShader, new Vector3(0, 10.0f, 10.0f), new Vector3(0, 0, 1), 10.0f);
                        //plane.pointLight(secondPassShader, new Vector3(0,10.0f,-10.0f), new Vector3(1, 1, 0), 10.0f);

                        //Spot Light
                    /*

                    lightViewMatrix = Matrix4.LookAt(new Vector3(0, 10, 0), new Vector3(0.0f, -40.0f, 0.0f), up);
                    lightProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(pi / 4, 1.0f, 1f, 1000f);
                    lightMatrix = renderSM(secondPassShader, viewMatrix, lightViewMatrix, lightProjectionMatrix);
                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, postFBO);
                    GL.Viewport(0, 0, w, h);
                    
                    GL.UseProgram(secondPassShader);
                    GL.Uniform1(lTUniform, 2.0f);

                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "diffuseTex"), 0);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "normalTex"), 1);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "depthTex"), 2);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "velTex"), 3);
                    GL.Uniform1(GL.GetUniformLocation(secondPassShader, "shadowMapTex"), 4);

                    GL.Uniform2(GL.GetUniformLocation(secondPassShader, "screenSize"), ref size);


                    GL.UniformMatrix4(GL.GetUniformLocation(secondPassShader, "lightMatrix"), false, ref lightMatrix);
                    GL.Uniform1(lTUniform, 2.0f);
                    */
                    //plane.spotLight(secondPassShader, new Vector3(0, 10, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0), 400.0f, (float)Math.Cos(pi / 4), camera_position, invProj,viewMatrix);
                    //plane.spotLight(secondPassShader, new Vector3(0, 3, 0), new Vector3(1, -1, 0), new Vector3(1, 0, 0), 15.0f, (float)Math.Cos(pi / 4));
                    //plane.spotLight(secondPassShader, new Vector3(0, 3, 0), new Vector3(0, -1, -1), new Vector3(0, 1, 0), 15.0f, (float)Math.Cos(pi / 4));
                    //plane.spotLight(secondPassShader, new Vector3(0, 3, 0), new Vector3(0, -1, 1), new Vector3(0, 1, 0), 15.0f, (float)Math.Cos(pi / 4));
                    

                    //GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    //plane.directionalLight(secondPassShader,projectionMatrix, viewMatrix, lightPosition, camera_position);
                    
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    GL.Disable(EnableCap.Blend);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    #endregion

                    //gaussBlurr.gaussianBlurr(postTex, game.Width, game.Height, projectionMatrix, viewMatrix);
                   
                    #region PostProcessing pass
                    
                    GL.UseProgram(postShader);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, w, h);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);


                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, postTex);
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, deferredVel);
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, deferredVel);
                    GL.Uniform1(GL.GetUniformLocation(postShader, "postTex"), 0);
                    GL.Uniform1(GL.GetUniformLocation(postShader, "postVel"), 1);
                    GL.Uniform1(GL.GetUniformLocation(postShader, "postDepth"), 1);
                    GL.Uniform1(GL.GetUniformLocation(postShader, "velScale"), (float)game.RenderFrequency/30.0f);

                    GL.BindVertexArray(plane.vertexArrayObject);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, plane.numOfTri * 3);
                    
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    
                    #endregion
                    
                    
                    //testPartSys.tick();
                    //testPartSys.render();
                    
                    GL.End();

                    game.SwapBuffers();
                    GL.UseProgram(0);

                };
                #endregion
                // Run the game at 60 updates per second
                game.Run(60.0,60);
            }
        }
    }



}
