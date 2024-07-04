using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace match3game2.Utilities
{
    internal class MouseHandler
    {

        public Vector2 MousePosition { get; private set; }

        private MouseState _mouseState;
        private bool _mouseDown;

        public Action<Vector2> MousePressed;

        public MouseHandler()
        {

            _mouseState = Mouse.GetState();
            _mouseDown = false;

        }

        public void Update()
        {
            _mouseState = Mouse.GetState();
            MousePosition = GetMousePosition();
            OnClick();
        }

        private bool CheckClick()
        {
            if (_mouseState.LeftButton == ButtonState.Pressed)
                return true;

            return false;
        }
        private Vector2 GetMousePosition() { return new Vector2(_mouseState.X, _mouseState.Y); }

        private void OnClick()
        {
            if (CheckClick() && !_mouseDown)
            {
                _mouseDown = true;
                MousePressed?.Invoke(GetMousePosition());
            }
            else
                _mouseDown = false;
        }

    }
}
