

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
        public Vehicle vehicle;
        // The wheels
        public List<CarWheel> wheels;
        // The angle between the direction and forward vectors
        private float turning_angle;
        // Friction coefficient, material-dependent
        private float friction_coefficient = 0.8f;
        // Do the wheels have contact with the ground?
        private bool ground_contact = true;
        int counter = 0;
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
        
        public Car(String bodyPath, String wheelPath, Vector3 pos) //Defacto  constructor!
            : base(bodyPath)
        {
            //position = pos;
            BEPUutilities.Matrix temp = BEPUutilities.Matrix.Identity;
            ConvexHull carHull = new ConvexHull(new List<BEPUutilities.Vector3>(Utilities.meshToVectorArray(mesh)),100); //Use with wrapped body?
            carHull.WorldTransform *= BEPUutilities.Matrix.CreateScale(scaling_factor, scaling_factor, scaling_factor);
            carHull.CollisionInformation.LocalPosition = carHull.Position;
            vehicle = new Vehicle(carHull);
            body = vehicle.Body;
            // Make sure we use the same transforms for both physics geometry and graphics!
            
            // Scaling
            //modelMatrix *= Matrix4.CreateScale(scaling_factor);
            // Rotation
            //Matrix4.LookAt(Vector3.Zero, direction, up);
            //body.Orientation = new Quaternion(Utilities.ConvertToBepu(direction), 1);
            vehicle.Body.WorldTransform *= BEPUutilities.Matrix.CreateScale(scaling_factor, scaling_factor, scaling_factor);
            // Translation
            
            vehicle.Body.WorldTransform *= BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));
            modelMatrix = vehicle.Body.WorldTransform;
            // Add wheels
            /*
            vänster fram: xyz = -19.5, 61, 12.5.
            höger fram: xyz = 35.5, 61, 12.5.
            vänster bak: xyz = -19.5, -34.5, 12.5
            höger bak: xyz = 35.5, -34.5, 12.5
             */

            wheels = new List<CarWheel>();
            Vector3 wheelPos = new Vector3(vehicle.Body.Position.X + -19.5f, vehicle.Body.Position.X + 61f, vehicle.Body.Position.X + 12.5f);
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
                CollisionRules.AddRule(w.wheel.Shape, vehicle.Body, CollisionRule.NoNarrowPhasePair);
                CollisionRules.AddRule(vehicle.Body, w.wheel.Shape, CollisionRule.NoNarrowPhasePair);

            }
            vehicle.Body.PositionUpdated += new Action<BEPUphysics.Entities.Entity>(PositionUpdated);
            vehicle.Body.CollisionInformation.Events.ContactCreated += new ContactCreatedEventHandler<EntityCollidable>(ContactCreated);
            vehicle.Body.CollisionInformation.Events.PairTouched += new PairTouchedEventHandler<EntityCollidable>(PairTouched);
            vehicle.Body.CollisionInformation.Events.CollisionEnded += new CollisionEndedEventHandler<EntityCollidable>(CollisionEnded);

            Console.WriteLine("car has id " + vehicle.Body.InstanceId);
        }

        #endregion
        #region Public Methods

        public override void render(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 lightPosition, Matrix4 lightViewMatrix, Matrix4 lightProjectionMatrix)
        {
            base.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
            foreach (CarWheel w in wheels)
            {
                w.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
            }
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.modelMatrix = body.WorldTransform;
            base.firstPass(program, projectionMatrix, viewMatrix);
            
            foreach (CarWheel w in wheels)
            {
                w.firstPass(program, projectionMatrix, viewMatrix);
            }
        }

        public override void Update()
        {
            //position = Utilities.ConvertToTK(body.Position);
            base.Update();
            foreach (CarWheel w in wheels)
            {
                w.Update();
            }
        }

       

        public void accelerate(float rate)
        {
            acceleration = Vector3.Zero;
            acceleration = direction;
            acceleration *= rate;
            body.LinearVelocity += Utilities.ConvertToBepu(acceleration);
        }
        // Angle in radians
        public void Turn(float angle)
        {
            direction = Vector3.Transform(direction, Matrix4.CreateRotationY(angle));
            velocity = Vector3.Transform(velocity, Matrix4.CreateRotationY(angle));
            turning_angle += angle;
            body.WorldTransform *= BEPUutilities.Matrix.CreateFromQuaternion(new BEPUutilities.Quaternion(angle,0,0,0));
        }

        public override ISpaceObject GetBody(){
            return vehicle.Body;
        }

        public void AddToSpace(Space s)
        {
            s.Add(vehicle);
        }

       

        #endregion
        #region Private Methods

        protected void ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            Console.WriteLine("Contact! " + sender.Entity.InstanceId + " and " + other);
        }


        protected override void PositionUpdated(BEPUphysics.Entities.Entity obj)
        {
            base.PositionUpdated(obj);
            //Console.WriteLine("Car position: " + position);
        }

        protected void PairTouched(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            //Console.WriteLine("touch! " + sender + " and " + other);
            //Console.WriteLine("pair Touched"+counter++);
        }

        protected void CollisionEnded(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            Console.WriteLine("Collision Ended");
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

        


        
    }
}
