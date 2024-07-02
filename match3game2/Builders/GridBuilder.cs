using match3game2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
