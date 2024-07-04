using match3game2.Configurations;
using match3game2.Models;

namespace match3game2.Builders
{
    internal class GridBuilder
    {

        private GridConfiguration _gridConfiguration;

        public GridBuilder(ConfigurationManager configurationManager)
        {
            _gridConfiguration = configurationManager.GridConfiguration;
        }

        public Grid Build()
        {
            return new Grid(_gridConfiguration.Width, _gridConfiguration.Height);
        } 

    }
}
