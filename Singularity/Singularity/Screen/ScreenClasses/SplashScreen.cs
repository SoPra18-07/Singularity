using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;


namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Shown when the game is first started and shows the logo and
    /// "Press any key to start".
    /// The logo will change depending on not the player has past the reveal
    /// point in the campaign. Not buttons are shown but it listens for any
    /// key input.
    /// </summary>
    class SplashScreen : ITransitionableMenu
    {
        // TODO either add bloom to the text or make it a sprite
        private Texture2D mLogoTexture2D;
        private Texture2D mSingularityText;
        private readonly Vector2 mLogoPosition;
        private readonly Vector2 mSingularityTextPosition;
        private readonly Vector2 mTextPosition;
        private SpriteFont mLibSans20;
        private Vector2 mStringCenter;
        private readonly string mContinueString;

        // Transition variables
        public bool TransitionRunning { get; private set; }
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private float mOpacity;

        /// <summary>
        /// Creates an instance of the splash screen.
        /// </summary>
        /// <param name="screenResolution">Viewport resolution used for scaling.</param>
        public SplashScreen(Vector2 screenResolution)
        {
            mLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 - 100);
            mSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            mTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);
            mContinueString = "Press any key to continue";
            TransitionRunning = false;
            mOpacity = 1f;
        }

        public void TransitionTo(EScreen eScreen, GameTime gameTime)
        {
            if (!TransitionRunning)
            {
                switch (eScreen)
                {
                    case EScreen.MainMenuScreen:
                        TransitionRunning = true;
                        mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        mTransitionDuration = 75d;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eScreen),
                            eScreen,
                            "Tried going from splash screen to somewhere inaccessible.");
                }
            }
        }
        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLogoTexture2D = content.Load<Texture2D>("Logo");
            mSingularityText = content.Load<Texture2D>("SingularityText");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mStringCenter = new Vector2(mLibSans20.MeasureString(mContinueString).X / 2, mLibSans20.MeasureString(mContinueString).Y / 2);
        }

        /// <summary>
        /// Updates the screen every tick.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time</param>
        public void Update(GameTime gametime)
        {
            if (TransitionRunning)
            {
                mOpacity = (float) Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gametime);

                if (gametime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                {
                    TransitionRunning = false;
                    mOpacity = 0f;
                }
            }
        }

        /// <summary>
        /// Draws all the objects that this screen uses.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch that the objects should be drawn onto.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the logo
            spriteBatch.Draw(mLogoTexture2D,
                origin: new Vector2(308, 279),
                position: mLogoPosition,
                color: Color.AliceBlue * mOpacity,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(mSingularityText,
                origin: new Vector2(322, 41),
                position: mSingularityTextPosition,
                color: Color.AliceBlue * mOpacity,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(mLibSans20,
                origin: mStringCenter,
                position: mTextPosition,
                color: Color.White * mOpacity,
                text: mContinueString,
                rotation: 0f,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0.2f);

            spriteBatch.End();

        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            return true;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            return true;
        }
    }
}
