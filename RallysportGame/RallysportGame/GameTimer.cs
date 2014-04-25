using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RallysportGame
{
    static class GameTimer
    {
        static private DateTime gameTimer;
        static private DateTime leaderTime;
        static private DateTime countDownTarget;

        static private TimeSpan timeLeft;

        static private int previousTimeDiff,timeDiff;

        static public void countDown(int seconds)
        {
            countDownTarget = DateTime.Now;
            timeLeft = new TimeSpan(0, 0, seconds);
            countDownTarget=countDownTarget.Add(timeLeft);
        }

        static public void externalCountDown(int seconds, string leaderTimeString)
        {
            leaderTime = DateTime.Parse(leaderTimeString);
        }

        static public void tick()
        {
            gameTimer = DateTime.Now;
            timeDiff = countDownTarget.Subtract(DateTime.Now).Seconds;
            if (timeDiff != previousTimeDiff && timeDiff >=0)
            {
                Console.WriteLine("Time left: " + timeDiff);
                previousTimeDiff = timeDiff;
            }
            if(timeDiff == 0)
            {
                RaceState.setCurrentState(RaceState.States.RACING);
            }
        }
    }
}
