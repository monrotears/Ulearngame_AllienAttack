using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using UlearnGame.Controller;
using UlearnGame.Managers;
using UlearnGame.Models;
using UlearnGame.Sprites;

namespace UlearnGame.States
{
    public class GameState : State
    {
        private Texture2D _blankTexture;
        private Button _muteButton;
        private Button _settingsButton;
        private List<Component> _settingsComponents;
        private List<Sprite> _sprites;
        private List<Player> _players;
        private EnemyManager _enemyManager;
        private SpriteFont _font;
        private bool _settingsOpen;

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
            var secondPlayerTexture = _content.Load<Texture2D>("pixil-frame-0");
            var bulletTexture = _content.Load<Texture2D>("Bullet");
            var buttonTexture = _content.Load<Texture2D>("Button");
            var explosionTexture = _content.Load<Texture2D>("Explosion");
            var shootSound = _content.Load<SoundEffect>("ShootSound");
            var secondPlayerShootSound = _content.Load<SoundEffect>("ShootSoundPlayer2");
            _font = _content.Load<SpriteFont>("Font");

            _blankTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });

            var explosion = new Explosion(new Dictionary<string, Animation>()
            {
                { "Explode", new Animation(explosionTexture, 3) { FrameSpeed = 0.06f } },
            });
            var playerBullet = new Bullet(bulletTexture)
            {
                Explosion = explosion,
            };
            var enemyBullet = new Bullet(bulletTexture)
            {
                Explosion = explosion,
            };
            CreateSettingsComponents(buttonTexture);

            _sprites = new List<Sprite>()
            {
                new Sprite(backgroundTexture)
                {
                    Layer = 0.0f,
                    Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2),
                },

                CreatePlayer(playerTexture, Color.White, new Vector2(100, 250), "Игрок 1", Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, playerBullet, shootSound)
            };

            if (PlayerCount >= 2)
            {
                _sprites.Add(CreatePlayer(secondPlayerTexture, Color.White, new Vector2(100, 350), "Игрок 2", Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl, playerBullet, secondPlayerShootSound));
            }

            _players = _sprites.Where(sprite => sprite is Player).Select(sprite => (Player)sprite).ToList();
            _enemyManager = new EnemyManager(_content)
            {
                Bullet = enemyBullet,
            };
        }

        private void CreateSettingsComponents(Texture2D buttonTexture)
        {
            _settingsButton = new Button(buttonTexture, _font)
            {
                Text = "Настройки",
                Position = new Vector2(Game1.ScreenWidth - 130, 40),
                Click = new System.EventHandler(SettingsButton_Clicked),
                Layer = 0.9f,
                Scale = 1.1f,
            };

            _muteButton = new Button(buttonTexture, _font)
            {
                Position = new Vector2(Game1.ScreenWidth / 2, 440),
                Click = new System.EventHandler(MuteButton_Clicked),
                Layer = 0.9f,
                Scale = 1.15f,
            };

            UpdateMuteButtonText();

            _settingsComponents = new List<Component>()
            {
                new Slider(_blankTexture, _font)
                {
                    Text = "Музыка меню",
                    Position = new Vector2(500, 250),
                    Value = AudioSettings.MenuVolume,
                    ValueChanged = value =>
                    {
                        AudioSettings.MenuVolume = value;
                        AudioSettings.ApplyMenuVolume();
                    },
                },
                new Slider(_blankTexture, _font)
                {
                    Text = "Звуки игры",
                    Position = new Vector2(500, 335),
                    Value = AudioSettings.GameVolume,
                    ValueChanged = value => AudioSettings.GameVolume = value,
                },
                _muteButton,
            };
        }

        private void SettingsButton_Clicked(object sender, System.EventArgs args)
        {
            _settingsOpen = !_settingsOpen;
        }

        private void MuteButton_Clicked(object sender, System.EventArgs args)
        {
            AudioSettings.IsMuted = !AudioSettings.IsMuted;
            AudioSettings.ApplyMenuVolume();
            UpdateMuteButtonText();
        }

        private void UpdateMuteButtonText()
        {
            if (_muteButton == null)
                return;

            _muteButton.Text = AudioSettings.IsMuted ? "Включить звук" : "Выключить звук";
        }

        private Player CreatePlayer(
            Texture2D playerTexture,
            Color colour,
            Vector2 position,
            string playerName,
            Keys up,
            Keys down,
            Keys left,
            Keys right,
            Keys shoot,
            Bullet bullet,
            SoundEffect shootSound)
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
                    Shoot = shoot,
                },
                Bullet = bullet,
                Health = 20,
                ShootSound = shootSound,
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

            _settingsButton.Update(gameTime);

            if (_settingsOpen)
            {
                foreach (var component in _settingsComponents)
                    component.Update(gameTime);

                return;
            }

            foreach (var sprite in _sprites)
                sprite.Update(gameTime);

            _enemyManager.Update(gameTime);

            if (_enemyManager.CanAdd && _sprites.Count(sprite => sprite is Enemy) < _enemyManager.MaxEnemies)
                _sprites.Add(_enemyManager.GetEnemy(_players));

            AddChildSprites();

            RemoveOldSprites();
        }

        private void RemoveOldSprites()
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i].IsRemoved)
                {
                    _sprites.RemoveAt(i);
                    i--;
                }
            }
        }

        private void AddChildSprites()
        {
            var children = _sprites.SelectMany(sprite => sprite.Children).ToList();

            foreach (var sprite in _sprites)
                sprite.Children.Clear();

            _sprites.AddRange(children);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            CheckCollisions();
            AddChildSprites();
            RemoveOldSprites();
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                var first = _sprites[i];

                if (first.IsRemoved || !(first is ICollidable))
                    continue;

                for (int j = i + 1; j < _sprites.Count; j++)
                {
                    var second = _sprites[j];

                    if (second.IsRemoved || !(second is ICollidable))
                        continue;

                    if (!first.CollisionArea.Intersects(second.CollisionArea) || !first.Intersects(second))
                        continue;

                    ((ICollidable)first).OnCollide(second);
                    ((ICollidable)second).OnCollide(first);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            foreach (var sprite in _sprites)
                sprite.Draw(gameTime, spriteBatch);

            _settingsButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(_font, "Prototype build", new Vector2(40, 10), Color.Red);
            spriteBatch.DrawString(_font, "WASD - движение игрока 1, Space - выстрел, Esc - выход", new Vector2(40, 35), Color.Red);

            if (PlayerCount >= 2)
                spriteBatch.DrawString(_font, "Стрелки - движение игрока 2, Right Ctrl - выстрел", new Vector2(40, 60), Color.Red);

            spriteBatch.End();

            if (_settingsOpen)
                DrawSettings(spriteBatch);
        }

        private void DrawSettings(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_blankTexture, new Rectangle(420, 180, 440, 320), Color.Black * 0.8f);
            spriteBatch.DrawString(_font, "Настройки звука", new Vector2(500, 205), Color.White);

            foreach (var component in _settingsComponents)
                component.Draw(null, spriteBatch);

            spriteBatch.End();
        }
    }
}
