using match3game2.Builders;
using match3game2.Models;
using match3game2.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace match3game2.Controllers
{
    internal class MenuController
    {

        private bool _active;
        private MouseHandler _mouseHandler;
        private GameController _gameController;
        private ButtonBuilder _buttonBuilder;
        private List<Button> _buttons;
        
        public MenuController(GameController gameController, ButtonBuilder buttonBuilder, MouseHandler mouseHandler) 
        {
            _active = true;
            _mouseHandler = mouseHandler;
            _gameController = gameController;
            _buttonBuilder = buttonBuilder;

            mouseHandler.MousePressed += OnClick;
        }

        public void Play()
        {

        }

        public void SetActive(bool state) { _active = state; }

        public void OnClick(Vector2 position)
        {

        }

    }
}
