using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
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
            modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateScale(scaling_factor);

            int[] indices = vertIndices.ToArray();
            BEPUutilities.Vector3[] vertices = Utilities.meshToVectorArray(mesh);

            bepu_mesh = new StaticMesh(vertices, indices, new AffineTransform(new BEPUutilities.Vector3(scaling_factor, scaling_factor, scaling_factor)));
         
        }
        #endregion

        #region Public methods
        public override void Update()
        {      
            //base.Update();
        }
        #endregion


    }
}
