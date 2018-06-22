using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class MainMenuManagerScreen : IScreen
    {
        public bool Loaded { get; set; }

        /// <inheritdoc cref="IScreen"/>
        /// <summary>
        /// Manages the main menu. This is the screen that is first loaded into the stack screen manager
        /// and loads all the other main menu screens. Also handles button events which involve switching
        /// between menu screens.
        /// </summary>
        private readonly IScreenManager mMScreenManager;
        private EScreen mMScreenState;

        // All connecting screens
        private ITransitionableMenu mMGameModeSelectScreen;
        private ITransitionableMenu mMLoadSelectScreen;
        private ITransitionableMenu mMAchievementsScreen;
        private ITransitionableMenu mMOptionsScreen;
        private ITransitionableMenu mMSplashScreen;
        private ITransitionableMenu mMMainMenuScreen;
        private ITransitionableMenu mMLoadingScreen;

        // Background
        private MenuBackgroundScreen mMMenuBackgroundScreen;

        // Screen transition variables
        private static string sSPressed;
        private int mMTransitionState;

        // The game itself (to allow for quitting)
        private readonly Game1 mMGame;

        // viewport resolution changes
        private static Vector2 sSViewportResolution;
        private static bool sSResolutionChanged;
        private ContentManager mMContent;

        /// <summary>
        /// Creates an instance of the MainMenuManagerScreen class
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game.</param>
        /// <param name="screenManager">Stack screen manager of the game.</param>
        /// <param name="showSplash">Defines if the splash screen should be shown
        /// (used when going back to main menu from within the game where showing the
        /// splash screen would not be necessary).</param>
        /// <param name="game">Used to pass on to the options screen to change game settings</param>
        public MainMenuManagerScreen(Vector2 screenResolution, IScreenManager screenManager, bool showSplash, Game1 game)
        {
            mMScreenManager = screenManager;
            mMGame = game;

            Initialize(screenResolution, false, game);

            mMScreenState = showSplash ? EScreen.SplashScreen : EScreen.MainMenuScreen;

            sSPressed = "None";
            sSResolutionChanged = false;
            mMTransitionState = 0;
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            LoadScreenContents(content);
            mMContent = content;

            // Add screen to screen manager
            mMScreenManager.AddScreen(mMMenuBackgroundScreen);

            if (mMScreenState == EScreen.SplashScreen)
            {
                mMScreenManager.AddScreen(mMSplashScreen);
            }
            else if (mMScreenState == EScreen.MainMenuScreen)
            {
                mMScreenManager.AddScreen(mMMainMenuScreen);
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
            if (sSResolutionChanged)
            {
                Initialize(sSViewportResolution, sSResolutionChanged, mMGame);
                LoadScreenContents(mMContent);
                mMScreenManager.RemoveScreen();
                mMScreenManager.RemoveScreen();
                mMMenuBackgroundScreen.TransitionTo(EScreen.OptionsScreen, EScreen.OptionsScreen, gametime);
                mMScreenManager.AddScreen(mMMenuBackgroundScreen);
                mMScreenManager.AddScreen(mMOptionsScreen);
                sSResolutionChanged = false;
            }
            switch (mMScreenState)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
                    if (sSPressed == "Free Play")
                    {
                        mMScreenManager.RemoveScreen();
                        mMScreenManager.RemoveScreen();
                        mMScreenManager.AddScreen(mMLoadingScreen);
                        mMScreenState = EScreen.LoadingScreen;
                    }

                    if (sSPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mMGameModeSelectScreen, mMMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.GameScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.LoadingScreen:
                    mMGame.mMGameScreen.LoadContent(mMContent);
                    mMScreenManager.RemoveScreen();
                    mMScreenState = EScreen.GameScreen;
                    break;
                case EScreen.MainMenuScreen:
                    if (sSPressed == "Play")
                    {
                        SwitchScreen(EScreen.GameModeSelectScreen, mMMainMenuScreen, mMGameModeSelectScreen, gametime);
                    }

                    if (sSPressed == "Load")
                    {
                        SwitchScreen(EScreen.LoadSelectScreen, mMMainMenuScreen, mMLoadSelectScreen, gametime);
                    }

                    if (sSPressed == "Options")
                    {
                        SwitchScreen(EScreen.OptionsScreen, mMMainMenuScreen, mMOptionsScreen, gametime);
                    }

                    if (sSPressed == "Achievments")
                    {
                        SwitchScreen(EScreen.AchievementsScreen, mMMainMenuScreen, mMAchievementsScreen, gametime);
                    }

                    if (sSPressed == "Quit")
                    {
                        mMGame.Exit();
                    }
                    break;
                case EScreen.OptionsScreen:
                    if (sSPressed == "Back")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mMOptionsScreen, mMMainMenuScreen, gametime);
                    }
                    break;
                case EScreen.SplashScreen:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0
                        || Mouse.GetState().LeftButton == ButtonState.Pressed
                        || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        sSPressed = "Pressed";
                    }

                    if (sSPressed == "Pressed")
                    {
                        SwitchScreen(EScreen.MainMenuScreen, mMSplashScreen, mMMainMenuScreen, gametime);
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
            switch (mMTransitionState)
            {
                case (0):
                    // start the necessary transitions
                    originScreen.TransitionTo(mMScreenState, targetEScreen, gameTime);
                    mMMenuBackgroundScreen.TransitionTo(mMScreenState, targetEScreen, gameTime);
                    mMTransitionState = 1;
                    break;

                case (1):
                    // Wait for the origin screen to finish transitioning out
                    if (!originScreen.TransitionRunning)
                    {
                        // once it is done transitioning out, remove it and add the target screen
                        mMScreenManager.RemoveScreen();
                        mMScreenManager.AddScreen(targetScreen);
                        // then start transitioning the target screen
                        targetScreen.TransitionTo(mMScreenState, targetEScreen, gameTime);
                        mMTransitionState = 2;
                    }
                    break;
                case (2):
                    // now wait for the target screen to finish transitioning in
                    if (!targetScreen.TransitionRunning)
                    {
                        // once it is done transitioning in, change the states of everyone to reflect the new state
                        mMScreenState = targetEScreen;
                        sSPressed = "None";
                        mMTransitionState = 0;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            /* old code
            var origin = new EScreen();
            // make sure that this isn't a switch to the gamescreen, represented by this being the screen that is passed
            if (targetScreen != mLoadingScreen)
            {
                // check if the transition is running. If yes, then don't do anything
                if (!mMenuBackgroundScreen.TransitionRunning)
                {
                    origin = mScreenState;
                    // if the transition hasn't started, start it
                    if (mMenuBackgroundScreen.CurrentScreen != eScreen)
                    {
                        mCurrentScreen.TransitionTo(origin, eScreen, gameTime);

                        mMenuBackgroundScreen.TransitionTo(origin, eScreen, gameTime);
                    }
                    // if it is finish, "reset" the transition states.
                    if (!mCurrentScreen.TransitionRunning)
                    {
                        mCurrentScreen = targetScreen;
                        mScreenState = eScreen;
                        sPressed = "None";
                    }
                }
            }
            // special switch case for loading screen
            else
            {
                // remove menu background if it is a switch to gamescreen
                mScreenManager.RemoveScreen();
            }
            // check to see if the transition out of the current screen is done but the transition in hasn't been completed yet
            if (!mCurrentScreen.TransitionRunning && mCurrentScreen == targetScreen)
            {
                // once the transition is finished, remove the screen
                mScreenManager.RemoveScreen();
                // add the target screen and replace the current screen with that screen
                mScreenManager.AddScreen(targetScreen);
                targetScreen.TransitionTo(origin, eScreen, gameTime);
                mCurrentScreen = targetScreen;

            }
            */
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
            // below this screen is the game so it shouldn't update the game
            return mMScreenState == EScreen.LoadingScreen || mMScreenState == EScreen.GameScreen;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            return mMScreenState == EScreen.LoadingScreen || mMScreenState == EScreen.GameScreen;
        }

        public static void SetResolution(Vector2 viewportResolution)
        {
            sSResolutionChanged = true;
            sSViewportResolution = viewportResolution;
        }

        /// <summary>
        /// Initialize the main menu screen by creating all the screens
        /// </summary>
        /// <param name="screenResolution"></param>
        /// <param name="screenResolutionChanged"></param>
        /// <param name="game"></param>
        private void Initialize(Vector2 screenResolution, bool screenResolutionChanged, Game1 game)
        {
            mMGameModeSelectScreen = new GameModeSelectScreen(screenResolution);
            mMLoadSelectScreen = new LoadSelectScreen();
            mMAchievementsScreen = new AchievementsScreen();
            mMOptionsScreen = new OptionsScreen(screenResolution, screenResolutionChanged, game);
            mMMenuBackgroundScreen = new MenuBackgroundScreen(screenResolution);
            mMSplashScreen = new SplashScreen(screenResolution);
            mMMainMenuScreen = new MainMenuScreen(screenResolution);
            mMLoadingScreen = new LoadingScreen(screenResolution);
        }

        /// <summary>
        /// Loads the contents of the main menu screens
        /// </summary>
        /// <param name="content">ContentManager that contains the content</param>
        private void LoadScreenContents(ContentManager content)
        {
            // Load content for all the other menu screens
            mMMenuBackgroundScreen.LoadContent(content);
            mMSplashScreen.LoadContent(content);
            mMMainMenuScreen.LoadContent(content);
            mMGameModeSelectScreen.LoadContent(content);
            mMLoadSelectScreen.LoadContent(content);
            mMAchievementsScreen.LoadContent(content);
            mMOptionsScreen.LoadContent(content);
            mMLoadingScreen.LoadContent(content);
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
            sSPressed = "Play";
        }

        /// <summary>
        /// Receives Load button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnLoadButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Load";
        }

        /// <summary>
        /// Receives Options button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnOptionsButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Options";

        }

        /// <summary>
        /// Receives Achievements button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnAchievementsButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Achievements";
        }

        /// <summary>
        /// Receives Quit button released event and
        /// exits game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnQuitButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Quit";
        }

        /// <summary>
        /// Used to create a new story mode game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnStoryButtonReleased(Object sender, EventArgs eventArgs)
        {
            // TODO: implement start game with story
            throw new NotImplementedException("No story yet unfortunately");


        }

        /// <summary>
        /// Used to create a new skirmish game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnFreePlayButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Free Play";
        }

        /// <summary>
        /// Used to go back to the main main menu screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnBackButtonReleased(Object sender, EventArgs eventArgs)
        {
            sSPressed = "Back";
        }
        #endregion


    }
}
