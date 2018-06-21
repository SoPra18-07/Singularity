using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Microsoft.Xna.Framework.Content;


namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="ITransitionableMenu"/>
    /// <summary>
    /// Shows the main menu screen with 5 options:
    /// New Game, Load Game, Achievements, Options, and Quit Game.
    /// </summary>
    internal sealed class MainMenuScreen : ITransitionableMenu
    {
        public bool Loaded { get; set; }

        private EScreen _mScreenState;

        // Fonts
        private SpriteFont _mLibSans36;
        private SpriteFont _mLibSans20;

        // all text is stored as string variables to allow for easy changes
        private readonly string _mPlayString;
        private readonly string _mLoadSelectString;
        private readonly string _mAchievementsString;
        private readonly string _mOptionsString;
        private readonly string _mQuitString;
        private readonly string _mTitle;

        // layout
        private readonly float _mButtonLeftPadding;
        private readonly float _mButtonTopPadding;
        private float _mButtonVerticalCenter;
        private readonly Vector2 _mScreenResolution;
        private Vector2 _mMenuBoxPosition;

        // Buttons on the main menu
        private Button _mPlayButton;
        private Button _mLoadButton;
        private Button _mAchievementsButton;
        private Button _mOptionsButton;
        private Button _mQuitButton;
        private readonly List<Button> _mButtonList;

        // Selector Triangle Variables
        private Texture2D _mSelectorTriangle;
        private Vector2 _mSelectorPosition;

        // Transition fields and properties
        private float _mMenuOpacity;
        private float _mWindowOpacity;
        private Vector2 _mMenuBoxSize;
        private double _mTransitionStartTime;
        private double _mTransitionDuration;
        private EScreen _mTargetScreen;
        private EScreen _mOriginScreen;
        public bool TransitionRunning { get; private set; }

        /// <summary>
        /// Used to construct a new instance of the main menu screen
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game</param>
        public MainMenuScreen(Vector2 screenResolution)
        {
            _mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 204, screenResolution.Y / 4);
            _mScreenResolution = screenResolution;
            _mMenuBoxSize = new Vector2(408, 420);

            _mButtonLeftPadding = _mMenuBoxPosition.X + 60;
            _mButtonTopPadding = _mMenuBoxPosition.Y + 90;
            
            _mPlayString = "New Game";
            _mLoadSelectString = "Load Game";
            _mAchievementsString = "Achievements";
            _mOptionsString = "Options";
            _mQuitString = "Quit";
            _mTitle = "Singularity";

            _mButtonList = new List<Button>(5);
            _mMenuOpacity = 1;
        }

        

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            _mLibSans36 = content.Load<SpriteFont>("LibSans36");
            _mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // Selector Triangle Positioning
            _mButtonVerticalCenter = _mLibSans20.MeasureString("Gg").Y / 2;
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter);


            _mPlayButton = new Button(_mPlayString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mLoadButton = new Button(_mLoadSelectString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 50), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mOptionsButton = new Button(_mOptionsString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 100), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mAchievementsButton = new Button(_mAchievementsString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 150), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mQuitButton = new Button(_mQuitString, _mLibSans20, new Vector2(_mButtonLeftPadding, _mButtonTopPadding + 200), new Color(new Vector3(.9137f, .9058f, .8314f)));
            _mButtonList.Add(_mPlayButton);
            _mButtonList.Add(_mLoadButton);
            _mButtonList.Add(_mOptionsButton);
            _mButtonList.Add(_mAchievementsButton);
            _mButtonList.Add(_mQuitButton);

            _mSelectorTriangle = content.Load<Texture2D>("SelectorTriangle");

            _mPlayButton.ButtonReleased += MainMenuManagerScreen.OnPlayButtonReleased;
            _mLoadButton.ButtonReleased += MainMenuManagerScreen.OnLoadButtonReleased;
            _mOptionsButton.ButtonReleased += MainMenuManagerScreen.OnOptionsButtonReleased;
            _mAchievementsButton.ButtonReleased += MainMenuManagerScreen.OnAchievementsButtonReleased;
            _mQuitButton.ButtonReleased += MainMenuManagerScreen.OnQuitButtonReleased;

            _mPlayButton.ButtonHovering += OnPlayHovering;
            _mLoadButton.ButtonHovering += OnLoadHovering;
            _mOptionsButton.ButtonHovering += OnOptionsHovering;
            _mAchievementsButton.ButtonHovering += OnAchievementsHovering;
            _mQuitButton.ButtonHovering += OnQuitHovering;

        }


        /// <summary>
        /// Updates the buttons within the MainMenuScreen.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {

            foreach (Button button in _mButtonList)
            {
                button.Update(gametime);
            }

            if (TransitionRunning)
            {
                Transition(gametime);
            }

            foreach (Button button in _mButtonList)
            {
                button.Opacity = _mMenuOpacity;
            }
        }

        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            _mOriginScreen = originScreen;
            _mTargetScreen = targetScreen;
            TransitionRunning = true;
            _mTransitionStartTime = gameTime.TotalGameTime.TotalMilliseconds;

            if (originScreen != EScreen.SplashScreen)
            {
                _mWindowOpacity = 1f;
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
                    _mTransitionDuration = 350d;
                    _mMenuOpacity = 0f;
                    _mMenuBoxSize = new Vector2(408, 420);
                    _mMenuBoxPosition = new Vector2(_mScreenResolution.X / 2 - 204, _mScreenResolution.Y / 4);
                    _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter);
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
            switch (_mTargetScreen)
            {
                case EScreen.AchievementsScreen:
                    
                    break;
                case EScreen.GameModeSelectScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 0;
                    }

                    _mMenuOpacity =
                        (float)Animations.Easing(1, 0, _mTransitionStartTime, _mTransitionDuration, gameTime);
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    if (_mOriginScreen == EScreen.SplashScreen)
                    {
                        if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                        {
                            TransitionRunning = false;
                            _mMenuOpacity = 1f;
                        }

                        var opacity =
                            (float) Animations.Easing(0, 1f, _mTransitionStartTime, _mTransitionDuration, gameTime);
                        Debug.WriteLine(_mMenuOpacity);
                        _mMenuOpacity = opacity;
                        _mWindowOpacity = opacity;
                    }
                    else
                    {
                        if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                        {
                            TransitionRunning = false;
                            _mMenuOpacity = 1f;
                        }

                        _mMenuOpacity =
                            (float)Animations.Easing(0, 1f, _mTransitionStartTime, _mTransitionDuration, gameTime);
                    }

                    break;
                case EScreen.OptionsScreen:
                    if (gameTime.TotalGameTime.TotalMilliseconds >= _mTransitionStartTime + _mTransitionDuration)
                    {
                        TransitionRunning = false;
                        _mMenuOpacity = 0f;
                        _mMenuBoxSize = new Vector2(612, 420);
                        _mMenuBoxPosition = new Vector2(_mScreenResolution.X / 2 - 306, _mScreenResolution.Y / 4);
                    }

                    var width = (float)Animations.Easing(408,
                        612,
                        _mTransitionStartTime,
                        _mTransitionDuration,
                        gameTime);

                    _mMenuBoxPosition = new Vector2(_mScreenResolution.X / 2 - (int)Math.Floor(width / 2), _mScreenResolution.Y / 4);
                    _mMenuBoxSize = new Vector2(width, 420);

                    _mMenuOpacity = (float)Animations.Easing(1f, 0f, _mTransitionStartTime, _mTransitionDuration, gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_mTargetScreen), _mTargetScreen, null);
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

            // draw selector triangle
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
                _mMenuBoxSize,
                Color.White * _mWindowOpacity,
                Color.White * _mWindowOpacity,
                .5f,
                .20f);
            spriteBatch.DrawString(_mLibSans36,
                _mTitle,
                new Vector2(_mMenuBoxPosition.X + 20, _mMenuBoxPosition.Y + 10),
                new Color(new Vector3(.9137f, .9058f, .8314f)) * _mMenuOpacity);

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
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter);
        }

        private void OnLoadHovering(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 50);
        }

        private void OnOptionsHovering(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 100);
        }

        private void OnAchievementsHovering(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 150);
        }

        private void OnQuitHovering(Object sender, EventArgs eventArgs)
        {
            _mSelectorPosition = new Vector2(_mMenuBoxPosition.X + 22, _mButtonTopPadding + _mButtonVerticalCenter + 200);
        }

        #endregion
    }
}
