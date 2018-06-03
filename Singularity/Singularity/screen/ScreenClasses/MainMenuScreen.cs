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
        private SpriteFont mLibSans36;
        private string mgameModeString;
        private string mloadSelectString;
        private string achievementsString;
        private string mOptionsString;



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
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

            //sSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            //sTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

        }

        public void LoadContent(ContentManager content)
        {
            mOptionsString = "Options";
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
        }
        public void Update(GameTime gametime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.StrokedRectangle(sMenuBox, new Vector2(300,350), Color.White, Color.White, .5f, .20f);

            spriteBatch.DrawString(mLibSans36,
                origin: Vector2.Zero, 
                position: new Vector2(sMenuBox.X + 50, sMenuBox.Y + 50), 
                color: Color.White,
                text: mOptionsString,
                rotation: 0f,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0.2f);

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
