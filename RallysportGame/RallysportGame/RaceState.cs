using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RallysportGame
{
    /// <summary>
    /// Handles the states of the game, which will describe what the user can do
    /// </summary>
    static class  RaceState
    {
        public enum States: byte
        {
            PRESTART,
            RACING,
            ENDING
        }
        static States currentState = States.PRESTART;

        public static States getCurrentState()
        {
            return currentState;
        }

        public static void setCurrentState(States state)
        {
            currentState = state;
        }

        public static void nextState()
        {
            switch (currentState)
            {
                case States.PRESTART:
                    currentState = States.RACING;
                    break;
                case States.RACING:
                    currentState = States.ENDING;
                    break;
                case States.ENDING:
                    break;
                default:
                    break;
            }
        }

        public static void StartRace(Car playerCar,ref ArrayList carList)
        {
            int offset = 0;
            bool playerAdded = false;
            for (int i = 0; i < carList.Count; i++ )
            {
                if(Network.getInstance().getUserID() < (int)Network.getInstance().getUserList()[i]&& !playerAdded)
                {
                    playerCar.setCarPos(new Vector3(182, 2, -6 + i*2));
                    playerAdded = true;
                    offset = 3;
                }
                object o = carList[i];
                Car c = o as Car;
                c.setCarPos(new Vector3(182, 2, -6+ i*4 + offset));
            }
            if(!playerAdded)
            {
                playerCar.setCarPos(new Vector3(182, 2, -6 + 4*carList.Count));
            }
            GameTimer.countDown(5);
        }
    }
}
