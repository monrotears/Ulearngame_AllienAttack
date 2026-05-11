using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UlearnGame.Controller
{
    public class Slider : Component
    {
        private readonly SpriteFont _font;
        private readonly Texture2D _texture;
        private MouseState _currentMouse;
        private bool _isDragging;
        private MouseState _previousMouse;

        public Slider(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
        }

        public string Text { get; set; }

        public Vector2 Position { get; set; }

        public int Width { get; set; } = 260;

        public float Value { get; set; }

        public Action<float> ValueChanged { get; set; }

        private Rectangle BarRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y + 28, Width, 8);
            }
        }

        private Rectangle HandleRectangle
        {
            get
            {
                var x = (int)(Position.X + Width * Value) - 8;
                return new Rectangle(x, (int)Position.Y + 20, 16, 24);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            if (_currentMouse.LeftButton == ButtonState.Pressed &&
                _previousMouse.LeftButton == ButtonState.Released &&
                (mouseRectangle.Intersects(BarRectangle) || mouseRectangle.Intersects(HandleRectangle)))
            {
                _isDragging = true;
            }

            if (_currentMouse.LeftButton == ButtonState.Released)
                _isDragging = false;

            if (_isDragging)
            {
                var value = (_currentMouse.X - Position.X) / Width;
                Value = MathHelper.Clamp(value, 0f, 1f);
                ValueChanged?.Invoke(Value);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, $"{Text}: {(int)(Value * 100)}%", Position, Color.White);
            spriteBatch.Draw(_texture, BarRectangle, Color.DarkGray);
            spriteBatch.Draw(_texture, new Rectangle(BarRectangle.X, BarRectangle.Y, (int)(BarRectangle.Width * Value), BarRectangle.Height), Color.LightSkyBlue);
            spriteBatch.Draw(_texture, HandleRectangle, Color.White);
        }
    }
}
