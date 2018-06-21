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

        private Texture2D _mGlowTexture2D;
        private Texture2D _mHoloProjectionTexture2D;
        private Vector2 _mScreenCenter;
        private Vector2 _mScreenResolutionScaling;
        private float _mHoloProjectionWidthScaling;
        private float _mHoloProjectionHeightScaling;
        private Vector2 _mHoloProjectionScaling;

        public EScreen CurrentScreen { get; private set; }
        private double _mTransitionStartTime;
        private double _mTransitionDuration;
        private float _mTransitionInitialValue;
        private float _mTransitionTargetValue;
        private EScreen _mTargetScreen;
        private int _mFrameCounter;

        public bool TransitionRunning { get; private set; }

        // Variables for flickering light
        private float _mFlickerDuration;
        private bool _mFlickerDirectionUp;
        private float _mHoloOpacity;
        private float _mTargetHoloOpacity;
        private float _mFlickerStep;
        private bool _mFlickering;


        /// <summary>
        /// Creates the MenuBackgroundScreen class.
        /// </summary>
        /// <param name="screenResolution">Current screen resolution.</param>
        public MenuBackgroundScreen(Vector2 screenResolution)
        {
            _mHoloProjectionWidthScaling = 1f;
            _mHoloProjectionHeightScaling = screenResolution.Y / 1024;
            SetResolution(screenResolution);
            CurrentScreen = EScreen.SplashScreen;

            TransitionRunning = false;
            _mHoloOpacity = 1;
            _mFlickerDirectionUp = true;
            _mFrameCounter = 0;
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
            _mScreenCenter = new Vector2(screenResolution.X / 2, screenResolution.Y / 2);
            _mScreenResolutionScaling = new Vector2(screenResolution.X / 1280, screenResolution.Y / 1024);
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
            _mTargetScreen = targetScreen;
            switch (targetScreen)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    _mTransitionTargetValue = 2f;
                    _mTransitionDuration = 500;
                    break;
                case EScreen.OptionsScreen:
                    _mTransitionTargetValue = 4f;
                    _mTransitionDuration = 300;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            // SetHoloProjectionScaling(mHoloProjectionWidthScaling);
            TransitionRunning = true;
            _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            _mTransitionInitialValue = _mHoloProjectionWidthScaling;
        }

        /// <summary>
        /// Loads content specific to this screen
        /// </summary>
        /// <param name="content">ContentManager for the entire game</param>
        public void LoadContent(ContentManager content)
        {
            _mGlowTexture2D = content.Load<Texture2D>("Glow");
            _mHoloProjectionTexture2D = content.Load<Texture2D>("HoloProjection");
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
            if (!_mFlickering)
            {
                // create a new random number generator
                var rnd = new Random();

                // choose target opacity and flicker duration
                var limit = (int) (_mHoloOpacity * 100);
                if (_mFlickerDirectionUp)
                {
                    if (limit <= 50)
                    {
                        limit = 50;
                    }
                    else
                    {
                        limit = limit - 10;
                    }

                    _mTargetHoloOpacity = rnd.Next(40, limit) / 100f;
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

                    _mTargetHoloOpacity = rnd.Next(limit, 100) / 100f;
                }

                _mFlickerDirectionUp = !_mFlickerDirectionUp;

                _mFlickerDuration = rnd.Next(10, 30);
                _mFlickerStep = (_mTargetHoloOpacity - _mHoloOpacity) / _mFlickerDuration;
                _mFlickering = true;
            }
            else
            {
                _mHoloOpacity += _mFlickerStep;

                // "Finish flicker" state set
                if (_mFlickerStep > 0)
                {
                    if (_mHoloOpacity >= _mTargetHoloOpacity)
                    {
                        _mFlickering = false;
                    }
                }
                else if (_mFlickerStep < 0)
                {
                    if (_mHoloOpacity <= _mTargetHoloOpacity)
                    {
                        _mFlickering = false;
                    }
                }
                else
                {
                    _mFlickering = false;
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
                    if (_mFrameCounter < 20)
                    {
                        _mFrameCounter += 1;
                    }
                    else if (_mFrameCounter == 20)
                    {
                        _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        _mFrameCounter += 1;
                    }
                    else
                    {
                        _mHoloProjectionWidthScaling = (float) Animations.Easing(_mTransitionInitialValue,
                            _mTransitionTargetValue,
                            _mTransitionStartTime,
                            _mTransitionDuration,
                            gameTime);

                        if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                        {
                            TransitionRunning = false;
                            CurrentScreen = _mTargetScreen;
                        }
                    }
                }
                else
                {
                    _mHoloProjectionWidthScaling = (float) Animations.Easing(_mTransitionInitialValue,
                        _mTransitionTargetValue,
                        _mTransitionStartTime,
                        _mTransitionDuration,
                        gameTime);

                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        CurrentScreen = _mTargetScreen;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw glow
            spriteBatch.Draw(_mGlowTexture2D,
                _mScreenCenter,
                null,
                Color.AliceBlue,
                0f,
                new Vector2(609, 553),
                _mScreenResolutionScaling.X < _mScreenResolutionScaling.Y ? _mScreenResolutionScaling.X : _mScreenResolutionScaling.Y, // Scales based on smaller scalar between height and width,
                SpriteEffects.None,
                1f);

            // draw holoProjection texture without scaling
            spriteBatch.Draw(_mHoloProjectionTexture2D,
                new Vector2(_mScreenCenter.X, _mScreenCenter.Y * 2 - 20),
                null,
                Color.White * _mHoloOpacity,
                0f,
                new Vector2(367, 1033),
                new Vector2(_mHoloProjectionWidthScaling, _mHoloProjectionHeightScaling),
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
