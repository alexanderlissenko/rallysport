using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.CollisionRuleManagement;


namespace RallysportGame
{
    class Missile 
    {
        
        /**
        * Referens till vilken bil som skjuter
        * (Missil-id)
        * Velocity (dubbel mot car, samma riktning?)
         * Position (car pos)
        **/
        private int TTL;
        public bool launched;
        private Entity triggerObj;
        ConvexHull triggerHull;
        public Missile(String name, Vector3 pos, Space space)
        {
            triggerObj = new Entity(name);
            triggerHull = new ConvexHull(Utilities.meshToVectorArray(triggerObj.mesh), 500f);
            triggerHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));
            
            triggerHull.IsAffectedByGravity = false;
            

            triggerHull.Tag = "Missile";
            space.Add(triggerHull);
        }
        // sets the missile to the specified location and speed aswell as setting the time it will live provided no collitions.
        public void launch(Vector3 start,Vector3 initialVel,int timeToLive){
            System.Console.WriteLine("Lanch Missile!!");
            launched = true;
            TTL = timeToLive;
            triggerHull.Position = start; // might desync if missile not hit anything this is whay // william
            //triggerHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(start));
            triggerHull.LinearVelocity = initialVel;
            
        }
       
        public void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            triggerObj.modelMatrix = triggerHull.WorldTransform;
            triggerObj.firstPass(program, projectionMatrix, viewMatrix);
        }



        public bool update() {

            triggerHull.LinearVelocity = Vector3.Add(triggerHull.LinearVelocity, Vector3.Mult(triggerHull.LinearVelocity, 0.1f)); // accelerate

            if (TTL-- <= 0) //decriment and compare
            { 
                launched = false;
                return true;
            }

            return false;
        }
        
    }
}
