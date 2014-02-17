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
        private Vector3 spawnFrustum; //(angle,x-aspect,y-aspect)
        private Entity particleObject;
        private int spawnRate;
        private ArrayList particleList;
        private TimeSpan meanLiveTime;
        
        //empty constructor, not to be used for real
        public ParticleSystem() : this(new Vector3(0.0f, 0.0f, 0.0f), 
                                    new Vector3(360.0f, 1.0f, 1.0f), 20, new TimeSpan(0,0,10), null)
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
        public ParticleSystem(Vector3 pos, Vector3 frustum, int rate,
                        TimeSpan liveTime, Entity particle)
        {
            emitterPos = pos;
            spawnFrustum = frustum;
            spawnRate = rate;
            meanLiveTime = liveTime;
            particleObject = particle;

            int capacity = (int)Math.Ceiling(meanLiveTime.Seconds * spawnRate * 1.5); 
            particleList = new ArrayList(capacity); //might be bad, if memory seems suspicious, double check

        }

    }

    class Particle
    {
        private Vector3 pVelocity;
        private TimeSpan pLiveTime;
        private DateTime pBirthTime;
        private static Random random;
        public Particle(Vector3 velocity, TimeSpan liveTime)
        {
            pVelocity = velocity;
            double value = random.NextDouble() * 1.5 - 0.25;
            pLiveTime = new TimeSpan(0,0, Convert.ToInt32(liveTime.Seconds + value*liveTime.Seconds));
        }
    }
}
