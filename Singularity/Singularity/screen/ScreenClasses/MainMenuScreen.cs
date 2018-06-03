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
        private string mgameModeString;
        private string mloadSelectString;
        private string achievementsString;
        private string mOptionsString;
        private string mTitle;
        private Button mPlay;
        private Button mLoad;
        private Button mOptions;
        private Button mAchievements;
        private Button mQuit;



        public MainMenuScreen(Vector2 screenResolution, IScreenManager screenManager, IScreen gameModeSelect,
            IScreen loadSelect, IScreen achievementsScreen, IScreen optionsScreen, IScreen gameScreen)
        {
            SetResolution(screenResolution);
            mScreenManager = screenManager;
            mGameModeSelectScreen = gameModeSelect;
            mLoadSelectScreen = loadSelect;
            mAchievementsScreen = achievementsScreen;
            mOptionsScreen = optionsScreen;
            mGameScreen = gameScreen;

        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sMenuBox = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            // TODO change size of menu box based on the resolution of the screen 

        }

        public void LoadContent(ContentManager content)
        {
            mOptionsString = "Options";
            mTitle = "Singularity";
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mPlay = new Button(1, "Play", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 80));
            mLoad = new Button(1, "Load", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 130));
            mOptions = new Button(1, "Options", mLibSans20, new Vector2(sMenuBox.X+30, sMenuBox.Y + 180));
            mAchievements = new Button(1, "Achievements", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 230));
            mQuit = new Button(1, "Quit", mLibSans20, new Vector2(sMenuBox.X + 30, sMenuBox.Y + 280));


        }

        public void Update(GameTime gametime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Debug.Print("Mouse is at: " + Mouse.GetState().X + ", " + Mouse.GetState().Y);
                Debug.Print("Menu Box is at: " + sMenuBox.X + ", " + sMenuBox.Y);
                if (Mouse.GetState().X < (sMenuBox.X + mLibSans20.MeasureString("Play").X + 30)
                    && (Mouse.GetState().X >= sMenuBox.X + 30)
                    && Mouse.GetState().Y >= (sMenuBox.Y + 80)
                    && Mouse.GetState().Y < (sMenuBox.Y + mLibSans20.MeasureString("Play").Y + 80))
                {
                    // TODO animate screen
                    MenuBackgroundScreen.SetScreen(EScreen.GameScreen);
                    mScreenManager.RemoveScreen();
                    mScreenManager.AddScreen(mGameScreen);
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.StrokedRectangle(sMenuBox, new Vector2(300,350), Color.White, Color.White, .5f, .20f);
            spriteBatch.DrawString(mLibSans36, mTitle, new Vector2(sMenuBox.X+30, sMenuBox.Y+10), Color.White);
            mPlay.Draw(spriteBatch);
            mLoad.Draw(spriteBatch);
            mOptions.Draw(spriteBatch);
            mAchievements.Draw(spriteBatch);
            mQuit.Draw(spriteBatch);
            
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
