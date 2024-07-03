using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match3game2.Controllers
{
    internal class MenuController
    {

        private bool _active;
        
        public MenuController() 
        {
            _active = true;
        }

        public void Play()
        {

        }

        public void SetActive(bool state) { _active = state; }

    }
}
