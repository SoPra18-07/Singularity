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
        private readonly bool mIsText;

        private readonly float mScale;
        private readonly Texture2D mButtonTexture;
        private readonly string mButtonText;
        private readonly SpriteFont mFont;

        // distinguish between mouse over hover or not
        private Color mColor;

        private Rectangle mBounds;
        private bool mClicked;
        private bool mWithBorder;

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
        /// todo: @N @yvan write comments ... everywhere, at least some.
        /// <param name="scale"> scale of the texture</param>
        /// <param name="buttonTexture"></param>
        /// <param name="position"></param>
        /// <param name="withBorder"></param>
        public Button(float scale, Texture2D buttonTexture, Vector2 position, bool withBorder)
        {
            mIsText = false;
            mScale = scale;
            mButtonTexture = buttonTexture;
            Position = position;
            Size = new Vector2(x: (int)(mButtonTexture.Width * scale), y: (int)(mButtonTexture.Height * scale));
            mColor = Color.White;
            CreateRectangularBounds();
            Opacity = 1;
            Active = true;
        }



        /// <summary>
        /// Creates a Button made of text
        /// </summary>
        /// <param name="buttonText">text that button will appear as</param>
        /// <param name="font"></param>
        /// <param name="position"></param>
        public Button(string buttonText, SpriteFont font, Vector2 position)
        {
            mIsText = true;
            mButtonText = buttonText;
            mFont = font;
            Position = position;
            Size = new Vector2(x: (int)mFont.MeasureString(text: mButtonText).X, y: (int)mFont.MeasureString(text: mButtonText).Y);
            mColor = Color.White;
            CreateRectangularBounds();
            Active = true;
        }

        public Button(string buttonText, SpriteFont font, Vector2 position, Color color)
        {
            mIsText = true;
            mButtonText = buttonText;
            mFont = font;
            Position = position;
            Size = new Vector2(x: (int)mFont.MeasureString(text: mButtonText).X, y: (int)mFont.MeasureString(text: mButtonText).Y);
            mColor = color;
            CreateRectangularBounds();
            Active = true;
        }


        /// <summary>
        /// Creates the bounding box that the button is contained in
        /// </summary>
        private void CreateRectangularBounds()
        {
            mBounds = new Rectangle(x: (int)Position.X, y: (int)Position.Y, width: (int)Size.X, height: (int)Size.Y);
        }

        /// <summary>
        /// Sends out event that button has been left click released
        /// </summary>
        protected virtual void OnButtonReleased()
        {
            if (ButtonReleased != null && Active)
            {
                ButtonReleased(sender: this, e: EventArgs.Empty);
            }

        }

        /// <summary>
        /// Sends out event that mouse is hovering over the button
        /// </summary>
        protected virtual void OnButtonHovering()
        {
            if (ButtonHovering != null && Active)
            {
                ButtonHovering(sender: this, e: EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sends out event that button has been right clicked
        /// </summary>
        protected virtual void OnButtonClicked()
        {
            if (Active)
            {
                ButtonClicked?.Invoke(sender: this, e: EventArgs.Empty);
            }
        }


        /// <summary>
        /// Draws either ther Texture2D or string representation of
        /// the button.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                // draw for button that uses a Texture2D
                if (mIsText == false)
                {
                    spriteBatch.Draw(texture: mButtonTexture,
                        position: Position,
                        sourceRectangle: null,
                        color: mColor * Opacity,
                        rotation: 0f,
                        origin: new Vector2(x: 0, y: 0),
                        scale: mScale,
                        effects: SpriteEffects.None,
                        layerDepth: 0f);
                    if (mWithBorder)
                    {
                        // draw border around texture if feauture selected
                        spriteBatch.DrawRectangle(location: Position, size: new Vector2(x: Size.X, y: Size.Y), color: Color.White, thickness: 1);
                    }

                }

                // draw for button that uses text
                else
                {
                    spriteBatch.DrawString(spriteFont: mFont,
                        origin: Vector2.Zero,
                        position: Position,
                        color: mColor * Opacity,
                        text: mButtonText,
                        rotation: 0f,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: 0.2f);
                }
            }
        }


        /// <summary>
        /// Updates the button so that it is displayed darker if hovered over,
        /// as well as sending out button events through user interactions.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (Active)
            {
                // if mouse is hovering over button then make draw color gray
                if (Mouse.GetState().X >= Position.X &&
                    Mouse.GetState().X <= Position.X + Size.X &&
                    Mouse.GetState().Y > Position.Y &&
                    Mouse.GetState().Y <= Position.Y + Size.Y)
                {
                    OnButtonHovering();
                    mColor = Color.Gray;
                }

                // otherwise keep draw color at white
                else
                {
                    mColor = Color.White;
                }

                // check if button has been left clicked
                if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                    Mouse.GetState().X >= Position.X &&
                    Mouse.GetState().X <= Position.X + Size.X &&
                    Mouse.GetState().Y >= Position.Y &&
                    Mouse.GetState().Y <= Position.Y + Size.Y)
                {
                    OnButtonClicked();
                    mClicked = true;
                }


                // check if left button is also released within button bounds to send out ButtonReleased event

                if (Mouse.GetState().LeftButton == ButtonState.Released && mClicked)
                {
                    if (Mouse.GetState().X >= Position.X &&
                        Mouse.GetState().X <= Position.X + Size.X &&
                        Mouse.GetState().Y >= Position.Y &&
                        Mouse.GetState().Y <= Position.Y + Size.Y)
                    {
                        OnButtonReleased();
                    }

                    // reset to not clicked
                    mClicked = false;
                }
            }
        }

        // position of the button
        public Vector2 Position { get; set; }

        // Size of the button
        public Vector2 Size { get; }

        // active button <-> inactive button
        public bool Active { get; set; }
    }
}
