using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using static System.Formats.Asn1.AsnWriter;
using match3game2.Models;

namespace match3game2.Utilities.Renderers
{
    internal class ButtonRenderer : Renderer
    {

        private SpriteFont _font;
        private string _label;

        public ButtonRenderer(SpriteBatch spriteBatch, ContentManager content) : base(spriteBatch, content)
        {

            _texture = _content.Load<Texture2D>("textures/white_box_50px"); //add button texture
            _font = _content.Load<SpriteFont>("fonts/font");

        }

        public override void Render<Button>(Button button)
        {

            /*_spriteBatch.Draw(_texture, button.GetPosition(), Color.White);
            _spriteBatch.DrawString(
                _font,
                button.GetLabel(),
                position - new Vector2(_font.MeasureString(_label).X / 2, _font.MeasureString(_label).Y / 2),
                Color.Black);*/
        }
    }
}
