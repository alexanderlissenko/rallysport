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


namespace RallysportGame
{
    class Trigger
    {
        private bool triggerHappend;
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

        void Events_PairRemoved(EntityCollidable sender, BroadPhaseEntry other)
        {
            triggerHappend = false;
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
