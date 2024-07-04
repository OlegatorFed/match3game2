using match3game2.Utilities.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using static System.Formats.Asn1.AsnWriter;

namespace match3game2
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

        public override void Render(Vector2 position)
        {

            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, position, Color.White);
            _spriteBatch.End();
            /*_spriteBatch.DrawString(
                _font,
                _label,
                _position - new Vector2(_font.MeasureString(_label).X / 2, _font.MeasureString(_label).Y / 2),
                Color.Black);*/
        }
    }
}
