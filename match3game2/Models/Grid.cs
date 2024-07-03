using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Models
{
    internal class Grid
    {

        public List<List<Gem>> Gems;

        public Grid(int width, int height)
        {

            Gems = new List<List<Gem>>(width);
            for (int i = 0; i < height; i++)
            {
                Gems.Add(new List<Gem>(height));
            }
        }

    }
}
