using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meshomatic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

/*
 * Class representing a generic entity, basically anything loaded from
 * an .obj file that should be shaded. The render method should be basically
 * the same for any subclasses.
 * */

namespace RallysportGame
{
    class Entity
    {
        private uint vertexArrayObject;
        private uint positionBuffer;
        public uint indexBuffer;
        public uint normalBuffer;
        public int numOfTri;
        
        private MeshData mesh;

        public Entity(MeshData mesh)
        {
            this.mesh = mesh;
            numOfTri = mesh.Tris.Length;
            makeVAO();
        }

        public Entity()
        {

        }

        /*
         *  Duh, renders the object using whatever shaders you've set up. 
         */
        public void render(){
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, numOfTri, DrawElementsType.UnsignedInt, 0);
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
            Point[] points;
            for(int i=0; i<mesh.Tris.Length; i++){
                points = mesh.Tris[i].Points();
                foreach(Point p in points){
                    vertIndices.Add(p.Vertex);
                }
            }

            //Sizes of the arrays, a bit less clutter here outside the calls
            IntPtr posSize = (IntPtr) ((sizeof(float))*mesh.VertexArray().Length);
            IntPtr indSize = (IntPtr) (sizeof(int)*vertIndices.Count);
            IntPtr norSize = (IntPtr) (sizeof(Meshomatic.Vector3)*mesh.Normals.Length);

            //Workaround, C# seems weird about pointers
            fixed(uint* pbp = &positionBuffer, ibp = &indexBuffer, nbp = &normalBuffer, vaop = &vertexArrayObject){
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
            //Finally, our VertexArrayObject
            GL.GenVertexArrays(1, vaop);
            GL.BindVertexArray(*vaop);
            GL.BindBuffer(BufferTarget.ArrayBuffer, *pbp);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, *nbp);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, *ibp);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            }

           }

        }
    }

