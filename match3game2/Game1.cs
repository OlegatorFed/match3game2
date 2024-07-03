using match3game2.Builders;
using match3game2.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace match3game2
{
    public class Game1 : Game
    {
        private ServiceProvider _serviceProvider;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private GridController _gridController;
        private TimerController _timerController;
        private ScoreContoller _scoreContoller;
        private MouseHandler _mouseHandler;

        public event Action Updated;
        //private Stopwatch _stopwatch;

        public Game1()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<GameController>();
            services.AddSingleton<GridController>();
            services.AddSingleton<GridBuilder>();
            services.AddSingleton<ConfigurationManager>();
            services.AddSingleton<TimerController>();
            services.AddSingleton<ScoreContoller>();
            services.AddSingleton<MouseHandler>();

            _serviceProvider = services.BuildServiceProvider();

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gridController = _serviceProvider.GetRequiredService<GridController>();
            _gridController.Fill();
            _gridController.GetGem(new Point(1, 0));
            
            _scoreContoller = _serviceProvider.GetRequiredService<ScoreContoller>();
            _scoreContoller.AddScore(100);

            _timerController = _serviceProvider.GetRequiredService<TimerController>();
            _timerController.StartTimer();

            _mouseHandler = _serviceProvider.GetRequiredService<MouseHandler>();

            //_stopwatch = Stopwatch.StartNew();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("fonts/font");

            // TODO: use this.Content to load your game content here
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

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"{timeLeft}", 
                Vector2.Zero, Color.Black);
            _spriteBatch.DrawString(_font, $"{score}",
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - _font.MeasureString(score.ToString()).X / 2, 0), Color.Black);
            _spriteBatch.DrawString(_font, $"{_mouseHandler.MousePosition}",
                _mouseHandler.GetMousePosition(), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
