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
        private Vector3 velocity;
        Car shooter;

        public Missile(String name, Car car)
            : base(name, car.getCarPos())
        {
            shooter = car;
        }

        public void fireMissile()
        {
            velocity = shooter.getVelocity() * 2;
        }
        
    }
}
