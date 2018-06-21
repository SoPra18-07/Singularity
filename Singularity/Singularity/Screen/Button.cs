using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Libraries;


namespace Singularity.Screen
{
    /// <summary>
    /// Button class allows buttons to be created using either
    /// text or Texture2D. The buttons can send out Clicked, Released,
    /// and Hovering events that other classes can subscribe to.
    /// </summary>
    class Button : IWindowItem
    {
        // bool to indicated if the button is made of text
        private readonly bool _mIsText;

        private readonly float _mScale;
        private readonly Texture2D _mButtonTexture;
        private readonly string _mButtonText;
        private readonly Vector2 _mPosition;
        private readonly int _mWidth;
        private readonly int _mHeight;
        private readonly SpriteFont _mFont;

        // distinguish between mouse over hover or not
        private Color _mColor;

        private Rectangle _mBounds;
        private bool _mClicked;
        private bool _mWithBorder;

        /// <summary>
        /// Opacity of the button useful for transitions or transparent buttons
        /// </summary>
        public float Opacity { private get; set; }

        // these events are sent out when they occur to an
        // instance of a button
        // HOW TO USE:
        // put this in the class where an instance of a button is generated:
        // NameOfButtonInsance.Event += ClassToReceiveEvent.NameOfMethodInWhichClassReceivesEvent;
        // the above subscribes the "ClassToReceiveEvent" to the button event (it activate the "NameOfMethodInWhichClassReceivesEvent")
        // when the event occurs
        // In the ClassToReceiveEvent create the method NameOfMethodInWhichClassReceivesEvent and write in the
        // code you which to be executed when Event occurs
        // public static void NameOfMethodInWhichClassReceivesEvent(Object sender, EventArgs eventArg){ .....}
        public event EventHandler ButtonReleased;
        public event EventHandler ButtonHovering;
        public event EventHandler ButtonClicked;


        /// <summary>
        /// Creates a button using a Texture2D
        /// </summary>
        /// <param name="scale"> scale of the texture</param>
        /// <param name="buttonTexture"></param>
        /// <param name="position"></param>
        public Button(float scale, Texture2D buttonTexture, Vector2 position, bool withBorder)
        {
            _mIsText = false;
            _mScale = scale;
            _mButtonTexture = buttonTexture;
            _mPosition = position;
            _mWidth = (int)(_mButtonTexture.Width * scale);
            _mHeight = (int)(_mButtonTexture.Height * scale);
            _mColor = Color.White;
            CreateRectangularBounds();
            Opacity = 1;

        }



        /// <summary>
        /// Creates a Button made of text
        /// </summary>
        /// <param name="buttonText">text that button will appear as</param>
        /// <param name="font"></param>
        /// <param name="position"></param>
        public Button(string buttonText, SpriteFont font, Vector2 position)
        {
            _mIsText = true;
            _mButtonText = buttonText;
            _mFont = font;
            _mPosition = position;
            _mWidth = (int)_mFont.MeasureString(_mButtonText).X;
            _mHeight = (int)_mFont.MeasureString(_mButtonText).Y;
            _mColor = Color.White;
            CreateRectangularBounds();
        }

        public Button(string buttonText, SpriteFont font, Vector2 position, Color color)
        {
            _mIsText = true;
            _mButtonText = buttonText;
            _mFont = font;
            _mPosition = position;
            _mWidth = (int)_mFont.MeasureString(_mButtonText).X;
            _mHeight = (int)_mFont.MeasureString(_mButtonText).Y;
            _mColor = color;
            CreateRectangularBounds();
        }


        /// <summary>
        /// Creates the bounding box that the button is contained in
        /// </summary>
        private void CreateRectangularBounds()
        {
            _mBounds = new Rectangle((int)_mPosition.X, (int)_mPosition.Y, _mWidth, _mHeight);
        }

        /// <summary>
        /// Sends out event that button has been left click released
        /// </summary>
        protected virtual void OnButtonReleased()
        {
            if (ButtonReleased != null)
            {
                ButtonReleased(this, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Sends out event that mouse is hovering over the button
        /// </summary>
        protected virtual void OnButtonHovering()
        {
            if (ButtonHovering != null)
            {
                ButtonHovering(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sends out event that button has been right clicked
        /// </summary>
        protected virtual void OnButtonClicked()
        {
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Draws either ther Texture2D or string representation of
        /// the button.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw for button that uses a Texture2D
            if (_mIsText == false)
            {
                spriteBatch.Draw(_mButtonTexture,
                    _mPosition,
                    null,
                    _mColor * Opacity,
                    0f,
                    new Vector2(0, 0),
                    _mScale,
                    SpriteEffects.None,
                    0f);
                if (_mWithBorder)
                {
                    // draw border around texture if feauture selected
                    spriteBatch.DrawRectangle(mPosition, new Vector2(mWidth, mHeight), Color.White, 1);
                }

            }

            // draw for button that uses text
            else
            {
                spriteBatch.DrawString(_mFont,
                    origin: Vector2.Zero,
                    position: _mPosition,
                    color: _mColor * Opacity,
                    text: _mButtonText,
                    rotation: 0f,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0.2f);
            }
        }


        /// <summary>
        /// Updates the button so that it is displayed darker if hovered over,
        /// as well as sending out button events through user interactions.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // if mouse is hovering over button then make draw color gray
            if (Mouse.GetState().X >= _mPosition.X &&
                Mouse.GetState().X <= _mPosition.X + _mWidth &&
                Mouse.GetState().Y > _mPosition.Y &&
                Mouse.GetState().Y <= _mPosition.Y + _mHeight)
            {
                OnButtonHovering();
                _mColor = Color.Gray;
            }

            // otherwise keep draw color at white
            else
            {
                _mColor = Color.White;
            }

            // check if button has been left clicked
            if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                Mouse.GetState().X >= _mPosition.X &&
                Mouse.GetState().X <= _mPosition.X + _mWidth &&
                Mouse.GetState().Y >= _mPosition.Y &&
                Mouse.GetState().Y <= _mPosition.Y + _mHeight)
            {
                OnButtonClicked();
                _mClicked = true;
            }


            // check if left button is also released within button bounds to send out ButtonReleased event

            if (Mouse.GetState().LeftButton == ButtonState.Released && _mClicked)
            {
                if (Mouse.GetState().X >= _mPosition.X &&
                    Mouse.GetState().X <= _mPosition.X + _mWidth &&
                    Mouse.GetState().Y >= _mPosition.Y &&
                    Mouse.GetState().Y <= _mPosition.Y + _mHeight)
                {
                    OnButtonReleased();
                }

                // reset to not clicked
                _mClicked = false;
            }
        }
    }
}
