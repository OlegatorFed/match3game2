using System;

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

        public void ResetScore()
        {
            Score = 0;
        }

    }
}
