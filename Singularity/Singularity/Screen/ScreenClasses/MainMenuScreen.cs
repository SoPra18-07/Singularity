using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Shows the main menu screen with 5 options:
    /// New Game, Load Game, Achievements, Options, and Quit Game.
    /// </summary>
    internal sealed class MainMenuScreen : ITransitionableMenu
    {

        public EScreen Screen { get; private set; } = EScreen.MainMenuScreen;
        public bool Loaded { get; set; }

        // Fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // all text is stored as string variables to allow for easy changes
        private readonly string mPlayString;
        private readonly string mLoadSelectString;
        private readonly string mAchievementsString;
        private readonly string mOptionsString;
        private readonly string mQuitString;
        private readonly string mTitle;

        // layout
        private readonly float mButtonLeftPadding;
        private readonly float mButtonTopPadding;
        private float mButtonVerticalCenter;
        private readonly Vector2 mScreenResolution;
        private Vector2 mMenuBoxPosition;

        // Buttons on the main menu
        private Button mPlayButton;
        private Button mLoadButton;
        private Button mAchievementsButton;
        private Button mOptionsButton;
        private Button mQuitButton;
        private readonly List<Button> mButtonList;

        // Selector Triangle Variables
        private Texture2D mSelectorTriangle;
        private Vector2 mSelectorPosition;

        // Transition fields and properties
        private float mMenuOpacity;
        private float mWindowOpacity;
        private Vector2 mMenuBoxSize;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;
        private EScreen mOriginScreen;
        public bool TransitionRunning { get; private set; }

        /// <summary>
        /// Used to construct a new instance of the main menu screen
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game</param>
        public MainMenuScreen(Vector2 screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 2 - 210);
            mScreenResolution = screenResolution;
            mMenuBoxSize = new Vector2(408, 420);

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mPlayString = "New Game";
            mLoadSelectString = "Load Game";
            mAchievementsString = "Achievements";
            mOptionsString = "Options";
            mQuitString = "Quit";
            mTitle = "Singularity";

            mButtonList = new List<Button>(5);
            mMenuOpacity = 1;
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


            mPlayButton = new Button(mPlayString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mLoadButton = new Button(mLoadSelectString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mOptionsButton = new Button(mOptionsString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mAchievementsButton = new Button(mAchievementsString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mQuitButton = new Button(mQuitString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mButtonList.Add(mPlayButton);
            mButtonList.Add(mLoadButton);
            mButtonList.Add(mOptionsButton);
            mButtonList.Add(mAchievementsButton);
            mButtonList.Add(mQuitButton);

            mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            mPlayButton.ButtonReleased += MainMenuManagerScreen.OnPlayButtonReleased;
            mLoadButton.ButtonReleased += MainMenuManagerScreen.OnLoadButtonReleased;
            mOptionsButton.ButtonReleased += MainMenuManagerScreen.OnOptionsButtonReleased;
            mAchievementsButton.ButtonReleased += MainMenuManagerScreen.OnAchievementsButtonReleased;
            mQuitButton.ButtonReleased += MainMenuManagerScreen.OnQuitButtonReleased;

            mPlayButton.ButtonHovering += OnPlayHovering;
            mLoadButton.ButtonHovering += OnLoadHovering;
            mOptionsButton.ButtonHovering += OnOptionsHovering;
            mAchievementsButton.ButtonHovering += OnAchievementsHovering;
            mQuitButton.ButtonHovering += OnQuitHovering;

            Loaded = true;
        }


        /// <summary>
        /// Updates the buttons within the MainMenuScreen.
        /// </summary>
        /// <param name="gametime"></param>
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
            mOriginScreen = originScreen;
            mTargetScreen = targetScreen;
            TransitionRunning = true;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;

            if (originScreen != EScreen.SplashScreen)
            {
                mWindowOpacity = 1f;
            }
            switch (targetScreen)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    // means transitioning into this screen
                    // simply pull opacity from 0 to 1.0
                    mTransitionDuration = 350d;
                    mMenuOpacity = 0f;
                    mMenuBoxSize = new Vector2(408, 420);
                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 204, mScreenResolution.Y / 2 - 210);
                    mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
                    break;
                case EScreen.OptionsScreen:
                    // grow
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(originScreen), originScreen, null);
            }
        }

        private void Transition(GameTime gameTime)
        {
            switch (mTargetScreen)
            {
                case EScreen.AchievementsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                        mMenuBoxSize = new Vector2(600, 650);
                        mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 300, mScreenResolution.Y / 2 - 325);
                    }

                    var widthAch = (float)Animations.Easing(408,
                        600,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    var heightAch = (float)Animations.Easing(420,
                        650,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mMenuBoxSize = new Vector2(widthAch, heightAch);
                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - (int)Math.Floor(widthAch / 2), mScreenResolution.Y / 2 - (int)Math.Floor(heightAch / 2));

                    mMenuOpacity = (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.GameModeSelectScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(1, 0, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.LoadSelectScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(1, 0, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.MainMenuScreen:
                    if (mOriginScreen == EScreen.SplashScreen)
                    {
                        if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                        {
                            TransitionRunning = false;
                            mMenuOpacity = 1f;
                        }

                        var opacity =
                            (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);
                        mMenuOpacity = opacity;
                        mWindowOpacity = opacity;
                    }
                    else
                    {
                        if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                        {
                            TransitionRunning = false;
                            mMenuOpacity = 1f;
                        }

                        mMenuOpacity =
                            (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);
                    }

                    break;
                case EScreen.OptionsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                        mMenuBoxSize = new Vector2(612, 420);
                        mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 306, mScreenResolution.Y / 2 - 210);
                    }

                    var width = (float)Animations.Easing(408,
                        612,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - (int)Math.Floor(width / 2), mScreenResolution.Y / 2 - 210);
                    mMenuBoxSize = new Vector2(width, 420);

                    mMenuOpacity = (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
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

            foreach (Button button in mButtonList)
            {
                button.Draw(spriteBatch);
            }

            // draw selector triangle
            spriteBatch.Draw(mSelectorTriangle,
                mSelectorPosition,
                null,
                Color.White * mMenuOpacity,
                0f,
                new Vector2(0, 11),
                1f,
                SpriteEffects.None,
                0f);

            // Draw menu window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                mMenuBoxSize,
                Color.White * mWindowOpacity,
                Color.White * mWindowOpacity,
                .5f,
                .20f);
            spriteBatch.DrawString(mLibSans36,
                mTitle,
                new Vector2(mMenuBoxPosition.X + 20, mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);

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

        private void OnPlayHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
        }

        private void OnLoadHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 50);
        }

        private void OnOptionsHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 100);
        }

        private void OnAchievementsHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 150);
        }

        private void OnQuitHovering(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 200);
        }

        #endregion
    }
}
