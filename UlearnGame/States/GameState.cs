using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using UlearnGame.Sprites;

namespace UlearnGame.States
{
    public class GameState : State
    {
        private List<Sprite> _sprites;
        private List<Player> _players;
        private SpriteFont _font;

        public int PlayerCount;

        public GameState(Game1 game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            MediaPlayer.Stop();

            var backgroundTexture = _content.Load<Texture2D>("backroad");
            var playerTexture = _content.Load<Texture2D>("Player");
            _font = _content.Load<SpriteFont>("Font");

            _sprites = new List<Sprite>()
            {
                new Sprite(backgroundTexture)
                {
                    Layer = 0.0f,
                    Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2),
                },

                new Player(playerTexture)
                {
                    Colour = Color.White,
                    Position = new Vector2(100, 250),
                    Layer = 0.3f,
                    Input = new Models.Input()
                    {
                        Up = Keys.W,
                        Down = Keys.S,
                        Left = Keys.A,
                        Right = Keys.D,
                        Shoot = Keys.None,
                    },
                    Health = 20,
                    Score = new Models.Score()
                    {
                        PlayerName = "Игрок 1",
                        Value = 0,
                    },
                }
            };

            _players = _sprites.Where(sprite => sprite is Player).Select(sprite => (Player)sprite).ToList();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.Exit();

            foreach (var sprite in _sprites)
                sprite.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // В первой версии пока нет врагов, стрельбы и столкновений.
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            foreach (var sprite in _sprites)
                sprite.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(_font, "Prototype build: movement only", new Vector2(40, 10), Color.Red);
            spriteBatch.DrawString(_font, "WASD - движение, Esc - выход", new Vector2(40, 35), Color.Red);
            spriteBatch.End();
        }
    }
}
