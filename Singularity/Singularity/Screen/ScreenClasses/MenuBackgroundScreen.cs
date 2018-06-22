using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    // TODO animate the menu screen
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// All the main menu screens are overlayed on top of this screen.
    /// Since the main menu will have the same animated background, it will
    /// simply use the same background screen and be overlayed on top of it.
    /// </summary>
    class MenuBackgroundScreen : ITransitionableMenu
    {
        public bool Loaded { get; set; }

        private Texture2D mGlowTexture2D;
        private Texture2D mHoloProjectionTexture2D;
        private Vector2 mScreenCenter;
        private Vector2 mScreenResolutionScaling;
        private float mHoloProjectionWidthScaling;
        private float mHoloProjectionHeightScaling;

        public EScreen CurrentScreen { get; private set; }
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private float mTransitionInitialValue;
        private float mTransitionTargetValue;
        private EScreen mTargetScreen;
        private int mFrameCounter;

        public bool TransitionRunning { get; private set; }

        // Variables for flickering light
        private float mFlickerDuration;
        private bool mFlickerDirectionUp;
        private float mHoloOpacity;
        private float mTargetHoloOpacity;
        private float mFlickerStep;
        private bool mFlickering;


        /// <summary>
        /// Creates the MenuBackgroundScreen class.
        /// </summary>
        /// <param name="screenResolution">Current screen resolution.</param>
        public MenuBackgroundScreen(Vector2 screenResolution)
        {
            mHoloProjectionWidthScaling = 1f;
            mHoloProjectionHeightScaling = screenResolution.Y / 1024;
            SetResolution(screenResolution);
            CurrentScreen = EScreen.SplashScreen;

            TransitionRunning = false;
            mHoloOpacity = 1;
            mFlickerDirectionUp = true;
            mFrameCounter = 0;
        }

        /*
        // TODO make holo projection width based on height. If this function proves unnecssary, then remove it.
        /// <summary>
        /// Determines the scaling of the holoprojection polygon by first making it fit within the screen then
        /// making it stretch to the appropriate width.
        /// </summary>
        /// <param name="widthScaling">Scalar factor of how stretched out the projection should be.</param>
        private void SetHoloProjectionScaling(float widthScaling)
        {
            if (mScreenResolutionScaling.X < mScreenResolutionScaling.Y)
            {
                mHoloProjectionScaling = new Vector2(mScreenResolutionScaling.X);
            }
            else
            {
                mHoloProjectionScaling = new Vector2(mScreenResolutionScaling.Y);
            }

            mHoloProjectionScaling = Vector2.Multiply(mHoloProjectionScaling, new Vector2(widthScaling, 1f));
        }
        */

        /// <summary>
        /// Changes the dimensions of the screen to fit the viewport resolution.
        /// </summary>
        /// <param name="screenResolution">Current viewport screen resolution</param>
        private void SetResolution(Vector2 screenResolution)
        {
            mScreenCenter = new Vector2(screenResolution.X / 2, screenResolution.Y / 2);
            mScreenResolutionScaling = new Vector2(screenResolution.X / 1280, screenResolution.Y / 1024);
            //SetHoloProjectionScaling(mHoloProjectionWidthScaling);
        }

        /// <summary>
        /// Changes the HoloProjectionTexture width based on the screen being shown and
        /// starts the animation for the screen.
        /// </summary>
        /// <param name="originSscreen">Choose the screen to be overlayed on top
        ///     of the menu background</param>
        /// <param name="targetScreen"></param>
        /// <param name="gameTime">gameTime used to calculate animations</param>
        public void TransitionTo(EScreen originSscreen, EScreen targetScreen, GameTime gameTime)
        {
            mTargetScreen = targetScreen;
            switch (targetScreen)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    mTransitionTargetValue = 2f;
                    mTransitionDuration = 500;
                    break;
                case EScreen.OptionsScreen:
                    mTransitionTargetValue = 4f;
                    mTransitionDuration = 300;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            // SetHoloProjectionScaling(mHoloProjectionWidthScaling);
            TransitionRunning = true;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            mTransitionInitialValue = mHoloProjectionWidthScaling;
        }

        /// <summary>
        /// Loads content specific to this screen
        /// </summary>
        /// <param name="content">ContentManager for the entire game</param>
        public void LoadContent(ContentManager content)
        {
            mGlowTexture2D = content.Load<Texture2D>("Glow");
            mHoloProjectionTexture2D = content.Load<Texture2D>("HoloProjection");
        }

        /// <summary>
        /// Update animates the background. It uses the Easing animation method and also implements
        /// a flickering animation to the HoloProjectionTexture.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // code for transitioning
            Transition(gameTime);

            Flicker();
        }

        /// <summary>
        /// Handles flickering of the HoloProjectionTexture
        /// </summary>
        private void Flicker()
        {
            if (!mFlickering)
            {
                // create a new random number generator
                var rnd = new Random();

                // choose target opacity and flicker duration
                var limit = (int) (mHoloOpacity * 100);
                if (mFlickerDirectionUp)
                {
                    if (limit <= 50)
                    {
                        limit = 50;
                    }
                    else
                    {
                        limit = limit - 10;
                    }

                    mTargetHoloOpacity = rnd.Next(40, limit) / 100f;
                }
                else
                {
                    if (limit >= 90)
                    {
                        limit = 90;
                    }
                    else
                    {
                        limit = limit + 10;
                    }

                    mTargetHoloOpacity = rnd.Next(limit, 100) / 100f;
                }

                mFlickerDirectionUp = !mFlickerDirectionUp;

                mFlickerDuration = rnd.Next(10, 30);
                mFlickerStep = (mTargetHoloOpacity - mHoloOpacity) / mFlickerDuration;
                mFlickering = true;
            }
            else
            {
                mHoloOpacity += mFlickerStep;

                // "Finish flicker" state set
                if (mFlickerStep > 0)
                {
                    if (mHoloOpacity >= mTargetHoloOpacity)
                    {
                        mFlickering = false;
                    }
                }
                else if (mFlickerStep < 0)
                {
                    if (mHoloOpacity <= mTargetHoloOpacity)
                    {
                        mFlickering = false;
                    }
                }
                else
                {
                    mFlickering = false;
                }
            }

        }

        /// <summary>
        /// Handles transitions between screens
        /// </summary>
        /// <param name="gameTime">Current GameTime and is required to make transitions work</param>
        private void Transition(GameTime gameTime)
        {
            if (TransitionRunning)
            {
                // to delay the transition by 20 frames to allow for the animation in the splash screen
                if (CurrentScreen == EScreen.SplashScreen)
                {
                    if (mFrameCounter < 20)
                    {
                        mFrameCounter += 1;
                    }
                    else if (mFrameCounter == 20)
                    {
                        mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        mFrameCounter += 1;
                    }
                    else
                    {
                        mHoloProjectionWidthScaling = (float) Animations.Easing(mTransitionInitialValue,
                            mTransitionTargetValue,
                            mTransitionStartTime,
                            mTransitionDuration,
                            gameTime);

                        if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                        {
                            TransitionRunning = false;
                            CurrentScreen = mTargetScreen;
                        }
                    }
                }
                else
                {
                    mHoloProjectionWidthScaling = (float) Animations.Easing(mTransitionInitialValue,
                        mTransitionTargetValue,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        CurrentScreen = mTargetScreen;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw glow
            spriteBatch.Draw(mGlowTexture2D,
                mScreenCenter,
                null,
                Color.AliceBlue,
                0f,
                new Vector2(609, 553),
                mScreenResolutionScaling.X < mScreenResolutionScaling.Y ? mScreenResolutionScaling.X : mScreenResolutionScaling.Y, // Scales based on smaller scalar between height and width,
                SpriteEffects.None,
                1f);

            // draw holoProjection texture without scaling
            spriteBatch.Draw(mHoloProjectionTexture2D,
                new Vector2(mScreenCenter.X, mScreenCenter.Y * 2 - 20),
                null,
                Color.White * mHoloOpacity,
                0f,
                new Vector2(367, 1033),
                new Vector2(mHoloProjectionWidthScaling, mHoloProjectionHeightScaling),
                SpriteEffects.None,
                0f);

            /*
             Other draw call for holoprojection texture that resizes
            // Draw holoProjection texture
            spriteBatch.Draw(mHoloProjectionTexture2D,
                mScreenCenter,
                null,
                Color.AliceBlue,
                0f,
                new Vector2(367, 515),
                mHoloProjectionScaling, // Scales based on smaller scalar between height and width
                SpriteEffects.None,
                0f);
            */
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
            return false;
        }
    }
}
