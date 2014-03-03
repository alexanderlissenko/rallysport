using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using OpenTK;
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;

namespace RallysportGame
{
    class CollisionHandler
    {
        private Space space;
        private List<DynamicEntity> objects;
        public CollisionHandler()
        {
            objects = new List<DynamicEntity>();
            space = new Space(); 
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, SettingsParser.GetFloat(Settings.GRAVITY), 0);
        }
        public void Update()
        {
            space.Update();
            foreach(DynamicEntity e in objects)
            {
                //System.Console.WriteLine(e.position); //TODO temove this line: this is a temporary physics tester

                e.Update();
            }
            
        }

        public void addObject(DynamicEntity e)
        {
            objects.Add(e);
            space.Add(e.GetBody());
        }

        //help method for convering a meshomatic meshData object to a Bepu vector array
        private BEPUutilities.Vector3[] meshToVectorArray(Meshomatic.MeshData mesh)
        {
            BEPUutilities.Vector3[] vectorArray = new BEPUutilities.Vector3[mesh.Vertices.Length];

            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                vectorArray[i] = Utilities.ConvertToBepu(mesh.Vertices[i]);
            }
            return vectorArray;
        }

        public void setupEnvironment(Entity environment, OpenTK.Vector3 position) {
            int[] indices = environment.vertIndices.ToArray();
            Meshomatic.MeshData mesh = environment.mesh;

            BEPUutilities.Vector3[] vertices = meshToVectorArray(mesh);

            var environmentMesh = new StaticMesh(vertices, indices, new AffineTransform(Utilities.ConvertToBepu(position)));
            space.Add(environmentMesh);
        }
    }

}
