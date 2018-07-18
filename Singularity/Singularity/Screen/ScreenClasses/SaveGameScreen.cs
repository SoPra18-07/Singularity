using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Serialization;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Shown after the "Save Game" button on the pause menu
    /// has been clicked. It shows your game save slots and if you click on them
    /// the game will be saved. //TODO: Implement actual saving function.
    /// </summary>

    internal sealed class SaveGameScreen : ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.SaveGameScreen;

        public bool Loaded { get; set; }

        public int Saves { get; private set; }
        private string[] mGameSaveStrings;
        private readonly string mBackString;
        private readonly string mWindowTitleString;

        private readonly List<Button> mButtonList;

        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        private Button mSave1;
        private Button mSave2;
        private Button mSave3;
        private Button mSave4;
        private Button mSave5;
        private Button mBackButton;

        // Transition variables
        private readonly Vector2 mMenuBoxPosition;
        private float mMenuOpacity;
        private readonly Vector2 mMenuBoxSize;
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
        private const float BottomPadding = 10;

        public SaveGameScreen(Vector2 screenResolution)
        {
            mMenuBoxSize = new Vector2(350, 400);
            mMenuBoxPosition = new Vector2((screenResolution.X / 2) - (mMenuBoxSize.X / 2), screenResolution.Y / 2 - (mMenuBoxSize.Y / 2));
            mBackString = "Back";
            mWindowTitleString = "Save Game";

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mButtonList = new List<Button>(6);

            mMenuOpacity = 0;
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

            mGameSaveStrings = XSerializer.GetSaveNames();
            switch (mGameSaveStrings.Length)
            {
                case 0:
                    mSave1 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 0;
                    break;
                case 1:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 1;

                    break;
                case 2:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 2;

                    break;
                case 3:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 3;

                    break;
                case 4:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button(mGameSaveStrings[3].Remove(mGameSaveStrings[3].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 4;

                    break;
                case 5:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button(mGameSaveStrings[3].Remove(mGameSaveStrings[3].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button(mGameSaveStrings[4].Remove(mGameSaveStrings[4].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 5;

                    break;
                default:
                    //Just load 5 saves, if by any chance there are more than 5 saves. It means that the player has added more saves and knows what he does...I hope...
                    //TODO: Spawn Textbox informing the player that there are only 5 slots but more saves
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button(mGameSaveStrings[3].Remove(mGameSaveStrings[3].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button(mGameSaveStrings[4].Remove(mGameSaveStrings[4].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 5;
                    break;
            }
            // First make a dummy version of BackButton to determine it's dimensions for centering purposes.
            mBackButton = new Button(mBackString, mLibSans20, new Vector2(0, 0), Color.White);
            var backButtonPosX = mMenuBoxPosition.X + (mMenuBoxSize.X / 2) - (mBackButton.Size.X / 2);
            var backButtonPosY = mMenuBoxPosition.Y + mMenuBoxSize.Y - mBackButton.Size.Y - BottomPadding;
            mBackButton = new Button(mBackString,
                    mLibSans20,
                    new Vector2(backButtonPosX, backButtonPosY),
                    Color.White)
            { Opacity = 0 };

            mButtonList.Add(mBackButton);
            mButtonList.Add(mSave1);
            mButtonList.Add(mSave2);
            mButtonList.Add(mSave3);
            mButtonList.Add(mSave4);
            mButtonList.Add(mSave5);

            mBackButton.ButtonReleased += GamePauseManagerScreen.OnBackButtonReleased;
            mSave1.ButtonClicked += GamePauseManagerScreen.OnSave1ButtonClicked;

            mBackButton.ButtonHovering += OnBackHover;
            mSave1.ButtonHovering += OnSave1;
            mSave2.ButtonHovering += OnSave2;
            mSave3.ButtonHovering += OnSave3;
            mSave4.ButtonHovering += OnSave4;
            mSave5.ButtonHovering += OnSave5;

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
                case EScreen.GamePauseScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMenuOpacity = 0f;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(1, 0f, mTransitionStartTime, mTransitionDuration, gameTime);
                    break;
                case EScreen.SaveGameScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mTransitionStartTime + mTransitionDuration)
                    {
                        TransitionRunning = false;
                        // Console.WriteLine("SGS Transition ended");
                        mMenuOpacity = 1f;
                    }

                    mMenuOpacity =
                        (float)Animations.Easing(0, 1f, mTransitionStartTime, mTransitionDuration, gameTime);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            if (originScreen == EScreen.GamePauseScreen)
            {
                mMenuOpacity = 0f;
            }
            if (originScreen == EScreen.SaveGameScreen)
            {
                mMenuOpacity = 1f;
            }
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

            // Draw menu window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                mMenuBoxSize,
                Color.White,
                new Color(0.27f, 0.5f, 0.7f, 1f),
                1f,
                1f);
            spriteBatch.DrawString(mLibSans36,
                mWindowTitleString,
                new Vector2(mMenuBoxPosition.X + 20, mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * mMenuOpacity);

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

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + (mMenuBoxSize.X / 2) - mBackButton.Size.X, mMenuBoxPosition.Y + mMenuBoxSize.Y - mBackButton.Size.Y + BottomPadding / 2);
        }

        private void OnSave1(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter);
        }

        private void OnSave2(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 50);
        }

        private void OnSave3(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 100);
        }

        private void OnSave4(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 150);
        }

        private void OnSave5(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 200);
        }
        #endregion
    }
}