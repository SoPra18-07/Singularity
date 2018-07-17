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
        private readonly float mBottomPadding = 10;

        // All strings are variables to allow for easy editing and localization
        private readonly string mWindowTitleStr = "Achievements";

        // Button colors
        private readonly Color mTextColor;

        // tab buttons
        private readonly List<Button> mTabButtons;
        private Button mBackButton;
        private Button mBackButtonDummy;

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
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);
            mMenuBoxSize = new Vector2(408, 420);

            mMenuOpacity = 0;

            mTextColor = new Color(new Vector3(.9137f, .9058f, .8314f));

            mTabButtons = new List<Button>(1);
            

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

        // Create a dummy button to get the dimensions of it so I can center the real button.
        mBackButtonDummy = new Button(mBackStr, mLibSans20, new Vector2(0, 0), mTextColor);
            var backButtonPosX = mMenuBoxPosition.X + (mMenuBoxSize.X / 2) - (mBackButtonDummy.Size.X / 2);
            var backButtonPosY = mMenuBoxPosition.Y + mMenuBoxSize.Y - mBackButtonDummy.Size.Y - mBottomPadding;
            mBackButton = new Button(mBackStr, mLibSans20, new Vector2(backButtonPosX, backButtonPosY), mTextColor);

            mTabButtons.Add(mBackButton);

            foreach (var tabButton in mTabButtons)
            {
                tabButton.Opacity = mMenuOpacity;
            }

            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

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

            foreach (var button in mTabButtons)
            {
                button.Update(gametime);
                button.Opacity = mMenuOpacity;
            }
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
                text: mWindowTitleStr,
                position: mMenuBoxPosition + new Vector2(20, 10),
                color: mTextColor * mMenuOpacity);

            // tab buttons
            foreach (var button in mTabButtons)
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

                    mMenuOpacity =
                        (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(1, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
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
            mTransitionDuration = 350;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;

        }
    }
}
