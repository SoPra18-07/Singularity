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

        public EScreen Screen { get; private set; } = EScreen.SaveGameScreen;
        public bool Loaded { get; set; }

        // TODO either add bloom to the text or make it a sprite
        private Texture2D mLogoTexture2D;
        private Texture2D mSingularityText;
        private readonly Vector2 mLogoPosition;
        private readonly float mScaleMultiplier;
        private readonly Vector2 mSingularityTextPosition;
        private readonly Vector2 mTextPosition;
        private SpriteFont mLibSans20;
        private Vector2 mStringCenter;
        private readonly string mContinueString;

        // Transition variables
        public bool TransitionRunning { get; private set; }
        private int mTransitionStep;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private float mHoloOpacity;
        private float mTextOpacity;
        private bool mSecondFrame;

        /// <summary>
        /// Creates an instance of the splash screen.
        /// </summary>
        /// <param name="screenResolution">Viewport resolution used for scaling.</param>
        public SplashScreen(Vector2 screenResolution)
        {
            mLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 3);
            mSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            mTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

            // logo should fill 0.42 * screen height
            mScaleMultiplier = screenResolution.Y * 0.42f / 557f;

            mContinueString = "Press any key to continue";

            TransitionRunning = false;
            mHoloOpacity = 1f;
            mTextOpacity = 1f;
            mTransitionStep = 0;
            mSecondFrame = false;
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (!TransitionRunning)
            {
                switch (targetScreen)
                {
                    case EScreen.MainMenuScreen:
                        TransitionRunning = true;
                        mTransitionStep = 1;
                        mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        mTransitionDuration = 500d;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(originScreen),
                            originScreen,
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
                switch (mTransitionStep)
                {
                    // the steps of the transition:
                    // step 0: initial pre animation
                    // step 1: text becomes invisible
                    // step 2: text becomes visible
                    // step 3: text becomes invisible
                    // step 4: text becomes visible
                    // step 5: start fade out
                    case (1):
                        mTextOpacity = 0f;
                        if (mSecondFrame)
                        {
                            mTransitionStep = 2;
                        }

                        mSecondFrame = !mSecondFrame;

                        break;
                    case (2):
                        mTextOpacity = 1f;
                        if (mSecondFrame)
                        {
                            mTransitionStep = 3;
                        }

                        mSecondFrame = !mSecondFrame;
                        break;
                    case (3):
                        mTextOpacity = 0f;
                        if (mSecondFrame)
                        {
                            mTransitionStep = 4;
                        }

                        mSecondFrame = !mSecondFrame;
                        break;
                    case (4):
                        mTextOpacity = 1f;
                        if (mSecondFrame)
                        {
                            mTransitionStep = 5;
                        }

                        mSecondFrame = !mSecondFrame;
                        break;
                    case (5):
                        mHoloOpacity = (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gametime);

                        if (gametime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                        {
                            TransitionRunning = false;
                            mHoloOpacity = 0f;
                        }

                        mTransitionStep = 5;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

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
                color: Color.AliceBlue * mHoloOpacity,
                rotation: 0f,
                scale: mScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(mSingularityText,
                origin: new Vector2(322, 41),
                position: mSingularityTextPosition,
                color: Color.AliceBlue * mHoloOpacity,
                rotation: 0f,
                scale: mScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(mLibSans20,
                origin: mStringCenter,
                position: mTextPosition,
                color: new Color(new Vector3(.9137f, .9058f, .8314f)) * mHoloOpacity * mTextOpacity,
                text: mContinueString,
                rotation: 0f,
                scale: mScaleMultiplier,
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
