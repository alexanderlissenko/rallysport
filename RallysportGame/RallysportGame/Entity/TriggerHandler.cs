using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RallysportGame
{
    /// <summary>
    /// Handles the triggers that are added
    /// </summary>
    static class TriggerHandler
    {
        static string[] splitString;
        static ArrayList passedCheckPoint = new ArrayList();
        static int nrCheckpoints = 0;
        static bool goalUnlocked = false;
        static Car car;
        public static void triggerEvent(string triggerSender,string triggerCauser)
        {
            splitString = triggerSender.Split(' ');
            switch (splitString[0])
            {
                case "goal":
                    handleGoal(triggerCauser);
                    break;
                case "checkpoint":
                    handleCheckpoint(splitString[1]);
                    break;
                case "powerUp":
                    handlePowerUp();
                    break;
            }
        }

        private static void handleGoal(string triggerCauser)
        {
            if (goalUnlocked)
            {
                Console.WriteLine(triggerCauser + " crossed the Goal!");
                RaceState.setCurrentState(RaceState.States.ENDING);
            }
            else
                Console.WriteLine("Goal not unlocked");
        }

        private static void handleCheckpoint(string maxCheckpoints)
        {
            if(!passedCheckPoint.Contains(nrCheckpoints))
            {
                passedCheckPoint.Add(nrCheckpoints);
                nrCheckpoints++;
                Console.WriteLine("Checkpoint " + nrCheckpoints);
            }
            if(passedCheckPoint.Count == int.Parse(maxCheckpoints))
            {
                goalUnlocked = true;
            }
        }

        public static void connectCar(ref Car coolCar)
        {
            car = coolCar;
        }

        private static void handlePowerUp()
        {
            Random randomGen = new Random();

            switch(randomGen.Next(4))
            {
                case 0:
                    Console.WriteLine("Missile!");
                    car.addPowerUp("Missile");
                    break;
                case 1:
                    Console.WriteLine("Boost!");
                    car.addPowerUp("SpeedBoost");
                    break;
                case 2:
                    Console.WriteLine("SpeedBoost");
                    break;
                case 3:
                    Console.WriteLine("SmookeScreen!");
                    car.addPowerUp("SmookeScreen");
                    break;
                default:
                    Console.WriteLine("Not added");
                    break;

            }

        }

    }
}
