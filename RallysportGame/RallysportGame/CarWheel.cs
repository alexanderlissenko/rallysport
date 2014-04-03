using BEPUphysics.Vehicle;
using BEPUphysics.Constraints.TwoEntity.Motors;
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
        private float scaling_factor = 1f;


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
            modelMatrix = Matrix4.Identity;
            Matrix4 translation = Matrix4.Identity;
            translation *= Matrix4.CreateTranslation(this.position);
            //translation *= Matrix4.CreateTranslation(new OpenTK.Vector3(-5f, -12.5f, 0f)); //Magic nuuumbeeers!
            //Matrix4 rotation = Matrix4.CreateRotationX(-OpenTK.MathHelper.Pi / 2);
            //modelMatrix *= translation;
            //modelMatrix *= Matrix4.CreateScale(scaling_factor);
            //modelMatrix *= rotation;
            OpenTK.Vector3.TransformPosition(position, modelMatrix);
            
            WheelShape shape = new CylinderCastWheelShape(1, 1, BEPUutilities.Quaternion.CreateFromRotationMatrix(Matrix3.CreateRotationZ(OpenTK.MathHelper.Pi/2)), Utilities.ConvertToBEPU(modelMatrix), false);
            
            WheelSuspension suspension = new WheelSuspension(1, 1, new BEPUutilities.Vector3(0, 0.1f, 0), 1, position);
            WheelDrivingMotor motor = new WheelDrivingMotor(0.5f, 50f, 20f);
            WheelBrake rollingFriction = new WheelBrake(0.5f, 0.5f, 0.5f);
            WheelSlidingFriction slidingFriction = new WheelSlidingFriction(0.8f, 0.8f);
            wheel = new Wheel(shape, suspension, motor, rollingFriction, slidingFriction);
            //modelMatrix = shape.LocalGraphicTransform;
        }

        public override void Update()
        {
            
            modelMatrix = wheel.Shape.WorldTransform;
            //modelMatrix *= Matrix4.CreateTranslation(car.vehicle.Body.LinearVelocity);
            //base.Update();
        }

    }
}
