using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace RallysportGame
{
    class ParticleSystem : Entity
    {

        static int ID=0; // debugg
        private Vector3 emitterPos;
        private Vector3 frustumDir;
        private float spawnFrustum; //angle
        private Entity particleObject;
        private int spawnRate;
        private static bool emit;
        private static Random random;
        private DateTime prevTime;

        //maybe better with Dictionary (~ HashMap)? we might want to know which particle to remove?
        //remove(particle) from arraylist might be slow, O(n)... remove from Dictionary is O(1)
        //capacity issue could be solved with some simple comparison, I guess...
        private ArrayList particleList; 
        private TimeSpan meanLiveTime;
        
        //empty constructor, not to be used for real
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
         
        public void startEmit()
        {
            emit = true;
        }

        public void stopEmit()
        {
            emit = false;
        }
        new public void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        { 
            foreach(Particle p in particleList){
                particleObject.setCoordiants(p.GetPosition().X, p.GetPosition().Y, p.GetPosition().Z);
                particleObject.firstPass(program,projectionMatrix,viewMatrix);
            }
        }
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

    }

    class Particle
    {
        public int ID_part;
        public Vector3 gravity;
        private Vector3 pVelocity;
        private Vector3 pPosition;
        private TimeSpan pLiveTime;
        private DateTime pBirthTime;
        private static Random random;

        //empty constructor, not to be used for real
        public Particle()
        {

        }
       
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
        
        
        /// <summary>
        /// MoveAndDie moves the particle and returns true if the particle has lived longer then it's intended live time
        /// </summary>
        /// <returns>true if it has lived long enugh</returns>
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
        public Vector3 GetPosition(){ return pPosition; }
    }
}
