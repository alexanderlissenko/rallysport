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
        int verticalGaussianFilterShader, horizontalGaussianFilterShader, gaussFBO, gaussFBO2, tempTex;
        Entity plane;
        public GaussianFilter(int vertShade, int horShade,int width,int height, int texture)
        {

            gaussFBO = GL.GenFramebuffer();
            gaussFBO2 = GL.GenFramebuffer();
            verticalGaussianFilterShader = vertShade;
            horizontalGaussianFilterShader = horShade;
            plane = new Entity("plane");

            tempTex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tempTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, tempTex, 0);
        
        }

        public void gaussianBlurr(int texture, int width, int height, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {

            #region Make temp texture and bind to FrameBuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO2);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
                    #endregion

                    
                    #region Horizontal
                    GL.UseProgram(horizontalGaussianFilterShader);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, gaussFBO);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texture);

                    GL.Uniform1(GL.GetUniformLocation(horizontalGaussianFilterShader, "diffuseTex"), 0);

                    //GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.directionalLight(horizontalGaussianFilterShader, projectionMatrix, viewMatrix, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    #endregion


                    #region Vertical
                    
                    GL.UseProgram(verticalGaussianFilterShader);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Viewport(0, 0, width, height);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f); //ambient light
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, tempTex);

                    GL.Uniform1(GL.GetUniformLocation(verticalGaussianFilterShader, "diffuseTex"), 0);

                    //GL.UniformMatrix4(GL.GetUniformLocation(verticalGaussianFilterShader, "projectionMatrix"), false, ref projectionMatrix);
                    plane.directionalLight(verticalGaussianFilterShader,projectionMatrix, viewMatrix, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                    #endregion
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }



    }
}
