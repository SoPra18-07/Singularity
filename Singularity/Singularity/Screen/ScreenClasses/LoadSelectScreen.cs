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
    /// Shown after the "Load Game" button on the main
    /// menu has been clicked. It shows your game saves and if you click on them
    /// they will load.
    /// </summary>

    internal sealed class LoadSelectScreen : MenuWindow, ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.GameModeSelectScreen;

        public bool Loaded { get; set; }

        public int Saves { get; private set; }
        private string[] mGameSaveStrings;
        private readonly string mBackString;
        private readonly string mWindowTitleString;

        private readonly List<Button> mMButtonList;

        private Button mSave1;
        private Button mSave2;
        private Button mSave3;
        private Button mSave4;
        private Button mSave5;
        private Button mMBackButton;

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


        public LoadSelectScreen(Vector2 screenResolution)
            : base(screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);
            mMenuBoxSize = new Vector2(408, 420);

            mBackString = "Back";
            mWindowTitleString = "Load Game";

            mButtonLeftPadding = mMenuBoxPosition.X + 60;
            mButtonTopPadding = mMenuBoxPosition.Y + 90;

            mMButtonList = new List<Button>(6);

            mMenuOpacity = 0;
            mWindowOpacity = 1;
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

            foreach (Button button in mMButtonList)
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
                case EScreen.LoadSelectScreen:
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

            foreach (var button in mMButtonList)
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
                mMenuBoxSize,
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
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
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
                    mSave1.ButtonReleased += LoadGameManagerScreen.OnSave1Released;
                    break;
                case 2:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 2;
                    mSave1.ButtonReleased += LoadGameManagerScreen.OnSave1Released;
                    mSave2.ButtonReleased += LoadGameManagerScreen.OnSave2Released;
                    break;
                case 3:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 3;
                    mSave1.ButtonReleased += LoadGameManagerScreen.OnSave1Released;
                    mSave2.ButtonReleased += LoadGameManagerScreen.OnSave2Released;
                    mSave3.ButtonReleased += LoadGameManagerScreen.OnSave3Released;
                    break;
                case 4:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button(mGameSaveStrings[3].Remove(mGameSaveStrings[3].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button("Empty Slot", mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 4;
                    mSave1.ButtonReleased += LoadGameManagerScreen.OnSave1Released;
                    mSave2.ButtonReleased += LoadGameManagerScreen.OnSave2Released;
                    mSave3.ButtonReleased += LoadGameManagerScreen.OnSave3Released;
                    mSave4.ButtonReleased += LoadGameManagerScreen.OnSave4Released;
                    break;
                case 5:
                    mSave1 = new Button(mGameSaveStrings[0].Remove(mGameSaveStrings[0].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave2 = new Button(mGameSaveStrings[1].Remove(mGameSaveStrings[1].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave3 = new Button(mGameSaveStrings[2].Remove(mGameSaveStrings[2].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave4 = new Button(mGameSaveStrings[3].Remove(mGameSaveStrings[3].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    mSave5 = new Button(mGameSaveStrings[4].Remove(mGameSaveStrings[4].Length - 4), mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
                    Saves = 5;
                    mSave1.ButtonReleased += LoadGameManagerScreen.OnSave1Released;
                    mSave2.ButtonReleased += LoadGameManagerScreen.OnSave2Released;
                    mSave3.ButtonReleased += LoadGameManagerScreen.OnSave3Released;
                    mSave4.ButtonReleased += LoadGameManagerScreen.OnSave4Released;
                    mSave5.ButtonReleased += LoadGameManagerScreen.OnSave5Released;
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
            mMBackButton = new Button(mBackString, mLibSans20, new Vector2(mButtonLeftPadding, mButtonTopPadding + 250), new Color(new Vector3(.9137f, .9058f, .8314f)));
            mMButtonList.Add(mMBackButton);
            mMButtonList.Add(mSave1);
            mMButtonList.Add(mSave2);
            mMButtonList.Add(mSave3);
            mMButtonList.Add(mSave4);
            mMButtonList.Add(mSave5);

            mMBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mMBackButton.ButtonHovering += OnBackHover;
            mSave1.ButtonHovering += OnSave1;
            mSave2.ButtonHovering += OnSave2;
            mSave3.ButtonHovering += OnSave3;
            mSave4.ButtonHovering += OnSave4;
            mSave5.ButtonHovering += OnSave5;

            Loaded = true;
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

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            mSelectorPosition = new Vector2(mMenuBoxPosition.X + 22, mButtonTopPadding + mButtonVerticalCenter + 250);
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