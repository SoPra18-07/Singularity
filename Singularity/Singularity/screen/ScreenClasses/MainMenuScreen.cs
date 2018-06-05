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
    /// <summary>
    /// Shows the main menu screen with 5 options:
    /// New Game, Load Game, Achievements, Options, and Quit Game.
    /// </summary>
    class MainMenuScreen : IScreen
    {
        private IScreenManager mScreenManager;
        private IScreen mGameModeSelectScreen;
        private IScreen mLoadSelectScreen;
        private IScreen mAchievementsScreen;
        private IScreen mOptionsScreen;
        private IScreen mGameScreen;
        private static Vector2 sMenuBox;
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;
        private string mTitle;
        private Button mPlay;
        private Button mLoad;
        private Button mOptions;
        private Button mAchievements;
        private Button mQuit;
        private List<Button> mButtonsList;
        private InputManager mInputManager;



        public MainMenuScreen(Vector2 screenResolution, IScreenManager screenManager, IScreen gameModeSelect,
            IScreen loadSelect, IScreen achievementsScreen, IScreen optionsScreen, IScreen gameScreen, InputManager inputManager)
        {
            SetResolution(screenResolution);
            mScreenManager = screenManager;
            mGameModeSelectScreen = gameModeSelect;
            mLoadSelectScreen = loadSelect;
            mAchievementsScreen = achievementsScreen;
            mOptionsScreen = optionsScreen;
            mGameScreen = gameScreen;
            mButtonsList = new List<Button>();
            mInputManager = inputManager;
        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

        }

        public void LoadContent(ContentManager content)
        {
            mTitle = "Singularity";
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mPlay = new Button("Play", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 90), mInputManager);
            mLoad = new Button("Load", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 140), mInputManager);
            mOptions = new Button("Options", mLibSans20, new Vector2(sMenuBox.X+30, sMenuBox.Y + 190), mInputManager);
            mAchievements = new Button("Achievements", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 240), mInputManager);
            mQuit = new Button("Quit", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 290), mInputManager);
            mButtonsList.Add(mPlay);
            mButtonsList.Add(mLoad);
            mButtonsList.Add(mOptions);
            mButtonsList.Add(mAchievements);
            mButtonsList.Add(mQuit);

        }

        public void Update(GameTime gametime)
        {
            foreach (Button button in mButtonsList)
            {
                button.Update(gametime);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.StrokedRectangle(sMenuBox, new Vector2(300,350), Color.White, Color.White, .5f, .20f);
            spriteBatch.DrawString(mLibSans36, mTitle, new Vector2(sMenuBox.X+30, sMenuBox.Y+20), Color.White);
            foreach (Button button in mButtonsList)
            {
                button.Draw(spriteBatch);
            }
            
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
