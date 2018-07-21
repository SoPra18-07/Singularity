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

        // bool to indicate if the button needs to crop the texture2D with a sourceRectangle
        private readonly bool mCrop;

        // bool to indicate if the mouse was just hovering or not
        private bool mHoveringStarted;

        // sourceRectangle to crop the texture2D if needed
        private readonly Rectangle mSourceRectangle;

        private readonly float mScale;
        private readonly Texture2D mButtonTexture;
        private string mButtonText;
        private readonly SpriteFont mFont;
        private readonly SpriteEffects mSpriteEffects;

        // distinguish between mouse over hover or not
        protected Color mColor;

        private Rectangle mBounds;
        private bool mClicked;
        private bool mWithBorder;
        private bool mImRelevant;
        private bool mClicking;

        /// <summary>
        /// Opacity of the button useful for transitions or transparent buttons
        /// </summary>
        public float Opacity { protected get; set; }

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
        public event EventHandler ButtonHoveringEnd;

        private readonly EventArgs mEventArgs;


        /// <summary>
        /// Creates a button using a Texture2D
        /// </summary>
        ///  <param name="scale"> Scale of the texture</param>
        /// <param name="buttonTexture">Texture to be used as the button.</param>
        /// <param name="position">Position of the button.</param>
        /// <param name="withBorder">Whether the texture should have a border or not.</param>
        /// <param name="eventArgs">Any custom event args that may be used.</param>
        /// <param name="spriteEffects">The Sprite Effects to be used on the texture. Defaults to none.</param>
        public Button(float scale, Texture2D buttonTexture, Vector2 position, bool withBorder, EventArgs eventArgs = default(EventArgs), SpriteEffects spriteEffects = SpriteEffects.None)
        {
            mIsText = false;
            mScale = scale;
            mButtonTexture = buttonTexture;
            Position = position;
            Size = new Vector2((int)(mButtonTexture.Width * scale), (int)(mButtonTexture.Height * scale));
            mColor = Color.White;
            CreateRectangularBounds();
            Opacity = 1;
            ActiveInWindow = true;
            mWithBorder = withBorder;
            mEventArgs = eventArgs;
            mSpriteEffects = spriteEffects;
        }


        /// <summary>
        /// Creates a button using a Texture2D + a cropping AKA source vector
        /// </summary>
        /// <param name="scale"> scale of the texture</param>
        /// <param name="buttonTexture"></param>
        /// <param name="sourceRectangle">crop the buttonTexture</param>
        /// <param name="position"></param>
        /// <param name="withBorder"></param>
        public Button(float scale, Texture2D buttonTexture, Rectangle sourceRectangle, Vector2 position, bool withBorder, EventArgs eventArgs = default(EventArgs), SpriteEffects spriteEffects = SpriteEffects.None)
        {
            mIsText = false;
            mCrop = true;
            mScale = scale;
            mButtonTexture = buttonTexture;
            mSourceRectangle = sourceRectangle;
            Position = position;
            Size = new Vector2(sourceRectangle.Width * scale, sourceRectangle.Height * scale);//new Vector2((int)(mButtonTexture.Width * scale), (int)(mButtonTexture.Height * scale));
            mColor = Color.White;
            CreateRectangularBounds();
            Opacity = 1;
            ActiveInWindow = true;
            mWithBorder = withBorder;
            mEventArgs = eventArgs;
        }


        /// <summary>
        /// Creates a Button made of text
        /// </summary>
        /// <param name="buttonText">text that button will appear as</param>
        /// <param name="font"></param>
        /// <param name="position"></param>
        public Button(string buttonText, SpriteFont font, Vector2 position, bool withBorder = false, EventArgs eventArgs = default(EventArgs))
        {
            mIsText = true;
            mButtonText = buttonText;
            mFont = font;
            Position = position;
            Size = new Vector2((int)mFont.MeasureString(mButtonText).X, (int)mFont.MeasureString(mButtonText).Y);
            mColor = Color.White;
            mWithBorder = withBorder;
            CreateRectangularBounds();
            ActiveInWindow = true;
            mEventArgs = eventArgs;
        }

        public Button(string buttonText, SpriteFont font, Vector2 position, Color color, bool withBorder = false, EventArgs eventArgs = default(EventArgs))
        {
            mIsText = true;
            mButtonText = buttonText;
            mFont = font;
            Position = position;
            Size = new Vector2((int)mFont.MeasureString(mButtonText).X, (int)mFont.MeasureString(mButtonText).Y);
            mColor = color;
            CreateRectangularBounds();
            mWithBorder = withBorder;
            ActiveInWindow = true;
            mEventArgs = eventArgs;
        }


        /// <summary>
        /// Creates the bounding box that the button is contained in
        /// </summary>
        private void CreateRectangularBounds()
        {
            mBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        /// <summary>
        /// Sends out event that button has been left click released
        /// </summary>
        protected virtual void OnButtonReleased()
        {
            if (ButtonReleased != null && ActiveInWindow)
            {
                ButtonReleased(this, mEventArgs);
            }

        }

        /// <summary>
        /// Sends out event that mouse is hovering over the button
        /// </summary>
        protected virtual void OnButtonHovering()
        {
            if (ButtonHovering != null && ActiveInWindow)
            {
                ButtonHovering(this, mEventArgs);
            }
        }

        /// <summary>
        /// Sends out event that mouse is no longer hovering over the button
        /// </summary>
        protected virtual void OnButtonHoveringEnd()
        {
            if (ButtonHoveringEnd != null && ActiveInWindow)
            {
                ButtonHoveringEnd(this, mEventArgs);
            }
        }

        /// <summary>
        /// Sends out event that button has been right clicked
        /// </summary>
        protected virtual void OnButtonClicked()
        {
            if (ActiveInWindow)
            {
                ButtonClicked?.Invoke(this, mEventArgs);
            }
        }


        /// <summary>
        /// Draws either ther Texture2D or string representation of
        /// the button.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle && !WindowIsInactive)
            {

                if (mWithBorder)
                {
                    // draw border around texture if feauture selected, also give a small padding
                    spriteBatch.DrawRectangle(new Vector2(Position.X - 2, Position.Y - 2), new Vector2(Size.X + 4, Size.Y + 4), Color.White, 1);
                }

                // draw for button that uses a Texture2D
                if (mIsText == false)
                {
                    // draw for buttons which need to crop the texture
                    if (mCrop)
                    {
                        spriteBatch.Draw(mButtonTexture,
                            Position,
                            mSourceRectangle,
                            mColor * Opacity,
                            0f,
                            new Vector2(0, 0),
                            mScale,
                            mSpriteEffects,
                            0f);
                    }
                    // draw for buttons which do not need to crop the texture
                    else
                    {
                        spriteBatch.Draw(mButtonTexture,
                            Position,
                            null,
                            mColor * Opacity,
                            0f,
                            new Vector2(0, 0),
                            mScale,
                            mSpriteEffects,
                            0f);
                    }

                }

                // draw for button that uses text
                else
                {
                    spriteBatch.DrawString(mFont,
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
        public virtual void Update(GameTime gametime)
        {
            if (ActiveInWindow && !InactiveInSelectedPlatformWindow && !OutOfScissorRectangle && !WindowIsInactive)
            {
                // if mouse is hovering over button then make draw color gray
                if (Mouse.GetState().X >= Position.X &&
                    Mouse.GetState().X <= Position.X + Size.X &&
                    Mouse.GetState().Y > Position.Y &&
                    Mouse.GetState().Y <= Position.Y + Size.Y)
                {
                    OnButtonHovering();
                    mColor = Color.Gray;

                    // set no hovering -> hovering
                    mHoveringStarted = true;
                }

                // otherwise keep draw color at white + call hoveringEnd event
                else
                {
                    mColor = Color.White;

                    // set hovering -> no hovering
                    if (mHoveringStarted)
                    {
                        OnButtonHoveringEnd();
                        mHoveringStarted = false;
                    }
                }

                // check if button has been left clicked
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mClicking)
                {
                    if (Mouse.GetState().X >= Position.X &&
                        Mouse.GetState().X <= Position.X + Size.X &&
                        Mouse.GetState().Y >= Position.Y &&
                        Mouse.GetState().Y <= Position.Y + Size.Y)
                    {
                        mClicked = true;
                    }
                    mClicking = true;
                }

                // check if left button is also released within button bounds to send out ButtonReleased event

                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    if (mClicked && mClicking)
                    {
                        if (Mouse.GetState().X >= Position.X &&
                            Mouse.GetState().X <= Position.X + Size.X &&
                            Mouse.GetState().Y >= Position.Y &&
                            Mouse.GetState().Y <= Position.Y + Size.Y)
                        {
                            OnButtonClicked();
                            OnButtonReleased();
                        }

                        // reset to not clicked
                        mClicked = false;
                    }
                    mClicked = false;
                    mClicking = false;
                }

            }
        }

        public void ChangeText(string newText)
        {
            if (!mIsText)
            {
                return;
            }

            mButtonText = newText;
            Size = new Vector2((int)mFont.MeasureString(mButtonText).X, (int)mFont.MeasureString(mButtonText).Y);
        }

        public void AddBorder()
        {
            mWithBorder = true;
        }

        public void RemoveBorder()
        {
            mWithBorder = false;
        }

        // position of the button
        public Vector2 Position { get; set; }

        // Size of the button
        public Vector2 Size { get; protected set; }

        // active button <-> inactive button
        public bool ActiveInWindow { get; set; }
        public bool InactiveInSelectedPlatformWindow { get; set; }
        public bool OutOfScissorRectangle { get; set; }
        public bool WindowIsInactive { get; set; }
    }
}
