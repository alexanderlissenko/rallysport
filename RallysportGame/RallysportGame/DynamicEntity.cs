using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;


namespace RallysportGame
{
    class DynamicEntity : Entity
    {
        #region Constants
        // The direction the whole car is facing in model space
        protected Vector3 forward = new Vector3(0f, 0f, -1f);
        protected readonly Vector3 up = new Vector3(0f, 1f, 0f);
        #endregion

        #region Instance Variables
        protected BEPUphysics.Entities.Entity body;
        
        // Direction in world space (orientation of the model, NOT velocity)
        protected Vector3 direction;
        // Velocity in world space (NOT same as direction) Should be used normalized, then multiplied with speed
        protected Vector3 velocity;
        // The speed of the object in the direction defined by the velocity vector
        protected float speed = 0f;
        // Acceleration in world space
        protected Vector3 acceleration;
        //All particle emitters attached to this entity
        protected List<ParticleSystem> emitters = new List<ParticleSystem>();
        #endregion

        #region Constructors
        public DynamicEntity(String name)
            : base(name)
        {
            direction = forward;
            position = velocity = acceleration = Vector3.Zero;
        }
        public DynamicEntity(String name, Vector3 pos)
            : base(name, pos)
        {
            direction = forward;
            velocity = acceleration = Vector3.Zero;
            body = new Box(Utilities.ConvertToBepu(pos), 5f, 5f, 5f, 5f);
            body.CollisionInformation.Events.PairCreated += new BEPUphysics.BroadPhaseEntries.Events.PairCreatedEventHandler<EntityCollidable>(eventTest);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the world matrix with the new translations and rotations of the car model
        /// </summary>
        public void Update()
        {
            position = Utilities.ConvertToTK(body.Position);
        
            
            worldMatrix = Matrix4.Identity;
            velocity += acceleration;
            //  - /*CalculateDrag() * */ (velocity.Normalized() * -1f); 
            position += Utilities.ConvertToTK(body.LinearVelocity);
            Matrix4 modelRotation = Matrix4.CreateRotationY(MathHelper.Pi / 2);
            //Matrix4 directionRotation = Matrix4.CreateRotationY(turning_angle);
            Matrix4 translation = Matrix4.CreateTranslation(position);
            Matrix4.Mult(ref worldMatrix, ref modelRotation, out worldMatrix);
            //Matrix4.Mult(ref worldMatrix, ref directionRotation, out worldMatrix);
            Matrix4.Mult(ref worldMatrix, ref translation, out worldMatrix);
            acceleration = Vector3.Zero;
            body.Position = Utilities.ConvertToBepu(position);

            //for all emitters do emitters.tick() 
        }

        public void eventTest(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair){
            Console.WriteLine("Pair detected");
            sender.Entity.LinearVelocity = Vector3.Zero;
        }

        public void rotate(float angle_x, float angle_y, float angle_z)
        {

        }

        //Adds a particle emitter to the entity
        public void AddEmitter(ParticleSystem pSys)
        {
            emitters.Add(pSys);
        }

        public ISpaceObject GetBody()
        {
            return body;
        }
        #endregion

        #region Protected Methods
        #endregion
    }
}
