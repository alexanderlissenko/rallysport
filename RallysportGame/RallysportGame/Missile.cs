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
        Car shooter;

        public Missile(String name, ref Car car)
            : base(name, car.getCarPos())
        {
            shooter = car;
            //fireMissile();
        }

        public override void render(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 lightPosition, Matrix4 lightViewMatrix, Matrix4 lightProjectionMatrix)
        {
            base.render(program, projectionMatrix, viewMatrix, lightPosition, lightViewMatrix, lightProjectionMatrix);
        }

        public override void firstPass(int program, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            base.firstPass(program, projectionMatrix, viewMatrix);
        }

        public void fireMissile()
        {
            shooter.renderPActive();
            missileVel = shooter.getVelocity() * 2;
        }
        
    }
}
