using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Utilities
{
    internal class BatchHandle
    {

        private SpriteBatch _spriteBatch;

        public void Configure(SpriteBatch spriteBatch)
        {

            if (_spriteBatch == null)
                _spriteBatch = spriteBatch;

        }


        public SpriteBatch? GetSpriteBatchOrNull()
        {
            return _spriteBatch;
        } 

    }
}
