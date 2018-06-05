using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Handles everything thats going on explicitly in the game.
    /// E.g. game objects, the map, camera. etc.
    /// </summary>
    class SplashScreen : IScreen
    {
        // TODO either add bloom to the text or make it a sprite
        private Texture2D mLogoTexture2D;
        private Texture2D mSingularityText;
        private readonly Vector2 mLogoPosition;
        private readonly Vector2 mSingularityTextPosition;
        private readonly Vector2 mTextPosition;
        private SpriteFont mLibSans20;
        private Vector2 mStringCenter;
        private readonly string mContinueString;

        /// <summary>
        /// Shown when the game is first started and shows the logo and
        /// "Press any key to start".
        /// The logo will change depending on not the player has past the reveal
        /// point in the campaign. Not buttons are shown but it listens for any
        /// key input.
        /// </summary>
        public SplashScreen(Vector2 screenResolution)
        {
            mLogoPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 - 100);
            mSingularityTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 150);
            mTextPosition = new Vector2(screenResolution.X / 2, screenResolution.Y / 2 + 250);
            mContinueString = "Press any key to continue";
        }

        public void LoadContent(ContentManager content)
        {
            mLogoTexture2D = content.Load<Texture2D>("Logo");
            mSingularityText = content.Load<Texture2D>("SingularityText");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mStringCenter = new Vector2(mLibSans20.MeasureString(mContinueString).X / 2, mLibSans20.MeasureString(mContinueString).Y / 2);
        }
        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the logo
            spriteBatch.Draw(mLogoTexture2D,
                origin: new Vector2(308, 279),
                position: mLogoPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0f,
                effects: SpriteEffects.None);

            // Draw the mSingularityText
            spriteBatch.Draw(mSingularityText,
                origin: new Vector2(322, 41),
                position: mSingularityTextPosition,
                color: Color.AliceBlue,
                rotation: 0f,
                scale: 0.5f,
                sourceRectangle: null,
                layerDepth: 0.1f,
                effects: SpriteEffects.None);

            // Draw the text
            spriteBatch.DrawString(mLibSans20,
                origin: mStringCenter,
                position: mTextPosition,
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
