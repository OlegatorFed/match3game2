using match3game2.Builders;
using match3game2.Configurations;
using match3game2.Controllers;
using match3game2.Models;
using match3game2.Utilities;
using match3game2.Utilities.Renderers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace match3game2
{
    public class Game1 : Game
    {

        private SpriteFont _font;

        private ServiceProvider _serviceProvider;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameController _gameController;
        private MenuController _menuController;
        private GridController _gridController;
        private TimerController _timerController;
        private ScoreContoller _scoreContoller;
        private MouseHandler _mouseHandler;
        private BatchHandle _batchHandle;

        public event Action Updated;

        public Game1()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<GameController>();
            services.AddSingleton<MenuController>();
            services.AddSingleton<GridController>();
            services.AddSingleton<GridBuilder>();
            services.AddSingleton<ConfigurationManager>();
            services.AddSingleton<TimerController>();
            services.AddSingleton<ScoreContoller>();
            services.AddSingleton<MouseHandler>();
            services.AddSingleton<BatchHandle>();
            services.AddSingleton<ButtonRenderer>();

            _serviceProvider = services.BuildServiceProvider();


            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gridController = _serviceProvider.GetRequiredService<GridController>();
            _gridController.Fill();
            _gridController.FindMatches();

            _scoreContoller = _serviceProvider.GetRequiredService<ScoreContoller>();
            _scoreContoller.AddScore(100);

            _timerController = _serviceProvider.GetRequiredService<TimerController>();
            _timerController.StartTimer();

            _mouseHandler = _serviceProvider.GetRequiredService<MouseHandler>();

            _batchHandle = _serviceProvider.GetRequiredService<BatchHandle>();

            _gameController = _serviceProvider.GetRequiredService<GameController>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _batchHandle.Configure(_spriteBatch);

            _gameController.RecieveSpriteBatch(_batchHandle.GetSpriteBatchOrNull());
            _gameController.RecieveContent(Content);
            _gameController.LoadContent();

            _font = Content.Load<SpriteFont>("fonts/font");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _mouseHandler.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            int timeLeft = _timerController.TimeLeft;
            int score = _scoreContoller.Score;
            Grid grid = _gridController.GetGrid();

            _spriteBatch.Begin();

            _gameController.Render();

            _spriteBatch.DrawString(
                _font,
                $"{timeLeft}", 
                Vector2.Zero,
                Color.Black);
            _spriteBatch.DrawString(
                _font,
                $"{score}",
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - _font.MeasureString(score.ToString()).X / 2, 0),
                Color.Black);

            _spriteBatch.DrawString(
                _font,
                $"{_mouseHandler.MousePosition}",
                _mouseHandler.MousePosition, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
