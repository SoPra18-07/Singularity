using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Microsoft.Xna.Framework.Content;

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
        private static Vector2 sMenuBox;


        public MainMenuScreen(Vector2 screenResolution, IScreenManager screenManager, IScreen gameModeSelect,
            IScreen loadSelect, IScreen achievementsScreen, IScreen optionsScreen)
        {
            SetResolution(screenResolution);
            mScreenManager = screenManager;
            mGameModeSelectScreen = gameModeSelect;
            mLoadSelectScreen = loadSelect;
            mAchievementsScreen = achievementsScreen;
            mOptionsScreen = optionsScreen;

        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 - 100);
            //sSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            //sTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

        }

        public void LoadContent(ContentManager content)
        {
        }
        public void Update(GameTime gametime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.StrokedRectangle(new Vector2(300,300), new Vector2(300,300), Color.White, Color.White, .5f, .20f);
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
