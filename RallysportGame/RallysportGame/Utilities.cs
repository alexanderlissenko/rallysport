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

        //help method for convering a meshomatic meshData object to a Bepu vector array
        public static BEPUutilities.Vector3[] meshToVectorArray(Meshomatic.MeshData mesh)
        {
            BEPUutilities.Vector3[] vectorArray = new BEPUutilities.Vector3[mesh.Vertices.Length];

            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                vectorArray[i] = Utilities.ConvertToBepu(mesh.Vertices[i]);
            }
            return vectorArray;
        }
    }

}
