using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Serialization;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Manages the pause menu. This is the screen that is first loaded into the stack screen manager
    /// and loads all the other pause menu screens. Also handles button events which involve switching
    /// between pause screens.
    /// </summary>
    internal sealed class GamePauseManagerScreen : IScreen
    {
        public EScreen Screen { get; private set; } = EScreen.GamePauseManagerScreen;

        public bool Loaded { get; set; }

        private readonly IScreenManager mScreenManager;

        private EScreen mScreenState;

        private readonly Vector2 mScreenResolution;

        // All connecting screens
        private GamePauseScreen mGamePauseScreen;
        private SaveGameScreen mSaveGameScreen;
        private StatisticsScreen mStatisticsScreen;

        // Screen transition variables
        private static string sPressed;
        private int mTransitionState;

        private static bool sPausedAgain;

        private string[] mGameSaveStrings;
        private static bool sSaved = false;

        private readonly Director mDirector;

        /// <summary>
        /// Creates an instance of the GamePauseManagerScreen class
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game.</param>
        /// <param name="screenManager">Stack screen manager of the game.</param>
        /// <param name="director">Director of the game.</param>
        public GamePauseManagerScreen(Vector2 screenResolution,
            IScreenManager screenManager,
            Director director)
        {
            mScreenManager = screenManager;
            mDirector = director;
            mScreenResolution = screenResolution;

            Initialize(screenResolution, director);

            mScreenState = EScreen.GamePauseScreen;

            sPressed = "None";
            mTransitionState = 0;
        }

        /// <summary>
        /// Initialize the game pause screen by creating all the screens
        /// </summary>
        /// <param name="screenResolution"></param>
        /// <param name="director"></param>
        private void Initialize(Vector2 screenResolution, Director director)
        {
            mGamePauseScreen = new GamePauseScreen(screenResolution);
            mSaveGameScreen = new SaveGameScreen(screenResolution);
            mStatisticsScreen = new StatisticsScreen(screenResolution, director);
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            // Add screen to screen manager
            if (mScreenState == EScreen.GamePauseScreen)
            {
                mScreenManager.AddScreen(mGamePauseScreen);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the state of the pause menu and changes the screen that is currently being displayed
        /// by the stack screen manager.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (sPausedAgain)
            {
                mScreenManager.AddScreen(mGamePauseScreen);
                sPausedAgain = false;
            }

            if (sSaved)
            {
                mSaveGameScreen = new SaveGameScreen(mScreenResolution);
                mSaveGameScreen.mMenuOpacity = 1f;
                sPressed = "Save Game";
                Console.WriteLine("Save Screen Updated");
                sSaved = false;
            }
            switch (mScreenState)
            {
                case EScreen.GameScreen:
                    break;
                case EScreen.GamePauseScreen:
                    if (sPressed == "Resume")
                    {
                        mScreenManager.RemoveScreen();
                        mScreenManager.RemoveScreen();
                        sPressed = "None";
                        sPausedAgain = true;
                    }
                    if (sPressed == "Save Game")
                    {
                        SwitchScreen(EScreen.SaveGameScreen, mGamePauseScreen, mSaveGameScreen, gametime);
                    }
                    if (sPressed == "Statistics")
                    {
                        SwitchScreen(EScreen.StatisticsScreen, mGamePauseScreen, mStatisticsScreen, gametime);
                    }
                    if (sPressed == "Main Menu") // TODO: Implement a way to get back to the Main Menu from Pause Menu.
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case EScreen.SaveGameScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.GamePauseScreen, mSaveGameScreen, mGamePauseScreen, gametime);
                    }
                    if (sPressed == "Save Game")
                    {
                        mScreenManager.RemoveScreen();
                        mScreenManager.AddScreen(mSaveGameScreen);
                        // SwitchScreen(EScreen.SaveGameScreen, mSaveGameScreen, mSaveGameScreen, gametime);
                        // SwitchScreen(EScreen.SaveGameScreen, mGamePauseScreen, mSaveGameScreen, gametime);
                        sPressed = "None";
                    }
                    if (sPressed == "Save1")
                    {
                        mGameSaveStrings = XSerializer.GetSaveNames();
                        var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
                        if (mGameSaveStrings.Length >= 1)
                        {
                            path = Environment.ExpandEnvironmentVariables(path);
                            path = path + @"\" + mGameSaveStrings[0];
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        SaveGame();
                    }
                    if (sPressed == "Save2")
                    {
                        mGameSaveStrings = XSerializer.GetSaveNames();
                        var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
                        if (mGameSaveStrings.Length >= 2)
                        {
                            path = Environment.ExpandEnvironmentVariables(path);
                            path = path + @"\" + mGameSaveStrings[1];
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        SaveGame();
                    }
                    if (sPressed == "Save3")
                    {
                        mGameSaveStrings = XSerializer.GetSaveNames();
                        var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
                        if (mGameSaveStrings.Length >= 3)
                        {
                            path = Environment.ExpandEnvironmentVariables(path);
                            path = path + @"\" + mGameSaveStrings[2];
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        SaveGame();
                    }
                    if (sPressed == "Save4")
                    {
                        mGameSaveStrings = XSerializer.GetSaveNames();
                        var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
                        if (mGameSaveStrings.Length >= 4)
                        {
                            path = Environment.ExpandEnvironmentVariables(path);
                            path = path + @"\" + mGameSaveStrings[3];
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        SaveGame();
                    }
                    if (sPressed == "Save5")
                    {
                        mGameSaveStrings = XSerializer.GetSaveNames();
                        var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
                        if (mGameSaveStrings.Length >= 5)
                        {
                            path = Environment.ExpandEnvironmentVariables(path);
                            path = path + @"\" + mGameSaveStrings[4];
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        SaveGame();
                    }

                    break;
                case EScreen.StatisticsScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.GamePauseScreen, mStatisticsScreen, mGamePauseScreen, gametime);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SaveGame()
        {
            mDirector.GetStoryManager.SaveAchievements();
            var saveName = DateTime.Now;
            XSerializer.Save(mDirector.GetStoryManager.Level, saveName.ToString(CultureInfo.CurrentCulture).Replace(':', '_') + ".xml", false);
            Console.WriteLine("Game Saved");
            sSaved = true;
        }


        /// <summary>
        /// Automates the process of removing and adding new screens
        /// that are part of the PauseMenu to the stack screen manager.
        /// </summary>
        /// <param name="targetEScreen"></param>
        /// <param name="originScreen"></param>
        /// <param name="targetScreen"></param>
        /// <param name="gameTime">Used for animations</param>
        private void SwitchScreen(EScreen targetEScreen,
            ITransitionableMenu originScreen,
            ITransitionableMenu targetScreen,
            GameTime gameTime)
        {
            switch (mTransitionState)
            {
                case 0:
                    // start the necessary transitions
                    originScreen.TransitionTo(mScreenState, targetEScreen, gameTime);
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
            // This screen draws nothing.
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            // below this screen is the GameScreen so don't update!
            return false;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            //The frozen GameScreen acts as background so draw it.
            return true;
        }

        #region GamePauseScreen Button Handlers

        /// <summary>
        /// Receives Resume button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        public static void OnResumeButtonReleased(Object sender, EventArgs eventArg)
        {
            sPressed = "Resume";
            GlobalVariables.mGameIsPaused = false;
        }

        /// <summary>
        /// Receives Save Game button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSaveGameButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Save Game";
        }

        /// <summary>
        /// Receives Save1 button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSave1ButtonReleased(object sender, EventArgs eventArgs)
        {
            sPressed = "Save1";
        }

        /// <summary>
        /// Receives Save2 button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSave2ButtonReleased(object sender, EventArgs eventArgs)
        {
            sPressed = "Save2";
        }

        /// <summary>
        /// Receives Save3 button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSave3ButtonReleased(object sender, EventArgs eventArgs)
        {
            sPressed = "Save3";
        }

        /// <summary>
        /// Receives Save4 button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSave4ButtonReleased(object sender, EventArgs eventArgs)
        {
            sPressed = "Save4";
        }

        /// <summary>
        /// Receives Save5 button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSave5ButtonReleased(object sender, EventArgs eventArgs)
        {
            sPressed = "Save5";
        }

        /// <summary>
        /// Receives Statistics button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnStatisticsButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Statistics";
        }

        /// <summary>
        /// Receives Main Menu button released event and exits game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnMainMenuButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Main Menu";
        }

        /// <summary>
        /// Used to go back to the game pause screen.
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