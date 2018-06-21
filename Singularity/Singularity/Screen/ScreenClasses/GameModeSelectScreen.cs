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
        public bool Loaded { get; set; }


        private readonly string _mStoryString;
        private readonly string _mFreePlayString;
        private readonly string _mBackString;
        private readonly string _mWindowTitleString;

        private readonly List<Button> _mButtonList;

        private SpriteFont _mLibSans36;
        private SpriteFont _mLibSans20;

        private Button _mStoryButton;
        private Button _mFreePlayButton;
        private Button _mBackButton;

        // Transition variables
        private readonly Vector2 _mMenuBoxPosition;
        private float _mMenuOpacity;
        private double _mTransitionStartTime;
        private double _mTransitionDuration;
        private EScreen _mTargetScreen;
        public bool TransitionRunning { get; private set; }

        // Selector Triangle
        private Texture2D _mSelectorTriangle;
        private float _mButtonVerticalCenter;
        private Vector2 _mSelectorPosition;

        // Layout Variables
        private readonly float _mButtonTopPadding;
        private readonly float _mButtonLeftPadding;
        

        public GameModeSelectScreen(Vector2 screenResolution)
        {
            _mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);

            _mStoryString = "Campaign Mode";
            _mFreePlayString = "Skirmish";
            _mBackString = "Back";
            _mWindowTitleString = "New Game";

            _mButtonLeftPadding = _mMenuBoxPosition.X + 60;
            _mButtonTopPadding = _mMenuBoxPosition.Y + 90;

            _mButtonList = new List<Button>(3);

            _mMenuOpacity = 0;
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

            foreach (Button button in _mButtonList)
            {
                button.Update(gametime);
                button.Opacity = _mMenuOpacity;
            }
        }

        /// <summary>
        /// Code that actually does the transition
        /// </summary>
        /// <param name="gameTime">Current gameTime</param>
        private void Transition(GameTime gameTime)
        {
            switch (_mTargetScreen)
            {
                case EScreen.GameModeSelectScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 1f;
                    }

                    _mMenuOpacity =
                        (float)Animations.Easing(0, 1f, _mTransitionStartTime, _mTransitionDuration, gameTime);
                    break;
                case EScreen.MainMenuScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 0f;
                    }

                    _mMenuOpacity =
                        (float)Animations.Easing(1, 0f, _mTransitionStartTime, _mTransitionDuration, gameTime);
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

            foreach (Button button in _mButtonList)
            {
                button.Draw(spriteBatch);
            }

            // Draw selector triangle
            spriteBatch.Draw(_mSelectorTriangle,
                position: _mSelectorPosition,
                sourceRectangle: null,
                color: Color.White * _mMenuOpacity,
                rotation: 0f,
                origin: new Vector2(0, 11),
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            // Draw menu window
            spriteBatch.StrokedRectangle(_mMenuBoxPosition,
                new Vector2(408, 420),
                Color.White,
                Color.White,
                .5f,
                .20f);
            spriteBatch.DrawString(_mLibSans36,
                _mWindowTitleString,
                new Vector2(_mMenuBoxPosition.X + 20, _mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * _mMenuOpacity);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            _mLibSans36 = content.Load<SpriteFont>("LibSans36");
            _mLibSans20 = content.Load<SpriteFont>("LibSans20");
            _mButtonVerticalCenter = _mLibSans20.MeasureString("Gg").Y / 2;

            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter);
            _mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            _mStoryButton = new Button(_mStoryString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mFreePlayButton = new Button(_mFreePlayString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mBackButton = new Button(_mBackString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mButtonList.Add(_mStoryButton);
            _mButtonList.Add(_mFreePlayButton);
            _mButtonList.Add(_mBackButton);

            _mStoryButton.ButtonReleased += MainMenuManagerScreen.OnStoryButtonReleased;
            _mFreePlayButton.ButtonReleased += MainMenuManagerScreen.OnFreePlayButtonReleased;
            _mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;

            _mStoryButton.ButtonHovering += OnStoryHover;
            _mFreePlayButton.ButtonHovering += OnFreePlayHover;
            _mBackButton.ButtonHovering += OnBackHover;
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
            _mTargetScreen = targetScreen;
            _mTransitionDuration = 350;
            _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            TransitionRunning = true;
        }

        #region Button Hover Handlers

        private void OnStoryHover(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter);
        }

        private void OnFreePlayHover(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 50);
        }

        private void OnBackHover(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 100);
        }
        #endregion
    }
}
