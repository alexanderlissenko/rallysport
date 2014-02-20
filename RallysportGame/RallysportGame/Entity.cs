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
        private OpenTK.Vector3 ambient, diffuse, specular, emisive; 
        String modelsDir = @"..\..\..\..\Models\";
        String fileName; 
        String texturePath;
        private int textureId;
        private float shininess;

        private uint vertexArrayObject;
        private uint positionBuffer;
        public uint indexBuffer;
        public uint normalBuffer;
        public uint textureBuffer;
        public int numOfTri;
        
        private MeshData mesh;
        /// <summary>
        /// Constructor for Entity
        /// Make sure that the .obj file and .mtl is named the same
        /// </summary>
        /// <param name="name">Name of the file starting from the model path no .obj its added automaticly</param>
        public Entity(String name)
        {
            fileName = name;
            this.mesh = new Meshomatic.ObjLoader().LoadFile(modelsDir+name +".obj");
            numOfTri = mesh.Tris.Length;
            makeVAO();
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
            GL.DrawElements(PrimitiveType.Triangles, numOfTri*3, DrawElementsType.UnsignedInt, 0);
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

            int texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(Target, texture);

            Bitmap bitmap = new Bitmap(filename);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.Finish();
            bitmap.UnlockBits(data);
            

            Version version = new Version(GL.GetString(StringName.Version).Substring(0, 3));
            Version target = new Version(1, 4);
            if (version >= target)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);
                GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            }
            else
            {
                GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception("Error loading texture " + filename);

            textureId= texture;
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
            float[] vertices = mesh.VertexArray();
            float[] normals = mesh.NormalArray();
            
            List<int> vertIndices = new List<int>();
            List<float> normIndices = new List<float>();
            // TODO tex coords
            Meshomatic.Point[] points;
            for(int i=0; i<mesh.Tris.Length; i++){
                points = mesh.Tris[i].Points();
                foreach (Meshomatic.Point p in points)
                {
                    vertIndices.Add(p.Vertex);
                }
            }
            

            //Sizes of the arrays, a bit less clutter here outside the calls
            IntPtr posSize = (IntPtr) ((sizeof(float))*mesh.VertexArray().Length);
            IntPtr indSize = (IntPtr) (sizeof(int)*vertIndices.Count);
            IntPtr norSize = (IntPtr) (sizeof(Meshomatic.Vector3)*mesh.Normals.Length);
            IntPtr texSize = (IntPtr)(sizeof(Meshomatic.Vector2) * mesh.TexCoords.Length);

            //Workaround, C# seems weird about pointers
            fixed(uint* pbp = &positionBuffer, ibp = &indexBuffer, nbp = &normalBuffer, vaop = &vertexArrayObject,txp = &textureBuffer)
            {
                //Buffer for the vertices
                GL.GenBuffers(1, pbp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *pbp);
                GL.BufferData(BufferTarget.ArrayBuffer, posSize, mesh.VertexArray(), BufferUsageHint.StaticDraw);
                //Buffer for indices into the vertex buffer. This is how we define the faces of our triangles.
                GL.GenBuffers(1, ibp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *ibp);
                GL.BufferData(BufferTarget.ArrayBuffer, indSize, vertIndices.ToArray(), BufferUsageHint.StaticDraw );
                //Buffer for the normals
                GL.GenBuffers(1, nbp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *nbp);
                GL.BufferData(BufferTarget.ArrayBuffer, norSize, mesh.NormalArray(), BufferUsageHint.StaticDraw);
                //Texture coords
  
                GL.GenBuffers(1, txp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *txp);
                GL.BufferData(BufferTarget.ArrayBuffer, texSize, mesh.TexcoordArray(), BufferUsageHint.StaticDraw);
                //Finally, our VertexArrayObject
                GL.GenVertexArrays(1, vaop);
                GL.BindVertexArray(*vaop);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *pbp);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *nbp);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, *txp);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, *ibp);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
            }

           }

        }
    }

