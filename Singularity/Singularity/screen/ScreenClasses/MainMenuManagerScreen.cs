using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class MainMenuManagerScreen : IScreen
    {
        private IScreenManager mScreenManager;
        private static Vector2 sMenuBox;
        private EScreen mScreenState;

        // Fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // All connecting screens
        private IScreen mGameModeSelectScreen;
        private IScreen mLoadSelectScreen;
        private IScreen mAchievementsScreen;
        private IScreen mOptionsScreen;
        private IScreen mSplashScreen;
        private IScreen mMainMenuScreen;

        // Background
        private MenuBackgroundScreen mMenuBackgroundScreen;

        // all text is stored as string variables to allow for easy changes
        private readonly string mPlayString;
        private readonly string mLoadSelectString;
        private readonly string mAchievementsString;
        private readonly string mOptionsString;
        private readonly string mQuitString;
        private readonly string mTitle;

        // Buttons on the main menu
        private Button mPlayButton;
        private Button mLoadButton;
        private Button mAchievementsButton;
        private Button mOptionsButton;
        private Button mQuitButton;

        /// <summary>
        /// Shows the main menu screen with 5 options:
        /// New Game, Load Game, Achievements, Options, and Quit Game.
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game</param>
        /// <param name="screenManager">Stack screen manager of the game</param>
        /// <param name="showSplash">Defines if the splash screen should be shown</param>
        public MainMenuManagerScreen(Vector2 screenResolution, IScreenManager screenManager, bool showSplash)
        {
            SetResolution(screenResolution);
            mScreenManager = screenManager;

            mGameModeSelectScreen = new GameModeSelectScreen();
            mLoadSelectScreen = new LoadSelectScreen();
            mAchievementsScreen = new AchievementsScreen();
            mOptionsScreen = new OptionsScreen(screenResolution, screenManager);
            mMenuBackgroundScreen = new MenuBackgroundScreen(screenResolution);
            mSplashScreen = new SplashScreen(screenResolution);
            mMainMenuScreen = new MainMenuScreen(screenResolution);

            mScreenState = showSplash ? EScreen.SplashScreen : EScreen.MainMenuScreen;
        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

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
            // TODO this is temporary. This will be replaced with event handlers from the buttons
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
            return false;
        }
    }
}
