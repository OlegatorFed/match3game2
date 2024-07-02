using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Models
{
    internal class Grid
    {

        private List<List<Gem>> Gems;

        public Grid(int width, int height)
        {

            Gems = new List<List<Gem>>(height);
            for (int i = 0; i < width; i++)
            {
                Gems.Add(new List<Gem>(width));
            }
        }

    }
}
