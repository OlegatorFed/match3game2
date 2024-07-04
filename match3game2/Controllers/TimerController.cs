using System;
using System.Threading.Tasks;
using match3game2.Configurations;

namespace match3game2.Controllers
{
    internal class TimerController
    {

        public int TimeLeft { get; private set; }

        public TimerController(ConfigurationManager configurationManager) 
        {
            TimeLeft = configurationManager.GameTime;
        }

        public async void StartTimer()
        {
            await Task.Run(() => CountDown());
        }

        public async void CountDown() 
        {
            while (TimeLeft > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                TimeLeft -= 1;
            }
        }

    }
}
