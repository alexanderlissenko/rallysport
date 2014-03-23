using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;

namespace RallysportGame
{
    class ImageProsessing
    {
        
        
        
        
     }

    class GaussianFilter{
        int verticalGaussianFilterShader, horizontalGaussianFilterShader, gaussFBO, gaussFBO2;
        Entity plane;
        public GaussianFilter(int vertShade, int horShade)
        {

            gaussFBO = GL.GenFramebuffer();
            gaussFBO2 = GL.GenFramebuffer();
            verticalGaussianFilterShader = vertShade;
            horizontalGaussianFilterShader = horShade;
            plane = new Entity("plane");

        
        }

        public void gaussianBlurr(int texture, int width, int height, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
                    #region Make temp texture and bind to FrameBuffer
                    int tempTex = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, tempTex);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO2);
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gaussFBO2);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);

                            

                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO);
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, gaussFBO);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, tempTex, 0);
                            


                    #endregion

                    
                    #region Horizontal

                    GL.UseProgram(horizontalGaussianFilterShader);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Enable(EnableCap.Blend);

                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);


                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texture);

                    GL.Uniform1(GL.GetUniformLocation(horizontalGaussianFilterShader, "diffuseTex"), 0);

                    GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.secondPass(horizontalGaussianFilterShader, viewMatrix, new Vector3(0,0,0), new Vector3(0,0,0));

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    GL.Disable(EnableCap.Blend);





            /*
                    GL.UseProgram(horizontalGaussianFilterShader);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Enable(EnableCap.Blend);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, tempTex);
          
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                    
                    
                    GL.Uniform1(GL.GetUniformLocation(horizontalGaussianFilterShader, "diffuseTex"), 1);
                    GL.Uniform1(GL.GetUniformLocation(horizontalGaussianFilterShader, "rt_w"), width);
                    GL.Uniform1(GL.GetUniformLocation(horizontalGaussianFilterShader, "rt_h"), height);
                    
                    GL.UniformMatrix4(GL.GetUniformLocation(horizontalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.secondPass(horizontalGaussianFilterShader,viewMatrix , new Vector3(0,0,0), new Vector3(0,0,0));
            */
                    #endregion


                  
                    #region Vertical
                    
                    GL.UseProgram(verticalGaussianFilterShader);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO2);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Enable(EnableCap.Blend);

                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);


                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, tempTex);

                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "diffuseTex"), 0);

                    GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.secondPass(verticalGaussianFilterShader, viewMatrix, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    GL.Disable(EnableCap.Blend);



            /*
                    GL.UseProgram(verticalGaussianFilterShader);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(1.0f, 0f, 0f, 0.1f);
                    GL.ClearDepth(1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.DepthMask(true);
                    GL.Enable(EnableCap.DepthTest);
                    GL.Disable(EnableCap.Blend);


                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, tempTex);

                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "diffuseTex"), 1);
                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "rt_w"), width);
                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "rt_h"), height);



                    plane.secondPass(verticalGaussianFilterShader, viewMatrix, new Vector3(0, 0, 0), new Vector3(0, 0, 0));


                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            */


/*            
                    GL.UseProgram(verticalGaussianFilterShader);
                    
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Enable(EnableCap.Blend);
                    
                    
                    
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                    
                    
                    
                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "diffuseTex"), 0);
                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "rt_w"), width);
                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "rt_h"), height);
                    
                    GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.secondPass(verticalGaussianFilterShader,viewMatrix,new Vector3(0,0,0),new Vector3(0,0,0));
                    
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    GL.Disable(EnableCap.Blend);
                    */
                    #endregion
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }



    }
}
