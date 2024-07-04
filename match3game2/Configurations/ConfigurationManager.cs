namespace match3game2.Configurations
{
    internal class ConfigurationManager
    {

        public readonly GridConfiguration GridConfiguration;
        public int GameTime { get; private set; }



        public ConfigurationManager()
        {

            GridConfiguration = new GridConfiguration(8, 8);
            GameTime = 60;

        }

        public ConfigurationManager(int gridWidth, int gridHeight, int gameTime)
        {

            GridConfiguration = new GridConfiguration(gridWidth, gridHeight);
            GameTime = gameTime;

        }

    }
}
