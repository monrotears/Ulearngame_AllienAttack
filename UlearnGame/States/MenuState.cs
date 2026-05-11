using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using UlearnGame.Controller;
using UlearnGame.Models;
using UlearnGame.Sprites;
using UlearnGame.States;

namespace UlearnGame
{
    public class MenuState : State // окно главного меню
    {
        private Texture2D _blankTexture;
        private List<Component> _components;
        private SpriteFont _font;
        private Button _muteButton;
        private Button _settingsButton;
        private List<Component> _settingsComponents;
        private bool _settingsOpen;

        

        public MenuState(Game1 game, ContentManager content) : base(game, content)
        {
        }
      

        public override void LoadContent()
        {
            var buttonTexture = _content.Load<Texture2D>("Button");
            _font = _content.Load<SpriteFont>("Font");

            _blankTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });

            PlayMainMenuMusic();
            CreateSettingsComponents(buttonTexture);

            _components = new List<Component>()
            {
                 new Sprite(_content.Load<Texture2D>("MainMenu"))
                 {
                      
                      Layer = 0f,
                      Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2),
                 },

                 new Button(buttonTexture, _font)
                 {
                      Text = "1 Игрок",
                      Position = new Vector2(Game1.ScreenWidth / 2, 400),
                      Click = new EventHandler(Button_1Player_Clicked),
                      Layer = 0.1f,
                      Scale = 1.15f,
                 },

                 new Button(buttonTexture, _font)
                 {
                      Text = "2 Игрока",
                      Position = new Vector2(Game1.ScreenWidth / 2, 440),
                      Click = new EventHandler(Button_2Player_Clicked),
                      Layer = 0.1f,
                      Scale = 1.15f,
                 },

                  new Button(buttonTexture, _font)
                  {
                      Text = "Рекорды",
                      Position = new Vector2(Game1.ScreenWidth / 2, 480),
                      Click = new EventHandler(Button_Highscores_Clicked),
                      Layer = 0.1f,
                      Scale = 1.15f,
                  },

                  _settingsButton,

                  new Button(buttonTexture, _font)
                  {
                      Text = "Выйти из игры",
                      Position = new Vector2(Game1.ScreenWidth / 2, 560),
                      Click = new EventHandler(Button_Quit_Clicked),
                      Layer = 0.1f,
                      Scale = 1.15f,
                  },

            };
        }

        private void PlayMainMenuMusic()
        {
            AudioSettings.ApplyMenuVolume();
            MediaPlayer.IsRepeating = true;

            if (MediaPlayer.State == MediaState.Playing)
                return;

            var songMainMenu = _content.Load<Song>("soundMainMenu");
            MediaPlayer.Play(songMainMenu);
        }

        private void CreateSettingsComponents(Texture2D buttonTexture)
        {
            _settingsButton = new Button(buttonTexture, _font)
            {
                Text = "Настройки",
                Position = new Vector2(Game1.ScreenWidth / 2, 520),
                Click = new EventHandler(SettingsButton_Clicked),
                Layer = 0.1f,
                Scale = 1.15f,
            };

            _muteButton = new Button(buttonTexture, _font)
            {
                Position = new Vector2(Game1.ScreenWidth / 2, 440),
                Click = new EventHandler(MuteButton_Clicked),
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

        private void SettingsButton_Clicked(object sender, EventArgs args)
        {
            _settingsOpen = !_settingsOpen;
        }

        private void MuteButton_Clicked(object sender, EventArgs args)
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

        private void Button_1Player_Clicked(object sender, EventArgs args)
        {
            _game.ChangeState(new GameState(_game, _content)
            {
                PlayerCount = 1,
            });
        }

        private void Button_2Player_Clicked(object sender, EventArgs args)
        {
            _game.ChangeState(new GameState(_game, _content)
            {
                PlayerCount = 2,
            });
        }

        private void Button_Highscores_Clicked(object sender, EventArgs args)
        {
            _game.ChangeState(new HighscoresState(_game, _content));
        }

        private void Button_Quit_Clicked(object sender, EventArgs args)
        {
            _game.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            if (_settingsOpen)
            {
                _settingsButton.Update(gameTime);

                foreach (var component in _settingsComponents)
                    component.Update(gameTime);

                return;
            }

            foreach (var component in _components)
                component.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

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
