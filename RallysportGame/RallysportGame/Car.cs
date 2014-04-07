

﻿using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using BEPUphysics.Vehicle;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.CollisionTests;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.JointLimits;

namespace RallysportGame
{
    /// <summary>
    /// Represents a car. Mainly a wrapper for bepu's Vehicle class.
    /// </summary>
    class Car : DynamicEntity
    {
        #region Constants
        
        // Drag coefficient (we use a simplified version of drag: D = 1/2 * V^2 * C where C is the coefficient)
        private const float drag_coefficient = 0.7f;
        private float scaling_factor = 0.5f;
        #endregion

        #region Instance Variables
        // The simulation representation of this car
        public ConvexHull carHull;
        public Vehicle vehicle;
        // The wheels
        public List<Entity> wheelents;
        public List<BEPUphysics.Entities.Entity> wheels;
        // The angle between the direction and forward vectors
        private float turning_angle;
        // Friction coefficient, material-dependent
        private float friction_coefficient = 0.8f;
        // Do the wheels have contact with the ground?
        private bool ground_contact = true;
        int counter = 0;
        private float speed=0;

        private Space space;
        private BEPUphysics.Entities.Entity wheel1;

        private readonly RevoluteMotor drivingMotor1,backMotor1;
        private readonly RevoluteMotor drivingMotor2,backMotor2;
        private readonly RevoluteMotor steeringMotor1;
        private readonly RevoluteMotor steeringMotor2;

        private float maximumTurnAngle = BEPUutilities.MathHelper.Pi * 0.2f;
        private BEPUutilities.Vector3 testDir;
        #endregion

        #region Constructors
        // Most rudimentary constructor
        public Car(String name)
            : this(name, new Vector3(0,0,0))
        {
        }
        // Initial starting position
        public Car(String name, Vector3 pos)
            : base(name, pos)
        {
            //Replace generic body with specific car body
            ConvexHull ch = new ConvexHull(new List<BEPUutilities.Vector3>(Utilities.meshToVectorArray(mesh))); //Use with wrapped body
            vehicle = new Vehicle(ch);
            foreach(BEPUutilities.Vector3 v in ch.Vertices){
                Console.WriteLine(v.ToString());
            }
        }
        
        public Car(String bodyPath, String wheelPath, Vector3 pos,Space space) //Defacto  constructor!
            : base(bodyPath)
        {
            this.space = space;
            //position = pos;
            BEPUutilities.Matrix temp = BEPUutilities.Matrix.Identity;
            carHull = new ConvexHull(new List<BEPUutilities.Vector3>(Utilities.meshToVectorArray(mesh)),10); //Use with wrapped body?
            //carHull.WorldTransform *= BEPUutilities.Matrix.CreateScale(scaling_factor, scaling_factor, scaling_factor);
            carHull.CollisionInformation.LocalPosition = carHull.Position;
            
            //vehicle = new Vehicle(carHull);
            //body = vehicle.Body;
            // Make sure we use the same transforms for both physics geometry and graphics!
            
            // Scaling
            //modelMatrix *= Matrix4.CreateScale(scaling_factor);
            // Rotation
            //Matrix4.LookAt(Vector3.Zero, direction, up);
            //body.Orientation = new Quaternion(Utilities.ConvertToBepu(direction), 1);
            //carHull.WorldTransform *= BEPUutilities.Matrix.CreateScale(scaling_factor, scaling_factor, scaling_factor);
            // Translation

            carHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));
            modelMatrix = carHull.WorldTransform;
            this.space.Add(carHull);
            // Add wheels
            /*
            vänster fram: xyz = -19.5, 61, 12.5.
            höger fram: xyz = 35.5, 61, 12.5.
            vänster bak: xyz = -19.5, -34.5, 12.5
            höger bak: xyz = 35.5, -34.5, 12.5
             */
            var backwheel1ent = new Entity(wheelPath);
            var backwheel2ent = new Entity(wheelPath);
            var backwheel1 = addBackWheel(new Vector3(28.5f, 12.5f, 35f), carHull,out backMotor1,backwheel1ent);//y = -15.5
            var backwheel2 = addBackWheel(new Vector3(-30.5f, 12.5f, 35f), carHull,out backMotor2, backwheel2ent);//y = -30 ger 50% hjul
            
            var wheel1ent = new Entity(wheelPath);
            wheel1 = addDriveWheel( new Vector3(-30.5f, 12.5f, -60f),carHull,out drivingMotor1,out steeringMotor1,wheel1ent);
            
            var wheel2ent = new Entity(wheelPath);
            var wheel2 = addDriveWheel(new Vector3(28.5f, 12.5f, -60f), carHull, out drivingMotor2, out steeringMotor2, wheel2ent);//x 28.5
            
            var steeringStabilizer = new RevoluteAngularJoint(wheel1, wheel2, BEPUutilities.Vector3.Right);
            this.space.Add(steeringStabilizer);
            wheels = new List<BEPUphysics.Entities.Entity>();
            wheels.Add(wheel1);
            wheels.Add(wheel2);
            wheels.Add(backwheel1);
            wheels.Add(backwheel2);

            wheelents = new List<Entity>();
            wheelents.Add(wheel1ent);
            wheelents.Add(wheel2ent);
            wheelents.Add(backwheel1ent);
            wheelents.Add(backwheel2ent);
            /*
            wheels.Add(new CarWheel(wheelPath, new Vector3(-30.5f,-10.5f,38f)));//new Vector3(-19.5f, 61f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(-30.5f, -10.5f, -57f)));//new Vector3(35.5f, 61f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(28.5f, -10.5f, 38f)));//new Vector3(-19.5f, -34.5f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(28.5f, -10.5f, -57f)));//new Vector3(35.5f, -34.5f, 12.5f)));
            // ...
            foreach (CarWheel w in wheels)
            {
                //w.setUp3DSModel();
                vehicle.AddWheel(w.wheel);
                w.car = this;
                CollisionRules.AddRule(w.wheel.Shape, vehicle.Body, CollisionRule.NoBroadPhase);//.NoNarrowPhasePair);
                CollisionRules.AddRule(vehicle.Body, w.wheel.Shape, CollisionRule.NoBroadPhase);//NoNarrowPhasePair);

            }*/
            carHull.PositionUpdated += new Action<BEPUphysics.Entities.Entity>(PositionUpdated);
            carHull.CollisionInformation.Events.ContactCreated += new ContactCreatedEventHandler<EntityCollidable>(ContactCreated);
            carHull.CollisionInformation.Events.PairTouched += new PairTouchedEventHandler<EntityCollidable>(PairTouched);
            carHull.CollisionInformation.Events.CollisionEnded += new CollisionEndedEventHandler<EntityCollidable>(CollisionEnded);
            foreach (BEPUphysics.Entities.Entity w in wheels)
            {
                w.CollisionInformation.Events.ContactCreated += new ContactCreatedEventHandler<EntityCollidable>(ContactCreated);
                w.CollisionInformation.Events.PairTouched += new PairTouchedEventHandler<EntityCollidable>(PairTouched);
                w.PositionUpdated += new Action<BEPUphysics.Entities.Entity>(PositionUpdated);
            }
            Console.WriteLine("car has id " + carHull.InstanceId);
        }

        #endregion
        #region Public Methods

        public override void render(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 lightPosition, Matrix4 lightViewMatrix, Matrix4 lightProjectionMatrix)
        {
            base.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
            foreach (Entity w in wheelents)
            {
                w.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
            }
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.modelMatrix = carHull.WorldTransform;
            base.firstPass(program, projectionMatrix, viewMatrix);
            
            foreach (Entity w in wheelents)
            {
                w.firstPass(program, projectionMatrix, viewMatrix);
            }
        }

        public override void Update()
        {
            //position = Utilities.ConvertToTK(body.Position);
            base.Update();
            for (int i = 0; i < wheelents.Count; i++)
            {
                wheelents[i].modelMatrix = wheels[i].WorldTransform;
            }
        }

       

        public void accelerate(float rate)
        {
            if (rate > 0)
            {
                drivingMotor1.Settings.VelocityMotor.GoalVelocity = 100;
                drivingMotor2.Settings.VelocityMotor.GoalVelocity = 100;
                backMotor1.Settings.VelocityMotor.GoalVelocity = 100;
                backMotor2.Settings.VelocityMotor.GoalVelocity = 100;

                drivingMotor1.IsActive = true;
                drivingMotor2.IsActive = true;
                backMotor1.IsActive = true;
                backMotor2.IsActive = true;
            }
            else if (rate < 0)
            {
                drivingMotor1.Settings.VelocityMotor.GoalVelocity = -100;
                drivingMotor2.Settings.VelocityMotor.GoalVelocity = -100;
                backMotor1.Settings.VelocityMotor.GoalVelocity = -100;
                backMotor2.Settings.VelocityMotor.GoalVelocity = -100;

                drivingMotor1.IsActive = true;
                drivingMotor2.IsActive = true;
                backMotor1.IsActive = true;
                backMotor2.IsActive = true;
            }
            else
            {
                drivingMotor1.IsActive = false;
                drivingMotor2.IsActive = false;
                backMotor1.IsActive = false;
                backMotor2.IsActive = false;
            }
            /*
            acceleration = direction;
            acceleration += direction* rate;
            carHull.LinearVelocity += Utilities.ConvertToBepu(acceleration);
            */
            //speed += rate;
            //foreach (CarWheel w in wheels)
            //{
            //    w.wheel.DrivingMotor.TargetSpeed = speed;
            //}
            
        }
        // Angle in radians
        public void Turn(float angle)
        {
            if (angle > 0)
            {
                steeringMotor1.Settings.Servo.Goal = maximumTurnAngle;
                steeringMotor2.Settings.Servo.Goal = maximumTurnAngle;
            }
            else if (angle < 0)
            {
                steeringMotor1.Settings.Servo.Goal = -maximumTurnAngle;
                steeringMotor2.Settings.Servo.Goal = -maximumTurnAngle;
            }
            else
            {
                steeringMotor1.Settings.Servo.Goal = 0;
                steeringMotor2.Settings.Servo.Goal = 0;
            }
            //direction = Vector3.Transform(direction, Matrix4.CreateRotationY(angle));
            //velocity = Vector3.Transform(velocity, Matrix4.CreateRotationY(angle));
            //turning_angle += angle;
            //body.OrientationMatrix = Matrix3.CreateRotationY(angle);
            //body.WorldTransform *= BEPUutilities.Matrix.CreateFromQuaternion(BEPUutilities.Quaternion.CreateFromRotationMatrix(Matrix4.CreateRotationY(angle)));
        }

        public Vector3 getCarPos()
        {
            return carHull.WorldTransform.Translation;
        }

        public Matrix3 getCarAngle()
        {
            return carHull.OrientationMatrix;
        }

        public override ISpaceObject GetBody(){
            return carHull;
        }

        public void AddToSpace(Space s)
        {
            s.Add(carHull);
        }

       

        #endregion
        #region Private Methods

        protected void ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            //Console.WriteLine("Contact! " + sender.Entity.InstanceId + " and " + other);
        }


        protected override void PositionUpdated(BEPUphysics.Entities.Entity obj)
        {
            base.PositionUpdated(obj);
            //for (int i = 0; i < wheelents.Count; i++)
           // {
            //    wheelents[i].modelMatrix = wheels[i].WorldTransform;
            //}
            //Console.WriteLine("Car position: " + position);
        }

        protected void PairTouched(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            //Console.WriteLine("touch! " + sender.Entity.InstanceId + " and " + other.Tag);
            //Console.WriteLine("pair Touched"+counter++);
        }

        protected void CollisionEnded(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            //Console.WriteLine("Collision Ended");
        }



        /// <summary>
        /// Calulates an approximate size of the drag force
        /// </summary>
        private float CalculateDrag()
        {
            float speedSquared = velocity.LengthSquared;
            return speedSquared*drag_coefficient/2;
        }
        #endregion


        BEPUphysics.Entities.Entity addBackWheel(BEPUutilities.Vector3 wheelOffSet, BEPUphysics.Entities.Entity body,out RevoluteMotor drivingMotor,Entity model)
        {
            var wheel = new ConvexHull(Utilities.meshToVectorArray(model.mesh), 5f);
            wheel.WorldTransform *= BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(wheelOffSet + body.Position));
            //wheel.CollisionInformation.LocalPosition = wheel.Position;
            model.modelMatrix = wheel.WorldTransform;
            //var wheel = new Cylinder(body.Position + wheelOffSet, 0.4f, 5f, 5f);
            wheel.Material.KineticFriction = 2.5f;
            wheel.Material.StaticFriction = 3.5f;

            wheel.Orientation = Quaternion.FromAxisAngle(BEPUutilities.Vector3.Up, 0);

            CollisionRules.AddRule(wheel, body, CollisionRule.NoBroadPhase);

            var pointOnLineJoint = new PointOnLineJoint(body, wheel, wheel.Position,BEPUutilities.Vector3.Up, wheel.Position);
            var suspensionLimit = new LinearAxisLimit(body, wheel, wheel.Position, wheel.Position, BEPUutilities.Vector3.Up, -0.5f, 0f);
            var suspensionSpring = new LinearAxisMotor(body, wheel, wheel.Position, wheel.Position, BEPUutilities.Vector3.Up);

            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.Stiffness = 300;
            suspensionSpring.Settings.Servo.SpringSettings.Damping = 70;
            //suspensionSpring.IsActive = true;
            var revoluteAngularJoint = new RevoluteAngularJoint(body, wheel, BEPUutilities.Vector3.Left);

            drivingMotor = new RevoluteMotor(body, wheel, BEPUutilities.Vector3.Left);

            //drivingMotor.TestAxis = BEPUutilities.Vector3.Forward;
            drivingMotor.Settings.VelocityMotor.Softness = 0.09f;
            //drivingMotor.Settings.MaximumForce = 1000;

            drivingMotor.IsActive = false;

            space.Add(wheel);
            space.Add(pointOnLineJoint);
            space.Add(suspensionLimit);
            space.Add(suspensionSpring);
            space.Add(revoluteAngularJoint);

            return wheel;

        }
        BEPUphysics.Entities.Entity addDriveWheel(BEPUutilities.Vector3 wheelOffSet, BEPUphysics.Entities.Entity body,out RevoluteMotor drivingMotor, out RevoluteMotor steeringMotor,Entity model)
        {
            var wheel = new ConvexHull(Utilities.meshToVectorArray(model.mesh), 5f);
            
            wheel.WorldTransform *= BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(wheelOffSet+ body.Position));
            //wheel.CollisionInformation.LocalPosition = wheel.Position;
            model.modelMatrix = wheel.WorldTransform;
            //var wheel = new Cylinder(body.Position + wheelOffSet, 0.4f, 5f, 5f);
            wheel.Material.KineticFriction = 2.5f;
            wheel.Material.StaticFriction = 3.5f;
            wheel.Material.Bounciness = 0;

            //wheel.Orientation = Quaternion.FromAxisAngle(BEPUutilities.Vector3.Up, 0);
            
            CollisionRules.AddRule(wheel, body, CollisionRule.NoBroadPhase);

            var pointOnLineJoint = new PointOnLineJoint(body, wheel, wheel.Position, BEPUutilities.Vector3.Up, wheel.Position);
            var suspensionLimit = new LinearAxisLimit(body, wheel, wheel.Position, wheel.Position, BEPUutilities.Vector3.Up, -0.5f, 0f);
            var suspensionSpring = new LinearAxisMotor(body, wheel, wheel.Position, wheel.Position, BEPUutilities.Vector3.Up);

            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            //suspensionSpring.IsActive = true;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.Stiffness = 300;
            suspensionSpring.Settings.Servo.SpringSettings.Damping = 70;

            var swivelHingeAngularJoing = new SwivelHingeAngularJoint(body, wheel, BEPUutilities.Vector3.Up, BEPUutilities.Vector3.Left);

            drivingMotor = new RevoluteMotor(body, wheel, BEPUutilities.Vector3.Left);
            drivingMotor.Settings.Mode = MotorMode.VelocityMotor;
            //drivingMotor.TestAxis = BEPUutilities.Vector3.Forward;
            drivingMotor.Settings.VelocityMotor.Softness = 0.09f;
            //drivingMotor.Settings.MaximumForce = 1000;

            drivingMotor.IsActive = false;

            steeringMotor = new RevoluteMotor(body, wheel, BEPUutilities.Vector3.Up);
            steeringMotor.Settings.Mode = MotorMode.Servomechanism;

            steeringMotor.Basis.SetWorldAxes(BEPUutilities.Vector3.Up, BEPUutilities.Vector3.Right);
            steeringMotor.TestAxis = BEPUutilities.Vector3.Right;
            steeringMotor.Settings.Servo.BaseCorrectiveSpeed = 5;
            //steeringMotor.Settings.VelocityMotor.Softness = 4f;
            var steeringConstraint = new RevoluteLimit(body, wheel, BEPUutilities.Vector3.Up, BEPUutilities.Vector3.Right, -maximumTurnAngle, maximumTurnAngle);

            space.Add(wheel);
            space.Add(pointOnLineJoint);
            space.Add(suspensionLimit);
            space.Add(suspensionSpring);
            space.Add(swivelHingeAngularJoing);
            space.Add(drivingMotor);
            space.Add(steeringMotor);
            space.Add(steeringConstraint);

            return wheel;
        }
        
    }
}
