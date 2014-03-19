using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace RallysportGame
{
    #region ParticleSystem class
    class ParticleSystem : Entity
    {
        #region Instance variables

        static int ID=0; // debugg
        private Vector3 emitterPos;
        private Vector3 frustumDir;
        private float spawnFrustum; //angle
        private Entity particleObject;
        private int spawnRate;
        private static bool emit;
        private static Random random;
        private DateTime prevTime;
        private ArrayList particleList; 
        private TimeSpan meanLiveTime;
        #endregion

        #region Constructors

        //empty constructor, not to be used for real but C# wants it
        public ParticleSystem() : this(new Vector3(0.0f, 0.0f, 0.0f),new Vector3(0.0f, -1.0f, 0.0f), 
                                    45.0f, 20, new TimeSpan(0,0,10), null)
        {

        }
        
        /// <summary>
        /// This is the constructor for the particle system
        /// </summary>
        /// <param name="pos">The emitter's initial position.</param>
        /// <param name="frustum">Spans the possible directions where particles may
        ///     be shot. Format is (angle, x-aspect, y-aspect).</param>
        /// <param name="rate">Number of particles that are spawned each second.</param>
        /// <param name="liveTime">The fuzzy value of how long a particle lives.</param>
        /// <param name="particle">The particle object that is to be rendered.</param>
        /// <param name="spawnRate">Particals spawned eatch tenth of a second</param>
        public ParticleSystem(Vector3 pos,Vector3 frustomDirIn, float frustum, int rate,
                        TimeSpan liveTime, Entity particle)
        {
            random = new Random();
            emitterPos = pos;
            spawnFrustum = frustum;
            spawnRate = rate;
            meanLiveTime = liveTime;
            particleObject = particle;
            emit = true;
            prevTime = new DateTime(0);
            frustumDir = frustomDirIn;
            
            int capacity = (int)Math.Ceiling(meanLiveTime.Seconds * spawnRate* 15.0f); // *10 * 1.5  10 bechus it's in milli seconds and 1.5 bechus of margins
            particleList = new ArrayList(capacity); //might be bad, if memory seems suspicious, double check
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the emitter
        /// </summary>
        public void startEmit()
        {
            emit = true;
        }

        /// <summary>
        /// Stops the emitter
        /// </summary>
        public void stopEmit()
        {
            emit = false;
        }

        /// <summary>
        /// Overloads the firstPass method in Entity. Renders the particleObject at each particles position into
        /// the textures.
        /// </summary>
        /// <param name="program">The shader program to be used.</param>
        /// <param name="projectionMatrix">Passed on to Entity's firstPass when rendering particles.</param>
        /// <param name="viewMatrix">Passed on to Entity's firstPass when rendering particles.</param>
        new public void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        { 
            foreach(Particle p in particleList){
                particleObject.setCoordiants(p.GetPosition().X, p.GetPosition().Y, p.GetPosition().Z);
                particleObject.firstPass(program,projectionMatrix,viewMatrix);
            }
        }

        /// <summary>
        /// Moves the particleSystem forward in time by one step, in other words moves and spawns particles.
        /// Particles are only spawned if it was more than 100 ms since the previous call to tick.
        /// </summary>
        public void tick()
        {

            if (emit && (prevTime.Add(new TimeSpan(0,0, 0, 0, 100)) <= DateTime.Now))
            {
                prevTime = DateTime.Now;

                for (int i = 0; i <= spawnRate; i++)
                {
                    //calculate the length of "Normal" in the XY-plane
                    float lengthxy = frustumDir.X * frustumDir.X + frustumDir.Y * frustumDir.Y; // sqared length
                    float alphaxy;
                    
                    if (lengthxy == 0)
                    {
                        alphaxy = 0.0f;
                    }
                    else
                    {
                        lengthxy = (float)Math.Sqrt(lengthxy); // real length

                        //calculate the angle between the x-axis and "Normal" in the XY-plane
                        alphaxy =(float)Math.Asin((double)(frustumDir.Y / lengthxy)); // sin alpha = y / hypotenuse
                    }

                    //calculate the length of "Normal" in the XZ-plane
                    float lengthxz = frustumDir.X * frustumDir.X + frustumDir.Z * frustumDir.Z; // sqared length
                    float alphaxz;
                    if (lengthxz == 0)
                    {
                        alphaxz = 0.0f; // 90 degrees
                    }
                    else
                    {
                        lengthxz = (float)Math.Sqrt(lengthxz); // real length

                        //calculate the angle between the x-axis and "Normal" in the XZ-plane
                        alphaxz = (float)Math.Asin((double)(frustumDir.Z / lengthxz)); // sin alpha = z / hypotenuse
                    }


                    Vector4 vector_x = new Vector4(1, 0, 0, 0);             
                    // pick angle and make rotation matrix
                    double tempRand = random.NextDouble() * spawnFrustum - spawnFrustum/2;


                     // pick a new angle and make another rotation matrix
                    Matrix4 mat = new Matrix4(new Vector4((float)Math.Cos(tempRand), (float)-Math.Sin(tempRand),0, 0),                  //rot z
                                                new Vector4((float)Math.Sin(tempRand), (float)Math.Cos(tempRand), 0, 0),
                                                new Vector4(0,0,1,0),
                                                new Vector4(0,0,0,1)
                                                );

                    vector_x = Vector4.Transform(vector_x, mat);
                   
                    tempRand = random.NextDouble() * 2 * 3.14 - 3.14;

                     mat = new Matrix4(new Vector4(1,0,0,0),                                                                         // rot x
                            new Vector4(0, (float)Math.Cos(tempRand), (float)-(Math.Sin(tempRand)),0),
                            new Vector4(0, (float)Math.Sin(tempRand), (float)Math.Cos(tempRand),0),
                            new Vector4(0,0,0,1)
                            );
                    
                    vector_x = Vector4.Transform(vector_x, mat);


                    mat = new Matrix4(new Vector4((float)Math.Cos(alphaxy), (float)-Math.Sin(alphaxy), 0, 0),                  //rot z
                                                new Vector4((float)Math.Sin(alphaxy), (float)Math.Cos(alphaxy), 0, 0),
                                                new Vector4(0, 0, 1, 0),
                                                new Vector4(0, 0, 0, 1)
                                                );
                    vector_x = Vector4.Transform(vector_x, mat);

                     mat = new Matrix4(
                              new Vector4((float)Math.Cos(alphaxz), 0, (float)Math.Sin(alphaxz), 0),
                              new Vector4(0, 1, 0, 0),
                              new Vector4((float)-Math.Sin(alphaxz), 0, (float)Math.Cos(alphaxz), 0),
                              new Vector4(0, 0, 0, 1)
                              );
                     vector_x = Vector4.Transform(vector_x, mat);

                     vector_x.Normalize();
                     tempRand = random.NextDouble();
                     vector_x.Scale((float)tempRand, (float)tempRand, (float)tempRand, 0);

                     

                    // rotate the vector
                     Vector4 velocity4 = vector_x;
                    //make it a 3 vec for the patricle constructor
                    Vector3 velocity3 = new Vector3(velocity4.X, velocity4.Y, velocity4.Z);
                    // Spawn the particle
                    ID++;
                    particleList.Add(new Particle(velocity3, emitterPos, meanLiveTime, ID));
                }
            }
            
            // if the particles are systems themselvs increment them.
            if (particleObject is ParticleSystem)
            {
                ((ParticleSystem)particleObject).tick();
            }

            ArrayList tempList = new ArrayList(particleList.Count);
            foreach (Particle p in particleList)
            {
                bool temp = p.MoveAndDie();


                if (temp)
                {
                    tempList.Add(p);  
                }
            }
            foreach (Particle p in tempList)
            {
                particleList.Remove(p);
            }

        }

        #endregion

    }

    #endregion

    #region Particle class
    class Particle
    {
        #region Instance variables

        public int ID_part;
        public Vector3 gravity;
        private Vector3 pVelocity;
        private Vector3 pPosition;
        private TimeSpan pLiveTime;
        private DateTime pBirthTime;
        private static Random random;

        #endregion

        #region Constructors
        //empty constructor, not to be used for real
        public Particle()
        {

        }
       
        /// <summary>
        /// Constructor that initiates each particle objects with an initial position, velocity, livetime and ID.
        /// </summary>
        /// <param name="velocity">The particle's initial velocity.</param>
        /// <param name="position">The particle's initial position.</param>
        /// <param name="liveTime">The particle's set livetime.</param>
        /// <param name="ID">The particle's set ID.</param>
        public Particle(Vector3 velocity, Vector3 position, TimeSpan liveTime, int ID)
        {
            ID_part = ID;
            gravity =  new Vector3(0, -0.000001f, 0);
            random = new Random();
            pBirthTime = DateTime.Now;
            pVelocity = velocity;
            pPosition = position;
            double value = random.NextDouble() * 1.5 - 0.25;
            pLiveTime = new TimeSpan(0,0, Convert.ToInt32(liveTime.Seconds + value*liveTime.Seconds));
        }

        #endregion

        #region Methods
        /// <summary>
        /// MoveAndDie moves the particle and kills it if the particles livetime is reached.
        /// </summary>
        /// <returns>if the particle has lived longer then it's set live time.</returns>
        public bool MoveAndDie()
        {
            pVelocity += gravity;
            pPosition += pVelocity ;

            if (DateTime.Now - pBirthTime >= pLiveTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get current position of the particle.
        /// </summary>
        /// <returns>the particle's current position.</returns>
        public Vector3 GetPosition(){ return pPosition; }
    }

    #endregion

    #endregion
}
