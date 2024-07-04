using match3game2.Builders;
using match3game2.Configurations;
using match3game2.Models;
using Microsoft.Xna.Framework;

namespace match3game2.Controllers
{
    internal class GridController
    {

        private Grid _grid;
        private GridConfiguration _configuration;
        private GridBuilder _gridBuilder;

        public GridController(ConfigurationManager configurationManager, GridBuilder gridBuilder) 
        {
            _configuration = configurationManager.GridConfiguration;
            _gridBuilder = gridBuilder;

            _grid = _gridBuilder.Build();
        }

        public void Fill()
        {
            for (int i = 0; i < _configuration.Width; i++)
            {
                for (int j = 0; j < _configuration.Height; j++)
                {
                    _grid.Gems[i].Add(new Gem());
                }
            }
        }

        public Gem GetGem(Point point)
        {
            return _grid.Gems[point.X][point.Y];
        }

        public void UpdateGem(Point point, Gem other)
        {
            _grid.Gems[point.X][point.Y] = other;
        }

        public void RemoveGem(Point point)
        {
            _grid.Gems[point.X][point.Y] = null;
        }

    }
}
