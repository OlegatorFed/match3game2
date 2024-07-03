using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace match3game2
{
    internal class MouseHandler
    {

        public Vector2 MousePosition { get; private set; }
        
        private MouseState _mouseState;
        private bool _mouseDown;

        public MouseHandler()
        {

            _mouseState = Mouse.GetState();
            _mouseDown = false;

        }

        public void Update()
        {
            _mouseState = Mouse.GetState();
            MousePosition = GetMousePosition();
            CheckClick();
        }

        public Vector2 GetMousePosition() { return new Vector2(_mouseState.X, _mouseState.Y); }

        public void CheckClick()
        {
            if (!_mouseDown && _mouseState.LeftButton == ButtonState.Pressed)
            {
                _mouseDown = true;
            }
            else
            {
                _mouseDown = false;
            }
        }

    }
}
