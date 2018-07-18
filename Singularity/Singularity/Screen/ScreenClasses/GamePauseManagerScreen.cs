﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

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

        // All connecting screens
        private GamePauseScreen mGamePauseScreen;
        private SaveGameScreen mSaveGameScreen;
        private StatisticsScreen mStatisticsScreen;

        // Screen transition variables
        private static string sPressed;
        private int mTransitionState;

        private static bool sPausedAgain;

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

                    if (sPressed == "Main Menu")
                    {
                        for (var i = 0; i < mScreenManager.GetScreenCount() - 1; i++)
                        {
                            mScreenManager.RemoveScreen();
                        }
                    }
                    break;
                case EScreen.SaveGameScreen:
                    if (sPressed == "Back")
                    {
                        SwitchScreen(EScreen.GamePauseScreen, mSaveGameScreen, mGamePauseScreen, gametime);
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
        }

        /// <summary>
        /// Receives Statistics button released event and changes sPressed
        /// to result in screen change within Update method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnSaveGameButtonReleased(Object sender, EventArgs eventArgs)
        {
            sPressed = "Save Game";
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