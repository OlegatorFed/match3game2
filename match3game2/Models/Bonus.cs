using match3game2.Enums;
using Microsoft.Xna.Framework;

namespace match3game2.Models
{
    internal class Bonus : Gem
    {

        private BonusType _bonusType;

        public Bonus(Point position, Colors color, BonusType bonusType) : base(position, color)
        {
            _bonusType = bonusType;
        }

        public BonusType GetBonusType()
        {
            return _bonusType;
        }

        public void Action(Point position)
        {

        }
    }
}
