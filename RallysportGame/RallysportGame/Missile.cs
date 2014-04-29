using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RallysportGame
{
    class Missile : DynamicEntity
    {
        
        /**
        * Referens till vilken bil som skjuter
        * (Missil-id)
        * Velocity (dubbel mot car, samma riktning?)
         * Position (car pos)
        **/
        private Vector3 missileVel;
        private float TTL;
        public Missile(String name, Vector3 pos,Vector3 orientation, Vector3 lin_vel, float ttl)
            : base(name)
        {
            TTL = ttl;

            base.position = Vector3.Add(pos,Vector3.Mult(orientation, 100)); // magic number is the distance from the car, the missile will spawn
            missileVel = lin_vel;
        }

        public override void render(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 lightPosition, Matrix4 lightViewMatrix, Matrix4 lightProjectionMatrix)
        {
            
            base.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.setCoordiants(base.position.X, base.position.Y, base.position.Z);
            base.firstPass(program, projectionMatrix, viewMatrix);
        }

        public bool update() {

            base.position = Vector3.Add(base.position, missileVel);
            missileVel = Vector3.Add(missileVel, Vector3.Mult(missileVel, 0.1f));

            

            Console.WriteLine(base.position);
            if (TTL-- <= 0)
                return true;

            return false;
        }
        
    }
}
