using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
