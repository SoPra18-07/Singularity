using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Screen
{
    /// <summary>
    /// Shown when the game is first started and shows the logo and
    /// "Press any key to start".
    /// The logo will change depending on not the player has past the reveal
    /// point in the campaign. Not buttons are shown but it listens for any
    /// key input.
    /// </summary>
    class SplashScreen : IScreen
    {
        // TODO either add bloom to the text or make it a sprite
        private Texture2D mLogoTexture2D;
        private Texture2D mSingularityText;
        private static Vector2 sLogoPosition;
        private static Vector2 sSingularityTextPosition;
        private static Vector2 sTextPosition;
        private SpriteFont mLibSans20;
        private Vector2 mStringCenter;
        private string mContinueString;
        private IScreenManager mScreenManager;
        private IScreen mMainMenuScreen;

        public SplashScreen(Vector2 screenResolution, IScreenManager screenManager, IScreen mainMenu)
        {
            SetResolution(screenResolution);
            mScreenManager = screenManager;
            mMainMenuScreen = mainMenu;
        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 - 100);
            sSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            sTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

        }

        public void LoadContent(ContentManager content)
        {
            mLogoTexture2D = content.Load<Texture2D>("Logo");
            mSingularityText = content.Load<Texture2D>("SingularityText");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mContinueString = "Press any key to continue";
            mStringCenter = new Vector2(mLibSans20.MeasureString(mContinueString).X / 2, mLibSans20.MeasureString(mContinueString).Y / 2);
        }
        public void Update(GameTime gametime)
        {
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                // TODO animate screen
                MenuBackgroundScreen.SetScreen(EScreen.MainMenuScreen);
                mScreenManager.RemoveScreen();
                //mScreenManager.AddScreen(mMainMenuScreen);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the logo
            spriteBatch.Draw(mLogoTexture2D,
                origin: new Vector2(308, 279),
                position: sLogoPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(mSingularityText,
                origin: new Vector2(322, 41),
                position: sSingularityTextPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(mLibSans20,
                origin: mStringCenter,
                position: sTextPosition,
                color: Color.White,
                text: mContinueString,
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
