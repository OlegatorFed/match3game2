using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace match3game2.Models
{
    internal class Grid
    {

        public Vector2 Position;
        public List<List<Gem>> Gems;
        public float GemSize;

        public Grid(Vector2 position, int width, int height, float gemSize)
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
