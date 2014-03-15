using BEPUphysics.Vehicle;
using BEPUutilities;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame
{
    /// <summary>
    /// Wraps the BEPU wheel class with our rendering code
    /// </summary>
    class CarWheel : DynamicEntity
    {
        public Wheel wheel;

        public CarWheel(String path)
            : this(path, OpenTK.Vector3.Zero)
        {
            
        }

        public CarWheel(String path, OpenTK.Vector3 pos)
            : base(path, pos)
        {
            wheel = new Wheel(new CylinderCastWheelShape(1, 1, BEPUutilities.Quaternion.Identity, Utilities.ConvertToBEPU(modelMatrix), false));
        }

        public override void Update()
        {
            modelMatrix += Utilities.ConvertToTK(wheel.Shape.WorldTransform);
            base.Update();
        }

        public override void eventTest(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            Console.WriteLine("Sent by wheel");
            //base.eventTest(sender, other, pair, contact);
            wheel.Shape.CollisionRules.Specific.ToString();
        }
    }
}
