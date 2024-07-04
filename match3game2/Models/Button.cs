using Microsoft.Xna.Framework;
using System;

namespace match3game2.Models
{
    internal class Button
    {

        private Vector2 _position;
        private string _label;
        private Action _action;

        public Button(Vector2 position, string label, Action action) 
        {
            _position = position;
            _action = action;
        }

        public void Press()
        {
            _action.Invoke();
        }

    }
}
