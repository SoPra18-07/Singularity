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
        public bool Loaded { get; set; }

        private readonly Game1 mMGame;

        // layout. Made only once to reduce unnecssary calculations at draw time
        private Vector2 mMBoxPosition;
        private readonly Vector2 mMWindowTitlePosition;
        private readonly Vector2 mMLinePosition;
        private readonly float mMTabPadding;
        private readonly float mMContentPadding;
        private readonly float mMTopContentPadding;
        private readonly Vector2 mMScreenResolution;

        // All strings are variables to allow for easy editing and localization
        private readonly string mMWindowTitleString;
        private readonly string mMGameplayString;
        private readonly string mMGraphicsString;
        private readonly string mMAudioString;
        private readonly string mMSaveChangesString;
        private readonly string mMBackString;

        private readonly string mMFullScreenString;
        private readonly string mMResolutionString; // used later
        private readonly string mMAntialiasingString;

        private readonly string mMMuteString;

        // fonts
        private SpriteFont mMLibSans36;
        private SpriteFont mMLibSans20;

        // Button colors
        private readonly Color mMTextColor;

        // tab buttons
        private readonly List<Button> mMTabButtons;
        private Button mMGameplayButton;
        private Button mMGraphicsButton;
        private Button mMAudioButton;
        private Button mMSaveButton;
        private Button mMBackButton;

        // Graphics tab
        private readonly List<Button> mMGraphicsButtons;
        private Button mMFullScreen; // todo replace with a toggle
        private Button mMResolution1; // todo replace with a better system
        private Button mMResolution2; // todo replace with a better system
        private Button mMAntialiasing; // todo replace with a toggle

        // Audio tab
        // todo add the following:
        private readonly List<Button> mMAudioButtons;
        private Button mMMuteButton; // add slider once completed
        // Background volume and toggle
        // Sound effect volume and toggle
        // 3D sound effect toggle

        // Transitions variables
        private float mMMenuOpacity;
        private Vector2 mMMenuBoxSize;
        private double mMTransitionStartTime;
        private double mMTransitionDuration;
        private EScreen mMTargetScreen;
        public bool TransitionRunning { get; private set; }

        private EOptionScreenState mMScreenState;

        /// <summary>
        /// Creates an instance of the Options screen.
        /// </summary>
        /// <param name="screenResolution">Screen resolution used for scaling</param>
        /// <param name="screenResolutionChanged"></param>
        /// <param name="game">Game1 class passed on to options to allow changing of options</param>
        public OptionsScreen(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            // scaling of all positions according to viewport size
            mMScreenResolution = screenResolution;
            mMBoxPosition = new Vector2(mMScreenResolution.X / 2 - 306, mMScreenResolution.Y / 4);
            mMMenuBoxSize = new Vector2(612, 420);

            mMTabPadding = mMBoxPosition.X + 36;
            mMContentPadding = mMBoxPosition.X + 204;
            mMTopContentPadding = mMBoxPosition.Y + 84;
            mMWindowTitlePosition = new Vector2(mMBoxPosition.X + 20, mMBoxPosition.Y + 20);
            mMLinePosition = new Vector2(mMBoxPosition.X + 180, mMBoxPosition.Y + 85);

            mMTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mMWindowTitleString = "Options";
            mMGameplayString = "Gameplay";
            mMGraphicsString = "Graphics";
            mMAudioString = "Audio";
            mMSaveChangesString = "Apply";
            mMBackString = "Back";

            mMFullScreenString = "Full Screen";
            mMResolutionString = "Resolution:";
            mMAntialiasingString = "Anti-Aliasing";

            mMMuteString = "Mute";

            mMTabButtons = new List<Button>(5);
            mMGraphicsButtons = new List<Button>(4);
            mMAudioButtons = new List<Button>(1);

            mMScreenState = EOptionScreenState.Gameplay;
            mMGame = game;

            mMMenuOpacity = screenResolutionChanged ? 1 : 0;

        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mMLibSans36 = content.Load<SpriteFont>("LibSans36");
            mMLibSans20 = content.Load<SpriteFont>("LibSans20");

            // make the tab select buttons
            mMGameplayButton = new Button(mMGameplayString, mMLibSans20, new Vector2(mMTabPadding, mMTopContentPadding), mMTextColor);
            mMGraphicsButton = new Button(mMGraphicsString, mMLibSans20, new Vector2(mMTabPadding, mMTopContentPadding + 40), mMTextColor);
            mMAudioButton = new Button(mMAudioString, mMLibSans20, new Vector2(mMTabPadding, mMTopContentPadding + 80), mMTextColor);
            mMSaveButton = new Button(mMSaveChangesString, mMLibSans20, new Vector2(mMTabPadding, mMTopContentPadding + 120), mMTextColor);
            mMBackButton = new Button(mMBackString, mMLibSans20, new Vector2(mMTabPadding, mMTopContentPadding + 160), mMTextColor);

            mMTabButtons.Add(mMGameplayButton);
            mMTabButtons.Add(mMGraphicsButton);
            mMTabButtons.Add(mMAudioButton);
            mMTabButtons.Add(mMSaveButton);
            mMTabButtons.Add(mMBackButton);

            foreach (Button tabButton in mMTabButtons)
            {
                tabButton.Opacity = mMMenuOpacity;
            }

            // Gameplay settings
            // TODO figure out what settings can be implemented in here

            // Graphics settings
            mMFullScreen = new Button(mMFullScreenString, mMLibSans20, new Vector2(mMContentPadding, mMTopContentPadding), mMTextColor);
            mMResolution1 = new Button("800 x 600", mMLibSans20, new Vector2(mMContentPadding, mMTopContentPadding + 40), mMTextColor);
            mMResolution2 = new Button("960 x 720", mMLibSans20, new Vector2(mMContentPadding, mMTopContentPadding + 80));
            mMAntialiasing = new Button(mMAntialiasingString, mMLibSans20, new Vector2(mMContentPadding, mMTopContentPadding + 120), mMTextColor);

            mMGraphicsButtons.Add(mMFullScreen);
            mMGraphicsButtons.Add(mMResolution1);
            mMGraphicsButtons.Add(mMResolution2);
            mMGraphicsButtons.Add(mMAntialiasing);

            foreach (Button graphicsButton in mMGraphicsButtons)
            {
                graphicsButton.Opacity = mMMenuOpacity;
            }

            // Audio settings
            mMMuteButton = new Button(mMMuteString, mMLibSans20, new Vector2(mMContentPadding, mMTopContentPadding), mMTextColor);

            mMAudioButtons.Add(mMMuteButton);

            // Button handler bindings
            mMGameplayButton.ButtonReleased += OnGameplayReleased;
            mMGraphicsButton.ButtonReleased += OnGraphicsReleased;
            mMAudioButton.ButtonReleased += OnAudioReleased;
            mMBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mMFullScreen.ButtonReleased += OnFullScreenReleased;
            mMResolution1.ButtonReleased += OnResoOneReleased;
            mMResolution2.ButtonReleased += OnResoTwoReleased;
            mMAntialiasing.ButtonReleased += OnAntialiasingReleased;

            mMMuteButton.ButtonReleased += OnMuteReleased;
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

            foreach (Button button in mMTabButtons)
            {
                button.Update(gametime);
                button.Opacity = mMMenuOpacity;
            }

            switch (mMScreenState)
            {
                case EOptionScreenState.Gameplay:

                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mMGraphicsButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = mMMenuOpacity;
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (Button button in mMAudioButtons)
                    {
                        button.Update(gametime);
                        button.Opacity = mMMenuOpacity;
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
            spriteBatch.StrokedRectangle(mMBoxPosition,
                mMMenuBoxSize,
                Color.White,
                Color.White,
                0.5f,
                0.21f);

            // line in the middle
            spriteBatch.DrawLine(point: mMLinePosition,
                angle: (float) Math.PI / 2,
                length: 301,
                color: new Color(new Vector4(1, 1, 1, 0.5f)) * mMMenuOpacity,
                thickness: 1);

            // window title
            spriteBatch.DrawString(mMLibSans36,
                text: mMWindowTitleString,
                position: mMWindowTitlePosition,
                color: mMTextColor * mMMenuOpacity);

            // tab buttons
            foreach (Button button in mMTabButtons)
            {
                button.Draw(spriteBatch);
            }

            // actual options
            switch (mMScreenState)
            {
                case EOptionScreenState.Gameplay:
                    spriteBatch.DrawString(mMLibSans20, "Difficulty", new Vector2(mMContentPadding, mMTopContentPadding), Color.White * mMMenuOpacity);
                    break;
                case EOptionScreenState.Graphics:
                    foreach (Button button in mMGraphicsButtons)
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                case EOptionScreenState.Audio:
                    foreach (var button in mMAudioButtons)
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
            switch (mMTargetScreen)
            {
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mMTransitionStartTime + mMTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMMenuOpacity = 0f;
                        mMMenuBoxSize = new Vector2(408, 420);
                        mMBoxPosition = new Vector2(mMScreenResolution.X / 2 - 204, mMScreenResolution.Y / 4);
                    }

                    var width = (float) Animations.Easing(612,
                        408,
                        mMTransitionStartTime,
                        mMTransitionDuration,
                        gameTime);

                    mMBoxPosition = new Vector2(mMScreenResolution.X / 2 - (int)Math.Floor(width / 2), mMScreenResolution.Y / 4);
                    mMMenuBoxSize = new Vector2(width, 420);

                    mMMenuOpacity = (float)Animations.Easing(1f, 0f, mMTransitionStartTime, mMTransitionDuration, gameTime);
                    break;
                case EScreen.OptionsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mMTransitionStartTime + mMTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMMenuOpacity = 1f;
                    }

                    mMMenuOpacity =
                        (float) Animations.Easing(0, 1f, mMTransitionStartTime, mMTransitionDuration, gameTime);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mMTargetScreen), mMTargetScreen, null);
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
            mMScreenState = EOptionScreenState.Gameplay;
        }

        /// <summary>
        /// Handler for the Graphics button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnGraphicsReleased(Object sender, EventArgs eventArgs)
        {
            mMScreenState = EOptionScreenState.Graphics;
        }

        /// <summary>
        /// Handler for the Audio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAudioReleased(Object sender, EventArgs eventArgs)
        {
            mMScreenState = EOptionScreenState.Audio;
        }

        /// <summary>
        /// Makes the game full screen. Currently makes the game full screen with the actual screen resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnFullScreenReleased(Object sender, EventArgs eventArgs)
        {
            var width = mMGame.mMGraphicsAdapter.CurrentDisplayMode.Width;
            var height = mMGame.mMGraphicsAdapter.CurrentDisplayMode.Height;
            var truth = false;
            if (mMGame.mMGraphics.IsFullScreen)
            {
                width = 1080;
                height = 720;
            }
            else
            {
                truth = true;
            }
            mMGame.mMGraphics.PreferredBackBufferWidth = width;
            mMGame.mMGraphics.PreferredBackBufferHeight = height;
            mMGame.mMGraphics.IsFullScreen = truth;
            mMGame.mMGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(width, height));
        }

        private void OnResoOneReleased(Object sender, EventArgs eventArgs)
        {
            mMGame.mMGraphics.PreferredBackBufferWidth = 800;
            mMGame.mMGraphics.PreferredBackBufferHeight = 600;
            mMGame.mMGraphics.ApplyChanges();
            MainMenuManagerScreen.SetResolution(new Vector2(800, 600));
        }

        private void OnResoTwoReleased(Object sender, EventArgs eventArgs)
        {
            mMGame.mMGraphics.PreferredBackBufferWidth = 960;
            mMGame.mMGraphics.PreferredBackBufferHeight = 720;
            mMGame.mMGraphics.ApplyChanges();
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
                mMMenuOpacity = 0f;
            }
            mMBoxPosition = new Vector2(mMScreenResolution.X / 2 - 306, mMScreenResolution.Y / 4);
            mMMenuBoxSize = new Vector2(612, 420);
            mMTargetScreen = targetScreen;
            mMTransitionDuration = 350f;
            mMTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;

        }
    }
}
