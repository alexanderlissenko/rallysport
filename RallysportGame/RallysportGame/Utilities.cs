using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame
{   
    //Class with helpful methods
    class Utilities
    {
        public static BEPUutilities.Vector3 ConvertToBepu(OpenTK.Vector3 v)
        {
            return new BEPUutilities.Vector3(v.X, v.Y, v.Z);
        }

        public static BEPUutilities.Vector3 ConvertToBepu(Meshomatic.Vector3 v)
        {
            return new BEPUutilities.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Vector3 ConvertToTK(BEPUutilities.Vector3 v)
        {
            return new OpenTK.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Vector3 ConvertToTK(Meshomatic.Vector3 v)
        {
            return new OpenTK.Vector3(v.X, v.Y, v.Z);
        }

        public static Meshomatic.Vector3 convertToMesho(OpenTK.Vector3 v)
        {
            return new Meshomatic.Vector3(v.X, v.Y, v.Z);
        }

        public static Meshomatic.Vector3 convertToMesho(BEPUutilities.Vector3 v)
        {
            return new Meshomatic.Vector3(v.X, v.Y, v.Z);
        }
    }

}
