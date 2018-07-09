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
        public EScreen Screen { get; private set; } = EScreen.OptionsScreen;

        public bool Loaded { get; set; }

        private readonly Game1 mGame;

        // layout. Made only once to reduce unnecssary calculations at draw time
        private Vector2 mBoxPosition;
        private readonly Vector2 mWindowTitlePosition;
        private readonly Vector2 mLinePosition;
        private readonly float mTabPadding;
        private readonly float mContentPadding;
        private readonly float mTopContentPadding;
        private readonly Vector2 mScreenResolution;

        // All strings are variables to allow for easy editing and localization
        private readonly string mWindowTitleString;
        private readonly string mGameplayString;
        private readonly string mGraphicsString;
        private readonly string mAudioString;
        private readonly string mSaveChangesString;
        private readonly string mBackString;

        private readonly string mFullScreenString;
        private readonly string mResolutionString; // used later .. apparently
        private readonly string mAntialiasingString;

        private readonly string mMuteString;

        // fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // Button colors
        private readonly Color mTextColor;

        // tab buttons
        private readonly List<Button> mTabButtons;
        private Button mGameplayButton;
        private Button mGraphicsButton;
        private Button mAudioButton;
        private Button mSaveButton;
        private Button mBackButton;

        // Graphics tab
        private readonly List<Button> mGraphicsButtons;
        private Button mFullScreen; // todo replace with a toggle
        private Button mResolution1; // todo replace with a better system
        private Button mResolution2; // todo replace with a better system
        private Button mAntialiasing; // todo replace with a toggle

        // Audio tab
        // todo add the following:
        private readonly List<Button> mAudioButtons;
        private Button mMuteButton; // add slider once completed
        // Background volume and toggle
        // Sound effect volume and toggle
        // 3D sound effect toggle

        // Transitions variables
        private float mMenuOpacity;
        private Vector2 mMenuBoxSize;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;
        public bool TransitionRunning { get; private set; }

        private EOptionScreenState mScreenState;

        /// <summary>
        /// Creates an instance of the Options screen.
        /// </summary>
        /// <param name="screenResolution">Screen resolution used for scaling</param>
        /// <param name="screenResolutionChanged"></param>
        /// <param name="game">Game1 class passed on to options to allow changing of options</param>
        public OptionsScreen(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            // scaling of all positions according to viewport size
            mScreenResolution = screenResolution;
            mBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 2 - 210);
            mMenuBoxSize = new Vector2(612, 420);

            mTabPadding = mBoxPosition.X + 36;
            mContentPadding = mBoxPosition.X + 204;
            mTopContentPadding = mBoxPosition.Y + 84;
            mWindowTitlePosition = new Vector2(mBoxPosition.X + 20, mBoxPosition.Y + 20);
            mLinePosition = new Vector2(mBoxPosition.X + 180, mBoxPosition.Y + 85);

            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mWindowTitleString = "Options";
            mGameplayString = "Gameplay";
            mGraphicsString = "Graphics";
            mAudioString = "Audio";
            mSaveChangesString = "Apply";
            mBackString = "Back";

            mFullScreenString = "Full Screen";
            mResolutionString = "Resolution:";
            mAntialiasingString = "Anti-Aliasing";
            mMuteString = "Mute";

            mTabButtons = new List<Button>(5);
            mGraphicsButtons = new List<Button>(4);
            mAudioButtons = new List<Button>(1);

            mScreenState = EOptionScreenState.Gameplay;
            mGame = game;

            mMenuOpacity = screenResolutionChanged ? 1 : 0;

        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // make the tab select buttons
            mGameplayButton = new Button(mGameplayString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding), mTextColor);
            mGraphicsButton = new Button(mGraphicsString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 40), mTextColor);
            mAudioButton = new Button(mAudioString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 80), mTextColor);
            mBackButton = new Button(mBackString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 160), mTextColor);

            mTabButtons.Add(mGameplayButton);
            mTabButtons.Add(mGraphicsButton);
            mTabButtons.Add(mAudioButton);
            mTabButtons.Add(mBackButton);

            foreach (Button tabButton in mTabButtons)
            {
                tabButton.Opacity = mMenuOpacity;
            }

            // Gameplay settings
            // TODO figure out what settings can be implemented in here

            // Graphics settings
            mFullScreen = new Button(mFullScreenString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding), mTextColor);
            mResolution1 = new Button("800 x 600", mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 40), mTextColor);
            mResolution2 = new Button("960 x 720", mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 80));
            mSaveButton = new Button(mSaveChangesString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 120), mTextColor);

            mGraphicsButtons.Add(mFullScreen);
            mGraphicsButtons.Add(mResolution1);
            mGraphicsButtons.Add(mResolution2);
            mGraphicsButtons.Add(mSaveButton);

            foreach (Button graphicsButton in mGraphicsButtons)
            {
                graphicsButton.Opacity = mMenuOpacity;
            }

            // Audio settings
            mMuteButton = new Button(mMuteString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding), mTextColor);

            mAudioButtons.Add(mMuteButton);

            // Button handler bindings
            mGameplayButton.ButtonReleased += OnGameplayReleased;
            mGraphicsButton.ButtonReleased += OnGraphicsReleased;
            mAudioButton.ButtonReleased += OnAudioReleased;
            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mFullScreen.ButtonReleased += OnFullScreenReleased;
            mResolution1.ButtonReleased += OnResoOneReleased;
            mResolution2.ButtonReleased += OnResoTwoReleased;

            mMuteButton.ButtonReleased += OnMuteReleased;

            Loaded = true;
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

            foreach (Button button in mTabButtons)
            {
                button.Update(gametime);
                button.Opacity = mMenuOpacity;
            }

            switch (mScreenState)
            {
                case EOptionScreenState.Gameplay:

                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mGraphicsButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = mMenuOpacity;
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (Button button in mAudioButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = mMenuOpacity;
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
            spriteBatch.StrokedRectangle(mBoxPosition,
                mMenuBoxSize,
                Color.White,
                Color.White,
                0.5f,
                0.21f);

            // line in the middle
            spriteBatch.DrawLine(point: mLinePosition,
                angle: (float)Math.PI / 2,
                length: 301,
                color: new Color(new Vector4(1, 1, 1, 0.5f)) * mMenuOpacity,
                thickness: 1);

            // window title
            spriteBatch.DrawString(mLibSans36,
                text: mWindowTitleString,
                position: mWindowTitlePosition,
                color: mTextColor * mMenuOpacity);

            // tab buttons
            foreach (Button button in mTabButtons)
            {
                button.Draw(spriteBatch);
            }

            // actual options
            switch (mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    spriteBatch.DrawString(mLibSans20, "Difficulty", new Vector2(mContentPadding, mTopContentPadding), Color.White * mMenuOpacity);
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mGraphicsButtons)
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (var button in mAudioButtons)
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
            switch (mTargetScreen)
            {
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                        mMenuBoxSize = new Vector2(408, 420);
                        mBoxPosition = new Vector2(mScreenResolution.X / 2 - 204, mScreenResolution.Y / 2 - 210);
                    }

                    var width = (float)Animations.Easing(612,
                        408,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mBoxPosition = new Vector2(mScreenResolution.X / 2 - (int)Math.Floor(width / 2), mScreenResolution.Y / 4);
                    mMenuBoxSize = new Vector2(width, 420);

                    mMenuOpacity = (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.OptionsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 1f;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mTargetScreen), mTargetScreen, null);
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
            mScreenState = EOptionScreenState.Gameplay;
        }

        /// <summary>
        /// Handler for the Graphics button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGraphicsReleased(Object sender, EventArgs eventArgs)
        {
            mScreenState = EOptionScreenState.Graphics;
        }

        /// <summary>
        /// Handler for the Audio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAudioReleased(Object sender, EventArgs eventArgs)
        {
            mScreenState = EOptionScreenState.Audio;
        }

        /// <summary>
        /// Makes the game full screen. Currently makes the game full screen with the actual screen resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnFullScreenReleased(Object sender, EventArgs eventArgs)
        {
            int width;
            int height;
            bool truth;


            if (mGame.mGraphics.IsFullScreen)
            {
                truth = false;
                // if it is already full screen, reset to a smaller screen size
                width = 960;
                height = 720;
            }
            else
            {
                // otherwise, do set up the game for full screen
                truth = true;
                width = mGame.mGraphicsAdapter.CurrentDisplayMode.Width;
                height = mGame.mGraphicsAdapter.CurrentDisplayMode.Height;
            }

            mGame.mGraphics.PreferredBackBufferWidth = width;
            mGame.mGraphics.PreferredBackBufferHeight = height;
            mGame.mGraphics.IsFullScreen = truth;
            mGame.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(width, height));
        }

        private void OnResoOneReleased(Object sender, EventArgs eventArgs)
        {
            mGame.mGraphics.PreferredBackBufferWidth = 800;
            mGame.mGraphics.PreferredBackBufferHeight = 600;
            mGame.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(800, 600));
        }

        private void OnResoTwoReleased(Object sender, EventArgs eventArgs)
        {
            mGame.mGraphics.PreferredBackBufferWidth = 960;
            mGame.mGraphics.PreferredBackBufferHeight = 720;
            mGame.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(960, 720));
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
                mMenuOpacity = 0f;
            }
            mBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 2 - 210);
            mMenuBoxSize = new Vector2(612, 420);
            mTargetScreen = targetScreen;
            mTransitionDuration = 350f;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;

        }
    }
}
