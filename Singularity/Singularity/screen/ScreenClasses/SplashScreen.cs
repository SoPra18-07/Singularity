using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

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
        private Texture2D LogoTexture2D;
        private Texture2D SingularityText;
        private static Vector2 sLogoPosition;
        private static Vector2 sSingularityTextPosition;
        private static Vector2 sTextPosition;

        public SplashScreen(Vector2 screenResolution)
        {
            SetResolution(screenResolution);
        }

        public static void SetResolution(Vector2 screenResolution)
        {
            sLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 - 100);
            sSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            sTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);

        }

        public void LoadContent(ContentManager content)
        {
            LogoTexture2D = content.Load<Texture2D>("Logo");
            SingularityText = content.Load<Texture2D>("SingularityText");
        }
        public void Update(GameTime gametime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the logo
            spriteBatch.Draw(LogoTexture2D,
                origin: new Vector2(308, 279),
                position: sLogoPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the SingularityText
            spriteBatch.Draw(SingularityText,
                origin: new Vector2(322, 41),
                position: sSingularityTextPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text

            // debug draw stroked rectangle
            spriteBatch.StrokedRectangle(sTextPosition, new Vector2(332, 40), Color.White, 0.21f);
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
