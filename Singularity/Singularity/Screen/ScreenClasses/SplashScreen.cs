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
        private Texture2D mMLogoTexture2D;
        private Texture2D mMSingularityText;
        private readonly Vector2 mMLogoPosition;
        private readonly float mMScaleMultiplier;
        private readonly Vector2 mMSingularityTextPosition;
        private readonly Vector2 mMTextPosition;
        private SpriteFont mMLibSans20;
        private Vector2 mMStringCenter;
        private readonly string mMContinueString;

        // Transition variables
        public bool TransitionRunning { get; private set; }
        private int mMTransitionStep;
        private double mMTransitionStartTime;
        private double mMTransitionDuration;
        private float mMHoloOpacity;
        private float mMTextOpacity;
        private bool mMSecondFrame;

        /// <summary>
        /// Creates an instance of the splash screen.
        /// </summary>
        /// <param name="screenResolution">Viewport resolution used for scaling.</param>
        public SplashScreen(Vector2 screenResolution)
        {
            mMLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 3);
            mMSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            mMTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

            // logo should fill 0.42 * screen height
            mMScaleMultiplier = screenResolution.Y * 0.42f / 557f;

            mMContinueString = "Press any key to continue";

            TransitionRunning = false;
            mMHoloOpacity = 1f;
            mMTextOpacity = 1f;
            mMTransitionStep = 0;
            mMSecondFrame = false;
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (!TransitionRunning)
            {
                switch (targetScreen)
                {
                    case EScreen.MainMenuScreen:
                        TransitionRunning = true;
                        mMTransitionStep = 1;
                        mMTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        mMTransitionDuration = 500d;
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
            mMLogoTexture2D = content.Load<Texture2D>("Logo");
            mMSingularityText = content.Load<Texture2D>("SingularityText");
            mMLibSans20 = content.Load<SpriteFont>("LibSans20");
            mMStringCenter = new Vector2(mMLibSans20.MeasureString(mMContinueString).X / 2, mMLibSans20.MeasureString(mMContinueString).Y / 2);
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
                switch (mMTransitionStep)
                {
                    // the steps of the transition:
                    // step 0: initial pre animation
                    // step 1: text becomes invisible
                    // step 2: text becomes visible
                    // step 3: text becomes invisible
                    // step 4: text becomes visible
                    // step 5: start fade out
                    case 1:
                        mMTextOpacity = 0f;
                        if (mMSecondFrame)
                        {
                            mMTransitionStep = 2;
                        }

                        mMSecondFrame = !mMSecondFrame;

                        break;
                    case 2:
                        mMTextOpacity = 1f;
                        if (mMSecondFrame)
                        {
                            mMTransitionStep = 3;
                        }

                        mMSecondFrame = !mMSecondFrame;
                        break;
                    case 3:
                        mMTextOpacity = 0f;
                        if (mMSecondFrame)
                        {
                            mMTransitionStep = 4;
                        }

                        mMSecondFrame = !mMSecondFrame;
                        break;
                    case 4:
                        mMTextOpacity = 1f;
                        if (mMSecondFrame)
                        {
                            mMTransitionStep = 5;
                        }

                        mMSecondFrame = !mMSecondFrame;
                        break;
                    case 5:
                        mMHoloOpacity = (float)Animations.Easing(1f, 0f, mMTransitionStartTime, mMTransitionDuration, gametime);

                        if (gametime.TotalGameTime.TotalMilliseconds >= mMTransitionStartTime + mMTransitionDuration)
                        {
                            TransitionRunning = false;
                            mMHoloOpacity = 0f;
                        }

                        mMTransitionStep = 5;
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
            spriteBatch.Draw(mMLogoTexture2D,
                origin: new Vector2(308, 279),
                position: mMLogoPosition,
                color: Color.AliceBlue * mMHoloOpacity,
                rotation: 0f,
                scale: mMScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(mMSingularityText,
                origin: new Vector2(322, 41),
                position: mMSingularityTextPosition,
                color: Color.AliceBlue * mMHoloOpacity,
                rotation: 0f,
                scale: mMScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(mMLibSans20,
                origin: mMStringCenter,
                position: mMTextPosition,
                color: new Color(new Vector3(.9137f, .9058f, .8314f)) * mMHoloOpacity * mMTextOpacity,
                text: mMContinueString,
                rotation: 0f,
                scale: mMScaleMultiplier,
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
