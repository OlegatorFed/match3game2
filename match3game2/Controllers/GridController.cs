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
using System.Threading.Tasks;

namespace match3game2.Controllers
{
    internal class GridController
    {


        private bool _active;
        private Dictionary<string, Texture2D> _gemTextures;
        private Texture2D _selectionTexture;

        private Grid _grid;
        private GridConfiguration _configuration;
        private GridBuilder _gridBuilder;
        private MouseHandler _mouseHandler;
        private ContentManager _content;

        private Colors[] _colors;

        private Point? _selectedPosition;
        private HashSet<Point> _gemsToDestroy;
        private List<Point> _movedGems;


        public GridController(ConfigurationManager configurationManager, GridBuilder gridBuilder, MouseHandler mouseHandler) 
        {
            _gemTextures = new Dictionary<string, Texture2D>();

            _configuration = configurationManager.GridConfiguration;
            _gridBuilder = gridBuilder;
            _mouseHandler = mouseHandler;
            _grid = _gridBuilder.Build();

            _selectedPosition = null;
            _gemsToDestroy = new HashSet<Point>();
            _movedGems = new List<Point>();


            _colors = (Colors[])Enum.GetValues(typeof(Colors));


            _mouseHandler.MousePressed += OnClick;

            _active = true;

        }

        public void Fill()
        {
            for (int i = 0; i < _configuration.Width; i++)
            {
                for (int j = 0; j < _configuration.Height; j++)
                {
                    _grid.Gems[i].Add(new Gem(new Point(i * _grid.GemSize + _grid.Position.X, j * _grid.GemSize + _grid.Position.Y), GetRandomColor()));
                }
            }
        }

        public void FillEmpty()
        {
            for (int x = 0; x < _configuration.Width; x++)
            {
                for (int y = 0; y < _configuration.Height; y++)
                {
                    if (_grid.Gems[x][y] != null) continue;
                    UpdateGem(new Point(x, y), new Gem(new Point(x * _grid.GemSize + _grid.Position.X, y * _grid.GemSize + _grid.Position.Y), GetRandomColor()));
                }
            }

            FindMatches();
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

        public void RemoveMatches()
        {
            foreach (var position in _gemsToDestroy)
            {
                RemoveGem(position);
            }
            _gemsToDestroy.Clear();

            
            CollapseAllGems();
            FillEmpty();
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
        public void LoadContent(ContentManager content)
        {
            _gemTextures.Add("red", content.Load<Texture2D>("textures/red_gem"));
            _gemTextures.Add("green", content.Load<Texture2D>("textures/green_gem"));
            _gemTextures.Add("blue", content.Load<Texture2D>("textures/blue_gem"));
            _gemTextures.Add("yellow", content.Load<Texture2D>("textures/yellow_gem"));
            _gemTextures.Add("magenta", content.Load<Texture2D>("textures/magenta_gem"));

            _selectionTexture = content.Load<Texture2D>("textures/selection");
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!_active) return;

            for (int x = 0;  x < _configuration.Width; x++)
            {
                for (int y = 0; y < _configuration.Height; y++)
                {
                    if (_grid.Gems[x][y] == null) continue;
                    _grid.Gems[x][y].Render(spriteBatch, _gemTextures[Enum.GetName(_grid.Gems[x][y].GetColor())], _grid.GemSize);
                }
            }

            if (_selectedPosition != null)
                spriteBatch.Draw(
                    _selectionTexture,
                    new Rectangle(new Point(_selectedPosition.Value.X * _grid.GemSize + _grid.Position.X, _selectedPosition.Value.Y * _grid.GemSize + _grid.Position.Y), new Point(_grid.GemSize)),
                    Color.White
                    );
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
            {
                SwapGems((Point)_selectedPosition, position);
                SwapAction(position, (Point)_selectedPosition);
                /*if (_grid.Gems[((Point)_selectedPosition).X][((Point)_selectedPosition).Y] != null && _grid.Gems[position.X][position.Y] != null)
                    FindSwapMatches((Point)_selectedPosition, position);*/
                _selectedPosition = null;
            }
            else
                _selectedPosition = position;
        }

        private void SwapGems(Point firstGem, Point secondGem)
        {
          
            if (CanSwap(firstGem, secondGem))
            {
                (_grid.Gems[firstGem.X][firstGem.Y], _grid.Gems[secondGem.X][secondGem.Y]) =
                    (_grid.Gems[secondGem.X][secondGem.Y], _grid.Gems[firstGem.X][firstGem.Y]);



                /*if (_grid.Gems[firstGem.X][firstGem.Y] != null)
                    _grid.Gems[firstGem.X][firstGem.Y].Move(new Point(firstGem.X * _grid.GemSize + _grid.Position.X, firstGem.Y * _grid.GemSize + _grid.Position.Y));
                if (_grid.Gems[secondGem.X][secondGem.Y] != null)
                    _grid.Gems[secondGem.X][secondGem.Y].Move(new Point(secondGem.X * _grid.GemSize + _grid.Position.X, secondGem.Y * _grid.GemSize + _grid.Position.Y));*/
                UpdateGemPosition(firstGem);
                UpdateGemPosition(secondGem);
            }

        }

        private async void SwapAction(Point firstGem, Point secondGem)
        {
            Task move1 = _grid.Gems[firstGem.X][firstGem.Y].Move();
            Task move2 = _grid.Gems[secondGem.X][secondGem.Y].Move();

            await Task.WhenAll(move1, move2);

            if (_grid.Gems[firstGem.X][firstGem.Y] != null && _grid.Gems[secondGem.X][secondGem.Y] != null)
                FindSwapMatches(firstGem, secondGem);
        }

        private bool CanSwap(Point firstGem, Point secondGem) { 

            return Math.Abs(firstGem.X - secondGem.X) == 1 && firstGem.Y == secondGem.Y ||
                firstGem.X == secondGem.X && Math.Abs(firstGem.Y - secondGem.Y) == 1; 

        }

        private void UpdateGemPosition(Point position)
        {
            if (_grid.Gems[position.X][position.Y] != null)
            {
                _grid.Gems[position.X][position.Y].Destination = new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y);
            }
        }

        private void FindSwapMatches(Point firstGem, Point secondGem)
        {
            List<Point> firstGemMatch = FindMatch(firstGem, _grid.Gems[firstGem.X][firstGem.Y].GetColor());
            List<Point> secondGemMatch = FindMatch(secondGem, _grid.Gems[secondGem.X][secondGem.Y].GetColor());

            if (firstGemMatch.Count == 0 && secondGemMatch.Count == 0)
            {
                SwapGems(firstGem, secondGem);
                SwapAction(firstGem, secondGem);
            }
            else
            {
                foreach (Point gemPostion in firstGemMatch)
                    _gemsToDestroy.Add(gemPostion);
                foreach (Point gemPostion in secondGemMatch)
                    _gemsToDestroy.Add(gemPostion);

                RemoveMatches();
            }

        }

        private List<Point> FindMatch(Point position, Colors matchingColor)
        {
            int hScore = 0;
            int vScore = 0;
            int totalScore = 0;
            List<Point> gemsInMatch = new List<Point> { new Point(position.X, position.Y) };
            List<Point> rightMatch;
            List<Point> leftMatch;
            List<Point> upMatch;
            List<Point> downMatch;

            rightMatch = FindHorizontalMatch(new Point(position.X + 1, position.Y), 1, _grid.Gems[position.X][position.Y].GetColor());
            leftMatch = FindHorizontalMatch(new Point(position.X - 1, position.Y), -1, _grid.Gems[position.X][position.Y].GetColor());
            leftMatch.Reverse();

            upMatch = FindVerticalMatch(new Point(position.X, position.Y - 1), -1, _grid.Gems[position.X][position.Y].GetColor());
            downMatch = FindVerticalMatch(new Point(position.X, position.Y + 1), 1, _grid.Gems[position.X][position.Y].GetColor());
            upMatch.Reverse();

            hScore += rightMatch.Count + leftMatch.Count;
            vScore += upMatch.Count + downMatch.Count;

            if (vScore >= 2)
                gemsInMatch = upMatch.Concat(gemsInMatch.Concat(downMatch).ToList()).ToList<Point>();

            if (hScore >= 2)
                gemsInMatch = leftMatch.Concat(gemsInMatch.Concat(rightMatch).ToList()).ToList<Point>(); ;

            totalScore = hScore + vScore;

            if (hScore < 2 && vScore < 2) return new List<Point>();
            else return gemsInMatch;
        }

        private List<Point> FindHorizontalMatch(Point position, int offset, Colors matchingColor)
        {
            List<Point> gemsInMatch = new List<Point> { };

            if (position.X >= 0 && position.X < _configuration.Width && _grid.Gems[position.X][position.Y] != null)
            {
                if (Enum.GetName(_grid.Gems[position.X][position.Y].GetColor()) == Enum.GetName(matchingColor))
                {
                    gemsInMatch.Add(new Point(position.X, position.Y));
                    return gemsInMatch.
                        Concat(FindHorizontalMatch(new Point(position.X + offset, position.Y), offset, matchingColor)).ToList<Point>();
                }
            }

            return gemsInMatch;
        }

        private List<Point> FindVerticalMatch(Point position, int offset, Colors matchingColor)
        {
            List<Point> gemsInMatch = new List<Point> { };

            if (position.Y >= 0 && position.Y < _configuration.Height && _grid.Gems[position.X][position.Y] != null)
            {
                if (Enum.GetName(_grid.Gems[position.X][position.Y].GetColor()) == Enum.GetName(matchingColor))
                {
                    gemsInMatch.Add(new Point(position.X, position.Y));
                    return gemsInMatch.
                        Concat(FindVerticalMatch(new Point(position.X, position.Y + offset), offset, matchingColor)).ToList<Point>();
                }
            }

            return gemsInMatch;
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

            if (_gemsToDestroy.Count > 0)
            {
                RemoveMatches();
                FillEmpty();
            }
        }

        private void CollapseAllGems()
        {
            for (int x = 0; x < _configuration.Width; x++)
            {
                for (int y = 0; y < _configuration.Height; y++)
                {
                    if (_grid.Gems[x][y] == null)
                    {
                        CollapseGemsAbove(new Point(x, y));
                    }
                }

                foreach (var gem in _grid.Gems[x])
                {
                    if (gem != null) 
                        gem.Move();
                }
            }

        }

        private void CollapseGemsAbove(Point fallPosition)
        {
            for (int y = fallPosition.Y; y > 0; y--)
            {
                SwapGems(new Point (fallPosition.X, y), new Point (fallPosition.X, y - 1));
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
