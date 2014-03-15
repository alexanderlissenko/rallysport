﻿using System;
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
        public CollisionHandler()
        {
            objects = new List<DynamicEntity>();
            space = new Space(new ParallelLooper()); 
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, SettingsParser.GetFloat(Settings.GRAVITY), 0);
        }
        public void Update()
        {
            space.Update();
            foreach(DynamicEntity e in objects)
            {
                e.Update();
            }
        }

        public void addObject(DynamicEntity e)
        {
            objects.Add(e);
            space.Add(e.GetBody());

            // If e is car, add wheels as well
            Car c = e as Car;
            if (c != null)
            {
                foreach(CarWheel w in c.wheels){
                    objects.Add(w);
                    space.Add(w.GetBody());
                }
            }
        }

        

        public void setupEnvironment(Entity environment, OpenTK.Vector3 position) {
            int[] indices = environment.vertIndices.ToArray();
            Meshomatic.MeshData mesh = environment.mesh;

            BEPUutilities.Vector3[] vertices = Utilities.meshToVectorArray(mesh);

            var environmentMesh = new StaticMesh(vertices, indices, new AffineTransform(Utilities.ConvertToBepu(position)));
            space.Add(environmentMesh);
        }
    }

}