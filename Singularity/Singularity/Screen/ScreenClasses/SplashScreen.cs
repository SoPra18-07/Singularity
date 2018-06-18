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
        private Texture2D _mLogoTexture2D;
        private Texture2D _mSingularityText;
        private readonly Vector2 _mLogoPosition;
        private readonly float _mScaleMultiplier;
        private readonly Vector2 _mSingularityTextPosition;
        private readonly Vector2 _mTextPosition;
        private SpriteFont _mLibSans20;
        private Vector2 _mStringCenter;
        private readonly string _mContinueString;

        // Transition variables
        public bool TransitionRunning { get; private set; }
        private int _mTransitionStep;
        private double _mTransitionStartTime;
        private double _mTransitionDuration;
        private float _mHoloOpacity;
        private float _mTextOpacity;
        private bool _mSecondFrame;

        /// <summary>
        /// Creates an instance of the splash screen.
        /// </summary>
        /// <param name="screenResolution">Viewport resolution used for scaling.</param>
        public SplashScreen(Vector2 screenResolution)
        {
            _mLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 3);
            _mSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            _mTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

            // logo should fill 0.42 * screen height
            _mScaleMultiplier = screenResolution.Y * 0.42f / 557f;

            _mContinueString = "Press any key to continue";

            TransitionRunning = false;
            _mHoloOpacity = 1f;
            _mTextOpacity = 1f;
            _mTransitionStep = 0;
            _mSecondFrame = false;
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (!TransitionRunning)
            {
                switch (targetScreen)
                {
                    case EScreen.MainMenuScreen:
                        TransitionRunning = true;
                        _mTransitionStep = 1;
                        _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        _mTransitionDuration = 500d;
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
            _mLogoTexture2D = content.Load<Texture2D>("Logo");
            _mSingularityText = content.Load<Texture2D>("SingularityText");
            _mLibSans20 = content.Load<SpriteFont>("LibSans20");
            _mStringCenter = new Vector2(_mLibSans20.MeasureString(_mContinueString).X / 2, _mLibSans20.MeasureString(_mContinueString).Y / 2);
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
                switch (_mTransitionStep)
                {
                    // the steps of the transition:
                    // step 0: initial pre animation
                    // step 1: text becomes invisible
                    // step 2: text becomes visible
                    // step 3: text becomes invisible
                    // step 4: text becomes visible
                    // step 5: start fade out
                    case (1):
                        _mTextOpacity = 0f;
                        if (_mSecondFrame)
                        {
                            _mTransitionStep = 2;
                        }

                        _mSecondFrame = !_mSecondFrame;

                        break;
                    case (2):
                        _mTextOpacity = 1f;
                        if (_mSecondFrame)
                        {
                            _mTransitionStep = 3;
                        }

                        _mSecondFrame = !_mSecondFrame;
                        break;
                    case (3):
                        _mTextOpacity = 0f;
                        if (_mSecondFrame)
                        {
                            _mTransitionStep = 4;
                        }

                        _mSecondFrame = !_mSecondFrame;
                        break;
                    case (4):
                        _mTextOpacity = 1f;
                        if (_mSecondFrame)
                        {
                            _mTransitionStep = 5;
                        }

                        _mSecondFrame = !_mSecondFrame;
                        break;
                    case (5):
                        _mHoloOpacity = (float)Animations.Easing(1f, 0f, _mTransitionStartTime, _mTransitionDuration, gametime);

                        if (gametime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                        {
                            TransitionRunning = false;
                            _mHoloOpacity = 0f;
                        }

                        _mTransitionStep = 5;
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
            spriteBatch.Draw(_mLogoTexture2D,
                origin: new Vector2(308, 279),
                position: _mLogoPosition,
                color: Color.AliceBlue * _mHoloOpacity,
                rotation: 0f,
                scale: _mScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(_mSingularityText,
                origin: new Vector2(322, 41),
                position: _mSingularityTextPosition,
                color: Color.AliceBlue * _mHoloOpacity,
                rotation: 0f,
                scale: _mScaleMultiplier,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(_mLibSans20,
                origin: _mStringCenter,
                position: _mTextPosition,
                color: new Color(new Vector3(.9137f, .9058f, .8314f)) * _mHoloOpacity * _mTextOpacity,
                text: _mContinueString,
                rotation: 0f,
                scale: _mScaleMultiplier,
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
