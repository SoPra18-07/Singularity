using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shows the pause menu when in the middle of a game. It shows the
    /// following: Resume, Quick Save, Save, Statistics, Quit Game.
    /// </summary>
    class GamePauseScreen : IScreen
    {

        // fonts
        private SpriteFont mLibSans20;

        // buttons
        private Button mAchievementButton;
        private Button mStatisticsButton;
        private Button mCloseButton;

        // Screens
        private StatisticsScreen mStatisticsScreen;

        // backup of pauseMenu position to update button positions
        private Vector2 mPrevPosition;

        // screen manager to add / close screens
        private IScreenManager mScreenManager;

        /// <summary>
        /// TODO
        /// </summary>
        public GamePauseScreen(Vector2 screenSize, IScreenManager screenManager, Director director)
        {
            mScreenManager = screenManager;

            // StatisticsScreen
            mStatisticsScreen = new StatisticsScreen(screenSize: screenSize, screenManager: screenManager, director: director);

            Position = new Vector2(x: screenSize.X / 2 - 150, y: screenSize.Y / 2 - 200);
            mPrevPosition = Position;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            mAchievementButton.Update(gametime: gametime);
            mStatisticsButton.Update(gametime: gametime);
            mCloseButton.Update(gametime: gametime);
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.StrokedRectangle(location: Position, size: new Vector2(x: 300,y: 400), colorBorder: Color.White, colorCenter: new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 0.8f), opacityBorder: 1f, opacityCenter: 1f);
            mAchievementButton.Draw(spriteBatch: spriteBatch);
            mStatisticsButton.Draw(spriteBatch: spriteBatch);
            mCloseButton.Draw(spriteBatch: spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>(assetName: "LibSans20");

            mAchievementButton = new Button(buttonText: "Achievements", font: mLibSans20, position: new Vector2(x: Position.X + 20, y: Position.Y + 80)) { Opacity = 1f };
            mStatisticsButton = new Button(buttonText: "Statistics", font: mLibSans20, position: new Vector2(x: Position.X + 20, y: Position.Y + 180)) { Opacity = 1f };
            mCloseButton = new Button(buttonText: "Close", font: mLibSans20, position: new Vector2(x: Position.X + 20, y: Position.Y + 280)) { Opacity = 1f };

            mAchievementButton.ButtonReleased += AchievementButtonReleased;
            mStatisticsButton.ButtonReleased += StatisticsButtonReleased;
            mCloseButton.ButtonReleased += CloseButtonReleased;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should update.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be updated.</returns>
        public bool UpdateLower()
        {
            return false;
        }

        /// <summary>
        /// Determines whether or not the screen below this on the stack should be drawn.
        /// </summary>
        /// <returns>Bool. If true, then the screen below this will be drawn.</returns>
        public bool DrawLower()
        {
            return true;
        }

        #region button managemenet

        private void AchievementButtonReleased(object sender, EventArgs eventArgs)
        {
            // TODO : ADD PARAMETERS IF NEEDED (FOR EXAMPLE GRAPHICS DEVICE TO CALCULATE POSITIONS/SIZES
            mScreenManager.AddScreen(screen: new AchievementsScreen());
        }

        private void StatisticsButtonReleased(object sender, EventArgs eventArgs)
        {
            // TODO : ADD PARAMETERS IF NEEDED (FOR EXAMPLE GRAPHICS DEVICE TO CALCULATE POSITIONS/SIZES
            mScreenManager.AddScreen(screen: mStatisticsScreen);
        }

        private void CloseButtonReleased(object sender, EventArgs eventArgs)
        {
            mScreenManager.RemoveScreen();
        }

        #endregion

        public EScreen Screen { get; } = EScreen.GamePauseScreen;
        public bool Loaded { get; set; }

        // TODO : USE MEMBER VARIABLE INSTEAD?
        private Vector2 Position { get; }
    }
}
