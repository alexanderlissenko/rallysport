using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RallysportGame
{
    class Car : Entity
    {
        public Box boundingBox;

        //default constructor for car
        public Car(OpenTK.Vector3 position):base("Cube\\testCube", position)
        {
            boundingBox = new Box(position,1,1,1,1);
        }

        public void update()
        {
            position = boundingBox.Position;
        }
    }
}
