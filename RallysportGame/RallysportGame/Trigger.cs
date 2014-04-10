using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.CollisionRuleManagement;

using OpenTK;


namespace RallysportGame
{
    class Trigger
    {
        private bool triggerHappend;
        Entity triggerObj;
        ConvexHull triggerHull;
        float rotation;

        public Trigger(BEPUutilities.Vector3 pos,string triggerType, Space space,StaticMesh world)
        {
            Box trigger = new Box(pos, 200, 200, 200);
            trigger.CollisionInformation.CollisionRules.Personal = BEPUphysics.CollisionRuleManagement.CollisionRule.NoSolver;
            CollisionRules.AddRule(trigger, world, CollisionRule.NoBroadPhase);
            trigger.CollisionInformation.Events.PairCreated += Events_PairCreated;
            trigger.CollisionInformation.Events.PairRemoved += Events_PairRemoved;

            trigger.Tag = triggerType;

            space.Add(trigger);
            triggerHappend = false;
        }
        public Trigger(string entityPath,BEPUutilities.Vector3 pos,string triggerType, Space space,StaticMesh world)
        {
            triggerObj = new Entity(entityPath);
            triggerHull = new ConvexHull(Utilities.meshToVectorArray(triggerObj.mesh), 0f);

            triggerHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));;

            triggerHull.CollisionInformation.CollisionRules.Personal = BEPUphysics.CollisionRuleManagement.CollisionRule.NoSolver;
            CollisionRules.AddRule(triggerHull, world, CollisionRule.NoBroadPhase);
            triggerHull.CollisionInformation.Events.PairCreated += Events_PairCreated;
            triggerHull.CollisionInformation.Events.PairRemoved += Events_PairRemoved;

            triggerHull.Tag = triggerType;

            space.Add(triggerHull);
            triggerHappend = false;

        }

        public void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (!triggerHappend)
            {
                triggerObj.modelMatrix = triggerHull.WorldTransform;
                triggerObj.firstPass(program, projectionMatrix, viewMatrix);
            }
        }


        public void update()
        {
            triggerHull.Orientation = Quaternion.FromAxisAngle(BEPUutilities.Vector3.Up, rotation / 180);
            rotation++;
        }

        void Events_PairRemoved(EntityCollidable sender, BroadPhaseEntry other)
        {
            var otherEnt = other as EntityCollidable;
            if (otherEnt.Entity.Tag.Equals("Player Car"))
            {
                triggerHappend = false;
            }
        }

        void Events_PairCreated(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            var otherEnt = other as EntityCollidable;
            if (!triggerHappend && otherEnt.Entity.Tag.Equals("Player Car"))
            {
                TriggerHandler.triggerEvent(sender.Entity.Tag.ToString(),otherEnt.Entity.Tag.ToString());
                triggerHappend = !triggerHappend;
            }
        }

    }
}
