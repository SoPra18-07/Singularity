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
    /// Used to show achievements that the player has earned.
    /// It shows achievements already earned in a list format, not
    /// unsimilar to how steam shows achievements. It has no buttons
    /// other than back and simply shows textures of the achievements
    /// and their description. If an achievement has not been earned yet,
    /// the achievement texture is blacked out.
    /// </summary>
    internal sealed class AchievementsScreen : MenuWindow, ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.AchievementsScreen;

        public bool Loaded { get; set; }

        // Layout.
        private const float AchievementOffset = 35;

        // All strings are variables to allow for easy editing and localization
        private const string WindowTitleStr = "Achievements";

        // Button colors
        private readonly Color mTextColor;

        // tab buttons
        private Button mBackButton;

        // Transitions variables
        private float mMenuOpacity;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;

        public bool TransitionRunning { get; private set; }

        /// <summary>
        /// Creates an instance of the Achievements screen.
        /// </summary>
        /// <param name="screenResolution">Screen resolution used for scaling</param>
        public AchievementsScreen(Vector2 screenResolution)
            : base(screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 283, screenResolution.Y / 4 - 120);
            mMenuBoxSize = new Vector2(566, 634);

            mMenuOpacity = 0;

            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mWindowOpacity = 1;
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            var mBackStr = "Back";
            
            // create the back button
            mBackButton = new Button(mBackStr,
                mLibSans20,
                mMenuBoxPosition + new Vector2(AchievementOffset, mMenuBoxSize.Y -40),
                mTextColor * mMenuOpacity);

            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

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

            mBackButton.Update(gametime);
            mBackButton.Opacity = mMenuOpacity;
            mBackButton.Position = mMenuBoxPosition + new Vector2(AchievementOffset, mMenuBoxSize.Y - 40);
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            base.Draw(spriteBatch);

            // background window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                mMenuBoxSize,
                Color.White,
                Color.White,
                0.5f,
                0.21f);

            // window title
            spriteBatch.DrawString(mLibSans36,
                text: WindowTitleStr,
                position: mMenuBoxPosition + new Vector2(20, 10),
                color: mTextColor * mMenuOpacity);

            // Back Button
            mBackButton.Draw(spriteBatch);

            #region Achievements

            // First
            spriteBatch.DrawString(mLibSans20,
                "The system goes online August 14th 1997",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 80),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Place 1 Platform",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 110),
                mTextColor * mMenuOpacity);

            // Second
            spriteBatch.DrawString(mLibSans20,
                "It becomes self aware at 2:14 AM",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 140),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Complete the tutorial",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 170),
                mTextColor * mMenuOpacity);

            // Third
            spriteBatch.DrawString(mLibSans20,
                "Skynet",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 200),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Build a network of 30 platforms",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 230),
                mTextColor * mMenuOpacity);

            // Fourth
            spriteBatch.DrawString(mLibSans20,
                "Wall E",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 260),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Burn 10,000 units of trash",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 290),
                mTextColor * mMenuOpacity);

            // Fifth
            spriteBatch.DrawString(mLibSans20,
                "HAL 9000",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 320),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Complete the campaign",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 350),
                mTextColor * mMenuOpacity);

            // Sixth
            spriteBatch.DrawString(mLibSans20,
                "Replicant",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 380),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Produce 50 units",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 410),
                mTextColor * mMenuOpacity);

            // Seventh
            spriteBatch.DrawString(mLibSans20,
                "Please rate our game perfect 5/7",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 440),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Build 1,000 Military units",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 470),
                mTextColor * mMenuOpacity);

            // Third
            spriteBatch.DrawString(mLibSans20,
                "Overachiever",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 500),
                mTextColor * mMenuOpacity);
            spriteBatch.DrawString(mLibSans12,
                "Unlock all achievements",
                new Vector2(mMenuBoxPosition.X + AchievementOffset, mMenuBoxPosition.Y + 530),
                mTextColor * mMenuOpacity);
            #endregion

            spriteBatch.End();
        }

        #region Screen Manager bools
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

        #endregion

        #region Transition
        private void Transition(GameTime gameTime)
        {
            switch (mTargetScreen)
            {
                case EScreen.AchievementsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 1f;
                    }

                    // opacity change
                    mMenuOpacity =
                        (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0;
                    }

                    // change menu opacity
                    mMenuOpacity =
                        (float)Animations.Easing(1, 0, mTransitionStartTime, mTransitionDuration, gameTime);

                    // position change
                    var xpos = (float)Animations.Easing(mScreenResolution.X / 2 - 283,
                        mScreenResolution.X / 2 - 204,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);
                    var ypos = (float)Animations.Easing(mScreenResolution.Y / 4 - 120,
                        mScreenResolution.Y / 4,
                        mTransitionStartTime,
                        mTransitionDuration,
                        gameTime);

                    mMenuBoxPosition = new Vector2(xpos, ypos);

                    // size change
                    var height =
                        (float)Animations.Easing(566, 408, mTransitionStartTime, mTransitionDuration, gameTime);
                    var menuWidth =
                        (float)Animations.Easing(634, 420, mTransitionStartTime, mTransitionDuration, gameTime);

                    mMenuBoxSize = new Vector2(height, menuWidth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (originScreen == EScreen.MainMenuScreen)
            {
                mMenuOpacity = 0f;
            }
            mTargetScreen = targetScreen;
            mTransitionDuration = 300;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            mMenuBoxPosition = new Vector2(mScreenResolution.X / 2 - 283, mScreenResolution.Y / 4 - 120);
            mMenuBoxSize = new Vector2(566, 634);
            TransitionRunning = true;

        }
        #endregion
    }
}
