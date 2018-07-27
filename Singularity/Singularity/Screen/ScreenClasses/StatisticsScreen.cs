using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown after Statistics has been selected in the in game pause menu.
    /// Shows an array of statistics of what the player has done. It will be
    /// shown in the form of a graph over time with different buttons to
    /// filter different statistics.
    /// </summary>

    class StatisticsScreen : ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.StatisticsScreen;

        public bool Loaded { get; set; }

        // Fonts
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans14;

        // All strings are variables to allow for easy editing and localization
        private const string WindowTitleStr = "Statistics";
        private const string BackStr = "Back";
        private const string TimeStr = "Time:";
        private const string UnitsCreatedStr = "Units created:";
        private const string UnitsLostStr = "Units lost:";
        private const string UnitsKilledStr = "Units killed:";
        private const string ResourcesStr = "Resources created:";
        private const string PlatformsCreatedStr = "Platforms created:";
        private const string PlatformsLostStr = "Platforms lost:";
        private const string PlatformsDestroyedStr = "Platforms destroyed:";

        // Layout.
        private Vector2 mMenuBoxPosition;
        private readonly Vector2 mWindowTitlePosition;
        private const float BottomPadding = 10;
        private readonly Vector2 mScreenResolution;

        // Buttons
        private Button mBackButton;

        // Button colors
        private readonly Color mTextColor;

        // Transitions variables
        private float mMenuOpacity;
        private Vector2 mMenuBoxSize;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;
        public bool TransitionRunning { get; private set; }

        // Director to gain access to StoryManager.
        private readonly Director mDirector;

        public StatisticsScreen(Vector2 screenSize, Director director)
        {
            mScreenResolution = screenSize;
            mDirector = director;
            mMenuBoxSize = new Vector2(400, 450);
            mMenuBoxPosition = new Vector2((screenSize.X / 2) - (mMenuBoxSize.X / 2), screenSize.Y / 2 - (mMenuBoxSize.Y / 2));
            mWindowTitlePosition = new Vector2(mMenuBoxPosition.X + 25, mMenuBoxPosition.Y + 20);
            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mMenuOpacity = 0;
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");

            // First make a dummy version of BackButton to determine it's dimensions for centering purposes.
            mBackButton = new Button(BackStr, mLibSans20, new Vector2(0, 0), mTextColor);
            var backButtonPosX = mMenuBoxPosition.X + (mMenuBoxSize.X / 2) - (mBackButton.Size.X / 2);
            var backButtonPosY = mMenuBoxPosition.Y + mMenuBoxSize.Y - mBackButton.Size.Y - BottomPadding;
            mBackButton = new Button(BackStr,
                mLibSans20,
                new Vector2(backButtonPosX, backButtonPosY),
                new Color(new Vector3(.9137f, .9058f, .8314f)))
            { Opacity = mMenuOpacity };

            mBackButton.ButtonReleased += GamePauseManagerScreen.OnBackButtonReleased;

            Loaded = true;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            mBackButton.Update(gametime);

            if (TransitionRunning)
            {
                Transition(gametime);
            }

            mBackButton.Opacity = mMenuOpacity;
        }

        public void Transition(GameTime gameTime)
        {
            switch (mTargetScreen)
            {
                case EScreen.GamePauseScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                        mMenuBoxSize = new Vector2(350, 400);
                        mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 175, mScreenResolution.Y / 2 - 200);
                    }

                    var width = (float)Animations.Easing(400,
                        350,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    var height = (float)Animations.Easing(450,
                        400,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mMenuBoxSize = new Vector2(width, height);
                    mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - (int)Math.Floor(width / 2), mScreenResolution.Y / 2 - (int)Math.Floor(height / 2));

                    mMenuOpacity = (float)Animations.Easing(1f, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.StatisticsScreen:
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

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (originScreen == EScreen.GamePauseScreen)
            {
                mMenuOpacity = 0f;
            }
            mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 200, mScreenResolution.Y / 2 - 225);
            mMenuBoxSize = new Vector2(400, 450);
            mTargetScreen = targetScreen;
            mTransitionDuration = 350f;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // background window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                mMenuBoxSize,
                Color.White,
                new Color(0.27f, 0.5f, 0.7f, 1f),
                1f,
                1f);

            // spriteBatch.FillRectangle(mMenuBoxPosition, new Vector2(400, 450), new Color(0.27f, 0.5f, 0.7f, 1f), 0f);
            mBackButton.Draw(spriteBatch);

            // window title
            spriteBatch.DrawString(mLibSans20,
                WindowTitleStr,
                mWindowTitlePosition,
                mTextColor);

            // Draw the stats names.
            spriteBatch.DrawString(mLibSans14, TimeStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 60), mTextColor);
            spriteBatch.DrawString(mLibSans14, UnitsCreatedStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 110), mTextColor);
            spriteBatch.DrawString(mLibSans14, UnitsLostStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 150), mTextColor);
            spriteBatch.DrawString(mLibSans14, UnitsKilledStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 190), mTextColor);
            spriteBatch.DrawString(mLibSans14, ResourcesStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 240), mTextColor);
            spriteBatch.DrawString(mLibSans14, PlatformsCreatedStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 290), mTextColor);
            spriteBatch.DrawString(mLibSans14, PlatformsLostStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 370), mTextColor);
            spriteBatch.DrawString(mLibSans14, PlatformsDestroyedStr, new Vector2(mMenuBoxPosition.X + 60, mMenuBoxPosition.Y + 330), mTextColor);
            // Draw the stats behind their names, live from the StoryManager.
            spriteBatch.DrawString(mLibSans14, mDirector.GetClock.GetIngameTime().ToString(), new Vector2(mMenuBoxPosition.X + 110, mMenuBoxPosition.Y + 60), mTextColor);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.UnitsCount["created"].ToString(), new Vector2(mMenuBoxPosition.X + 180, mMenuBoxPosition.Y + 110), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.UnitsCount["lost"].ToString(), new Vector2(mMenuBoxPosition.X + 180, mMenuBoxPosition.Y + 150), Color.Red);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.UnitsCount["killed"].ToString(), new Vector2(mMenuBoxPosition.X + 180, mMenuBoxPosition.Y + 190), mTextColor);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.ResourcesCount.Sum(x => x.Value).ToString(), new Vector2(mMenuBoxPosition.X + 245, mMenuBoxPosition.Y + 240), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.PlatformsCount["created"].ToString(), new Vector2(mMenuBoxPosition.X + 240, mMenuBoxPosition.Y + 290), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.PlatformsCount["lost"].ToString(), new Vector2(mMenuBoxPosition.X + 240, mMenuBoxPosition.Y + 370), Color.Red);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.PlatformsCount["destroyed"].ToString(), new Vector2(mMenuBoxPosition.X + 240, mMenuBoxPosition.Y + 330), mTextColor);

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
