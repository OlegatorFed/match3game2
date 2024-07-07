using match3game2.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace match3game2.Models
{
    internal class Gem
    {

        public Point Position;
        private Colors _color;

        public Gem(Point position, Colors color) 
        {
            
            Position = position;
            _color = color;

        }

        public Colors GetColor() { return _color; }

        public void Render(SpriteBatch spriteBatch, Texture2D texture, int size, Point gridPosition)
        {
            spriteBatch.Draw(texture,
                new Rectangle(Position.X + gridPosition.X, Position.Y + gridPosition.Y, size, size),
                Color.White);
        }

    }
}
