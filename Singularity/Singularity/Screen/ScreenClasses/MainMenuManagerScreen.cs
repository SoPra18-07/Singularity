using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class MainMenuManagerScreen : IScreen
    {
        /// <inheritdoc cref="IScreen"/>
        /// <summary>
        /// Manages the main menu. This is the screen that is first loaded into the stack screen manager
        /// and loads all the other main menu screens. Also handles button events which involve switching
        /// between menu screens.
        /// </summary>
        private readonly IScreenManager mScreenManager;
        private EScreen mScreenState;

        // All connecting screens
        private IScreen mGameModeSelectScreen;
        private IScreen mLoadSelectScreen;
        private IScreen mAchievementsScreen;
        private IScreen mOptionsScreen;
        private IScreen mSplashScreen;
        private IScreen mMainMenuScreen;
        private IScreen mLoadingScreen;

        // Background
        private MenuBackgroundScreen mMenuBackgroundScreen;

        // Which button pressed
        private static string sPressed;

        // The game itself (to allow for quitting)
        private readonly Game1 mGame;
        
        // viewport resolution changes
        private static Vector2 sViewportResolution;
        private static bool sResolutionChanged;
        private ContentManager mContent;

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
            mScreenManager = screenManager;
            mGame = game;
            
            Initialize(screenResolution, game);
            
            mScreenState = showSplash ? EScreen.SplashScreen : EScreen.MainMenuScreen;

            sPressed = "None";
            sResolutionChanged = false;


        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            LoadScreenContents(content);
            mContent = content;

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
                Initialize(sViewportResolution, mGame);
                LoadScreenContents(mContent);
                mScreenManager.RemoveScreen();
                mScreenManager.RemoveScreen();
                mMenuBackgroundScreen.SetScreen(EScreen.OptionsScreen);
                mScreenManager.AddScreen(mMenuBackgroundScreen);
                mScreenManager.AddScreen(mOptionsScreen);
                sResolutionChanged = false;
            }
            switch (mScreenState)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
                    if (sPressed == "Free Play")
                    {
                        SwitchScreen(EScreen.LoadingScreen, this); // Hack to pass something to switchscreen without a nullable type
                        mScreenManager.AddScreen(mLoadingScreen);
                        mGame.mGameScreen.LoadContent(mContent);
                        SwitchScreen(EScreen.GameScreen, this);
                    }
                    break;
                case EScreen.GameScreen:
                    
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    if (sPressed == "Play")
                    {
                        SwitchScreen(EScreen.GameModeSelectScreen, mGameModeSelectScreen);
                    }

                    if (sPressed == "Load")
                    {
                        SwitchScreen(EScreen.LoadSelectScreen, mLoadSelectScreen);
                    }

                    if (sPressed == "Options")
                    {
                        SwitchScreen(EScreen.OptionsScreen, mOptionsScreen);
                    }

                    if (sPressed == "Achievments")
                    {
                        SwitchScreen(EScreen.AchievementsScreen, mAchievementsScreen);
                    }

                    if (sPressed == "Quit")
                    {
                        mGame.Exit();
                    }
                    break;
                case EScreen.OptionsScreen:
                    break;
                case EScreen.SplashScreen:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0
                        || Mouse.GetState().LeftButton == ButtonState.Pressed
                        || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        // TODO animate screen
                        SwitchScreen(EScreen.MainMenuScreen, mMainMenuScreen);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (sPressed == "Back")
            {
                SwitchScreen(EScreen.MainMenuScreen, mMainMenuScreen);
            }
        }

        /// <summary>
        /// Automates the process of removing and adding new screens
        /// that are part of the MainMenu to the stack manager.
        /// </summary>
        /// <param name="eScreen"></param>
        /// <param name="iScreen"></param>
        private void SwitchScreen(EScreen eScreen, IScreen iScreen)
        {
            // remove current top screen
            mScreenManager.RemoveScreen();
            if (iScreen != this)
            {
                mScreenManager.AddScreen(iScreen);
                mMenuBackgroundScreen.SetScreen(eScreen);
            }
            else
            {
                // remove menu background
                mScreenManager.RemoveScreen();
            }
            mScreenState = eScreen;
            sPressed = "None";
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
            return false;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            return mScreenState == EScreen.GameScreen? true : false;
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
        /// <param name="game"></param>
        private void Initialize(Vector2 screenResolution, Game1 game)
        {
            mGameModeSelectScreen = new GameModeSelectScreen(screenResolution);
            mLoadSelectScreen = new LoadSelectScreen();
            mAchievementsScreen = new AchievementsScreen();
            mOptionsScreen = new OptionsScreen(screenResolution, game);
            mMenuBackgroundScreen = new MenuBackgroundScreen(screenResolution);
            mSplashScreen = new SplashScreen(screenResolution);
            mMainMenuScreen = new MainMenuScreen(screenResolution);
            mLoadingScreen = new LoadingScreen(screenResolution);
        }

        /// <summary>
        /// Loads the contents of the main menu screens
        /// </summary>
        /// <param name="content">ContentManager that contains the content</param>
        private void LoadScreenContents(ContentManager content)
        {
            // Load content for all the other menu screens
            mMenuBackgroundScreen.LoadContent(content);
            mSplashScreen.LoadContent(content);
            mMainMenuScreen.LoadContent(content);
            mGameModeSelectScreen.LoadContent(content);
            mLoadSelectScreen.LoadContent(content);
            mAchievementsScreen.LoadContent(content);
            mOptionsScreen.LoadContent(content);
            mLoadingScreen.LoadContent(content);
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
            sPressed = "Free Play";
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
