using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame
{
    /// <summary>
    /// Handles the triggers that are added
    /// </summary>
    static class TriggerHandler
    {
        public static void triggerEvent(string triggerSender,string triggerCauser)
        {
            switch(triggerSender)
            {
                case "goal":
                    handleGoal(triggerCauser);
                    break;
                case "powerUp":
                    handlePowerUp();
                    break;
            }
        }

        private static void handleGoal(string triggerCauser)
        {
            Console.WriteLine(triggerCauser + " crossed the Goal!");
        }

        private static void handlePowerUp()
        {
            Random randomGen = new Random();

            switch(randomGen.Next(3))
            {
                case 0:
                    Console.WriteLine("Missile!");
                    break;
                case 1:
                    Console.WriteLine("Boost!");
                    break;
                case 2:
                    Console.WriteLine("Lights out!");
                    break;
                default:
                    Console.WriteLine("Not added");
                    break;
            }

        }

    }
}
