using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown when resources are being loaded into the game.
    /// As long as not all resources have been loaded (e.g. map generation
    /// or on a save file load), this screen will be shown.
    /// </summary>
    class LoadingScreen : ITransitionableMenu
    {
        public EScreen Screen { get; private set; } = EScreen.LoadingScreen;
        public bool Loaded { get; set; }

        private Texture2D mLogo;
        private readonly Vector2 mLogoPosition;
        public bool TransitionRunning { get; }

        /// <summary>
        /// Constructs a new instance of the loading screen.
        /// </summary>
        /// <param name="screenResolution">Viewport resolution for scaling</param>
        public LoadingScreen(Vector2 screenResolution)
        {
            mLogoPosition = Vector2.Add(screenResolution, new Vector2(-106, -97));
            TransitionRunning = false;

        }
        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            // doesn't update
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(mLogo, mLogoPosition, null, Color.White, 0f, Vector2.Zero, 0.14f, SpriteEffects.None, 0f);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLogo = content.Load<Texture2D>("Logo");
            Loaded = true;
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
            return false;
        }


        public void TransitionTo(EScreen originScreen, EScreen targetScreen, GameTime gameTime)
        {
            // transition not necessary
        }
    }
}
