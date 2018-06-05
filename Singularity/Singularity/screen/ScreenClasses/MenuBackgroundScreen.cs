using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    // TODO animate the menu screen
    /// <summary>
    /// All the main menu screens are overlayed on top of this screen.
    /// Since the main menu will have the same animated background, it will
    /// simply use the same background screen and be overlayed on top of it.
    /// </summary>
    class MenuBackgroundScreen : IScreen
    {
        private Texture2D mGlowTexture2D;
        private Texture2D mHoloProjectionTexture2D;
        private Vector2 mScreenResolution;
        private Vector2 mScreenResolutionScaling;
        private float mHoloProjectionWidthScaling;
        private Vector2 mHoloProjectionScaling;
        private EScreen mCurrentScreen;

        /// <summary>
        /// Creates the MenuBackgroundScreen class.
        /// </summary>
        /// <param name="screenResolution">Current screen resolution.</param>
        public MenuBackgroundScreen(Vector2 screenResolution)
        {
            mHoloProjectionWidthScaling = 1f;
            SetResolution(screenResolution);
            mCurrentScreen = EScreen.SplashScreen;

            Debug.Print("sScreenResolution: " + mScreenResolution.X + ", " + mScreenResolution.Y);
            Debug.Print("sScreenResolutionScaling: " + mScreenResolutionScaling.X + ", " + mScreenResolutionScaling.Y);
        }

        private void SetHoloProjectionScaling(float widthScaling)
        {
            if (mScreenResolutionScaling.X < mScreenResolutionScaling.Y)
            {
                mHoloProjectionScaling = new Vector2(mScreenResolutionScaling.X);
            }
            else
            {
                mHoloProjectionScaling = new Vector2(mScreenResolutionScaling.Y);
            }

            mHoloProjectionScaling = Vector2.Multiply(mHoloProjectionScaling, new Vector2(widthScaling, 1f));
        }

        /// <summary>
        /// Changes the dimensions of the screen to fit the viewport resolution.
        /// </summary>
        /// <param name="screenResolution">Current viewport screen resolution</param>
        public void SetResolution(Vector2 screenResolution)
        {
            mScreenResolution = screenResolution;
            mScreenResolutionScaling = new Vector2(screenResolution.X / 1280, screenResolution.Y / 1024);
            SetHoloProjectionScaling(mHoloProjectionWidthScaling);
        }

        /// <summary>
        /// Changes the HoloProjectionTexture width based on the screen being shown and
        /// starts the animation for the screen.
        /// </summary>
        /// <param name="screen">Choose the screen to be overlayed on top
        /// of the menu background</param>
        public void SetScreen(EScreen screen)
        {
            mCurrentScreen = screen;

            switch (mCurrentScreen)
            {
                case EScreen.AchievementsScreen:
                    mHoloProjectionWidthScaling = 1.5f;
                    break;
                case EScreen.GameModeSelectScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    mHoloProjectionWidthScaling = 4f;
                    break;
                case EScreen.OptionsScreen:
                    break;
                case EScreen.SplashScreen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SetHoloProjectionScaling(mHoloProjectionWidthScaling);
        }

        /// <summary>
        /// Loads content specific to this screen
        /// </summary>
        /// <param name="content">ContentManager for the entire game</param>
        public void LoadContent(ContentManager content)
        {
            mGlowTexture2D = content.Load<Texture2D>("Glow");
            mHoloProjectionTexture2D = content.Load<Texture2D>("HoloProjection");
        }

        /// <summary>
        /// Update animates the background. It uses the Easing animation method and also implements
        /// a flickering animation to the HoloProjectionTexture.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // TODO (used for animation)
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw glow
            spriteBatch.Draw(mGlowTexture2D,
                new Vector2(mScreenResolution.X / 2, mScreenResolution.Y / 2),
                null,
                Color.AliceBlue,
                0f,
                new Vector2(609, 553),
                mScreenResolutionScaling.X < mScreenResolutionScaling.Y ? mScreenResolutionScaling.X : mScreenResolutionScaling.Y, // Scales based on smaller scalar between height and width,
                SpriteEffects.None,
                1f);

            // Draw holoProjection texture
            spriteBatch.Draw(mHoloProjectionTexture2D,
                new Vector2(540, 360),
                null,
                Color.AliceBlue,
                0f,
                new Vector2(367, 515),
                mHoloProjectionScaling, // Scales based on smaller scalar between height and width
                SpriteEffects.None,
                0f);
            
            spriteBatch.End();
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return false;
        }
    }
}
