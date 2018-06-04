using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Handles everything thats going on explicitly in the game.
    /// E.g. game objects, the map, camera. etc.
    /// </summary>
    class MainMenuScreen : IScreen
    {
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
        public MainMenuScreen(Vector2 screenResolution)
        {
            SetResolution(screenResolution);

            mPlayString = "New Game";
            mLoadSelectString = "Load Game";
            mAchievementsString = "Achivements";
            mOptionsString = "Options";
            mQuitString = "Quit";
            mTitle = "Singularity";
        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

        }

        public void LoadContent(ContentManager content)
        {
            // Load Fonts
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            // Create buttons
            mPlayButton = new Button(1,
                mPlayString,
                mLibSans20,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 80));
            mLoadButton = new Button(1,
                mLoadSelectString,
                mLibSans20,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 130));
            mOptionsButton = new Button(1,
                mOptionsString,
                mLibSans20,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 180));
            mAchievementsButton = new Button(1,
                mAchievementsString,
                mLibSans20,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 230));
            mQuitButton = new Button(1,
                mQuitString,
                mLibSans20,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 280));
            
        }

        public void Update(GameTime gametime)
        {
            // TODO
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw menu window
            spriteBatch.StrokedRectangle(sMenuBox,
                new Vector2(300, 350),
                Color.White,
                Color.White,
                .5f,
                .20f);
            spriteBatch.DrawString(mLibSans36,
                mTitle,
                new Vector2(sMenuBox.X + 30, sMenuBox.Y + 10), Color.White);

            // Draw menu buttons
            mPlayButton.Draw(spriteBatch);
            mLoadButton.Draw(spriteBatch);
            mOptionsButton.Draw(spriteBatch);
            mAchievementsButton.Draw(spriteBatch);
            mQuitButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }
    }
}
