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

    internal sealed class GameModeSelectScreen : ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.GameModeSelectScreen;

        public bool Loaded { get; set; }


        private readonly string mMStoryString;
        private readonly string mMFreePlayString;
        private readonly string mMBackString;
        private readonly string mMWindowTitleString;

        private readonly List<Button> mMButtonList;

        private SpriteFont mMLibSans36;
        private SpriteFont mMLibSans20;

        private Button mMStoryButton;
        private Button mMFreePlayButton;
        private Button mMBackButton;

        // Transition variables
        private readonly Vector2 mMMenuBoxPosition;
        private float mMMenuOpacity;
        private double mMTransitionStartTime;
        private double mMTransitionDuration;
        private EScreen mMTargetScreen;
        public bool TransitionRunning { get; private set; }

        // Selector Triangle
        private Texture2D mMSelectorTriangle;
        private float mMButtonVerticalCenter;
        private Vector2 mMSelectorPosition;

        // Layout Variables
        private readonly float mMButtonTopPadding;
        private readonly float mMButtonLeftPadding;


        public GameModeSelectScreen(Vector2 screenResolution)
        {
            mMMenuBoxPosition = new Vector2(x: screenResolution.X / 2 - 204, y: screenResolution.Y / 4);

            mMStoryString = "Campaign Mode";
            mMFreePlayString = "Skirmish";
            mMBackString = "Back";
            mMWindowTitleString = "New Game";

            mMButtonLeftPadding = mMMenuBoxPosition.X + 60;
            mMButtonTopPadding = mMMenuBoxPosition.Y + 90;

            mMButtonList = new List<Button>(capacity: 3);

            mMMenuOpacity = 0;
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
                Transition(gameTime: gametime);
            }

            foreach (Button button in mMButtonList)
            {
                button.Update(gametime: gametime);
                button.Opacity = mMMenuOpacity;
            }
        }

        /// <summary>
        /// Code that actually does the transition
        /// </summary>
        /// <param name="gameTime">Current gameTime</param>
        private void Transition(GameTime gameTime)
        {
            switch (mMTargetScreen)
            {
                case EScreen.GameModeSelectScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mMTransitionStartTime + mMTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMMenuOpacity = 1f;
                    }

                    mMMenuOpacity =
                        (float)Animations.Easing(startValue: 0, endValue: 1f, startTime: mMTransitionStartTime, duration: mMTransitionDuration, gameTime: gameTime);
                    break;
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= mMTransitionStartTime + mMTransitionDuration)
                    {
                        TransitionRunning = false;
                        mMMenuOpacity = 0f;
                    }

                    mMMenuOpacity =
                        (float)Animations.Easing(startValue: 1, endValue: 0f, startTime: mMTransitionStartTime, duration: mMTransitionDuration, gameTime: gameTime);
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

            foreach (Button button in mMButtonList)
            {
                button.Draw(spriteBatch: spriteBatch);
            }

            // Draw selector triangle
            spriteBatch.Draw(texture: mMSelectorTriangle,
                position: mMSelectorPosition,
                sourceRectangle: null,
                color: Color.White * mMMenuOpacity,
                rotation: 0f,
                origin: new Vector2(x: 0, y: 11),
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            // Draw menu window
            spriteBatch.StrokedRectangle(location: mMMenuBoxPosition,
                size: new Vector2(x: 408, y: 420),
                colorBorder: Color.White,
                colorCenter: Color.White,
                opacityBorder: .5f,
                opacityCenter: .20f);
            spriteBatch.DrawString(spriteFont: mMLibSans36,
                text: mMWindowTitleString,
                position: new Vector2(x: mMMenuBoxPosition.X + 20, y: mMMenuBoxPosition.Y + 10),
                color: new Color(color: new Vector3(x: .9137f, y: .9058f, z: .8314f)) * mMMenuOpacity);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mMLibSans36 = content.Load<SpriteFont>(assetName: "LibSans36");
            mMLibSans20 = content.Load<SpriteFont>(assetName: "LibSans20");
            mMButtonVerticalCenter = mMLibSans20.MeasureString(text: "Gg").Y / 2;

            mMSelectorPosition = new Vector2(x: mMMenuBoxPosition.X + 22, y: mMButtonTopPadding + mMButtonVerticalCenter);
            mMSelectorTriangle = content.Load<Texture2D>(assetName: "SelectorTriangle");

            mMStoryButton = new Button(buttonText: mMStoryString, font: mMLibSans20, position: new Vector2(x: mMButtonLeftPadding, y: mMButtonTopPadding), color: new Color(color: new Vector3(x: .9137f, y: .9058f, z: .8314f)));
            mMFreePlayButton = new Button(buttonText: mMFreePlayString, font: mMLibSans20, position: new Vector2(x: mMButtonLeftPadding, y: mMButtonTopPadding + 50), color: new Color(color: new Vector3(x: .9137f, y: .9058f, z: .8314f)));
            mMBackButton = new Button(buttonText: mMBackString, font: mMLibSans20, position: new Vector2(x: mMButtonLeftPadding, y: mMButtonTopPadding + 100), color: new Color(color: new Vector3(x: .9137f, y: .9058f, z: .8314f)));
            mMButtonList.Add(item: mMStoryButton);
            mMButtonList.Add(item: mMFreePlayButton);
            mMButtonList.Add(item: mMBackButton);

            mMStoryButton.ButtonReleased += MainMenuManagerScreen.OnStoryButtonReleased;
            mMFreePlayButton.ButtonReleased += MainMenuManagerScreen.OnFreePlayButtonReleased;
            mMBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            mMStoryButton.ButtonHovering += OnStoryHover;
            mMFreePlayButton.ButtonHovering += OnFreePlayHover;
            mMBackButton.ButtonHovering += OnBackHover;

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
            mMTargetScreen = targetScreen;
            mMTransitionDuration = 350;
            mMTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;
        }

        #region Button Hover Handlers

        private void OnStoryHover(Object sender, EventArgs eventArgs)
        {
            mMSelectorPosition = new Vector2(x: mMMenuBoxPosition.X + 22, y: mMButtonTopPadding + mMButtonVerticalCenter);
        }

        private void OnFreePlayHover(Object sender, EventArgs eventArgs)
        {
            mMSelectorPosition = new Vector2(x: mMMenuBoxPosition.X + 22, y: mMButtonTopPadding + mMButtonVerticalCenter + 50);
        }

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            mMSelectorPosition = new Vector2(x: mMMenuBoxPosition.X + 22, y: mMButtonTopPadding + mMButtonVerticalCenter + 100);
        }
        #endregion
    }
}
