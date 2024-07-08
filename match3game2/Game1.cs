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

            _gameController.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            _gameController.Render(_graphics);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
