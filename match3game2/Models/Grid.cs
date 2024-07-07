using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace match3game2.Models
{
    internal class Grid
    {

        public Point Position;
        public List<List<Gem>> Gems;
        public int GemSize;

        public Grid(Point position, int width, int height, int gemSize)
        {

            Position = position;
            GemSize = gemSize;
            Gems = new List<List<Gem>>(width);
            for (int i = 0; i < height; i++)
            {
                Gems.Add(new List<Gem>(height));
            }
        }

    }
}
