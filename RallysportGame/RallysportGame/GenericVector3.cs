using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using BEPUutilities;

namespace RallysportGame
{
    class GenericVector3
    {
        private BEPUutilities.Vector3 bepuVector;
        private OpenTK.Vector3 tkVector;

        public GenericVector3(float posX, float posY, float posZ)
        {
            bepuVector = new BEPUutilities.Vector3(posX, posY, posZ);
            tkVector = new OpenTK.Vector3(posX, posY, posZ);
        }

        public BEPUutilities.Vector3 getBepuVector()
        {
            return bepuVector;
        }

        public OpenTK.Vector3 getOpenTKVector()
        {
            return tkVector;
        }
        public void setVector(float posX, float posY, float posZ)
        {
            bepuVector = new BEPUutilities.Vector3(posX, posY, posZ);
            tkVector = new OpenTK.Vector3(posX, posY, posZ);

        }
    }
}