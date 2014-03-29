

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
        public Car(String bodyPath, String wheelPath)
            : base(bodyPath)
        {
            ConvexHull carHull = new ConvexHull(new List<BEPUutilities.Vector3>(Utilities.meshToVectorArray(mesh))/*,10*/); //Use with wrapped body?
            vehicle = new Vehicle(carHull);
            // Add wheels
            /*
            vänster fram: xyz = -19.5, 61, 12.5.
            höger fram: xyz = 35.5, 61, 12.5.
            vänster bak: xyz = -19.5, -34.5, 12.5
            höger bak: xyz = 35.5, -34.5, 12.5
             */
            wheels = new List<CarWheel>();
            wheels.Add(new CarWheel(wheelPath, new Vector3(-19.5f, 61f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(35.5f, 61f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(-19.5f, -34.5f, 12.5f)));
            wheels.Add(new CarWheel(wheelPath, new Vector3(35.5f, -34.5f, 12.5f)));
            // ...
            foreach (CarWheel w in wheels)
            {
                w.setUp3DSModel();
                vehicle.AddWheel(w.wheel);
                CollisionRules.AddRule(w.wheel.Shape, vehicle.Body, CollisionRule.NoBroadPhase);
                CollisionRules.AddRule(vehicle.Body, w.wheel.Shape, CollisionRule.NoBroadPhase);
            }
            
            body = vehicle.Body;
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
            base.firstPass(program, projectionMatrix, viewMatrix);
            foreach (CarWheel w in wheels)
            {
                w.firstPass(program, projectionMatrix, viewMatrix);
            }
        }

        public override void Update()
        {
            
            base.Update();
        }

        public override void eventTest(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            Console.WriteLine("Sent by car");
            base.eventTest(sender, other, pair, contact);
        }

        public void accelerate(float rate)
        {
            acceleration = direction;
            acceleration *= rate;
            worldMatrix = Matrix4.CreateTranslation(new Vector3(0, 50, 0));
            Console.WriteLine(worldMatrix);
        }
        // Angle in radians
        public void Turn(float angle)
        {
            direction = Vector3.Transform(direction, Matrix4.CreateRotationY(angle));
            velocity = Vector3.Transform(velocity, Matrix4.CreateRotationY(angle));
            turning_angle += angle;
        }

        public override ISpaceObject GetBody(){
            return vehicle.Body;
        }
        
        #endregion

        #region Private Methods
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
