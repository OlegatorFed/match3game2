using match3game2.Builders;
using match3game2.Models;
using match3game2.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace match3game2.Controllers
{
    internal class MenuController
    {

        public Action StartAction;
        public Action ResetAction;

        private bool _active;
        private MouseHandler _mouseHandler;
        //private GameController _gameController;
        private Button _startButton;
        private Button _resetButton;

        private Texture2D _texture;
        private SpriteFont _font;

        public MenuController(MouseHandler mouseHandler)
        {
            _active = true;
            _mouseHandler = mouseHandler;

            _startButton = new Button("start", new Point(100, 100), new Point(100, 50), "Start", Play);
            _resetButton = new Button("reset", new Point(100, 100), new Point(100, 50), "Ok", Reset);

            mouseHandler.MousePressed += OnClick;

            Reset();
        }

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("fonts/font");
            _texture = content.Load<Texture2D>("textures/white_box_50px");
        }

        public void Play()
        {
            _startButton.SetActive(false);
            _resetButton.SetActive(false);
            _active = false;
            StartAction.Invoke();
        }

        public void GameOver()
        {
            _active = true;
            _resetButton.SetActive(true);
            _startButton.SetActive(false);
        }

        public void Reset()
        {
            _active = true;
            _resetButton.SetActive(false);
            _startButton.SetActive(true);
            ResetAction?.Invoke();
        }

        public bool IsActive() { return _active; }

        public void SetActive(bool state) { _active = state; }

        public void OnClick(Vector2 position) { CheckButtonClicked(position); }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!_active) return; 

            if (_startButton.IsActive())
                _startButton.Render(spriteBatch, _texture, _font);

            if (_resetButton.IsActive())
                _resetButton.Render(spriteBatch, _texture, _font);
        }

        private void CheckButtonClicked(Vector2 position)
        {

            if (!_active) return;

            if (_startButton.IsActive() && Button.CheckIntersection(position, _startButton))
            {
                _startButton.Press();
            }

            if (_resetButton.IsActive() && Button.CheckIntersection(position, _resetButton))
            {
                _resetButton.Press();
            }

        }

    }
}
