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
        private float scaling_factor = 20f;

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
            AffineTransform test = new AffineTransform(new BEPUutilities.Vector3(scaling_factor, scaling_factor, scaling_factor), tempRot, new BEPUutilities.Vector3(0,5f, 0));//new AffineTransform(Matrix3x3.CreateFromAxisAngle(BEPUutilities.Vector3.Up,BEPUutilities.MathHelper.Pi),new BEPUutilities.Vector3(0, -20, 4));
            //bepu_mesh = new InstancedMesh(new BEPUphysics.CollisionShapes.InstancedMeshShape(vertices, indices),test);
            bepu_mesh = new StaticMesh(vertices, indices,test);
            base.modelMatrix = bepu_mesh.WorldTransform.Matrix;
            //Console.WriteLine("Env has id " + bepu_mesh);
        }
        #endregion

        #region Public methods
        public override void Update()
        {      
            //base.Update();
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
