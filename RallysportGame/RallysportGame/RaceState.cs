﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
