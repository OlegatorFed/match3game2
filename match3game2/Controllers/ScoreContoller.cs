using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Controllers
{
    internal class ScoreContoller
    {

        public int Score { get; private set; }

        public ScoreContoller() 
        { 
            Score = 0;
        }

        public void AddScore(int score)
        {
            Score += score;
        }

        public void ResetScore(int score)
        {
            Score = 0;
        }

    }
}
