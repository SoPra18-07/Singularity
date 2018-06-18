using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Shown after Options on the main menu or pause menu has been clicked.
    /// Allows different settings and options to be set. Buttons include
    /// for the different settings and a back button.
    /// </summary>
    internal sealed class OptionsScreen : ITransitionableMenu
    {
        private readonly Game1 _mGame;

        // layout. Made only once to reduce unnecssary calculations at draw time
        private Vector2 _mBoxPosition;
        private readonly Vector2 _mWindowTitlePosition;
        private readonly Vector2 _mLinePosition;
        private readonly float _mTabPadding;
        private readonly float _mContentPadding;
        private readonly float _mTopContentPadding;
        private readonly Vector2 _mScreenResolution;

        // All strings are variables to allow for easy editing and localization
        private readonly string _mWindowTitleString;
        private readonly string _mGameplayString;
        private readonly string _mGraphicsString;
        private readonly string _mAudioString;
        private readonly string _mSaveChangesString;
        private readonly string _mBackString;

        private readonly string _mFullScreenString;
        private readonly string _mResolutionString; // used later
        private readonly string _mAntialiasingString;

        private readonly string _mMuteString;

        // fonts
        private SpriteFont _mLibSans36;
        private SpriteFont _mLibSans20;

        // Button colors
        private readonly Color _mTextColor;

        // tab buttons
        private readonly List<Button> _mTabButtons;
        private Button _mGameplayButton;
        private Button _mGraphicsButton;
        private Button _mAudioButton;
        private Button _mSaveButton;
        private Button _mBackButton;

        // Graphics tab
        private readonly List<Button> _mGraphicsButtons;
        private Button _mFullScreen; // todo replace with a toggle
        private Button _mResolution1; // todo replace with a better system
        private Button _mResolution2; // todo replace with a better system
        private Button _mAntialiasing; // todo replace with a toggle
        
        // Audio tab
        // todo add the following:
        private readonly List<Button> _mAudioButtons;
        private Button _mMuteButton; // add slider once completed
        // Background volume and toggle
        // Sound effect volume and toggle
        // 3D sound effect toggle

        // Transitions variables
        private float _mMenuOpacity;
        private Vector2 _mMenuBoxSize;
        private double _mTransitionStartTime;
        private double _mTransitionDuration;
        private EScreen _mTargetScreen;
        public bool TransitionRunning { get; private set; }

        private EOptionScreenState _mScreenState;

        /// <summary>
        /// Creates an instance of the Options screen.
        /// </summary>
        /// <param name="screenResolution">Screen resolution used for scaling</param>
        /// <param name="game">Game1 class passed on to options to allow changing of options</param>
        public OptionsScreen(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            // scaling of all positions according to viewport size
            _mScreenResolution = screenResolution;
            _mBoxPosition = new Vector2(_mScreenResolution.X / 2 - 306, _mScreenResolution.Y / 4);
            _mMenuBoxSize = new Vector2(612, 420);

            _mTabPadding = _mBoxPosition.X + 36;
            _mContentPadding = _mBoxPosition.X + 204;
            _mTopContentPadding = _mBoxPosition.Y + 84;
            _mWindowTitlePosition = new Vector2(_mBoxPosition.X + 20, _mBoxPosition.Y + 20);
            _mLinePosition = new Vector2(_mBoxPosition.X + 180, _mBoxPosition.Y + 85);

            _mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            _mWindowTitleString = "Options";
            _mGameplayString = "Gameplay";
            _mGraphicsString = "Graphics";
            _mAudioString = "Audio";
            _mSaveChangesString = "Apply";
            _mBackString = "Back";

            _mFullScreenString = "Full Screen";
            _mResolutionString = "Resolution:";
            _mAntialiasingString = "Anti-Aliasing";

            _mMuteString = "Mute";

            _mTabButtons = new List<Button>(5);
            _mGraphicsButtons = new List<Button>(4);
            _mAudioButtons = new List<Button>(1);

            _mScreenState = EOptionScreenState.Gameplay;
            _mGame = game;

            _mMenuOpacity = screenResolutionChanged ? 1 : 0;

        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            _mLibSans36 = content.Load<SpriteFont>("LibSans36");
            _mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // make the tab select buttons
            _mGameplayButton = new Button(_mGameplayString, _mLibSans20, new Vector2(_mTabPadding, _mTopContentPadding), _mTextColor);
            _mGraphicsButton = new Button(_mGraphicsString, _mLibSans20, new Vector2(_mTabPadding, _mTopContentPadding + 40), _mTextColor);
            _mAudioButton = new Button(_mAudioString, _mLibSans20, new Vector2(_mTabPadding, _mTopContentPadding + 80), _mTextColor);
            _mSaveButton = new Button(_mSaveChangesString, _mLibSans20, new Vector2(_mTabPadding, _mTopContentPadding + 120), _mTextColor);
            _mBackButton = new Button(_mBackString, _mLibSans20, new Vector2(_mTabPadding, _mTopContentPadding + 160), _mTextColor);

            _mTabButtons.Add(_mGameplayButton);
            _mTabButtons.Add(_mGraphicsButton);
            _mTabButtons.Add(_mAudioButton);
            _mTabButtons.Add(_mSaveButton);
            _mTabButtons.Add(_mBackButton);

            foreach (Button tabButton in _mTabButtons)
            {
                tabButton.Opacity = _mMenuOpacity;
            }

            // Gameplay settings
            // TODO figure out what settings can be implemented in here

            // Graphics settings
            _mFullScreen = new Button(_mFullScreenString, _mLibSans20, new Vector2(_mContentPadding, _mTopContentPadding), _mTextColor);
            _mResolution1 = new Button("800 x 600", _mLibSans20, new Vector2(_mContentPadding, _mTopContentPadding + 40), _mTextColor);
            _mResolution2 = new Button("960 x 720", _mLibSans20, new Vector2(_mContentPadding, _mTopContentPadding + 80));
            _mAntialiasing = new Button(_mAntialiasingString, _mLibSans20, new Vector2(_mContentPadding, _mTopContentPadding + 120), _mTextColor);

            _mGraphicsButtons.Add(_mFullScreen);
            _mGraphicsButtons.Add(_mResolution1);
            _mGraphicsButtons.Add(_mResolution2);
            _mGraphicsButtons.Add(_mAntialiasing);

            foreach (Button graphicsButton in _mGraphicsButtons)
            {
                graphicsButton.Opacity = _mMenuOpacity;
            }

            // Audio settings
            _mMuteButton = new Button(_mMuteString, _mLibSans20, new Vector2(_mContentPadding, _mTopContentPadding), _mTextColor);

            _mAudioButtons.Add(_mMuteButton);

            // Button handler bindings
            _mGameplayButton.ButtonReleased += OnGameplayReleased;
            _mGraphicsButton.ButtonReleased += OnGraphicsReleased;
            _mAudioButton.ButtonReleased += OnAudioReleased;
            _mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            _mFullScreen.ButtonReleased += OnFullScreenReleased;
            _mResolution1.ButtonReleased += OnResoOneReleased;
            _mResolution2.ButtonReleased += OnResoTwoReleased;
            _mAntialiasing.ButtonReleased += OnAntialiasingReleased;

            _mMuteButton.ButtonReleased += OnMuteReleased;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            if (TransitionRunning)
            {
                Transition(gametime);
            }

            foreach (Button button in _mTabButtons)
            {
                button.Update(gametime);
                button.Opacity = _mMenuOpacity;
            }

            switch (_mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in _mGraphicsButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = _mMenuOpacity;
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (Button button in _mAudioButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = _mMenuOpacity;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // background window
            spriteBatch.StrokedRectangle(_mBoxPosition,
                _mMenuBoxSize,
                Color.White,
                Color.White,
                0.5f,
                0.21f);

            // line in the middle
            spriteBatch.DrawLine(point: _mLinePosition,
                angle: (float) Math.PI / 2,
                length: 301,
                color: new Color(new Vector4(1, 1, 1, 0.5f)) * _mMenuOpacity,
                thickness: 1);

            // window title
            spriteBatch.DrawString(_mLibSans36,
                text: _mWindowTitleString,
                position: _mWindowTitlePosition,
                color: _mTextColor * _mMenuOpacity);

            // tab buttons
            foreach (Button button in _mTabButtons)
            {
                button.Draw(spriteBatch);
            }

            // actual options
            switch (_mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    spriteBatch.DrawString(_mLibSans20, "Difficulty", new Vector2(_mContentPadding, _mTopContentPadding), Color.White * _mMenuOpacity);
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in _mGraphicsButtons)
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (var button in _mAudioButtons)
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Calculates the appropriate values for use in the transition
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
        private void Transition(GameTime gameTime)
        {
            switch (_mTargetScreen)
            {
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 0f;
                        _mMenuBoxSize = new Vector2(408, 420);
                        _mBoxPosition = new Vector2(_mScreenResolution.X / 2 - 204, _mScreenResolution.Y / 4);
                    }

                    var width = (float) Animations.Easing(612,
                        408,
                        _mTransitionStartTime,
                        _mTransitionDuration,
                        gameTime);

                    _mBoxPosition = new Vector2(_mScreenResolution.X / 2 - (int)Math.Floor(width / 2), _mScreenResolution.Y / 4);
                    _mMenuBoxSize = new Vector2(width, 420);

                    _mMenuOpacity = (float)Animations.Easing(1f, 0f, _mTransitionStartTime, _mTransitionDuration, gameTime);
                    break;
                case EScreen.OptionsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 1f;
                    }

                    _mMenuOpacity =
                        (float) Animations.Easing(0, 1f, _mTransitionStartTime, _mTransitionDuration, gameTime);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_mTargetScreen), _mTargetScreen, null);
            }
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

        #region Button Handlers

        /// <summary>
        /// Handler for the Gameplay button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGameplayReleased(Object sender, EventArgs eventArgs)
        {
            _mScreenState = EOptionScreenState.Gameplay;
        }

        /// <summary>
        /// Handler for the Graphics button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGraphicsReleased(Object sender, EventArgs eventArgs)
        {
            _mScreenState = EOptionScreenState.Graphics;
        }

        /// <summary>
        /// Handler for the Audio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAudioReleased(Object sender, EventArgs eventArgs)
        {
            _mScreenState = EOptionScreenState.Audio;
        }

        /// <summary>
        /// Makes the game full screen. Currently makes the game full screen with the actual screen resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnFullScreenReleased(Object sender, EventArgs eventArgs)
        {
            var width = _mGame.MGraphicsAdapter.CurrentDisplayMode.Width;
            var height = _mGame.MGraphicsAdapter.CurrentDisplayMode.Height;
            var truth = false;
            if (_mGame.MGraphics.IsFullScreen)
            {
                width = 1080;
                height = 720;
            }
            else
            {
                truth = true;
            }
            _mGame.MGraphics.PreferredBackBufferWidth = width;
            _mGame.MGraphics.PreferredBackBufferHeight = height;
            _mGame.MGraphics.IsFullScreen = truth;
            _mGame.MGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(width, height));
        }

        private void OnResoOneReleased(Object sender, EventArgs eventArgs)
        {
            _mGame.MGraphics.PreferredBackBufferWidth = 800;
            _mGame.MGraphics.PreferredBackBufferHeight = 600;
            _mGame.MGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(800, 600));
        }

        private void OnResoTwoReleased(Object sender, EventArgs eventArgs)
        {
            _mGame.MGraphics.PreferredBackBufferWidth = 960;
            _mGame.MGraphics.PreferredBackBufferHeight = 720;
            _mGame.MGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(960, 720));
        }

        private void OnAntialiasingReleased(Object sender, EventArgs eventArgs)
        {
            // potentially impossible
        }

        private void OnMuteReleased(Object sender, EventArgs eventArgs)
        {
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }

        #endregion

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (originScreen == EScreen.MainMenuScreen)
            {
                _mMenuOpacity = 0f;
            }
            _mBoxPosition = new Vector2(_mScreenResolution.X / 2 - 306, _mScreenResolution.Y / 4);
            _mMenuBoxSize = new Vector2(612, 420);
            _mTargetScreen = targetScreen;
            _mTransitionDuration = 350f;
            _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;
        
        }
    }
}
