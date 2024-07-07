using match3game2.Configurations;
using match3game2.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace match3game2.Controllers
{
    internal class GameController
    {

        private int _gameState;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private BatchHandle _batchHandle;
        private GridController _gridController;
        private MenuController _menuController;
        private TimerController _timerController;
        private ScoreContoller _scoreContoller;
        private ConfigurationManager _configurationManager;

        private SpriteFont _font;
        private Texture2D _texture;
        private Dictionary<string, Texture2D> _gemTextures;


        public GameController(GridController gridController, MenuController menuController, ConfigurationManager configurationManager) 
        {
            
            _gridController = gridController;
            _menuController = menuController;
            _configurationManager = configurationManager;

            _gemTextures = new Dictionary<string, Texture2D>();

            _gameState = 1;

        }

        public void RecieveSpriteBatch(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public void RecieveContent(ContentManager content)
        {
            _content = content;
        }

        public void LoadContent()
        {
            _font = _content.Load<SpriteFont>("fonts/font");
            _texture = _content.Load<Texture2D>("textures/white_box_50px");
            

            _gridController.LoadContent(_content);
            _menuController.ButtonTexture = _texture;
        }

        public int GetGameState() { return _gameState; }

        public void Render()
        {
            if (_spriteBatch == null)
            {
                return;
            }
            else
            {
                switch (_gameState)
                {
                    case 0:
                        _menuController.Render(_spriteBatch);
                        break;
                    case 1:
                        _gridController.Render(_spriteBatch);
                        break;
                    case 2:
                        _menuController.Render(_spriteBatch);
                        break;
                }
            }
        }

    }
}
