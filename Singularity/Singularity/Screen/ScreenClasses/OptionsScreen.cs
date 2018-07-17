using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;
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
        private readonly string mWindowTitleString = "Options";
        private readonly string mResolutionString = "Resolution:";

        // The choosable resolutions. This works by having a list of possible resolutions and choosing by getting the index of the current wanted resolution.
        private readonly List<Pair<int, int>> mResolutionList = new List<Pair<int, int>>
        {
            new Pair<int, int>(960, 720),
            new Pair<int, int>(1024, 576),
            new Pair<int, int>(1024, 768),
            new Pair<int, int>(1024, 800),
            new Pair<int, int>(1152, 648),
            new Pair<int, int>(1280, 720),
            new Pair<int, int>(1280, 800),
            new Pair<int, int>(1280, 960),
            new Pair<int, int>(1280, 1024),
            new Pair<int, int>(1366, 768),
            new Pair<int, int>(1440, 900),
            new Pair<int, int>(1600, 900),
            new Pair<int, int>(1680, 1050),
            new Pair<int, int>(1920, 1080),
            new Pair<int, int>(1920, 1200),
            new Pair<int, int>(2560, 1440),
            new Pair<int, int>(2560, 1600),
            new Pair<int, int>(3840, 2160)
        };

        private int mResolutionChosen;

        // fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans14;

        // Button colors
        private readonly Color mTextColor;

        // tab buttons
        private readonly List<Button> mTabButtons = new List<Button>(5);

        // Graphics tab
        private readonly List<Checkbox> mGraphicCheckboxes = new List<Checkbox>(1);
        private readonly List<Button> mGraphicsButtons = new List<Button>(3);

        // pre-apply state save basically
        private int mWidth;
        private int mHeight;
        private bool mTruth;

        // Audio tab
        private readonly List<Checkbox> mAudioCheckboxes = new List<Checkbox>(1);
        private readonly List<Slider> mAudioSliders = new List<Slider>(4);
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
        /// <param name="screenResolution">Screen resolution used for scaling.</param>
        /// <param name="screenResolutionChanged">True if the screen resolution has changed.</param>
        /// <param name="game">Game1 class passed on to options to allow changing of options.</param>
        /// 
        public OptionsScreen(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            // scaling of all positions according to viewport size
            mScreenResolution = screenResolution;
            mBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 4);
            mMenuBoxSize = new Vector2(612, 420);

            mResolutionChosen =
                mResolutionList.IndexOf(new Pair<int, int>((int) screenResolution.X, (int) screenResolution.Y));

            if (mResolutionChosen == -1)
            {
                mResolutionChosen = 0;
            }
            
            mTabPadding = mBoxPosition.X + 36;
            mContentPadding = mBoxPosition.X + 204;
            mTopContentPadding = mBoxPosition.Y + 84;
            mWindowTitlePosition = new Vector2(mBoxPosition.X + 20, mBoxPosition.Y + 20);
            mLinePosition = new Vector2(mBoxPosition.X + 180, mBoxPosition.Y + 85);

            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mScreenState = EOptionScreenState.Graphics;
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
            mLibSans14 = content.Load<SpriteFont>("LibSans14");

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

            #region Tab Buttons

            var graphicsButton = new Button(graphicsString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding), mTextColor);
            var gameplayButton = new Button(gameplayString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 40), mTextColor);
            var audioButton = new Button(audioString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 80), mTextColor);
            var backButton = new Button(backString, mLibSans20, new Vector2(mTabPadding, mTopContentPadding + 160), mTextColor);

            mTabButtons.Add(gameplayButton);
            mTabButtons.Add(graphicsButton);
            mTabButtons.Add(audioButton);
            mTabButtons.Add(backButton);

            foreach (Button tabButton in mTabButtons)
            {
                tabButton.Opacity = mMenuOpacity;
            }


            #endregion

            #region Gameplay Settings

            // todo difficulty button
            #endregion

            #region Graphics Settings


            var fullScreen = new Checkbox(fullScreenString,
                mLibSans20,
                new Vector2(mContentPadding,
                    mTopContentPadding),
                new Vector2(mContentPadding +
                            mLibSans20.MeasureString("Full Screen        ")
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
                new Vector2(mContentPadding + mLibSans20.MeasureString("Full Screen        ").X, mTopContentPadding + 80),
                false,
                spriteEffects: SpriteEffects.FlipHorizontally);

            #endregion

            var saveButton = new Button(saveChangesString, mLibSans20, new Vector2(mContentPadding, mTopContentPadding + 160), mTextColor);

            mGraphicCheckboxes.Add(fullScreen);
            mGraphicsButtons.Add(saveButton);
            mGraphicsButtons.Add(resolutionDown);
            mGraphicsButtons.Add(resolutionUp);

            foreach (Button graphButton in mGraphicsButtons)
            {
                graphButton.Opacity = mMenuOpacity;
            }


            #endregion

            #region Audio Settings

            var muteButton = new Checkbox(muteString,
                mLibSans20,
                new Vector2(mContentPadding, mTopContentPadding),
                new Vector2(mContentPadding + mLibSans20.MeasureString("Mute        ").X, mTopContentPadding),
                mTextColor)
            {
                CheckboxState = GlobalVariables.AudioMute
            };
            var masterSlider = new Slider(new Vector2(mContentPadding, mTopContentPadding + 90), 300, 30, mLibSans14) ;
            var musicSlider = new Slider(new Vector2(mContentPadding, mTopContentPadding + 150), 300, 30, mLibSans14);
            var soundEffectSlider = new Slider(new Vector2(mContentPadding, mTopContentPadding + 210), 30, 5, mLibSans14);
            var uiSlider = new Slider(new Vector2(mContentPadding, mTopContentPadding + 270), 300, 30, mLibSans14);

            // Background volume and toggle
            // Sound effect volume and toggle
            // 3D sound effect toggle

            mAudioCheckboxes.Add(muteButton);
            mAudioSliders.Add(masterSlider);
            mAudioSliders.Add(musicSlider);
            mAudioSliders.Add(soundEffectSlider);
            mAudioSliders.Add(uiSlider);

            #endregion

            #region Button Handler Subscriptions
            
            gameplayButton.ButtonReleased += OnGameplayReleased;
            graphicsButton.ButtonReleased += OnGraphicsReleased;
            audioButton.ButtonReleased += OnAudioReleased;
            backButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            fullScreen.ButtonReleased += OnFullScreenReleased;
            resolutionDown.ButtonReleased += OnResoDownReleased;
            resolutionUp.ButtonReleased += OnResoUpReleased;
            saveButton.ButtonReleased += OnSaveReleased;

            muteButton.ButtonReleased += OnMuteReleased;
            masterSlider.SliderMoving += OnMasterAudioMoving;
            musicSlider.SliderMoving += OnMusicSliderMoving;
            soundEffectSlider.SliderMoving += OnSoundEffectSliderMoving;
            uiSlider.SliderMoving += OnUiSliderMoving;

            #endregion

            Loaded = true;
        }

        private void OnUiSliderMoving(object source, EventArgs args, float percentMoved)
        {
            
        }

        private void OnSoundEffectSliderMoving(object source, EventArgs args, float percentMoved)
        {
            throw new NotImplementedException();
        }

        private void OnMusicSliderMoving(object source, EventArgs args, float percentMoved)
        {
            throw new NotImplementedException();
        }

        private void OnMasterAudioMoving(object source, EventArgs args, float percentMoved)
        {
            Debug.WriteLine("Master audio slider value: " + percentMoved);
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
                    mGraphicsButtons[0].Update(gametime);
                    mGraphicsButtons[0].Opacity = mMenuOpacity;

                    mGraphicCheckboxes[0].Update(gametime);
                    mGraphicCheckboxes[0].Opacity = mMenuOpacity;
                    
                    // makes it impossible to change resolution while full screened
                    if (!mGame.mGraphics.IsFullScreen)
                    {
                        foreach (var button in mGraphicsButtons.GetRange(1, 2))
                        {
                            button.Update(gametime);
                            button.Opacity = mMenuOpacity;
                        }

                        if (mResolutionChosen > mResolutionList.Count - 1)
                        {
                            mResolutionChosen = 0;
                        }

                        if (mResolutionChosen < 0)
                        {
                            mResolutionChosen = mResolutionList.Count - 1;
                        }
                    }

                    break;
                case EOptionScreenState.Audio:
                    foreach (var button in mAudioCheckboxes)
                    {
                        button.Update(gametime);
                        button.Opacity = mMenuOpacity;
                    }

                    foreach (var slider in mAudioSliders)
                    {
                        slider.Update(gametime);
                        slider.Opacity = mMenuOpacity;
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
            foreach (var button in mTabButtons)
            {
                button.Draw(spriteBatch);
            }

            // actual options
            switch (mScreenState)
            {
                case EOptionScreenState.Gameplay:
                    spriteBatch.DrawString(mLibSans20, "Difficulty", new Vector2(mContentPadding, mTopContentPadding), mTextColor * mMenuOpacity);
                    break;
                case EOptionScreenState.Graphics:
                    // Don't allow resolution changes when full screen.
                    mGraphicCheckboxes[0].Draw(spriteBatch);
                    mGraphicsButtons[0].Draw(spriteBatch);

                    if (!mGame.mGraphics.IsFullScreen)
                    {
                        foreach (var button in mGraphicsButtons.GetRange(1, 2))
                        {
                            button.Draw(spriteBatch);
                        }
                        
                        // Draw the resolution text string
                        spriteBatch.DrawString(mLibSans20, mResolutionString, new Vector2(mContentPadding, mTopContentPadding + 40), mTextColor * mMenuOpacity);

                        // Draw the resolution selector
                        var spacingWidth = (int) mLibSans20.MeasureString("Full Screen        ").X;
                        var currentResoString = mResolutionList[mResolutionChosen].GetFirst() + " x "
                                                + mResolutionList[mResolutionChosen].GetSecond();

                        // figure out centering for the text
                        var resoStringSize = mLibSans20.MeasureString(currentResoString);
                        var resoStringPosition =
                            new Vector2(mContentPadding + 16+ spacingWidth * 0.5f - resoStringSize.X * 0.5f,
                                mTopContentPadding + 96 - resoStringSize.Y * 0.5f);

                        spriteBatch.DrawString(mLibSans20, currentResoString, resoStringPosition, mTextColor);  

                        spriteBatch.DrawRectangle(new Rectangle((int)mContentPadding, (int)mTopContentPadding + 80, spacingWidth + 32, 32), mTextColor);
                        
                    }
                    
                    break;
                case EOptionScreenState.Audio:
                    foreach (var button in mAudioCheckboxes)
                    {
                        button.Draw(spriteBatch);
                    }

                    foreach (var slider in mAudioSliders)
                    {
                        slider.Draw(spriteBatch);
                    }

                    spriteBatch.DrawString(mLibSans20, "Master Volume", new Vector2(mContentPadding, mTopContentPadding + 50), mTextColor * mMenuOpacity);
                    spriteBatch.DrawString(mLibSans20, "Music Volume", new Vector2(mContentPadding, mTopContentPadding + 110), mTextColor * mMenuOpacity);
                    spriteBatch.DrawString(mLibSans20, "Effects Volume", new Vector2(mContentPadding, mTopContentPadding + 170), mTextColor * mMenuOpacity);
                    spriteBatch.DrawString(mLibSans20, "UI Volume", new Vector2(mContentPadding, mTopContentPadding + 230), mTextColor * mMenuOpacity);
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
            mTruth = !mGame.mGraphics.IsFullScreen;
            mGame.mGraphics.IsFullScreen = mTruth;
            
        }

        private void OnResoDownReleased(Object sender, EventArgs eventArgs)
        {
            mResolutionChosen--;
        }

        private void OnResoUpReleased(Object sender, EventArgs eventArgs)
        {
            mResolutionChosen++;
        }

        private void OnSaveReleased(Object sender, EventArgs eventArgs)
        {
            if (mTruth)
            {
                mWidth = mGame.mGraphicsAdapter.CurrentDisplayMode.Width;
                mHeight = mGame.mGraphicsAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                mWidth = mResolutionList[mResolutionChosen].GetFirst();
                mHeight = mResolutionList[mResolutionChosen].GetSecond();
            }

            mGame.mGraphics.PreferredBackBufferWidth = mWidth;
            mGame.mGraphics.PreferredBackBufferHeight = mHeight;
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