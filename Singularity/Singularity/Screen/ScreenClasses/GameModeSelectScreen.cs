using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    internal sealed class GameModeSelectScreen : ITransitionableMenu
    {

        private readonly string mStoryString;
        private readonly string mFreePlayString;
        private readonly string mBackString;
        private readonly string mWindowTitleString;

        private readonly List<Button> mButtonList;

        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        private Button mStoryButton;
        private Button mFreePlayButton;
        private Button mBackButton;

        // Transition variables
        private readonly Vector2 mMenuBoxPosition;
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
        private float mButtonTopPadding;
        private float mButtonLeftPadding;
        

        public GameModeSelectScreen(Vector2 screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);

            mStoryString = "Campaign Mode";
            mFreePlayString = "Skirmish";
            mBackString = "Back";
            mWindowTitleString = "New Game";

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mButtonList = new List<Button>(3);

            mMenuOpacity = 0;
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

            foreach (Button button in mButtonList)
            {
                button.Update(gametime);
                button.Opacity = mMenuOpacity;
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
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (Button button in mButtonList)
            {
                button.Draw(spriteBatch);
            }

            // Draw selector triangle
            spriteBatch.Draw(mSelectorTriangle,
                position: mSelectorPosition,
                sourceRectangle: null,
                color: Color.White * mMenuOpacity,
                rotation: 0f,
                origin: new Vector2(0, 11),
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            // Draw menu window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                new Vector2(408, 420),
                Color.White,
                Color.White,
                .5f,
                .20f);
            spriteBatch.DrawString(mLibSans36,
                mWindowTitleString,
                new Vector2(mMenuBoxPosition.X + 20, mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mButtonVerticalCenter = mLibSans20.MeasureString("Gg").Y / 2;

            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
            mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            mStoryButton = new Button(mStoryString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mFreePlayButton = new Button(mFreePlayString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mBackButton = new Button(mBackString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mButtonList.Add(mStoryButton);
            mButtonList.Add(mFreePlayButton);
            mButtonList.Add(mBackButton);

            mStoryButton.ButtonReleased += MainMenuManagerScreen.OnStoryButtonReleased;
            mFreePlayButton.ButtonReleased += MainMenuManagerScreen.OnFreePlayButtonReleased;
            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mStoryButton.ButtonHovering += OnStoryHover;
            mFreePlayButton.ButtonHovering += OnFreePlayHover;
            mBackButton.ButtonHovering += OnBackHover;
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

        #region Button Hover Handlers

        private void OnStoryHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
        }

        private void OnFreePlayHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 50);
        }

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 100);
        }
        #endregion
    }
}
