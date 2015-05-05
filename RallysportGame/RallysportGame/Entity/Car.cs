

﻿using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
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
        public ParticleSystem exhaust;
        // The wheels
        public List<Entity> wheelents;
        public List<BEPUphysics.Entities.Entity> wheels;
        public float carRate;
        private int userid = -1;
        // The angle between the direction and forward vectors
        private float turning_angle;
        // Friction coefficient, material-dependent
        private float friction_coefficient = 0.8f;
        // Do the wheels have contact with the ground?
        private bool ground_contact = false;
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

        private static String powerUpSlot = "Missile";
        private bool renderPower = false;
        static System.Timers.Timer boostTime;
        private static bool boostTimeActive = false;

        static DateTime countDownTarget, gameTimer;
        static TimeSpan timeLeft;
        static int timeDiff = 0, previousTimeDiff = 0;
        
        private Missile m;


        private int smookeScreenCounter;
        
        
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
            smookeScreenCounter = 0;
            this.space = space;
            //position = pos;
            BEPUutilities.Matrix temp = BEPUutilities.Matrix.Identity;
            carHull = new ConvexHull(new List<BEPUutilities.Vector3>(Utilities.meshToVectorArray(mesh)),10); 
            carHull.CollisionInformation.LocalPosition = carHull.Position;
            
            // Translation

            carHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));
            modelMatrix = carHull.WorldTransform;
            if (userid == -1)
            {
            carHull.Tag = "Player Car";
            }
            else
            {
                carHull.Tag = "Player Car "+userid;
            }
            this.space.Add(carHull);
            // Add wheels
            /*
            vänster fram: xyz = -19.5, 61, 12.5.
            höger fram: xyz = 35.5, 61, 12.5.
            vänster bak: xyz = -19.5, -34.5, 12.5
            höger bak: xyz = 35.5, -34.5, 12.5
             */

            /*
            Höger fram xyz = 1.7 , 0, 1.0 
            Höger bak xyz = -1.5, 0, ,1.0
            */
            var backwheel1ent = new Entity(wheelPath);
            var backwheel2ent = new Entity(wheelPath);
            var wheel1ent = new Entity(wheelPath);
            var wheel2ent = new Entity(wheelPath);
            var backwheel1 = addBackWheel(new Vector3(1f, -0.2f, 1.6f), carHull,out backMotor1,backwheel1ent);//y = -15.5
            var backwheel2 = addBackWheel(new Vector3(-0.9f, -0.2f, 1.6f), carHull,out backMotor2, backwheel2ent);//y = -30 ger 50% hjul


            wheel1 = addDriveWheel(new Vector3(-0.9f, -0.2f, -1.7f), carHull, out drivingMotor1, out steeringMotor1, wheel1ent);
            
            var wheel2 = addDriveWheel(new Vector3(1f, -0.2f, -1.7f), carHull, out drivingMotor2, out steeringMotor2, wheel2ent);//x 28.5
            
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


            carHull.PositionUpdated += new Action<BEPUphysics.Entities.Entity>(PositionUpdated);
            carHull.CollisionInformation.Events.ContactCreated += new ContactCreatedEventHandler<EntityCollidable>(ContactCreated);
            carHull.CollisionInformation.Events.PairTouched += new PairTouchedEventHandler<EntityCollidable>(PairTouched);
            carHull.CollisionInformation.Events.CollisionEnded += new CollisionEndedEventHandler<EntityCollidable>(CollisionEnded);
            foreach (BEPUphysics.Entities.Entity w in wheels)
            {
                w.CollisionInformation.Events.ContactCreated += new ContactCreatedEventHandler<EntityCollidable>(ContactWheelCreated);
                w.CollisionInformation.Events.CollisionEnded += new CollisionEndedEventHandler<EntityCollidable>(CollisionWheelEnded);
                w.PositionUpdated += new Action<BEPUphysics.Entities.Entity>(PositionUpdated);
            }
            Console.WriteLine("car has id " + carHull.InstanceId);
            m = new Missile(@"Missile", new Vector3(0, -200, 0), space);
            exhaust = new ParticleSystem(new Entity(@"Cube\\smoke"), carHull.Position, -carHull.LinearVelocity, (20f * 3.14f / 180f), 20, 0.1f, new Vector3(0f, 0f, 0f), new TimeSpan(0, 0, 1));
            exhaust.setScale(0.5f);
            exhaust.setThrottle(1);
        }
         public Car(String bodyPath, String wheelPath, Vector3 pos,Space space,int userid) //Defacto  constructor!
            : this(bodyPath, wheelPath, pos, space)
        {
           this.userid = userid;
           carHull.Tag = "Player Car " + userid;
        }

        #endregion
        #region Public Methods


        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.modelMatrix = carHull.WorldTransform;
            base.firstPass(program, projectionMatrix, viewMatrix);
            /*
            OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            foreach (Vector3 v in carHull.Vertices)
            {
                OpenTK.Graphics.OpenGL.GL.Vertex3(v);//OpenTK.Vector3.Transform(v, carHull.WorldTransform));
            }
            OpenTK.Graphics.OpenGL.GL.End();
            */
            
            foreach (Entity w in wheelents)
            {
                w.firstPass(program, projectionMatrix, viewMatrix);
            }
            
            /*
            OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
            foreach(BEPUphysics.Entities.Entity w in wheels)
            {
                ConvexHull temp = w as ConvexHull;
                foreach (Vector3 v in temp.Vertices)
                    OpenTK.Graphics.OpenGL.GL.Vertex3(v);//OpenTK.Vector3.Transform( v, temp.WorldTransform));
            }
            OpenTK.Graphics.OpenGL.GL.End();
            */
                m.firstPass(program, projectionMatrix, viewMatrix);
        }
        public override void renderShadowMap(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.modelMatrix = carHull.WorldTransform;
            base.renderShadowMap(program, projectionMatrix, viewMatrix);

            foreach (Entity w in wheelents)
            {
                w.renderShadowMap(program, projectionMatrix, viewMatrix);
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
            #region Powerup Update
            if (renderPower)
            {
                if (powerUpSlot.Equals("Missile"))
                {
                    if (!m.launched)
                    {
                        Vector3 temp = Vector3.Add(getCarPos()+ new Vector3(0,1,0), Vector3.Mult(Vector3.Transform(new Vector3(0,0,-1), carHull.Orientation),10));
                        m.launch(temp, Vector3.Add(carHull.LinearVelocity,Vector3.Mult(Vector3.Transform(new Vector3(0,0,-1), carHull.Orientation),10)),60*5); 
                    }

                    if (m.update())
                    {
                        renderPower = false;
                        powerUpSlot="None";
                    }

                }
                else if (powerUpSlot.Equals("SmookeScreen"))
                {
                    if (smookeScreenCounter <= 0){
                        smookeScreenCounter=1;
                        //exhaust.move(carHull.Position - carHull.LinearVelocity, -carHull.LinearVelocity);
                        exhaust.setScale(4f);
                    }
                    else
                    {
                        smookeScreenCounter++;
                        if (smookeScreenCounter >= 8 * 60)
                        {
                            exhaust.setScale(0.5f);
                            smookeScreenCounter = 0;
                            powerUpSlot = "None";
                            renderPower = false;
                        }
                    }
                }
                else if (powerUpSlot.Equals("SpeedBoost"))
                {
                }
                else if (powerUpSlot.Equals("LightsOut"))
                {
                }
                else
                {
                    renderPower = false;
                    powerUpSlot = "None";
                }
            }
            #endregion


            Vector3 temp1 = new Vector3(0.3f, -0.1f, 3f);
            Vector3 temp3 = new Vector3(0f, 0f, -1f);
            Quaternion temp2 = getCarAngle();
            
            Vector3.Transform(ref temp1, ref temp2, out temp1);
            Vector3.Transform(ref temp3, ref temp2, out temp3);

            temp3 = Vector3.Normalize(temp3);

            //System.Console.WriteLine(carHull.LinearVelocity.Length());
            exhaust.setThrottle((float)Math.Floor((double)carHull.LinearVelocity.Length())/10);
            exhaust.move(Utilities.ConvertToTK(carHull.Position) + temp1, carHull.LinearVelocity);
            exhaust.tick();

        }

        public void accelerate(float rate)
        {
            carRate = rate;
            if (ground_contact)
            {
                if (rate > 0)
                {
                    drivingMotor1.Settings.VelocityMotor.GoalVelocity = 10;
                    drivingMotor2.Settings.VelocityMotor.GoalVelocity = 10;
                    backMotor1.Settings.VelocityMotor.GoalVelocity = 10;
                    backMotor2.Settings.VelocityMotor.GoalVelocity = 10;

                    drivingMotor1.IsActive = true;
                    drivingMotor2.IsActive = true;
                    backMotor1.IsActive = true;
                    backMotor2.IsActive = true;
                }
                else if (rate < 0)
                {
                    drivingMotor1.Settings.VelocityMotor.GoalVelocity = -10;
                    drivingMotor2.Settings.VelocityMotor.GoalVelocity = -10;
                    backMotor1.Settings.VelocityMotor.GoalVelocity = -10;
                    backMotor2.Settings.VelocityMotor.GoalVelocity = -10;

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

                Vector3 leftRot;
                Quaternion rot2 = carHull.Orientation;
                Vector3.Transform(ref forward, ref rot2, out leftRot);

                Vector3.Multiply(ref leftRot, rate * 0.8f, out acceleration);
                carHull.LinearVelocity += Utilities.ConvertToBepu(acceleration);
            }
        }

        public void networkAccel(float rate)
        {
            Vector3 leftRot;
            Quaternion rot = carHull.Orientation;
            Vector3.Transform(ref forward, ref rot, out leftRot);

            Vector3.Multiply(ref leftRot, rate, out acceleration);
            carHull.LinearVelocity = Utilities.ConvertToBepu(acceleration); 
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
        }

        public Vector3 getCarPos()
        {
            return carHull.WorldTransform.Translation;
        }

        public void setCarPos(Vector3 pos)
        {
            Vector3 frontRight = new Vector3(-0.9f, -0.2f, -1.7f);
            Vector3 frontLeft = new Vector3(1f, -0.2f, -1.7f);
            Vector3 backLeft = new Vector3(1f, -0.2f, 1.6f);
            Vector3 backRight = new Vector3(-0.9f, -0.2f, 1.6f);
            carHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));

            wheels[0].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + frontRight);
            wheels[1].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + frontLeft);
            wheels[2].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + backRight);
            wheels[3].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + backLeft);
        }

        public void setCarPos(Vector3 pos,Quaternion rot)
        {
            Vector3 frontRight = new Vector3(-0.9f, -0.2f, -1.7f);
            Vector3 frontLeft = new Vector3(1f, -0.2f, -1.7f);
            Vector3 backLeft = new Vector3(1f, -0.2f, 1.6f);
            Vector3 backRight = new Vector3(-0.9f, -0.2f, 1.6f);
            carHull.WorldTransform = BEPUutilities.Matrix.CreateTranslation(Utilities.ConvertToBepu(pos));
            carHull.Orientation = rot;
            Quaternion rot2 = carHull.Orientation;
            
            Vector3 frontRRot;
            Vector3.Transform(ref frontRight, ref rot, out frontRRot);
            
            Vector3 frontLRot;
            Vector3.Transform(ref frontLeft, ref rot2, out frontLRot);

            Quaternion rot3 = carHull.Orientation;
            Quaternion rot4 = carHull.Orientation;

            Vector3 backRRot;
            Vector3.Transform(ref backLeft, ref rot3, out backRRot);
            Vector3 backLRot;
            Vector3.Transform(ref backRight, ref rot4, out backLRot);

            wheels[0].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + frontRRot);
            wheels[1].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + frontLRot);
            wheels[2].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + backRRot);
            wheels[3].WorldTransform = BEPUutilities.Matrix.CreateTranslation(pos + backLRot);

            wheels[0].Orientation = rot;
            wheels[1].Orientation = rot;
            wheels[2].Orientation = rot;
            wheels[3].Orientation = rot;

        }

        public OpenTK.Quaternion getCarAngle()
        {
            return carHull.Orientation;
        }

        public override ISpaceObject GetBody(){
            return carHull;
        }

        public void AddToSpace(Space s)
        {
            s.Add(carHull);
        }

        public void deleteCarFromSpace()
        {
            space.Remove(carHull);
            foreach(BEPUphysics.Entities.Entity w in wheels)
            {
                space.Remove(w);
            }
        }

       public void renderBackLight(int program, Entity renderTarget)
        {
           Vector3 rightLight = new Vector3(0.8f, 0, 3.0f);
           Vector3 leftLight = new Vector3(-0.8f, 0, 3.0f);
           Vector3 rightFrontLight = new Vector3(0.8f, 0.3f, -2.3f);
           Vector3 leftFrontLight = new Vector3(-0.8f, 0.3f, -2.3f);

           Vector3 rightRot;
           Quaternion rot = carHull.Orientation;
           Vector3.Transform(ref rightLight, ref rot, out rightRot);
           
           Vector3 leftRot;
           Quaternion rot2 = carHull.Orientation;
           Vector3.Transform(ref leftLight, ref rot2, out leftRot);

           renderTarget.pointLight(program, Utilities.ConvertToTK(carHull.WorldTransform.Translation) + rightRot, new Vector3(1, 0, 0), 1);
           renderTarget.pointLight(program, Utilities.ConvertToTK(carHull.WorldTransform.Translation) + leftRot, new Vector3(1, 0, 0), 1);

           Quaternion rot3 = carHull.Orientation;
           Quaternion rot4 = carHull.Orientation;
           Vector3.Transform(ref rightFrontLight, ref rot3, out rightRot);
           Vector3.Transform(ref leftFrontLight, ref rot4, out leftRot);

           Vector3 front = new Vector3(0, 0, -1);
           Vector3 test;
           Vector3.Transform(ref front, ref rot, out test);
           test.Normalize();
           renderTarget.spotLight(program, Utilities.ConvertToTK(carHull.WorldTransform.Translation) + rightRot, test, new Vector3(0.6f, 0.6f, 0.6f), 125, (float)Math.Cos(BEPUutilities.MathHelper.Pi / 8));
           renderTarget.spotLight(program, Utilities.ConvertToTK(carHull.WorldTransform.Translation) + leftRot, test, new Vector3(0.6f, 0.6f, 0.6f), 125, (float)Math.Cos(BEPUutilities.MathHelper.Pi / 8));


        }

       public void addPowerUp(String type)
       {
           powerUpSlot = type;
       }

       public void timerBoost(int seconds)
       {
           countDownTarget = DateTime.Now;
           timeLeft = new TimeSpan(0, 0, seconds);
           countDownTarget = countDownTarget.Add(timeLeft);
       }

       public void tick()
       {
           //Console.WriteLine("Car tick entered");
           gameTimer = DateTime.Now;
           timeDiff = countDownTarget.Subtract(DateTime.Now).Seconds;
           if (timeDiff != previousTimeDiff && timeDiff >= 0)
           {
               //Console.WriteLine("Timer is active");
               boostTimeActive = true;
               previousTimeDiff = timeDiff;
           }
           if (timeDiff == 0)
           {
               //Console.WriteLine("Not active");
               powerUpSlot = "None";
               boostTimeActive = false;
               //RaceState.setCurrentState(RaceState.States.RACING);
           }



       }

       public void usePowerUp()
       {
            renderPower = true; 

            if (powerUpSlot.Equals("SpeedBoost"))
            {                
                Console.WriteLine("Timer started: 20s");
                
                timerBoost(20);
            }
            else if (powerUpSlot.Equals("Missile"))
            {
                
            }
            else if (powerUpSlot.Equals("LightsOut"))
            {

            }
           else if (powerUpSlot.Equals("SmookeScreen"))
           {

           }
            else
            {
                powerUpSlot = "None";
            }
            Network.getInstance().sendPowerUp(powerUpSlot, getCarPos(), getCarAngle(), carRate);
       }

        //stops the timer after 20 s
        static void boostTimeStop(object sender, System.Timers.ElapsedEventArgs e)
        {
            boostTime.Stop();
            boostTimeActive = false;
            powerUpSlot = "None";
            Console.WriteLine("Timer stop!");
        }

       public bool boostActive()
       {
           return boostTimeActive;
       }

        
       public String getPowerUp()
       {
           return powerUpSlot;
       }

       public Missile getM()
       {
           return m;
       }

       public void renderPActive()
       {
           renderPower = true;
       }

       public bool getRenderP()
       {
           return renderPower;
       }

       public String getPowerType()
       {
           return powerUpSlot;
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
        protected void ContactWheelCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            ground_contact = true;
            //Console.WriteLine("Contact! " + sender.Entity.InstanceId + " and " + other);
        }

        protected void CollisionWheelEnded(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            ground_contact = false;
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
            ConvexHull wheel = new ConvexHull(Utilities.meshToVectorArray(model.mesh), 5f);
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

            wheel.Tag = "backwheel";
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

            wheel.Tag = "frontWheel";

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