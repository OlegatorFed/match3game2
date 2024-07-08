using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace match3game2.Models
{
    internal class Button
    {

        private string _tag;
        private Point _position;
        private Point _size;
        private string _label;
        private Action _action;
        private bool _active;

        public Button(string tag, Point position, Point size, string label, Action action) 
        {
            
            _position = position;
            _size = size;
            _label = label;
            _action = action;
            _active = true;
        }

        public static bool CheckIntersection(Vector2 point, Button button)
        {
            return point.X > button._position.X &&
                point.Y > button._position.Y &&
                point.X < button._position.X + button._size.X &&
                point.Y < button._position.Y + button._size.Y;
        }

        public bool IsActive() {  return _active; }

        public void SetActive(bool state) { _active = state; }

        public string GetTag() { return _tag; }

        public Point GetPosition() { return _position; }

        public Point GetSize() { return _size;}

        public string GetLabel() { return _label; }

        public void Press()
        {
            _action.Invoke();
        }

        public void Render(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font)
        {
            spriteBatch.Draw(
                texture,
                new Rectangle(GetPosition(), GetSize()),
                Color.Gray
                );
            spriteBatch.DrawString(
                font,
                _label,
                new Vector2(GetPosition().X + GetSize().X / 2 - font.MeasureString(_label).X / 2, GetPosition().Y + GetSize().Y / 2 - font.MeasureString(_label).Y / 2),
                Color.Black
                );
        }

    }
}
