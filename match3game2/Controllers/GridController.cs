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

        public Action Scored;

        private bool _active;
        private Dictionary<string, Texture2D> _gemTextures;
        private Dictionary<string, Texture2D> _bonusTextures;
        private Dictionary<string, Texture2D> _destroyerTextures;
        private Texture2D _selectionTexture;

        private Grid _grid;
        private GridConfiguration _configuration;
        private GridBuilder _gridBuilder;
        private MouseHandler _mouseHandler;
        private ContentManager _content;

        private Colors[] _colors;
        private int _bombPower;

        private Point? _selectedPosition;
        private HashSet<Point> _gemsToDestroy;
        private List<Point> _movedGems;
        private Dictionary<Point, Gem> _bonusCandidates;
        private List<Destroyer> _destroyers;

        private List<Task> _taskQueue;

        public GridController(ConfigurationManager configurationManager, GridBuilder gridBuilder, MouseHandler mouseHandler) 
        {
            _gemTextures = new Dictionary<string, Texture2D>();
            _bonusTextures = new Dictionary<string, Texture2D>();
            _destroyerTextures = new Dictionary<string, Texture2D>();

            _configuration = configurationManager.GridConfiguration;
            _gridBuilder = gridBuilder;
            _mouseHandler = mouseHandler;
            _grid = _gridBuilder.Build();

            _selectedPosition = null;
            _gemsToDestroy = new HashSet<Point>();
            _movedGems = new List<Point>();
            _bonusCandidates = new Dictionary<Point, Gem>();
            _destroyers = new List<Destroyer>();
            _taskQueue = new List<Task>();

            _colors = (Colors[])Enum.GetValues(typeof(Colors));
            _bombPower = 3;

            _mouseHandler.MousePressed += OnClick;

            _active = true;

        }

        public bool IsActive() { return _active; }

        public void SetActive(bool state) { _active = state; }

        public void Reset()
        {
            _bonusCandidates.Clear();
            foreach (var destroyer in _destroyers)
            {
                destroyer.Moved -= DestroyOnFly;
            }
            _destroyers.Clear();
            _taskQueue.Clear();
            _selectedPosition = null;
            _grid = null;
            _grid = _gridBuilder.Build();
            //Fill();
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

        public async void FillEmpty()
        {
            for (int x = 0; x < _configuration.Width; x++)
            {
                for (int y = 0; y < _configuration.Height; y++)
                {
                    if (_grid.Gems[x][y] != null) continue;
                    UpdateGem(new Point(x, y), new Gem(new Point(x * _grid.GemSize + _grid.Position.X, y * _grid.GemSize + _grid.Position.Y), GetRandomColor()));
                    _grid.Gems[x][y].Scale = 0f;

                    _taskQueue.Add(_grid.Gems[x][y].ScaleTo(1));
                }
            }

            await Task.WhenAll(_taskQueue);
            _taskQueue.Clear();

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

        public async void RemoveMatches()
        {
            List<Point> bonusesToActivate = new List<Point>();

            foreach (var position in _gemsToDestroy)
            {
                _taskQueue.Add(_grid.Gems[position.X][position.Y].ScaleTo(0));
                if (_grid.Gems[position.X][position.Y].GetBonusType() != null)
                    bonusesToActivate.Add(position);
            }

            await Task.WhenAll(_taskQueue);
            _taskQueue.Clear();

            foreach (var position in bonusesToActivate)
            {
                BonusType? bonusType = _grid.Gems[position.X][position.Y].GetBonusType();
                await ActivateBonus(position, bonusType);
            }

            foreach (var position in _gemsToDestroy)
            {
                Scored?.Invoke();
                RemoveGem(position);

                if (_bonusCandidates.ContainsKey(position))
                {
                    UpdateGem(position, _bonusCandidates[position]);
                    _bonusCandidates[position].Scale = 0f;
                    _taskQueue.Add(_bonusCandidates[position].ScaleTo(1));
                    _bonusCandidates.Remove(position);
                }
            }
            _gemsToDestroy.Clear();
            
            await CollapseAllGems();

            FillEmpty();
        }

        public Grid GetGrid() { return _grid; }

        public void OnClick(Vector2 position) 
        {
            if (!_active || _taskQueue.Count > 0) return;

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

            _bonusTextures.Add("vLine", content.Load<Texture2D>("textures/v_line_mark"));
            _bonusTextures.Add("hLine", content.Load<Texture2D>("textures/h_line_mark"));
            _bonusTextures.Add("bomb", content.Load<Texture2D>("textures/bomb_mark"));

            _destroyerTextures.Add("up", content.Load<Texture2D>("textures/v_destroyer_up"));
            _destroyerTextures.Add("down", content.Load<Texture2D>("textures/v_destroyer_down"));
            _destroyerTextures.Add("right", content.Load<Texture2D>("textures/h_destroyer_right"));
            _destroyerTextures.Add("left", content.Load<Texture2D>("textures/h_destroyer_left"));

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
                    if (_grid.Gems[x][y].BonusType != null)
                        _grid.Gems[x][y].Render(spriteBatch, _gemTextures[Enum.GetName(_grid.Gems[x][y].GetColor())], _bonusTextures[Enum.GetName((BonusType)_grid.Gems[x][y].GetBonusType())], _grid.GemSize);
                    else
                        _grid.Gems[x][y].Render(spriteBatch, _gemTextures[Enum.GetName(_grid.Gems[x][y].GetColor())], _grid.GemSize);
                }
            }

            if (_selectedPosition != null)
                spriteBatch.Draw(
                    _selectionTexture,
                    new Rectangle(new Point(_selectedPosition.Value.X * _grid.GemSize + _grid.Position.X, _selectedPosition.Value.Y * _grid.GemSize + _grid.Position.Y), new Point(_grid.GemSize)),
                    Color.White
                    );

            for (int i = 0; i < _destroyers.Count; i++)
            {
                _destroyers[i].Render(spriteBatch, _destroyerTextures[_destroyers[i].GetDirection()], _grid.GemSize);
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

        private Point GetGridPosition(Point position) { return new Point((position.X - _configuration.Position.X) / _grid.GemSize, (position.Y - _configuration.Position.Y) / _grid.GemSize); }

        private void SelectGem(Point position)
        {

            if (_selectedPosition != null)
            {
                //SwapGems((Point)_selectedPosition, position);
                SwapAction(position, (Point)_selectedPosition);
                _movedGems.Add(position);
                _movedGems.Add((Point)_selectedPosition);
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

                UpdateGemPosition(firstGem);
                UpdateGemPosition(secondGem);
            }

        }

        private async void SwapAction(Point firstGem, Point secondGem)
        {
            SwapGems(firstGem, secondGem);


            if (CanSwap(firstGem, secondGem))
            {
                Task move1 = _grid.Gems[firstGem.X][firstGem.Y].Move();
                Task move2 = _grid.Gems[secondGem.X][secondGem.Y].Move();

                _taskQueue.Add(move1);
                _taskQueue.Add(move2);

                await Task.WhenAll(_taskQueue);

                _taskQueue.Clear();

                if (_grid.Gems[firstGem.X][firstGem.Y] != null && _grid.Gems[secondGem.X][secondGem.Y] != null && _movedGems.Count > 0)
                    FindSwapMatches(firstGem, secondGem);
            }
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

        private void AddBonusCandidate(Point position, Colors color, BonusType bonusType)
        {
            _bonusCandidates.Add(position, new Gem(new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y), color, bonusType));
        }

        private async Task ActivateBonus(Point position, BonusType? bonusType)
        {
            switch (bonusType)
            {
                case BonusType.vLine:
                    await VerticalLineAction(position);
                    break;
                case BonusType.hLine:
                    await HorizontalLineAction(position);
                    break;
                case BonusType.bomb:
                    await BombAction(position);
                    break;
            }
        }

        private async Task VerticalLineAction(Point position)
        {
            List<Task> tasks = new List<Task>();

            Destroyer destroyer1 = new Destroyer(new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y),
                new Point(position.X * _grid.GemSize + _grid.Position.X, _grid.Position.Y));
            Destroyer destroyer2 = new Destroyer(new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y),
                new Point(position.X * _grid.GemSize + _grid.Position.X, _grid.Position.Y + _configuration.Height * _grid.GemSize));

            destroyer1.Moved += DestroyOnFly;
            destroyer2.Moved += DestroyOnFly;

            _destroyers.Add(destroyer1);
            _destroyers.Add(destroyer2);

            tasks.Add(destroyer1.Move());
            tasks.Add(destroyer2.Move());
            await Task.WhenAll(tasks);
            tasks.Clear();

            destroyer1.Moved -= DestroyOnFly;
            destroyer2.Moved -= DestroyOnFly;

            _destroyers.Remove(destroyer1);
            _destroyers.Remove(destroyer2);
            destroyer1 = null;
            destroyer2 = null;
        }

        private async Task HorizontalLineAction(Point position)
        {
            List<Task> tasks = new List<Task>();

            Destroyer destroyer1 = new Destroyer(new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y),
                new Point(_grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y));
            Destroyer destroyer2 = new Destroyer(new Point(position.X * _grid.GemSize + _grid.Position.X, position.Y * _grid.GemSize + _grid.Position.Y),
                new Point(position.X + _configuration.Width * _grid.GemSize, position.Y * _grid.GemSize + _grid.Position.Y));

            destroyer1.Moved += DestroyOnFly;
            destroyer2.Moved += DestroyOnFly;

            _destroyers.Add(destroyer1);
            _destroyers.Add(destroyer2);

            tasks.Add(destroyer1.Move());
            tasks.Add(destroyer2.Move());
            await Task.WhenAll(tasks);
            tasks.Clear();

            destroyer1.Moved -= DestroyOnFly;
            destroyer2.Moved -= DestroyOnFly;

            _destroyers.Remove(destroyer1);
            _destroyers.Remove(destroyer2);
            destroyer1 = null;
            destroyer2 = null;
        }

        private async Task BombAction(Point position)
        {
            List<Point> bonusesToActivate = new List<Point>();

            await Task.Delay(250);

            for (int x = position.X - _bombPower / 2; x <= position.X + _bombPower / 2; x++)
            {
                for (int y = position.Y - _bombPower / 2; y <= position.Y + _bombPower / 2; y++)
                {
                    if (x >= 0 && x < _configuration.Width &&
                        y >= 0 && y < _configuration.Height &&
                        !_gemsToDestroy.Contains(new Point(x, y)) &&
                        new Point(x, y) != position &&
                        _grid.Gems[x][y] != null)
                    {
                        bonusesToActivate.Add(new Point(x, y));
                        _taskQueue.Add(_grid.Gems[x][y].ScaleTo(0));
                        _gemsToDestroy.Add(new Point(x, y));
                    }
                }
            }
            await Task.WhenAll(_taskQueue);
            _taskQueue.Clear();

            foreach (var bonus in bonusesToActivate)
            {
                BonusType? bonusType = _grid.Gems[bonus.X][bonus.Y].GetBonusType();
                await ActivateBonus(bonus, bonusType);
            }
        }

        private async void DestroyOnFly(Point position)
        {

            Point gridPosition = GetGridPosition(position);

            if (gridPosition.X >= 0 && gridPosition.X < _configuration.Width &&
                gridPosition.Y >= 0 && gridPosition.Y < _configuration.Height &&
                !_gemsToDestroy.Contains(gridPosition) &&
                _grid.Gems[gridPosition.X][gridPosition.Y] != null)
            {
                await _grid.Gems[gridPosition.X][gridPosition.Y].ScaleTo(0);
                _gemsToDestroy.Add(gridPosition);
                if (_grid.Gems[gridPosition.X][gridPosition.Y].GetBonusType() != null)
                    await ActivateBonus(gridPosition, (BonusType)_grid.Gems[gridPosition.X][gridPosition.Y].GetBonusType());
            }

        }

        private bool CheckQueue()
        {
            return _taskQueue.Count > 0;
        }

        private void FindSwapMatches(Point firstGem, Point secondGem)
        {
            BonusType? firstMatchDirection;
            BonusType? secondMatchDirection;
            List<Point> firstGemMatch = FindMatch(firstGem, _grid.Gems[firstGem.X][firstGem.Y].GetColor(), out firstMatchDirection);
            List<Point> secondGemMatch = FindMatch(secondGem, _grid.Gems[secondGem.X][secondGem.Y].GetColor(), out secondMatchDirection);

            if (firstGemMatch.Count == 0 && secondGemMatch.Count == 0)
            {
                _movedGems.Clear();
                SwapAction(firstGem, secondGem);
            }
            else
            {
                foreach (Point gemPostion in firstGemMatch)
                    _gemsToDestroy.Add(gemPostion);
                foreach (Point gemPostion in secondGemMatch)
                    _gemsToDestroy.Add(gemPostion);

                if (firstGemMatch.Count >= 4)
                {
                    AddBonusCandidate(firstGem, _grid.Gems[firstGem.X][firstGem.Y].GetColor(), (BonusType)firstMatchDirection);
                }
                if (secondGemMatch.Count >= 4)
                {
                    AddBonusCandidate(secondGem, _grid.Gems[secondGem.X][secondGem.Y].GetColor(), (BonusType)secondMatchDirection);
                }

                RemoveMatches();
            }

        }

        private List<Point> FindMatch(Point position, Colors matchingColor, out BonusType? bonusType)
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
            else
                vScore = 0;

            if (hScore >= 2)
                gemsInMatch = leftMatch.Concat(gemsInMatch.Concat(rightMatch).ToList()).ToList<Point>();
            else
                hScore = 0;

            totalScore = hScore + vScore;


            if (totalScore >= 3)
            {
                if (vScore > hScore)
                    bonusType = BonusType.vLine;
                else
                    bonusType = BonusType.hLine;

                if (totalScore >= 4)
                    bonusType = BonusType.bomb;
            }
            else
                bonusType = null;

            if (hScore < 2 && vScore < 2)
                return new List<Point>();
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
                if (possibleMatch.Count >= 3)
                    _gemsToDestroy.UnionWith(possibleMatch);
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
                if (possibleMatch.Count >= 3)
                    _gemsToDestroy.UnionWith(possibleMatch);
                currentColor = null;
                possibleMatch.Clear();
            }

            if (_gemsToDestroy.Count > 0)
            {
                RemoveMatches();
                //FillEmpty();
            }
        }

        private async Task CollapseAllGems()
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
                        _taskQueue.Add(gem.Move());
                }

            }
            await Task.WhenAll(_taskQueue);
            _taskQueue.Clear();

            //FindMatches();

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
