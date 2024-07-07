using match3game2.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace match3game2.Models
{
    internal class Gem
    {

        public Point Position;
        public Point Destination;
        private Colors _color;

        public Gem(Point position, Colors color) 
        {
            
            Position = position;
            Destination = position;
            _color = color;

        }

        public Colors GetColor() { return _color; }

        public void Render(SpriteBatch spriteBatch, Texture2D texture, int size)
        {
            spriteBatch.Draw(texture,
                new Rectangle(Position.X, Position.Y, size, size),
                Color.White);
        }

        public async Task Move()
        {
            int speed = 5;
            int hDirection = Math.Sign(Destination.X - Position.X);
            int vDirection = Math.Sign(Destination.Y - Position.Y);
            int hDistance = Math.Abs(Destination.X - Position.X);
            int vDistance = Math.Abs(Destination.Y - Position.Y);

            while (hDistance > speed && vDistance == 0 || vDistance > speed && hDistance == 0)
            {
                await Task.Delay(10);
                Position = new Point(Position.X + speed * hDirection, Position.Y + speed * vDirection);
                hDistance = Math.Abs(Destination.X - Position.X);
                vDistance = Math.Abs(Destination.Y - Position.Y);
            }
            
            Position = Destination;

        }

    }
}
