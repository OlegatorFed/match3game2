using Microsoft.Xna.Framework;

namespace match3game2.Configurations
{
    internal class ConfigurationManager
    {

        public readonly GridConfiguration GridConfiguration;
        public int GameTime { get; private set; }



        public ConfigurationManager()
        {

            GridConfiguration = new GridConfiguration(new Vector2(300, 300), 8, 8, 50);
            GameTime = 60;

        }

        public ConfigurationManager(Vector2 position, int gridWidth, int gridHeight, int gameTime, float gemSize)
        {

            GridConfiguration = new GridConfiguration(position, gridWidth, gridHeight, gemSize);
            GameTime = gameTime;

        }

    }
}
