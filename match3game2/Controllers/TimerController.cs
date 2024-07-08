using System;
using System.Threading.Tasks;
using match3game2.Configurations;

namespace match3game2.Controllers
{
    internal class TimerController
    {

        public int TimeLeft { get; private set; }

        private Action _action;

        public TimerController(ConfigurationManager configurationManager, Action action) 
        {
            TimeLeft = configurationManager.GameTime;
            _action = action;
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

            FinishTimer();
        }

        public void FinishTimer()
        {
            _action.Invoke();
        }

        public void SetTimer(int time) { TimeLeft = time; }

    }
}
