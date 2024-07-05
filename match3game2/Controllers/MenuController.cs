using match3game2.Builders;
using match3game2.Models;
using match3game2.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace match3game2.Controllers
{
    internal class MenuController
    {

        private bool _active;
        private MouseHandler _mouseHandler;
        private GameController _gameController;
        private Button _startButton;
        private Button _resetButton;

        public MenuController(GameController gameController, MouseHandler mouseHandler)
        {
            _active = true;
            _mouseHandler = mouseHandler;
            _gameController = gameController;


            _startButton = new Button("start", new Vector2(200, 200), new Vector2(50, 100), "Start", Play);
            _resetButton = new Button("reset", new Vector2(200, 200), new Vector2(50, 100), "Ok", Reset);

            mouseHandler.MousePressed += OnClick;
        }

        public void Play()
        {
            _startButton.SetActive(false);
            _resetButton.SetActive(false);
            _active = false;
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
        }

        public bool IsActive() { return _active; }

        public void SetActive(bool state) { _active = state; }

        public void OnClick(Vector2 position) { CheckButtonClicked(position); }

        private void CheckButtonClicked(Vector2 position)
        {

            if (_active)
            {
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
}
