using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RallysportGame
{
    class GameTimer
    {
        private DateTime gameTimer;
        private DateTime leaderTime;
        private DateTime countDownTarget;

        private TimeSpan timeLeft;

        private int previousTimeDiff,timeDiff;

        public GameTimer()
        {

        }

        public void countDown(int seconds)
        {
            countDownTarget = DateTime.Now;
            timeLeft = new TimeSpan(0, 0, seconds);
            countDownTarget=countDownTarget.Add(timeLeft);
        }

        public void externalCountDown(int seconds, string leaderTimeString)
        {
            leaderTime = DateTime.Parse(leaderTimeString);
        }

        public void tick()
        {
            gameTimer = DateTime.Now;
            timeDiff = countDownTarget.Subtract(DateTime.Now).Seconds;
            if (timeDiff != previousTimeDiff && timeDiff >=0)
            {
                Console.WriteLine("Time left: " + timeDiff);
                previousTimeDiff = timeDiff;
            }
        }
    }
}
