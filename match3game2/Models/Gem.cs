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
        public float Scale = 1f;
        public BonusType? BonusType;
        protected Colors _color;

        public Gem(Point position, Colors color) 
        {
            
            Position = position;
            Destination = position;
            BonusType = null;
            _color = color;

        }

        public Gem(Point position, Colors color, BonusType bonusType)
        {

            Position = position;
            Destination = position;
            BonusType = bonusType;
            _color = color;

        }

        public Colors GetColor() { return _color; }

        public BonusType? GetBonusType()
        {
            return BonusType;
        }

        public void Render(SpriteBatch spriteBatch, Texture2D texture, int size)
        {
            spriteBatch.Draw(texture,
                new Rectangle(Position.X + (int)((1f - Scale) * size) / 2, Position.Y + +(int)((1f - Scale) * size) / 2, (int)(size * Scale), (int)(size * Scale)),
                Color.White);
        }

        public void Render(SpriteBatch spriteBatch, Texture2D texture, Texture2D bonusTexture, int size)
        {
            spriteBatch.Draw(texture,
                new Rectangle(Position.X + (int)((1f - Scale) * size) / 2, Position.Y + +(int)((1f - Scale) * size) / 2, (int)(size * Scale), (int)(size * Scale)),
                Color.White);
            spriteBatch.Draw(bonusTexture,
                new Rectangle(Position.X + (int)((1f - Scale) * size) / 2, Position.Y + +(int)((1f - Scale) * size) / 2, (int)(size * Scale), (int)(size * Scale)),
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

        public async Task ScaleTo(int target)
        {
            float speed = 0.1f;
            int sign = MathF.Sign(target - Scale);


            while (Scale > target && Scale - target > target || Scale < target && Scale + speed < target)
            {
                await Task.Delay(10);
                Scale += sign * speed;
            }

            Scale = target;
        }
    }
}
