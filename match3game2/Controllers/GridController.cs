using match3game2.Builders;
using match3game2.Configurations;
using match3game2.Enums;
using match3game2.Models;
using match3game2.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace match3game2.Controllers
{
    internal class GridController
    {

        private Grid _grid;
        private GridConfiguration _configuration;
        private GridBuilder _gridBuilder;
        private MouseHandler _mouseHandler;

        private Colors[] _colors;

        private Point? _selectedPosition;

        public GridController(ConfigurationManager configurationManager, GridBuilder gridBuilder, MouseHandler mouseHandler) 
        {
            _configuration = configurationManager.GridConfiguration;
            _gridBuilder = gridBuilder;
            _mouseHandler = mouseHandler;

            _selectedPosition = null;

            _colors = (Colors[])Enum.GetValues(typeof(Colors));

            _grid = _gridBuilder.Build();
        }

        public void Fill()
        {
            for (int i = 0; i < _configuration.Width; i++)
            {
                for (int j = 0; j < _configuration.Height; j++)
                {
                    _grid.Gems[i].Add(new Gem(GetRandomColor()));
                }
            }
        }

        public Gem GetGem(Point point)
        {
            return _grid.Gems[point.X][point.Y];
        }

        public void UpdateGem(Point point, Gem other)
        {
            _grid.Gems[point.X][point.Y] = other;
        }

        public void RemoveGem(Point point)
        {
            _grid.Gems[point.X][point.Y] = null;
        }

        public void OnClick(Vector2 position) 
        {
            Point gridPosition;

            if (GetGemPositionClicked(position, out gridPosition))
            {
                SelectGem(gridPosition);
            }
        }

        private bool GetGemPositionClicked(Vector2 position, out Point gridPosition)
        {
            gridPosition = GetGridPosition(position);

            return position.X > _configuration.Position.X && 
                position.X < _configuration.Position.X + _configuration.Width * _configuration.GemSize &&
                position.Y > _configuration.Position.Y &&
                position.Y < _configuration.Position.Y + _configuration.Height * _configuration.GemSize;
        }

        private Point GetGridPosition(Vector2 position) { return new Point((int)(position.X - _configuration.Position.X), (int)(position.Y - _configuration.Position.Y)); }

        private void SelectGem(Point position)
        {

            if (_selectedPosition != null)
                SwapGems((Point)_selectedPosition, position);
            else
                _selectedPosition = position;
        }

        private void SwapGems(Point firstGem, Point secondGem)
        {
            if (CanSwap(firstGem, secondGem))
            {
                (_grid.Gems[firstGem.X][firstGem.Y], _grid.Gems[secondGem.X][secondGem.Y]) =
                    (_grid.Gems[secondGem.X][secondGem.Y], _grid.Gems[firstGem.X][firstGem.Y]);
            }
        }

        private bool CanSwap(Point firstGem, Point secondGem) { 

            return Math.Abs(firstGem.X - secondGem.X) == 1 && firstGem.Y == secondGem.Y ||
                firstGem.X == secondGem.X && Math.Abs(firstGem.Y - secondGem.Y) == 1; 

        }

        private Colors GetRandomColor()
        {
            Random random = new Random();

            return _colors[random.Next(_colors.Length)];

        }

    }
}
