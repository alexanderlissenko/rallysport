using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using OpenTK;
using BEPUphysics.BroadPhaseEntries;

namespace RallysportGame
{
    class CollisionHandler
    {
        private Box car;
        private Space space;
        private List<DynamicEntity> objects;

        public CollisionHandler()
        {
            objects = new List<DynamicEntity>();
            space = new Space();
           // Box ground = new Box(Vector3.Zero, 30, 1, 30); //temporary invisible floor
            //space.Add(ground);
            //car = new Box(new Vector3(0, 4, 0), 1, 1, 1, 1);
            foreach(DynamicEntity e in objects){
                space.Add(e.GetBody());
            }
            
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
        }
        public void Update()
        {
            space.Update();
            foreach(DynamicEntity e in objects)
            {
                System.Console.WriteLine(e.position); //TODO temove this line: this is a temporary physics tester

                e.Update();
            }
            
        }
        public void addObject(DynamicEntity e)
        {
            objects.Add(e);
        }
        public void setupEnvironment(Vector3[] vertices, int[] indices, Vector3 position) {
            //var mesh = new StaticMesh(vertices, indices, new AffineTransform(new Vector3(0, -40, 0)));

        }
    }

}
