using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Utilities.Renderers
{
    internal abstract class Renderer
    {

        protected SpriteBatch _spriteBatch;
        protected Texture2D _texture;
        protected ContentManager _content;
        private SpriteBatch spriteBatch;

        public Renderer(SpriteBatch spriteBatch, ContentManager content)
        {

            _spriteBatch = spriteBatch;
            _content = content;

        }

        protected Renderer(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public abstract void Render(Vector2 position);
    }
}
