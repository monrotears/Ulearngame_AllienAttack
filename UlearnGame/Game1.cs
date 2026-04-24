using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UlearnGame.States;

namespace UlearnGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random Random;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        private State _currentState;
        private State _nextState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Alien Attack";
        }   

        protected override void Initialize()
        {
            Random = new Random();

            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Prototype build: сразу запускаем игровое поле без меню.
            _currentState = new GameState(this, Content)
            {
                PlayerCount = 1,
            };
            _currentState.LoadContent();
            _nextState = null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _currentState.LoadContent();
                _nextState = null;
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _currentState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
