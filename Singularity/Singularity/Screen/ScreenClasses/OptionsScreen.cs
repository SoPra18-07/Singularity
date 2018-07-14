using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Singularity.Libraries;
using Singularity.Utils;

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
        private readonly string mResolutionString;

        // The current resolution
        private Pair<int, int> mResolutionChosen;

        // fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans16;

        // Button colors
        private readonly Color mTextColor;

        // tab buttons
        private readonly List<Button> mTabButtons;

        // Graphics tab
        private readonly List<Button> mGraphicsButtons;

        // pre-apply state save basically
        private int mWidth;
        private int mHeight;
        private bool mTruth;


        // Audio tab
        // todo add the following:
        private readonly List<Button> mAudioButtons;

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
            mBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 4);
            mMenuBoxSize = new Vector2(612, 420);

            mResolutionChosen = new Pair<int, int>((int) screenResolution.X, (int) screenResolution.Y);

            mTabPadding = mBoxPosition.X + 36;
            mContentPadding = mBoxPosition.X + 204;
            mTopContentPadding = mBoxPosition.Y + 84;
            mWindowTitlePosition = new Vector2(mBoxPosition.X + 20, mBoxPosition.Y + 20);
            mLinePosition = new Vector2(mBoxPosition.X + 180, mBoxPosition.Y + 85);

            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mWindowTitleString = "Options";
            mResolutionString = "Resolution:";
            

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
            mLibSans16 = content.Load<SpriteFont>("LibSans20");

            #region Strings for easy editing and localization

            // tabs
            const string gameplayString = "Gameplay";
            const string graphicsString = "Graphics";
            const string audioString = "Audio";
            const string saveChangesString = "Apply";
            const string backString = "Back";

            // Gameplay

            // Graphics
            const string fullScreenString = "Full Screen";

            // Audio
            const string muteString = "Mute";
            #endregion

            // make the tab select buttons
            var gameplayButton = new Button(gameplayString, mLibSans16, new Vector2(mTabPadding, mTopContentPadding), mTextColor);
            var graphicsButton = new Button(graphicsString, mLibSans16, new Vector2(mTabPadding, mTopContentPadding + 40), mTextColor);
            var audioButton = new Button(audioString, mLibSans16, new Vector2(mTabPadding, mTopContentPadding + 80), mTextColor);
            var backButton = new Button(backString, mLibSans16, new Vector2(mTabPadding, mTopContentPadding + 160), mTextColor);

            mTabButtons.Add(gameplayButton);
            mTabButtons.Add(graphicsButton);
            mTabButtons.Add(audioButton);
            mTabButtons.Add(backButton);

            foreach (Button tabButton in mTabButtons)
            {
                tabButton.Opacity = mMenuOpacity;
            }

            // Gameplay settings
            // TODO figure out what settings can be implemented in here

            // Graphics settings
            var fullScreen = new Checkbox(fullScreenString,
                mLibSans16,
                new Vector2(mContentPadding,
                    mTopContentPadding),
                new Vector2(mContentPadding +
                            mLibSans16.MeasureString("Full Screen        ")
                                .X,
                    mTopContentPadding),
                mTextColor)
            {
                CheckboxState = mGame.mGraphics.IsFullScreen
            };

            // set the check box for the full screen toggle.

            #region Selector for Screen Resolution

            // This region is to make the selector button. The button is custom because it only happens once and I'm too lazy to
            // make a completely new class that's modular with the right arguments etc.
            var selectButtonTexture = content.Load<Texture2D>("SelectorButton");
            var resolutionDown = new Button(1,
                selectButtonTexture,
                new Vector2(mContentPadding, mTopContentPadding + 80),
                false);
            var resolutionUp = new Button(1,
                selectButtonTexture,
                new Vector2(mContentPadding + mLibSans16.MeasureString("Full Screen        ").X, mTopContentPadding + 80),
                false,
                spriteEffects: SpriteEffects.FlipHorizontally);

            #endregion
                 
            var saveButton = new Button(saveChangesString, mLibSans16, new Vector2(mContentPadding, mTopContentPadding + 160), mTextColor);

            mGraphicsButtons.Add(fullScreen);
            mGraphicsButtons.Add(saveButton);
            mGraphicsButtons.Add(resolutionDown);
            mGraphicsButtons.Add(resolutionUp);
            
            foreach (Button graphButton in mGraphicsButtons)
            {
                graphButton.Opacity = mMenuOpacity;
            }

            // Audio settings
            var muteButton = new Button(muteString, mLibSans16, new Vector2(mContentPadding, mTopContentPadding), mTextColor);

            mAudioButtons.Add(muteButton);

            // Button handler bindings
            gameplayButton.ButtonReleased += OnGameplayReleased;
            graphicsButton.ButtonReleased += OnGraphicsReleased;
            audioButton.ButtonReleased += OnAudioReleased;
            backButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            fullScreen.ButtonReleased += OnFullScreenReleased;
            resolutionDown.ButtonReleased += OnResoDownReleased;
            resolutionUp.ButtonReleased += OnResoUpReleased;
            saveButton.ButtonReleased += OnSaveReleased;

            muteButton.ButtonReleased += OnMuteReleased;

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
                    foreach (var button in mGraphicsButtons.GetRange(0, 2))
                    {
                        button.Update(gametime);
                        button.Opacity = mMenuOpacity;
                    }


                    // makes it impossible to change resolution while full screened
                    if (!mGame.mGraphics.IsFullScreen)
                    {
                        foreach (Button button in mGraphicsButtons.GetRange(2, 2))
                        {
                            button.Update(gametime);
                            button.Opacity = mMenuOpacity;
                        }
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
            Debug.WriteLine("Full screen active: " + mGame.mGraphics.IsFullScreen);
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
                    spriteBatch.DrawString(mLibSans16, "Difficulty", new Vector2(mContentPadding, mTopContentPadding), mTextColor * mMenuOpacity);
                    break;
                case EOptionScreenState.Graphics:
                    // Don't allow resolution changes when full screen.
                    foreach (var button in mGraphicsButtons.GetRange(0, 2))
                    {
                        button.Draw(spriteBatch);
                    }

                    if (!mGame.mGraphics.IsFullScreen)
                    {
                        foreach (var button in mGraphicsButtons.GetRange(2, 2))
                        {
                            button.Draw(spriteBatch);
                        }
                        
                        // Draw the resolution text string
                        spriteBatch.DrawString(mLibSans16, mResolutionString, new Vector2(mContentPadding, mTopContentPadding + 40), mTextColor * mMenuOpacity);

                        // Draw the box for the selector
                        var selectorWidth = (int)mLibSans16.MeasureString("Full Screen        ").X + 32;
                        spriteBatch.DrawRectangle(new Rectangle((int)mContentPadding, (int)mTopContentPadding + 80, selectorWidth, 32), mTextColor);
                        
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
                        mBoxPosition = new Vector2(mScreenResolution.X / 2 - 204, mScreenResolution.Y / 4);
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
            if (mGame.mGraphics.IsFullScreen)
            {
                mTruth = false;
                // if it is already full screen, reset to a smaller screen size
                mWidth = 960;
                mHeight = 720;
            }
            else
            {
                // otherwise, do set up the game for full screen
                mTruth = true;
                mWidth = mGame.mGraphicsAdapter.CurrentDisplayMode.Width;
                mHeight = mGame.mGraphicsAdapter.CurrentDisplayMode.Height;
            }

            mGame.mGraphics.PreferredBackBufferWidth = mWidth;
            mGame.mGraphics.PreferredBackBufferHeight = mHeight;
            mGame.mGraphics.IsFullScreen = mTruth;
            
        }

        private void OnResoDownReleased(Object sender, EventArgs eventArgs)
        {
            mGame.mGraphics.PreferredBackBufferWidth = 800;
            mGame.mGraphics.PreferredBackBufferHeight = 600;
            MainMenuManagerScreen.SetResolution(new Vector2(800, 600));
        }

        private void OnResoUpReleased(Object sender, EventArgs eventArgs)
        {
            mGame.mGraphics.PreferredBackBufferWidth = 960;
            mGame.mGraphics.PreferredBackBufferHeight = 720;
            MainMenuManagerScreen.SetResolution(new Vector2(960, 720));
        }

        private void OnSaveReleased(Object sender, EventArgs eventArgs)
        {
            mGame.mGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(mWidth, mHeight));
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
            mBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 4);
            mMenuBoxSize = new Vector2(612, 420);
            mTargetScreen = targetScreen;
            mTransitionDuration = 350f;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;

        }
    }
}