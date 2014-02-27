using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using OpenTK;

namespace RallysportGame
{
    class CollisionHandler
    {
        private Box car;
        private Space space;
        private List<Car> cars;

        public CollisionHandler()
        {
            cars = new List<Car>();
            space = new Space();
            Box ground = new Box(Vector3.Zero, 30, 1, 30);
            space.Add(ground);
            //car = new Box(new Vector3(0, 4, 0), 1, 1, 1, 1);
            //space.Add(car);
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
        }
        public void update()
        {
            foreach(Car car in cars)
            {
                System.Console.WriteLine(car.position); //TODO this is a temporary physics tester
                car.update();
            }
            space.Update();
        }
        public void addCar(Car car)
        {
            cars.Add(car);
            space.Add(car.boundingBox);
        }
    }

}
