using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meshomatic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

//for mtl reading might need to move that out later
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;



/*
 * Class representing a generic entity, basically anything loaded from
 * an .obj file that should be shaded. The render method should be basically
 * the same for any subclasses.
 * */

namespace RallysportGame
{
    class Entity
    {
        private OpenTK.Vector3 ambient, emisive;
        public OpenTK.Vector3 diffuse, specular; // test so return to private when possible
        String modelsDir = @"..\..\..\..\Models\";
        String fileName; 
        String texturePath;
        private int textureId;
        private float shininess;

        public uint vertexArrayObject;  // make private later, made public for testing
        private uint positionBuffer;
        public uint indexBuffer;
        public uint normalBuffer;
        public uint textureBuffer;
        public int numOfTri;

        protected Matrix4 modelMatrix;
        protected Matrix4 worldMatrix;
        // 3D position in world space
        public OpenTK.Vector3 position;
        public List<int> vertIndices;
        //TODO rotation variable and stoff
        
        public MeshData mesh;
        /// <summary>
        /// Constructor for Entity
        /// Make sure that the .obj file and .mtl is named the same
        /// </summary>
        /// <param name="name">Name of the file starting from the model path no .obj its added automaticly</param>
        public Entity()
        {

        }
        
        public Entity(String name)
        {
            modelMatrix = worldMatrix = Matrix4.Identity;
            fileName = name;
            this.mesh = new Meshomatic.ObjLoader().LoadFile(modelsDir+name +".obj");
            numOfTri = mesh.Tris.Length;
            makeVAO();
        }

        public Entity(String name, OpenTK.Vector3 position): this(name)
        {
            this.position = position;
        }


        public void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {

            setMatrices(program, projectionMatrix, viewMatrix);

            GL.UniformMatrix4(GL.GetUniformLocation(program, "projectionMatrix"),false,ref projectionMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "viewMatrix"), false, ref viewMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "modelMatrix"), false, ref modelMatrix);

            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numOfTri * 3);

        }
        public void secondPass(int program)
        {

            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numOfTri * 3);

        }

        /*
         *  Duh, renders the object using whatever shaders you've set up. 
         */
        public void render(int program){
            GL.Uniform3(GL.GetUniformLocation(program, "material_diffuse_color"), diffuse);
            GL.Uniform3(GL.GetUniformLocation(program, "material_specular_color"), specular);
            GL.Uniform3(GL.GetUniformLocation(program, "material_emissive_color"), emisive);
            GL.Uniform1(GL.GetUniformLocation(program, "material_shininess"), shininess);
            
            GL.BindVertexArray(vertexArrayObject);
            //GL.DrawElements(PrimitiveType.Triangles, numOfTri*3, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numOfTri * 3);
        }

        public void render(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix,OpenTK.Vector3 lightPosition,Matrix4 lightViewMatrix,Matrix4 lightProjectionMatrix)
        {

            setMatrices(program, projectionMatrix, viewMatrix);

            OpenTK.Vector3 viewSpaceLightPosition = OpenTK.Vector3.Transform(lightPosition, viewMatrix);
            GL.Uniform3(GL.GetUniformLocation(program, "viewSpaceLightPosition"), viewSpaceLightPosition);


            GL.Uniform3(GL.GetUniformLocation(program, "material_diffuse_color"), diffuse);
            GL.Uniform3(GL.GetUniformLocation(program, "material_specular_color"), specular);
            GL.Uniform3(GL.GetUniformLocation(program, "material_emissive_color"), emisive);
            GL.Uniform1(GL.GetUniformLocation(program, "material_shininess"), shininess);

            GL.BindVertexArray(vertexArrayObject);
            //GL.DrawElements(PrimitiveType.Triangles, numOfTri*3, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numOfTri * 3);
        }


        public void renderShadowMap(int program, Matrix4 lightProjectionMatrix, Matrix4 lightViewMatrix)
        {

            setMatrices(program, lightProjectionMatrix, lightViewMatrix);

            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numOfTri * 3);
        }

        private void setMatrices(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            Matrix4 modelWorldMatrix;
            // Transform from model to world
            Matrix4.Mult(ref modelMatrix, ref worldMatrix, out modelWorldMatrix);
            Matrix4 modelViewMatrix;
            Matrix4.Mult(ref modelWorldMatrix, ref viewMatrix, out modelViewMatrix);

            Matrix4 modelViewProjectionMatrix;
            Matrix4.Mult(ref modelViewMatrix, ref projectionMatrix, out modelViewProjectionMatrix);

            Matrix4 normalMatrix;
            Matrix4.Mult(ref modelMatrix, ref viewMatrix, out normalMatrix);
            normalMatrix.Transpose();
            normalMatrix.Invert();

            GL.UniformMatrix4(GL.GetUniformLocation(program, "worldMatrix"), false, ref modelWorldMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "normalMatrix"), false, ref normalMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "modelViewMatrix"), false, ref modelViewMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "modelViewProjectionMatrix"), false, ref modelViewProjectionMatrix);  
        }

        public void setUp3DSModel()
        {
            modelMatrix = modelMatrix + Matrix4.CreateScale(0.1f); //TODO magic numbers go away!
        }

        public void setUpBlenderModel()
        {
            modelMatrix = modelMatrix + Matrix4.CreateScale(10f);
        }
        public void render(int program, OpenTK.Vector3 position)
        {



        }

        /// <summary>
        /// sets the mtl to load the uniforms for the shaders
        /// </summary>
        public void setUpMtl()
        {

            FileStream stream = new FileStream(modelsDir +fileName + ".mtl", FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line;
            char[] splitChars = { ' ' };
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "Ka":
                        //ambient
                        float ar = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float ab = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float ag = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        ambient = new OpenTK.Vector3(ar, ab, ag);
                        break;
                    
                    case "Kd":
                        //diffuse
                        float dr = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float db = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float dg = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        diffuse = new OpenTK.Vector3(dr, db, dg);
                        break;

                    case "Ks":
                        //specular
                        float sr = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float sb = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float sg = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        specular = new OpenTK.Vector3(sr, sb, sg);
                        break;

                    case "Ke":
                        //emisive
                        float er = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float eb = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float eg = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        emisive = new OpenTK.Vector3(er, eb, eg);
                        break;

                    case "Ns":
                        //Shininess
                        shininess = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        break;

                    case "map_Kd":
                        texturePath = parameters[1];
                        break;
                    
                    default:
                        break;
                }
            }
        }

        public void loadTexture()
        {
            TextureTarget Target = TextureTarget.Texture2D;
            String filename = modelsDir + texturePath;

            int texture= GL.GenTexture();
            GL.BindTexture(Target, texture);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Modulate);
           
            Bitmap bitmap = new Bitmap(filename);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.Finish();
            bitmap.UnlockBits(data);
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(Target,TextureParameterName.TextureMagFilter,(int)TextureMagFilter.Linear);
            GL.TexParameter(Target,TextureParameterName.TextureMinFilter,(int)TextureMinFilter.LinearMipmapLinear);
           
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception("Error loading texture " + filename);

            textureId= texture;
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int getTextureId()
        {
            return textureId;
        }
        /*
         *  I made this a separate method to make things a bit more readable. 
         *  Only called once for initialization
         */
        unsafe private void makeVAO()
        {

            vertIndices = new List<int>();
            List<int> normIndices = new List<int>();
            List<int> texIndices = new List<int>();
            // TODO tex coords
            Meshomatic.Point[] points;
            for(int i=0; i<mesh.Tris.Length; i++){
                points = mesh.Tris[i].Points();
                foreach (Meshomatic.Point p in points)
                {
                    vertIndices.Add(p.Vertex);
                    normIndices.Add(p.Normal);
                    texIndices.Add(p.TexCoord);
                }
            }
            
            
            float[] vertices = mesh.VertexArray();
            float[] normals = mesh.NormalArray();
            float[] texCoords = mesh.TexcoordArray();
            
            //Test thing/////////////////////////////////////////////
            List<float> finalVertex = new List<float>();
            for (int i = 0; i < vertIndices.Count; i++)
            {
                //index irån normindicies ska indexera listan av normals 
                //som ska lagras till en ny lista so ska skickas till buffern
                finalVertex.Add(vertices[vertIndices[i] * 3]);
                finalVertex.Add(vertices[vertIndices[i] * 3 + 1]);
                finalVertex.Add(vertices[vertIndices[i] * 3 + 2]);
            }

            List<float> finalNormal = new List<float>();
            for(int i = 0; i < normIndices.Count;i++)
            {
                //index irån normindicies ska indexera listan av normals 
                //som ska lagras till en ny lista so ska skickas till buffern
                finalNormal.Add(normals[normIndices[i]*3]);
                finalNormal.Add(normals[normIndices[i]*3+1]); 
                finalNormal.Add(normals[normIndices[i]*3+2]);
            }

            List<float> finalTexture = new List<float>();
            for (int i = 0; i < texIndices.Count; i++)
            {
                //index irån normindicies ska indexera listan av normals 
                //som ska lagras till en ny lista so ska skickas till buffern
                finalTexture.Add(texCoords[texIndices[i]*2]);
                finalTexture.Add(texCoords[texIndices[i]*2+1]);
            }

            //////////////////////////////////////////////////////////

            //Sizes of the arrays, a bit less clutter here outside the calls
            IntPtr posSize = (IntPtr)(sizeof(float) * finalVertex.Count);//((sizeof(float))*mesh.VertexArray().Length);
            IntPtr indSize = (IntPtr) (sizeof(int)*vertIndices.Count);
            IntPtr norSize = (IntPtr) (sizeof(float)*finalNormal.Count);
            IntPtr texSize = (IntPtr)(sizeof(float) * finalTexture.Count);

            //Workaround, C# seems weird about pointers
            fixed(uint* pbp = &positionBuffer, ibp = &indexBuffer, nbp = &normalBuffer, vaop = &vertexArrayObject,txp = &textureBuffer)
            {
                //Buffer for the vertices
                GL.GenBuffers(1, pbp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *pbp);
                GL.BufferData(BufferTarget.ArrayBuffer, posSize, finalVertex.ToArray(), BufferUsageHint.StaticDraw);
                //Buffer for indices into the vertex buffer. This is how we define the faces of our triangles.
                //GL.GenBuffers(1, ibp);
                //GL.BindBuffer(BufferTarget.ArrayBuffer, *ibp);
                //GL.BufferData(BufferTarget.ArrayBuffer, indSize, vertIndices.ToArray(), BufferUsageHint.StaticDraw );
                //Buffer for the normals
                GL.GenBuffers(1, nbp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *nbp);
                GL.BufferData(BufferTarget.ArrayBuffer, norSize, finalNormal.ToArray(), BufferUsageHint.StaticDraw);
                //Texture coords
  
                GL.GenBuffers(1, txp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *txp);
                GL.BufferData(BufferTarget.ArrayBuffer, texSize, finalTexture.ToArray(), BufferUsageHint.StaticDraw);
                //Finally, our VertexArrayObject
                GL.GenVertexArrays(1, vaop);
                GL.BindVertexArray(*vaop);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *pbp);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *nbp);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *txp);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, *ibp);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
            }

           }

        }
    }

