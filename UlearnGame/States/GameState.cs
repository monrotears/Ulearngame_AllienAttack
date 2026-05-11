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

                CreatePlayer(playerTexture, Color.White, new Vector2(100, 250), "Игрок 1", Keys.W, Keys.S, Keys.A, Keys.D)
            };

            if (PlayerCount >= 2)
            {
                _sprites.Add(CreatePlayer(playerTexture, Color.LightBlue, new Vector2(100, 350), "Игрок 2", Keys.Up, Keys.Down, Keys.Left, Keys.Right));
            }

            _players = _sprites.Where(sprite => sprite is Player).Select(sprite => (Player)sprite).ToList();
        }

        private Player CreatePlayer(Texture2D playerTexture, Color colour, Vector2 position, string playerName, Keys up, Keys down, Keys left, Keys right)
        {
            return new Player(playerTexture)
            {
                Colour = colour,
                Position = position,
                Layer = 0.3f,
                Input = new Models.Input()
                {
                    Up = up,
                    Down = down,
                    Left = left,
                    Right = right,
                    Shoot = Keys.None,
                },
                Health = 20,
                Score = new Models.Score()
                {
                    PlayerName = playerName,
                    Value = 0,
                },
            };
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
            spriteBatch.DrawString(_font, "WASD - движение игрока 1, Esc - выход", new Vector2(40, 35), Color.Red);

            if (PlayerCount >= 2)
                spriteBatch.DrawString(_font, "Стрелки - движение игрока 2", new Vector2(40, 60), Color.Red);

            spriteBatch.End();
        }
    }
}
