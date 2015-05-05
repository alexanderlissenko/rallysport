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
        public Space space;
        private List<DynamicEntity> objects;
        private Environment world;

        Trigger powerUp;

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
                world = en;
            }
            else
            {
                space.Add(e.GetBody());
                Console.WriteLine("Added " + e + " to space! as ID " + space.Entities[space.Entities.Count - 1].InstanceId);
            }
        }
    }
}
