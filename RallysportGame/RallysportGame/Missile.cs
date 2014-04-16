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
        

        public Missile(String name, Car car)
            : base(name, car.getCarPos())
        {

        }

        public void fireMissile()
        {

        }
        
    }
}
