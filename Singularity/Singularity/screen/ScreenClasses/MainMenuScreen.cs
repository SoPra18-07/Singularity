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
using Singularity.Input;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shows the main menu screen with 5 options:
    /// New Game, Load Game, Achievements, Options, and Quit Game.
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
        private List<Button> mButtonList;

        /// <summary>
        /// 
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
            mButtonList = new List<Button>();
        }

        /// <summary>
        /// Used for scaling of the menu box
        /// </summary>
        /// <param name="screenResolution">Current screen resolution</param>
        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mPlayButton = new Button("Play", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 90));
            mLoadButton = new Button("Load", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 140));
            mOptionsButton = new Button("Options", mLibSans20, new Vector2(sMenuBox.X+30, sMenuBox.Y + 190));
            mAchievementsButton = new Button("Achievements", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 240));
            mQuitButton = new Button("Quit", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 290));
            mButtonList.Add(mPlayButton);
            mButtonList.Add(mLoadButton);
            mButtonList.Add(mOptionsButton);
            mButtonList.Add(mAchievementsButton);
            mButtonList.Add(mQuitButton);

            mPlayButton.ButtonReleased += MainMenuManagerScreen.OnPlayButtonReleased;
            mLoadButton.ButtonReleased += MainMenuManagerScreen.OnLoadButtonReleased;
            mOptionsButton.ButtonReleased += MainMenuManagerScreen.OnOptionsButtonReleased;
            mAchievementsButton.ButtonReleased += MainMenuManagerScreen.OnAchievementsButtonReleased;
            mQuitButton.ButtonReleased += MainMenuManagerScreen.OnQuitButtonReleased;

        }

        /// <summary>
        /// Updates the buttons within the MainMenuScreen.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {

            foreach (Button button in mButtonList)
            {
                button.Update(gametime);
            }
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (Button button in mButtonList)
            {
                button.Draw(spriteBatch);
            }
           

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
    }
}
