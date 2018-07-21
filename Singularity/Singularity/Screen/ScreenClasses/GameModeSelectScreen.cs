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
    /// Shown after the "New Game" button on the main
    /// menu has been clicked. It shows the option to either
    /// play a new campaign or a new skirmish. It uses two text buttons
    /// and a back button.
    /// </summary>

    internal sealed class GameModeSelectScreen : MenuWindow, ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.GameModeSelectScreen;

        public bool Loaded { get; set; }

        private readonly string mMWindowTitleString;

        private readonly List<Button> mButtonList;

        // Transition variables
        private float mMenuOpacity;
        private double mTransitionStartTime;
        private double mTransitionDuration;
        private EScreen mTargetScreen;
        public bool TransitionRunning { get; private set; }

        // Selector Triangle
        private Texture2D mSelectorTriangle;
        private float mButtonVerticalCenter;
        private Vector2 mSelectorPosition;

        // Layout Variables
        private readonly float mButtonTopPadding;
        private readonly float mButtonLeftPadding;

        private bool mStoryButtonSelected;

        public GameModeSelectScreen(Vector2 screenResolution)
            : base(screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);

            mMenuBoxSize = new Vector2(408, 420);

            mMWindowTitleString = "New Game";

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mButtonList = new List<Button>(3);

            mMenuOpacity = 0;
            mWindowOpacity = 1;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            // Strings
            const string storyString = "Campaign Mode";
            const string freePlayString = "Skirmish";
            const string techDemoString = "Tech Demo";
            const string backString = "Back";

            // fonts and variables
            mButtonVerticalCenter = mLibSans20.MeasureString("Gg").Y / 2;

            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
            mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            var storyButton = new Button(storyString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
            var freePlayButton = new Button(freePlayString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
            var techDemoButton = new Button(techDemoString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
            var backButton = new Button(backString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mButtonList.Add(storyButton);
            mButtonList.Add(freePlayButton);
            mButtonList.Add(techDemoButton);
            mButtonList.Add(backButton);

            storyButton.ButtonReleased += OnStoryButtonReleased;
            freePlayButton.ButtonReleased += LoadGameManagerScreen.OnSkirmishReleased;
            techDemoButton.ButtonReleased += LoadGameManagerScreen.OnTechDemoButtonReleased;

            backButton.ButtonReleased += OnBackButtonReleased;

            storyButton.ButtonHovering += OnStoryHover;
            freePlayButton.ButtonHovering += OnSkirmishHover;
            techDemoButton.ButtonHovering += OnTechDemoHover;
            backButton.ButtonHovering += OnBackHover;

            Loaded = true;
        }

        public void Update(GameTime gametime)
        {
            if (TransitionRunning)
            {
                Transition(gametime);
            }

            if (!mStoryButtonSelected)
            {
                foreach (var button in mButtonList)
                {
                    button.Update(gametime);
                    button.Opacity = mMenuOpacity;
                }
            }
            else
            {
                mButtonList[3].Update(gametime);
                mButtonList[3].Opacity = mMenuOpacity;
            }
        }

        /// <summary>
        /// Code that actually does the transition
        /// </summary>
        /// <param name="gameTime">Current gameTime</param>
        private void Transition(GameTime gameTime)
        {
            switch (mTargetScreen)
            {
                case EScreen.GameModeSelectScreen:
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

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            base.Draw(spriteBatch);

            spriteBatch.DrawString(mLibSans36,
                mMWindowTitleString,
                new Vector2(mMenuBoxPosition.X + 20, mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);
            
            // Draw menu window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                mMenuBoxSize,
                Color.White,
                Color.White,
                .5f,
                .20f);

            // campaign text
            if (!mStoryButtonSelected)
            {
                foreach (var button in mButtonList)
                {
                    button.Draw(spriteBatch);
                }

                // Draw selector triangle
                spriteBatch.Draw(mSelectorTriangle,
                    mSelectorPosition,
                    null,
                    Color.White * mMenuOpacity,
                    0f,
                    new Vector2(0, 11),
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                // Draw the text
                spriteBatch.DrawString(mLibSans14,
                    "Campaign is available as a separate",
                    new Vector2(mButtonLeftPadding, mButtonTopPadding),
                    new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);
                spriteBatch.DrawString(mLibSans14,
                    "DLC for € 19,99",
                    new Vector2(mButtonLeftPadding, mButtonTopPadding + 50),
                    new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);

                // Draw the back button
                mButtonList[3].Draw(spriteBatch);

                // Draw selector triangle
                spriteBatch.Draw(mSelectorTriangle,
                    mSelectorPosition,
                    null,
                    Color.White * mMenuOpacity,
                    0f,
                    new Vector2(0, 11),
                    1f,
                    SpriteEffects.None,
                    0f);
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

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            mTargetScreen = targetScreen;
            mTransitionDuration = 350;
            mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;
        }

        #region Button Release Handlers

        /// <summary>
        /// Used to create a new story mode game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnStoryButtonReleased(Object sender, EventArgs eventArgs)
        {
            mStoryButtonSelected = true;

        }
        
        /// <summary>
        /// Used to create a new story mode game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnBackButtonReleased(Object sender, EventArgs eventArgs)
        {
            if (mStoryButtonSelected)
            {
                mStoryButtonSelected = false;
            }
            else
            {
                MainMenuManagerScreen.OnBackButtonReleased(sender, eventArgs);
            }
        }

        #endregion
        #region Button Hover Handlers

        private void OnStoryHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
        }

        private void OnSkirmishHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 50);
        }

        private void OnTechDemoHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 100);
        }

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 150);
        }
        #endregion
    }
}
