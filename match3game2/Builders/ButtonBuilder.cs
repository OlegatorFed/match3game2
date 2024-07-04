using match3game2.Configurations;
using match3game2.Models;

namespace match3game2.Builders
{
    internal class ButtonBuilder
    {

        private ButtonCofiguration _buttonConfiguration;

        public ButtonBuilder(ConfigurationManager configurationManager) //refactor to support views
        {
        }

        public Button Build()
        {
            return new Button(_buttonConfiguration.position, _buttonConfiguration.label, _buttonConfiguration.action);
        }

    }
}
