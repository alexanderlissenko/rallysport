

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
    class Car : Entity
    {
        #region Constants
        // The direction the whole car is facing in model space
        private readonly Vector3 forward = new Vector3(0f, 0f, -1f);
        private readonly Vector3 up = new Vector3(0f, 1f, 0f);
        // Drag coefficient (we use a simplified version of drag: D = 1/2 * V^2 * C where C is the coefficient)
        private const float drag_coefficient = 0.7f;
        #endregion

        #region Instance Variables
        public Box boundingBox;
        // 3D position in world space
        private Vector3 position;
        // Direction in world space (orientation of the model, NOT velocity)
        private Vector3 direction;
        // Velocity in world space (NOT same as direction) Should be used normalized, then multiplied with speed
        private Vector3 velocity;
        // The speed of the object in the direction defined by the velocity vector
        private float speed = 0f;
        // Acceleration in world space
        private Vector3 acceleration;
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
            : base(name)
        {
            direction = forward;
            position = velocity = acceleration = Vector3.Zero;
        }
        // Initial starting position
        public Car(String name, Vector3 pos)
            : base(name)
        {
            position = pos;
            direction = forward;
            velocity = acceleration = Vector3.Zero;
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
        /// <summary>
        /// Updates the world matrix with the new translations and rotations of the car model
        /// </summary>
        public void Update()
        {
            worldMatrix = Matrix4.Identity;
            velocity += acceleration;
            //  - /*CalculateDrag() * */ (velocity.Normalized() * -1f); 
            position += velocity;
            Matrix4 modelRotation = Matrix4.CreateRotationY(MathHelper.Pi/2);
            Matrix4 directionRotation = Matrix4.CreateRotationY(turning_angle);
            Matrix4 translation = Matrix4.CreateTranslation(position);
            Matrix4.Mult(ref worldMatrix, ref modelRotation, out worldMatrix);
            Matrix4.Mult(ref worldMatrix, ref directionRotation, out worldMatrix);
            Matrix4.Mult(ref worldMatrix, ref translation, out worldMatrix);
            acceleration = Vector3.Zero;
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
        public Car(OpenTK.Vector3 position):base("Cube\\testCube", position)
        {
            boundingBox = new Box(position,1,1,1,1);
        }

        public void update()
        {
            position = boundingBox.Position;
        }
    }
}
