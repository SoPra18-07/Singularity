using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;


namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shows the pause menu when in the middle of a game. It shows the
    /// following: Resume, Save, Statistics, Back to Main Menu.
    /// </summary>
    internal sealed class GamePauseScreen : ITransitionableMenu
    {
        public EScreen Screen { get; } = EScreen.GamePauseScreen;
        public bool Loaded { get; set; }

        // fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // all text is stored as string variables to allow for easy changes
        private const string ResumeString = "Resume";
        private const string SaveGameString = "Save Game";
        private const string StatisticsString = "Statistics";
        private const string MainMenuString = "Back to Main Menu";
        private const string Title = "Pause Menu";

        // layout
        private readonly float mButtonLeftPadding;
        private readonly float mButtonTopPadding;
        private float mButtonVerticalCenter;
        private readonly Vector2 mScreenResolution;
        private Vector2 mMenuBoxPosition;

        // Buttons on the pause menu
        private Button mResumeButton;
        private Button mSaveGameButton;
        private Button mStatisticsButton;
        private Button mMainMenuButton;

        private readonly List<Button> mButtonList;

        // Selector Triangle Variables
        private Texture2D mSelectorTriangle;
        private Vector2 mSelectorPosition;

        // Transition fields and properties
        private float mMenuOpacity;
        private Vector2 mMenuBoxSize;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;
        public bool TransitionRunning { get; private set; }

        /// <summary>
        /// TODO
        /// </summary>
        public GamePauseScreen(Vector2 screenSize)
        {
            mMenuBoxPosition = new Vector2(screenSize.X / 2 - 175, screenSize.Y / 2 - 200);
            mScreenResolution = screenSize;
            mMenuBoxSize = new Vector2(350, 400);

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mButtonList = new List<Button>(3);
            mMenuOpacity = 1f;
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // Selector Triangle Positioning
            mButtonVerticalCenter = mLibSans20.MeasureString("Gg").Y / 2;
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);

            mResumeButton = new Button(ResumeString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), Color.White);
            mSaveGameButton = new Button(SaveGameString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), Color.White);
            mStatisticsButton = new Button(StatisticsString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), Color.White);
            mMainMenuButton = new Button(MainMenuString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), Color.White);
            mButtonList.Add(mResumeButton);
            mButtonList.Add(mSaveGameButton);
            mButtonList.Add(mStatisticsButton);
            mButtonList.Add(mMainMenuButton);

            mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            mResumeButton.ButtonReleased += GamePauseManagerScreen.OnResumeButtonReleased;
            mSaveGameButton.ButtonReleased += GamePauseManagerScreen.OnSaveGameButtonReleased;
            mStatisticsButton.ButtonReleased += GamePauseManagerScreen.OnStatisticsButtonReleased;
            mMainMenuButton.ButtonReleased += GamePauseManagerScreen.OnMainMenuButtonReleased;
            mMainMenuButton.ButtonReleased += LoadGameManagerScreen.OnReturnToMainMenuClicked;

            mResumeButton.ButtonHovering += OnResumeHovering;
            mSaveGameButton.ButtonHovering += OnSaveGameHovering;
            mStatisticsButton.ButtonHovering += OnStatisticsHovering;
            mMainMenuButton.ButtonHovering += OnMainMenuHovering;

            Loaded = true;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            foreach (Button button in mButtonList)
            {
                button.Update(gametime);
            }

            if (TransitionRunning)
            {
                Transition(gametime);
            }

            foreach (Button button in mButtonList)
            {
                button.Opacity = mMenuOpacity;
            }
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            mTargetScreen = targetScreen;
            TransitionRunning = true;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;

            switch (targetScreen)
            {
                case EScreen.SaveGameScreen:
                    break;
                case EScreen.StatisticsScreen:
                    break;
                case EScreen.GamePauseScreen:
                    // means transitioning into this screen
                    // simply pull opacity from 0 to 1.0
                    mTransitionDuration = 350d;
                    mMenuOpacity = 0f;
                    mMenuBoxSize = new Vector2(350, 400);
                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 175, mScreenResolution.Y / 2 - 200);
                    mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(originScreen), originScreen, null);
            }
        }

        private void Transition(GameTime gameTime)
        {
            switch (mTargetScreen)
            {
                case EScreen.SaveGameScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 1f;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.StatisticsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                        mMenuBoxSize = new Vector2(400, 450);
                        mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 200, mScreenResolution.Y / 2 - 225);
                    }

                    var widthAch = (float)Animations.Easing(350,
                        400,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    var heightAch = (float)Animations.Easing(400,
                        450,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mMenuBoxSize = new Vector2(widthAch, heightAch);
                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - (int)Math.Floor(widthAch / 2),
                        mScreenResolution.Y / 2 - (int)Math.Floor(heightAch / 2));

                    mMenuOpacity =
                        (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.GamePauseScreen:
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
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw menu window
            spriteBatch.StrokedRectangle(location: mMenuBoxPosition,
                size: mMenuBoxSize,
                colorBorder: Color.White,
                colorCenter: new Color(0.27f, 0.5f, 0.7f, 1f),
                opacityBorder: 1f,
                opacityCenter: 1f);
            spriteBatch.DrawString(spriteFont: mLibSans36,
                text: Title,
                position: new Vector2(x: mMenuBoxPosition.X + 20, y: mMenuBoxPosition.Y + 10),
                color: new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);

            // draw selector triangle
            spriteBatch.Draw(texture: mSelectorTriangle,
                position: mSelectorPosition,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(x: 0, y: 11),
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            foreach (Button button in mButtonList)
            {
                button.Draw(spriteBatch);
            }

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

        #region Button Hover Handlers

        private void OnResumeHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
        }

        private void OnSaveGameHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 50);
        }

        private void OnStatisticsHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 100);
        }

        private void OnMainMenuHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 150);
        }

        #endregion

    }
}
