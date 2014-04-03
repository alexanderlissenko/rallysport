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
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.BroadPhaseEntries.Events;


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
            : this(name, new Vector3(0,0,0))   
        {
        }
        public DynamicEntity(String name, Vector3 pos)
            : base(name, pos)
        {
            direction = new Vector3(0,0,-1);
            velocity = acceleration = Vector3.Zero;
            body = new Box(Utilities.ConvertToBepu(pos), 5f, 5f, 5f, 5f);
            //CollisionHandler.environment.Events.ContactCreated += new ContactCreatedEventHandler<StaticMesh>(eventTest);
            
            
            worldMatrix = Matrix4.Identity;
            
        }

        
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the world matrix with the new translations and rotations of the car model
        /// </summary>
        public virtual void Update()
        {
            //modelMatrix *= Matrix4.CreateTranslation(body.LinearVelocity);
        }

        public virtual void eventTest(StaticMesh sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            Console.WriteLine("Contact detected between "+sender + " and " + other);
            
        }

        public void rotate(float angle_x, float angle_y, float angle_z)
        {

        }

        //Adds a particle emitter to the entity
        public void AddEmitter(ParticleSystem pSys)
        {
            emitters.Add(pSys);
        }

        public virtual ISpaceObject GetBody()
        {
            return body;
        }
        #endregion

        #region Protected Methods
        protected virtual void PositionUpdated(BEPUphysics.Entities.Entity obj)
        {
            position = Utilities.ConvertToTK(body.Position);
        }
        #endregion
    }
}
