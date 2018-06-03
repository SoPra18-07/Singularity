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
        private static Vector2 sScreenResolution;
        private static Vector2 sScreenResolutionScaling;
        private static float sHoloProjectionWidthScaling;
        private static Vector2 sHoloProjectionScaling;
        private static EScreen sCurrentScreen;

        /// <summary>
        /// Creates the MenuBackgroundScreen class.
        /// </summary>
        /// <param name="screenResolution">Current screen resolution.</param>
        public MenuBackgroundScreen(Vector2 screenResolution)
        {
            sHoloProjectionWidthScaling = 1f;
            SetResolution(screenResolution);
            sCurrentScreen = EScreen.SplashScreen;

            Debug.Print("sScreenResolution: " + sScreenResolution.X + ", " + sScreenResolution.Y);
            Debug.Print("sScreenResolutionScaling: " + sScreenResolutionScaling.X + ", " + sScreenResolutionScaling.Y);
        }

        private static void SetHoloProjectionScaling(float widthScaling)
        {
            if (sScreenResolutionScaling.X < sScreenResolutionScaling.Y)
            {
                sHoloProjectionScaling = new Vector2(sScreenResolutionScaling.X);
            }
            else
            {
                sHoloProjectionScaling = new Vector2(sScreenResolutionScaling.Y);
            }

            sHoloProjectionScaling = Vector2.Multiply(sHoloProjectionScaling, new Vector2(widthScaling, 1f));
        }

        /// <summary>
        /// Changes the dimensions of the screen to fit the viewport resolution.
        /// </summary>
        /// <param name="screenResolution">Current viewport screen resolution</param>
        public static void SetResolution(Vector2 screenResolution)
        {
            sScreenResolution = screenResolution;
            sScreenResolutionScaling = new Vector2(screenResolution.X / 1280, screenResolution.Y / 1024);
            SetHoloProjectionScaling(sHoloProjectionWidthScaling);
        }

        /// <summary>
        /// Changes the HoloProjectionTexture width based on the screen being shown and
        /// starts the animation for the screen.
        /// </summary>
        /// <param name="screen">Choose the screen to be overlayed on top
        /// of the menu background</param>
        public static void SetScreen(EScreen screen)
        {
            sCurrentScreen = screen;

            switch (sCurrentScreen)
            {
                case EScreen.AchievementsScreen:
                    sHoloProjectionWidthScaling = 1.5f;
                    break;
                case EScreen.GameModeSelectScreen:
                    break;
                case EScreen.LoadSelectScreen:
                    break;
                case EScreen.MainMenuScreen:
                    sHoloProjectionWidthScaling = 4f;
                    break;
                case EScreen.OptionsScreen:
                    break;
                case EScreen.SplashScreen:
                    break;
                default:
                    break;
            }
            SetHoloProjectionScaling(sHoloProjectionWidthScaling);
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
                new Vector2(sScreenResolution.X / 2, sScreenResolution.Y / 2),
                null,
                Color.AliceBlue,
                0f,
                new Vector2(609, 553),
                sScreenResolutionScaling.X < sScreenResolutionScaling.Y ? sScreenResolutionScaling.X : sScreenResolutionScaling.Y, // Scales based on smaller scalar between height and width,
                SpriteEffects.None,
                1f);

            // Draw holoProjection texture
            spriteBatch.Draw(mHoloProjectionTexture2D,
                new Vector2(540, 360),
                null,
                Color.AliceBlue,
                0f,
                new Vector2(367, 515),
                sHoloProjectionScaling, // Scales based on smaller scalar between height and width
                SpriteEffects.None,
                0f);
            
            spriteBatch.End();
        }

        public bool UpdateLower()
        {
            return false;
        }

        public bool DrawLower()
        {
            return false;
        }
    }
}
