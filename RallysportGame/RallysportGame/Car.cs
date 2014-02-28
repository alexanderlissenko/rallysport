

﻿using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RallysportGame
{
    class Car : DynamicEntity
    {
        #region Constants
        
        // Drag coefficient (we use a simplified version of drag: D = 1/2 * V^2 * C where C is the coefficient)
        private const float drag_coefficient = 0.7f;
        #endregion

        #region Instance Variables
        public Box boundingBox;
        
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

        }
        #endregion

        #region Public Methods
        public void accelerate(float rate)
        {
            if (ground_contact)
            {
                acceleration += direction * rate;
                acceleration *= friction_coefficient;
            }
        }
        // Angle in radians
        public void Turn(float angle)
        {
            direction = Vector3.Transform(direction, Matrix4.CreateRotationY(angle));
            velocity = Vector3.Transform(velocity, Matrix4.CreateRotationY(angle));
            turning_angle += angle;
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

        

        //default constructor for car
        public Car(Vector3 position):base("Cube\\testCube", position)
        {
            //boundingBox = new Box(position,1,1,1,1);
        }

        
    }
}
