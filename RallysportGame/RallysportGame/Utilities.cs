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
        public static BEPUutilities.Matrix ConvertToBEPU(OpenTK.Matrix4 open)
        {
            BEPUutilities.Matrix bepu;
            bepu.M11 = open.M11;
            bepu.M12 = open.M12;
            bepu.M13 = open.M13;
            bepu.M14 = open.M14;
            bepu.M21 = open.M21;
            bepu.M22 = open.M22;
            bepu.M23 = open.M23;
            bepu.M24 = open.M24;
            bepu.M31 = open.M31;
            bepu.M32 = open.M32;
            bepu.M33 = open.M33;
            bepu.M34 = open.M34;
            bepu.M41 = open.M41;
            bepu.M42 = open.M42;
            bepu.M43 = open.M43;
            bepu.M44 = open.M44;

            return bepu;
        }

        public static OpenTK.Matrix4 ConvertToTK(BEPUutilities.Matrix bepu)
        {
            BEPUutilities.Matrix open;
            open.M11 = bepu.M11;
            open.M12 = bepu.M12;
            open.M13 = bepu.M13;
            open.M14 = bepu.M14;
            open.M21 = bepu.M21;
            open.M22 = bepu.M22;
            open.M23 = bepu.M23;
            open.M24 = bepu.M24;
            open.M31 = bepu.M31;
            open.M32 = bepu.M32;
            open.M33 = bepu.M33;
            open.M34 = bepu.M34;
            open.M41 = bepu.M41;
            open.M42 = bepu.M42;
            open.M43 = bepu.M43;
            open.M44 = bepu.M44;

            return open;
        }
    }

}
