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
        public Car car;

        public CarWheel(String path)
            : this(path, OpenTK.Vector3.Zero)
        {
            
        }

        public CarWheel(String path, OpenTK.Vector3 pos)
            : base(path, pos)
        {
            // All of these values will have to be tweaked later
            Matrix4 translation = Matrix4.CreateTranslation(this.position);
            modelMatrix *= translation;
            modelMatrix *= Matrix4.CreateRotationX(OpenTK.MathHelper.Pi / 2);
            WheelShape shape = new CylinderCastWheelShape(1, 1, BEPUutilities.Quaternion.Identity, Utilities.ConvertToBEPU(translation), false);
            WheelSuspension suspension = new WheelSuspension(1, 1, new BEPUutilities.Vector3(0, -1, 0), 1, position);
            WheelDrivingMotor motor = new WheelDrivingMotor(0.5f, 50f, 20f);
            WheelBrake rollingFriction = new WheelBrake(0.5f, 0.5f, 0.5f);
            WheelSlidingFriction slidingFriction = new WheelSlidingFriction(0.8f, 0.8f);
            wheel = new Wheel(shape, suspension, motor, rollingFriction, slidingFriction);
            
        }

        public override void Update()
        {
            modelMatrix *= Matrix4.CreateTranslation(car.vehicle.Body.LinearVelocity);
            base.Update();
        }

        public override void eventTest(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            Console.WriteLine("wheel collided");
        }
    }
}
