using match3game2.Configurations;
using match3game2.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace match3game2.Controllers
{
    internal class GameController
    {

        private int _gameState;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private BatchHandle _batchHandle;
        private MouseHandler _mouseHandler;
        private GridController _gridController;
        private MenuController _menuController;
        private TimerController _timerController;
        private ScoreContoller _scoreContoller;
        private ConfigurationManager _configurationManager;

        private SpriteFont _font;
        private Dictionary<string, Texture2D> _gemTextures;


        public GameController(GridController gridController, MenuController menuController, ConfigurationManager configurationManager, MouseHandler mouseHandler) 
        {
            
            _gridController = gridController;
            _menuController = menuController;
            _configurationManager = configurationManager;
            _mouseHandler = mouseHandler;

            _timerController = new TimerController(configurationManager, GameOver);
            _scoreContoller = new ScoreContoller();

            _gemTextures = new Dictionary<string, Texture2D>();

            _gameState = 0;

            _gridController.Fill();

            _menuController.StartAction = StartGame;
            _menuController.ResetAction = ResetGame;

            _gridController.Scored += OnScore;

            ResetGame();

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
            _gridController.LoadContent(_content);
            _menuController.LoadContent(_content);
        }

        public int GetGameState() { return _gameState; }

        public void StartGame() 
        {
            _gameState = 1; 
            _timerController.StartTimer();
            _scoreContoller.ResetScore();
            _gridController.SetActive(true);
            _gridController.Fill();
            _gridController.FindMatches();
        }

        public void GameOver() 
        { 
            _gameState = 2;
            _menuController.GameOver();
            _gridController.SetActive(false);
        }

        public void ResetGame() 
        { 
            _gameState = 0;
            _timerController.SetTimer(_configurationManager.GameTime);
            _scoreContoller.ResetScore();
            _gridController.SetActive(false);
            _gridController.Reset();
        }

        public void Update()
        {
            _mouseHandler.Update();
        }

        public void Render(GraphicsDeviceManager graphics)
        {
            int timeLeft = _timerController.TimeLeft;
            int score = _scoreContoller.Score;

            if (_spriteBatch == null)
            {
                return;
            }
            else
            {
                _spriteBatch.DrawString(
                    _font,
                    $"{_mouseHandler.MousePosition}",
                    _mouseHandler.MousePosition, Color.Black);
                switch (_gameState)
                {
                    case 0:
                        _menuController.Render(_spriteBatch);
                        break;
                    case 1:
                        _gridController.Render(_spriteBatch);
                            _spriteBatch.DrawString(
                            _font,
                            $"{timeLeft}",
                            Vector2.Zero,
                            Color.Black);

                        _spriteBatch.DrawString(
                            _font,
                            $"{score}",
                            new Vector2(graphics.PreferredBackBufferWidth / 2 - _font.MeasureString(score.ToString()).X / 2, 0),
                            Color.Black);
                        break;
                    case 2:
                        _menuController.Render(_spriteBatch);
                        break;
                }
            }
        }

        private void OnScore()
        {
            _scoreContoller.AddScore(100);
        }

    }
}
