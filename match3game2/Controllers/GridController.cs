using match3game2.Builders;
using match3game2.Configurations;
using match3game2.Enums;
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
    internal class GridController
    {
        public Dictionary<string, Texture2D> GemTextures;

        private bool _active;

        private Grid _grid;
        private GridConfiguration _configuration;
        private GridBuilder _gridBuilder;
        private MouseHandler _mouseHandler;

        private Colors[] _colors;

        public Point? _selectedPosition;
        private HashSet<Point> _gemsToDestroy;

        public GridController(ConfigurationManager configurationManager, GridBuilder gridBuilder, MouseHandler mouseHandler) 
        {
            _configuration = configurationManager.GridConfiguration;
            _gridBuilder = gridBuilder;
            _mouseHandler = mouseHandler;

            _selectedPosition = null;
            _gemsToDestroy = new HashSet<Point>();

            _colors = (Colors[])Enum.GetValues(typeof(Colors));

            _grid = _gridBuilder.Build();

            _mouseHandler.MousePressed += OnClick;

            _active = true;

        }

        public void Fill()
        {
            for (int i = 0; i < _configuration.Width; i++)
            {
                for (int j = 0; j < _configuration.Height; j++)
                {
                    _grid.Gems[i].Add(new Gem(new Point(i * _grid.GemSize, j * _grid.GemSize), GetRandomColor()));
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

        public Grid GetGrid() { return _grid; }

        public void OnClick(Vector2 position) 
        {
            if (!_active) return;

            Point gridPosition;

            if (GetGemPositionClicked(position, out gridPosition))
            {
                SelectGem(gridPosition);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!_active) return;

            for (int x = 0;  x < _configuration.Width; x++)
            {
                for (int y = 0; y < _configuration.Height; y++)
                {
                    _grid.Gems[x][y].Render(spriteBatch, GemTextures[Enum.GetName(_grid.Gems[x][y].GetColor())], _grid.GemSize, _grid.Position);
                }
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

        private Point GetGridPosition(Vector2 position) { return new Point((int)(position.X - _configuration.Position.X ) / _grid.GemSize, (int)(position.Y - _configuration.Position.Y) / _grid.GemSize); }

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

                (_grid.Gems[firstGem.X][firstGem.Y].Position, _grid.Gems[secondGem.X][secondGem.Y].Position) =
                    (_grid.Gems[secondGem.X][secondGem.Y].Position, _grid.Gems[firstGem.X][firstGem.Y].Position);
            }

            _selectedPosition = null;
        }

        private bool CanSwap(Point firstGem, Point secondGem) { 

            return Math.Abs(firstGem.X - secondGem.X) == 1 && firstGem.Y == secondGem.Y ||
                firstGem.X == secondGem.X && Math.Abs(firstGem.Y - secondGem.Y) == 1; 

        }

        public void FindMatches()
        {

            Point positionToCheck;
            List<Point> possibleMatch = new List<Point>();
            Colors? currentColor = null;

            //vertical matches
            for (int x = 0; x < _grid.Gems.Count; x++)
            {
                for (int y = 0; y < _grid.Gems[x].Count; y++)
                {
                    positionToCheck = new Point(x, y);
                    if (currentColor != _grid.Gems[x][y].GetColor())
                    {
                        if (possibleMatch.Count < 3)
                        {
                            possibleMatch.Clear();
                            possibleMatch.Add(positionToCheck);
                            currentColor = _grid.Gems[x][y].GetColor();
                        }
                        else
                        {
                            _gemsToDestroy.UnionWith(possibleMatch);
                            possibleMatch.Clear();
                            possibleMatch.Add(positionToCheck);
                            currentColor = _grid.Gems[x][y].GetColor();
                        }

                    }
                    else
                    {
                        possibleMatch.Add(positionToCheck);
                    }
                }
                currentColor = null;
                possibleMatch.Clear();
            }

            //horizontal matches
            for (int y = 0; y < _grid.Gems[0].Count; y++)
            {
                for (int x = 0; x < _grid.Gems.Count; x++)
                {
                    positionToCheck = new Point(x, y);
                    if (currentColor != _grid.Gems[x][y].GetColor())
                    {
                        if (possibleMatch.Count < 3)
                        {
                            possibleMatch.Clear();
                            possibleMatch.Add(positionToCheck);
                            currentColor = _grid.Gems[x][y].GetColor();
                        }
                        else
                        {
                            _gemsToDestroy.UnionWith(possibleMatch);
                            possibleMatch.Clear();
                            possibleMatch.Add(positionToCheck);
                            currentColor = _grid.Gems[x][y].GetColor();
                        }

                    }
                    else
                    {
                        possibleMatch.Add(positionToCheck);
                    }
                }
                currentColor = null;
                possibleMatch.Clear();
            }

        }

        private void ClearMatches()
        {

            

        }

        private Colors GetRandomColor()
        {
            Random random = new Random();

            return _colors[random.Next(_colors.Length)];

        }

    }
}
