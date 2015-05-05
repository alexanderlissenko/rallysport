using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
using BEPUphysics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame
{
    class Environment : DynamicEntity
    {
        private float scaling_factor = 200f;

        public readonly StaticMesh bepu_mesh;

        #region Constructors
        public Environment(String path)
            :base(path)
        {
            // Set up model matrix
            //modelMatrix = Matrix4.Identity;
            //modelMatrix *= Matrix4.CreateScale(scaling_factor);
            
            int[] indices = vertIndices.ToArray();
            BEPUutilities.Vector3[] vertices = Utilities.meshToVectorArray(mesh);
            BEPUutilities.Quaternion tempRot = BEPUutilities.Quaternion.Identity; //BEPUutilities.Quaternion.CreateFromRotationMatrix(Matrix4.CreateRotationX(-3.14f / 2));
            AffineTransform test = new AffineTransform(new BEPUutilities.Vector3(scaling_factor, scaling_factor, scaling_factor), tempRot, new BEPUutilities.Vector3(0,0f, 0));//new AffineTransform(Matrix3x3.CreateFromAxisAngle(BEPUutilities.Vector3.Up,BEPUutilities.MathHelper.Pi),new BEPUutilities.Vector3(0, -20, 4));
            //bepu_mesh = new InstancedMesh(new BEPUphysics.CollisionShapes.InstancedMeshShape(vertices, indices),test);
            bepu_mesh = new StaticMesh(vertices, indices,test);
            base.modelMatrix = bepu_mesh.WorldTransform.Matrix;
            //Console.WriteLine("Env has id " + bepu_mesh);
            bepu_mesh.Tag = "Environment";
        }
        #endregion

        #region Public methods
        public override void Update()
        {      
            //base.Update();
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.firstPass(program, projectionMatrix, viewMatrix);
            /*
            base.setMatrices(program, projectionMatrix, viewMatrix);

            OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(program, "projectionMatrix"), false, ref projectionMatrix);
            OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(program, "viewMatrix"), false, ref viewMatrix);
            OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(program, "modelMatrix"), false, ref modelMatrix);

            OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.LinesAdjacency);
            foreach (BEPUutilities.Vector3 v in bepu_mesh.Shape.TriangleMeshData.Vertices)
                OpenTK.Graphics.OpenGL.GL.Vertex3(Utilities.ConvertToTK(v));
            OpenTK.Graphics.OpenGL.GL.End();
            */
        }
        public void addToSpace(Space s)
        {
            s.Add(bepu_mesh);
        }
        #endregion

        #region Private Methods
        protected override void PositionUpdated(BEPUphysics.Entities.Entity obj)
        {
            base.PositionUpdated(obj);
            //Console.WriteLine("Car position: " + position);
        }
        #endregion
    }
}
