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
using BEPUphysics.Threading;

namespace RallysportGame
{
    class CollisionHandler
    {
        private Space space;
        private List<DynamicEntity> objects;
        // For derpy testing
        public static StaticMesh plane;
        public static StaticMesh environment;
        public CollisionHandler()
        {
            objects = new List<DynamicEntity>();
            space = new Space(new ParallelLooper());
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, SettingsParser.GetFloat(Settings.GRAVITY), 0);
        }
        public void Update()
        {
            space.Update();
            
            foreach (DynamicEntity e in objects)
            {
                e.Update();
            }
        }

        public void addObject(DynamicEntity e)
        {
            
            objects.Add(e);
            Car c = e as Car;
            Environment en = e as Environment;
            if (c != null)
            {
                c.AddToSpace(space);
                Console.WriteLine("Car " + e + " added to space! as ID " + space.Entities[space.Entities.Count - 1].InstanceId);
            }
            else if (en != null)
            {
                en.addToSpace(space);
                Console.WriteLine("Environment added to space");
            }
            else
            {
                space.Add(e.GetBody());
                Console.WriteLine("Added " + e + " to space! as ID " + space.Entities[space.Entities.Count - 1].InstanceId);
            }
        }



        public void setupEnvironment(Entity environment, OpenTK.Vector3 position)
        {
            int[] indices = environment.vertIndices.ToArray();
            Meshomatic.MeshData mesh = environment.mesh;

            BEPUutilities.Vector3[] vertices = Utilities.meshToVectorArray(mesh);

            var environmentMesh = new StaticMesh(vertices, indices, new AffineTransform(Utilities.ConvertToBepu(position)));
            CollisionHandler.environment = environmentMesh;
            space.Add(environmentMesh);
        }

        public void setupPlane(Entity plane, OpenTK.Vector3 position)
        {
            int[] indices = plane.vertIndices.ToArray();
            Meshomatic.MeshData mesh = plane.mesh;

            BEPUutilities.Vector3[] vertices = Utilities.meshToVectorArray(mesh);
            
            foreach(OpenTK.Vector3 v in vertices){
                Console.WriteLine(v + position);
            }
            



            var planeMesh = new StaticMesh(vertices, indices, new AffineTransform(Utilities.ConvertToBepu(position)));
            CollisionHandler.plane = planeMesh;
            //space.Add(planeMesh);

        }
    }
}
