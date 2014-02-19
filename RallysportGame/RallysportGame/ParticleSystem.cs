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
        private Vector3 emitterPos;
        private Vector3 frustumDir;
        private float spawnFrustum; //angle
        private Entity particleObject;
        private int spawnRate;
        private bool emit;
        private static Random random;
        private DateTime prevTime;


        //maybe better with Dictionary (~ HashMap)? we might want to know which particle to remove?
        //remove(particle) from arraylist might be slow, O(n)... remove from Dictionary is O(1)
        //capacity issue could be solved with some simple comparison, I guess...
        private ArrayList particleList; 
        private TimeSpan meanLiveTime;
        
        //empty constructor, not to be used for real
        public ParticleSystem() : this(new Vector3(0.0f, 0.0f, 0.0f), 
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
        public ParticleSystem(Vector3 pos, float frustum, int rate,
                        TimeSpan liveTime, Entity particle)
        {
            emitterPos = pos;
            spawnFrustum = frustum;
            spawnRate = rate;
            meanLiveTime = liveTime;
            particleObject = particle;
            emit = true;
            prevTime = new DateTime(0);
            frustumDir = new Vector3(0.0f, 0.0f, 1.0f);
            
            int capacity = (int)Math.Ceiling(meanLiveTime.Seconds * spawnRate * 1.5); 
            particleList = new ArrayList(capacity); //might be bad, if memory seems suspicious, double check
        }
         
        void startEmit()
        {
            emit = true;
        }

        void stopEmit()
        {
            emit = false;
        }
        void render()
        { 
            foreach(Particle p in particleList){



                // This method should render a particleObject at eatch particle possition
                // This is how I whant it to wark, but dosen't as of yet
               // particleObject.render(p.GetPosition().X, p.GetPosition().Y, p.GetPosition().Z);
            }
            
        
        }
        void tick()
        {

            if (prevTime.Add(new TimeSpan(0, 0, 1)) <= DateTime.Now && emit)
            {
                prevTime = DateTime.Now;

                Vector4 tmpPos = new Vector4(frustumDir.X, frustumDir.Y, frustumDir.Z, 1.0f);


                for (int i = 0; i <= spawnRate; i++)
                {
                    

                    // this is soo that eatch vector has a different length and thus different speed
                    // it's cast to float becuse Vector4 whants floats.
                    float tempScaleingFactor=(float)random.NextDouble();
                    // here the scaling is applied in the matrix that moves the vector from origin back to where it was.
                    Matrix4 transMatToOrigin =  new Matrix4(new Vector4(tempScaleingFactor, 0, 0, -emitterPos.X),
                                                            new Vector4(0, tempScaleingFactor, 0, -emitterPos.Y),
                                                            new Vector4(0, 0, tempScaleingFactor, -emitterPos.Z),
                                                            new Vector4(0, 0, 0, 1)
                                                            );


                    // uses random to select an angle in the interval +- spawnFrustum/2
                    /*************************************
                        * -spawnFrustum/2  +spawnFrustum/2  *
                        *          |    |    |              *
                        *           |   |   |               *
                        *            |  |  |                *
                        *             | | |                 *
                        *              |||                  *
                        *               |                   *
                        *************************************/
                    
                    
                    // pick angle and make rotation matrix
                    double tmpAngle = random.NextDouble() * spawnFrustum - spawnFrustum/2;
                    Matrix4 rotMatX = new Matrix4(new Vector4(1,0,0,0),
                                                new Vector4(0, (float)Math.Cos(tmpAngle), (float)-(Math.Sin(tmpAngle)),0),
                                                new Vector4(0, (float)Math.Sin(tmpAngle), (float)Math.Cos(tmpAngle),0),
                                                new Vector4(0,0,0,1)
                                                );
                    // pick angle and make rotation matrix
                    Matrix4 rotMatY = new Matrix4(new Vector4((float)Math.Cos(tmpAngle), 0, 0, (float)Math.Sin(tmpAngle)),
                                                  new Vector4(0, 1, 0,0),
                                                  new Vector4((float)-Math.Cos(tmpAngle),0,(float)Math.Sin(tmpAngle),1),
                                                  new Vector4 (0,0,0,1)
                                                  );
                                                    


                    // pick a new angle and make another rotation matrix
                    tmpAngle = random.NextDouble()*spawnFrustum-spawnFrustum/2;
                    Matrix4 rotMatZ = new Matrix4(new Vector4((float)Math.Cos(tmpAngle), (float)Math.Sin(tmpAngle),0, 0),
                                                new Vector4(-(float)Math.Sin(tmpAngle), (float)Math.Cos(tmpAngle), 0, 0),
                                                new Vector4((float)-Math.Sin(tmpAngle),(float)Math.Cos(tmpAngle),0,0),
                                                new Vector4(0,0,0,1)
                                                );
 
                    // rotate the vector
                    Vector4 velocity4 = Vector4.Transform((new Vector4(frustumDir.X, frustumDir.Y, frustumDir.Z, 1.0f)), transMatToOrigin * rotMatX * rotMatY * rotMatZ);
                    //make it a 3 vec for the patricle constructor
                    Vector3 velocity3 = new Vector3(velocity4.X, velocity4.Y, velocity4.Z);
                    // Spawn the particle
                    particleList.Add(new Particle(velocity3, emitterPos, meanLiveTime));
                }
            }
            foreach (Particle p in particleList)
            {
                if (p.MoveAndDie())
                {
                    particleList.Remove(p);  
                }
            }
        
        }
    }

    class Particle
    {
        private Vector3 pVelocity;
        private Vector3 pPosition;
        private TimeSpan pLiveTime;
        private DateTime pBirthTime;
        private static Random random;

        //empty constructor, not to be used for real
        public Particle()
        {

        }
       
        public Particle(Vector3 velocity, Vector3 position, TimeSpan liveTime)
        {
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
            pPosition += pVelocity;
            if (pBirthTime - DateTime.Now >= pLiveTime)
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
