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

        void tick()
        {
            if (prevTime.Add(new TimeSpan(0, 0, 1)) <= DateTime.Now)
            {
                prevTime = DateTime.Now;
                Vector3 velocity = new Vector3();
                Vector4 tmpPos = new Vector4(frustumDir.X, frustumDir.Y, frustumDir.Z, 1.0f);
                Matrix4 transMat = new Matrix4(new Vector4(1,0,0,emitterPos.X),
                                    new Vector4(0,1,0,emitterPos.Y),
                                    new Vector4(0,0,1,emitterPos.Z),
                                    new Vector4(0,0,0,1));
                
                for (int i = 0; i <= spawnRate; i++)
                {
                    double tmpAngle = random.NextDouble() * spawnFrustum - spawnFrustum/2;

                    Matrix4 rotMatX = new Matrix4(new Vector4(1,0,0,0),
                                    new Vector4(0, (float)Math.Cos(tmpAngle), (float)-(Math.Sin(tmpAngle)),0),
                                    new Vector4(0, (float)Math.Sin(tmpAngle), (float)Math.Cos(tmpAngle),0),
                                    new Vector4(0,0,0,1));

                    //OBS! ANNAN TMPANGLE HÄR!!!
                    Matrix4 rotMatY = new Matrix4(new Vector4((float)Math.Cos(tmpAngle),0,(float)Math.Sin(tmpAngle),0),
                                    new Vector4(0,1,0,0),
                                    new Vector4((float)-Math.Sin(tmpAngle),0,(float)Math.Cos(tmpAngle),0),
                                    new Vector4(0,0,0,1));
                    
                    particleList.Add(new Particle(new Vector3(1.0f,1.0f,1.0f), emitterPos, meanLiveTime));
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
    }
}
