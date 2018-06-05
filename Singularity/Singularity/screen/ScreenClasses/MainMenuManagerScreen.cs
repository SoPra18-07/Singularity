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
        /// Handles everything thats going on explicitly in the game.
        /// E.g. game objects, the map, camera. etc.
        /// </summary>
        private readonly IScreenManager mScreenManager;
        private EScreen mScreenState;

        // All connecting screens
        private readonly IScreen mGameModeSelectScreen;
        private readonly IScreen mLoadSelectScreen;
        private readonly IScreen mAchievementsScreen;
        private readonly IScreen mOptionsScreen;
        private readonly IScreen mSplashScreen;
        private readonly IScreen mMainMenuScreen;

        // Background
        private readonly MenuBackgroundScreen mMenuBackgroundScreen;

        // Which button pressed
        private static string sPressed;

        /// <summary>
        /// Manages the main menu. This is the screen that is first loaded into the stack screen manager
        /// and loads all the other main menu screens. Also handles button events which involve switching
        /// between menu screens.
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game.</param>
        /// <param name="screenManager">Stack screen manager of the game.</param>
        /// <param name="showSplash">Defines if the splash screen should be shown
        /// (used when going back to main menu from within the game where showing the
        /// splash screen would not be necessary).</param>
        public MainMenuManagerScreen(Vector2 screenResolution, IScreenManager screenManager, bool showSplash)
        {
            mScreenManager = screenManager;

            mGameModeSelectScreen = new GameModeSelectScreen();
            mLoadSelectScreen = new LoadSelectScreen();
            mAchievementsScreen = new AchievementsScreen();
            mOptionsScreen = new OptionsScreen(screenResolution, screenManager);
            mMenuBackgroundScreen = new MenuBackgroundScreen(screenResolution);
            mSplashScreen = new SplashScreen(screenResolution);
            mMainMenuScreen = new MainMenuScreen(screenResolution);

            mScreenState = showSplash ? EScreen.SplashScreen : EScreen.MainMenuScreen;

            sPressed = "None";
        }

        public void LoadContent(ContentManager content)
        {
            // Load content for all the other menu screens
            mMenuBackgroundScreen.LoadContent(content);
            mSplashScreen.LoadContent(content);
            mMainMenuScreen.LoadContent(content);
            mGameModeSelectScreen.LoadContent(content);
            mLoadSelectScreen.LoadContent(content);
            mAchievementsScreen.LoadContent(content);
            mOptionsScreen.LoadContent(content);

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

        public void Update(GameTime gametime)
        {
            // TODO this is temporary. This will be replaced with event handlers from the buttons or input
            switch (mScreenState)
            {
                case EScreen.AchievementsScreen:
                    break;
                case EScreen.GameModeSelectScreen:
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
                    break;
                case EScreen.OptionsScreen:
                    break;
                case EScreen.SplashScreen:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0
                        || Mouse.GetState().LeftButton == ButtonState.Pressed
                        || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        // TODO animate screen
                        mMenuBackgroundScreen.SetScreen(EScreen.MainMenuScreen);
                        mScreenManager.RemoveScreen();
                        mScreenManager.AddScreen(mMainMenuScreen);
                        mScreenState = EScreen.MainMenuScreen;
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
        /// <param name="eScreen"></param>
        /// <param name="iScreen"></param>
        private void SwitchScreen(EScreen eScreen, IScreen iScreen)
        {
            mMenuBackgroundScreen.SetScreen(eScreen);
            mScreenManager.RemoveScreen();
            mScreenManager.AddScreen(iScreen);
            mScreenState = eScreen;
            sPressed = "None";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // This screen draws nothing
        }

        public bool UpdateLower()
        {
            // below this screen is the game so it shouldn't update the game
            return false;
        }

        public bool DrawLower()
        {
            return mScreenState == EScreen.GameScreen? true : false;
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
            //TODO: handle when quit button is clicked
            throw new DivideByZeroException("Game Quit not yet Implemented");
            
        }

        #endregion


    }
}
