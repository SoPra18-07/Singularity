using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Manages the main menu. This is the screen that is second loaded into the stack screen manager
    /// and loads all the other main menu screens. Also handles button events which involve switching
    /// between menu screens.
    /// </summary>
    internal sealed class MainMenuManagerScreen : IScreen
    {
        public EScreen Screen { get; private set; } = EScreen.MainMenuManagerScreen;

        public bool Loaded { get; set; }

        private readonly IScreenManager mScreenManager;
        private EScreen mScreenState;

        private Director mDirector;

        // All connecting screens
        private ITransitionableMenu mGameModeSelectScreen;
        private ITransitionableMenu mLoadSelectScreen;
        private ITransitionableMenu mAchievementsScreen;
        private ITransitionableMenu mOptionsScreen;
        private ITransitionableMenu mSplashScreen;
        private ITransitionableMenu mMainMenuScreen;

        // Background
        private MenuBackgroundScreen mMenuBackgroundScreen;
        
        // Screen transition variables
        private static string sPressed;
        private int mTransitionState;

        // The game itself (to allow for quitting)
        private readonly Game1 mGame;

        // viewport resolution changes
        private static Vector2 sViewportResolution;
        private static bool sResolutionChanged;

        /// <summary>
        /// Creates an instance of the MainMenuManagerScreen class
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game.</param>
        /// <param name="screenManager">Stack screen manager of the game.</param>
        /// <param name="showSplash">Defines if the splash screen should be shown
        /// (used when going back to main menu from within the game where showing the
        /// splash screen would not be necessary).</param>
        /// <param name="game">Used to pass on to the options screen to change game settings</param>
        public MainMenuManagerScreen(Vector2 screenResolution, IScreenManager screenManager, ref Director director, bool showSplash, Game1 game)
        {
            mScreenManager = screenManager;
            mGame = game;

            mDirector = director;

            Initialize(screenResolution, false, game);

            mScreenState = showSplash ? EScreen.SplashScreen : EScreen.MainMenuScreen;

            sPressed = "None";
            sResolutionChanged = false;
            mTransitionState = 0;
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {

            // Add screen to screen manager
            mScreenManager.AddScreen(mMenuBackgroundScreen);

            if (mScreenState == EScreen.SplashScreen)
            {
                mScreenManager.AddScreen(mSplashScreen);
            }
            else if (mScreenState == EScreen.MainMenuScreen)
            {
                mScreenManager.AddScreen(mMainMenuScreen);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }


        }

        /// <summary>
        /// Updates the state of the main menu and changes the screen that is currently being displayed
        /// by the stack screen manager
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (sResolutionChanged)
            {
                Initialize(sViewportResolution, sResolutionChanged, mGame);
                // LoadScreenContents(mContent);
                mScreenManager.RemoveScreen();
                mScreenManager.RemoveScreen();
                mMenuBackgroundScreen.TransitionTo(EScreen.OptionsScreen, EScreen.OptionsScreen, gametime);
                mScreenManager.AddScreen(mMenuBackgroundScreen);
                mScreenManager.AddScreen(mOptionsScreen);
                sResolutionChanged = false;
            }
            switch (mScreenState)
            {
                case EScreen.AchievementsScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mAchievementsScreen, mMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.GameModeSelectScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mGameModeSelectScreen, mMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.GameScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    if (sPressed == "Load1")
                    {
                        break;
                    }
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mLoadSelectScreen, mMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.MainMenuScreen:
                    if (sPressed == "Play")
                    {
                        SwitchScreen(EScreen.GameModeSelectScreen, mMainMenuScreen, mGameModeSelectScreen, gametime);
                    }

                    if (sPressed == "Load")
                    {
                        SwitchScreen(EScreen.LoadSelectScreen, mMainMenuScreen, mLoadSelectScreen, gametime);
                    }

                    if (sPressed == "Options")
                    {
                        SwitchScreen(EScreen.OptionsScreen, mMainMenuScreen, mOptionsScreen, gametime);
                    }

                    if (sPressed == "Achievements")
                    {
                        SwitchScreen(EScreen.AchievementsScreen, mMainMenuScreen, mAchievementsScreen, gametime);
                    }

                    if (sPressed == "Quit")
                    {
                        mGame.Exit();
                    }
                    break;
                case EScreen.OptionsScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mOptionsScreen, mMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.SplashScreen:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0
                        || Mouse.GetState().LeftButton == ButtonState.Pressed
                        || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        sPressed = "Pressed";
                    }

                    if (sPressed == "Pressed")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mSplashScreen, mMainMenuScreen, gametime);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        /// <summary>
        /// Automates the process of removing and adding new screens
        /// that are part of the MainMenu to the stack manager.
        /// </summary>
        /// <param name="targetEScreen"></param>
        /// <param name="originScreen"></param>
        /// <param name="targetScreen"></param>
        /// <param name="gameTime">Used for animations</param>
        private void SwitchScreen(EScreen targetEScreen, ITransitionableMenu originScreen, ITransitionableMenu targetScreen, GameTime gameTime)
        {
            switch (mTransitionState)
            {
                case 0:
                    // start the necessary transitions
                    originScreen.TransitionTo(mScreenState, targetEScreen, gameTime);
                    mMenuBackgroundScreen.TransitionTo(mScreenState, targetEScreen, gameTime);
                    mTransitionState = 1;
                    break;

                case 1:
                    // Wait for the origin screen to finish transitioning out
                    if (!originScreen.TransitionRunning)
                    {
                        // once it is done transitioning out, remove it and add the target screen
                        mScreenManager.RemoveScreen();
                        mScreenManager.AddScreen(targetScreen);
                        // then start transitioning the target screen
                        targetScreen.TransitionTo(mScreenState, targetEScreen, gameTime);
                        mTransitionState = 2;
                    }
                    break;
                case 2:
                    // now wait for the target screen to finish transitioning in
                    if (!targetScreen.TransitionRunning)
                    {
                        // once it is done transitioning in, change the states of everyone to reflect the new state
                        mScreenState = targetEScreen;
                        sPressed = "None";
                        mTransitionState = 0;
                    }
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
            // This screen draws nothing
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            // below this screen is the LoadGameManagerScreen so update!
            return true;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            //The loadgamemanagerscreen doesnt have to be drawn.
            return false;
        }

        public static void SetResolution(Vector2 viewportResolution)
        {
            sResolutionChanged = true;
            sViewportResolution = viewportResolution;
        }

        /// <summary>
        /// Initialize the main menu screen by creating all the screens
        /// </summary>
        /// <param name="screenResolution"></param>
        /// <param name="screenResolutionChanged"></param>
        /// <param name="game"></param>
        private void Initialize(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            mGameModeSelectScreen = new GameModeSelectScreen(screenResolution);
            mLoadSelectScreen = new LoadSelectScreen(screenResolution);
            mAchievementsScreen = new AchievementsScreen(screenResolution);

            mOptionsScreen = new OptionsScreen(screenResolution, screenResolutionChanged, game, ref mDirector);
            mMenuBackgroundScreen = new MenuBackgroundScreen(screenResolution);
            mSplashScreen = new SplashScreen(screenResolution);
            mMainMenuScreen = new MainMenuScreen(screenResolution);
        }


        #region MainMenuScreen Button Handlers

        /// <summary>
        /// Receives Play button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnPlayButtonReleased(Object sender, EventArgs eventArg)
        {
            sPressed = "Play";
        }

        /// <summary>
        /// Receives Load button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnLoadButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Load";
        }

        /// <summary>
        /// Receives Options button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnOptionsButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Options";

        }

        /// <summary>
        /// Receives Achievements button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnAchievementsButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Achievements";
        }

        /// <summary>
        /// Receives Quit button released event and
        /// exits game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnQuitButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Quit";
        }

        /// <summary>
        /// Used to go back to the main main menu screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnBackButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Back";
        }
        #endregion


    }
}
