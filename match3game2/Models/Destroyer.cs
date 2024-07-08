using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace match3game2.Models
{
    internal class Destroyer
    {

        public Point Position;
        public Point Destination;
        public float Scale = 1f;

        public Action<Point> Moved;

        private int _vDirection;
        private int _hDirection;

        public Destroyer(Point position, Point destination) 
        {
            Position = position;
            Destination = destination;

            _vDirection = Math.Sign(destination.Y - position.Y);
            _hDirection = Math.Sign(destination.X - position.X);
        }

        public string GetDirection()
        {
            if (_vDirection == 0)
                if (_hDirection < 0)
                    return "left";
                else
                    return "right";
            else 
                if (_vDirection < 0)
                    return "up";
                else
                    return "down";
        }
            

        public void Render(SpriteBatch spriteBatch, Texture2D texture, int size)
        {
            spriteBatch.Draw(texture,
                new Rectangle(Position.X + (int)((1f - Scale) * size) / 2, Position.Y + +(int)((1f - Scale) * size) / 2, (int)(size * Scale), (int)(size * Scale)),
                Color.White);
        }

        public async Task Move()
        {
            int speed = 5;
            int hDistance = Math.Abs(Destination.X - Position.X);
            int vDistance = Math.Abs(Destination.Y - Position.Y);

            while (hDistance > speed && vDistance == 0 || vDistance > speed && hDistance == 0)
            {
                await Task.Delay(10);
                Position = new Point(Position.X + speed * _hDirection, Position.Y + speed * _vDirection);
                hDistance = Math.Abs(Destination.X - Position.X);
                vDistance = Math.Abs(Destination.Y - Position.Y);

                Moved?.Invoke(Position);
            }

            Position = Destination;

        }

    }
}
