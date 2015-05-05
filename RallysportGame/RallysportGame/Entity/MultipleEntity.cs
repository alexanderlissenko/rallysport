using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RallysportGame
{
    class MultipleEntity : Entity
    {
        List<Matrix4> modelMatrices;

        public MultipleEntity(String name) : base(name)
        {
            addModelMatrix(modelMatrix);
        }

        public void addModelMatrix(Matrix4 modelMatrix)
        {
            modelMatrices.Add(modelMatrix);
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            foreach (Matrix4 m in modelMatrices) 
            {
                base.modelMatrix = m;
                base.firstPass(program, projectionMatrix, viewMatrix);
            }
        }
    }
}
