using Microsoft.Xna.Framework;

namespace match3game2.Configurations
{
    internal class ConfigurationManager
    {

        public readonly GridConfiguration GridConfiguration;
        public int GameTime { get; private set; }



        public ConfigurationManager()
        {

            GridConfiguration = new GridConfiguration(new Point(50, 50), 8, 8, 50);
            GameTime = 60;

        }

        public ConfigurationManager(Point position, int gridWidth, int gridHeight, int gameTime, int gemSize)
        {

            GridConfiguration = new GridConfiguration(position, gridWidth, gridHeight, gemSize);
            GameTime = gameTime;

        }

    }
}
