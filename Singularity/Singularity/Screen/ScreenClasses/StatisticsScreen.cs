using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown after Statistics has been selected in the in game pause menu.
    /// Shows an array of statistics of what the player has done. It will be
    /// shown in the form of a graph over time with different buttons to
    /// filter different statistics.
    /// </summary>

    class StatisticsScreen : IScreen
    {
        // Fonts
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans14;
        
        // Text stored as string variables to allow for easy changes, if needed.
        private const string StatisticsStr = "Statistics";
        private const string TimeStr = "Time:";
        private const string UnitsCreatedStr = "Units created:";
        private const string UnitsLostStr = "Units lost:";
        private const string UnitsKilledStr = "Units killed:";
        private const string ResourcesStr = "Resources created";
        private const string PlatformsCreatedStr = "Platforms created";
        private const string PlatformsLostStr = "Platforms lost:";
        private const string PlatformsDestroyedStr = "Platforms destroyed:";

        // Buttons
        private Button mCloseButton;

        // Backup of StatisticsScreen position to update button positions.
        private Vector2 mPrevPosition;

        // ScreenManager to add / close screens.
        private readonly IScreenManager mScreenManager;

        // Director to gain access to StoryManager.
        private readonly Director mDirector;


        public StatisticsScreen(Vector2 screenSize, IScreenManager screenManager, Director director)
        {
            mScreenManager = screenManager;
            mDirector = director;
            

            Position = new Vector2(x: screenSize.X / 2 - 205, y: screenSize.Y / 2 - 225);
            mPrevPosition = Position;

        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            mCloseButton.Update(gametime: gametime);
        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>(assetName: "LibSans20");
            mLibSans14 = content.Load<SpriteFont>(assetName: "LibSans14");

            mCloseButton = new Button(buttonText: "Close", font: mLibSans20, position: new Vector2(x: Position.X + 167, y: Position.Y + 410)) { Opacity = 1f };

            mCloseButton.ButtonReleased += CloseButtonReleased;
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.FillRectangle(location: Position, size: new Vector2(x: 400, y: 450), color: new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 1f), angle: 0f);
            mCloseButton.Draw(spriteBatch: spriteBatch);
            // Draw the window name and stats names.
            spriteBatch.DrawString(spriteFont: mLibSans20, text: StatisticsStr, position: new Vector2(x: Position.X + 145, y: Position.Y + 10), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: TimeStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 60), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: UnitsCreatedStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 110), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: UnitsLostStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 150), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: UnitsKilledStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 190), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: ResourcesStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 240), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: PlatformsCreatedStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 290), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: PlatformsLostStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 370), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: PlatformsDestroyedStr, position: new Vector2(x: Position.X + 60, y: Position.Y + 330), color: Color.Black);
            // Draw the stats behind their names, live from the StoryManager.
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.Time.ToString(), position: new Vector2(x: Position.X + 110, y: Position.Y + 60), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mUnits[key: "created"].ToString(), position: new Vector2(x: Position.X + 180, y: Position.Y + 110), color: Color.LawnGreen);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mUnits[key: "lost"].ToString(), position: new Vector2(x: Position.X + 180, y: Position.Y + 150), color: Color.Red);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mUnits[key: "killed"].ToString(), position: new Vector2(x: Position.X + 180, y: Position.Y + 190), color: Color.Black);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mResources.Sum(selector: x => x.Value).ToString(), position: new Vector2(x: Position.X + 245, y: Position.Y + 240), color: Color.LawnGreen);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mPlatforms[key: "created"].ToString(), position: new Vector2(x: Position.X + 240, y: Position.Y + 290), color: Color.LawnGreen);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mPlatforms[key: "lost"].ToString(), position: new Vector2(x: Position.X + 240, y: Position.Y + 370), color: Color.Red);
            spriteBatch.DrawString(spriteFont: mLibSans14, text: mDirector.GetStoryManager.mPlatforms[key: "destroyed"].ToString(), position: new Vector2(x: Position.X + 240, y: Position.Y + 330), color: Color.Black);

            spriteBatch.End();
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

        private void CloseButtonReleased(object sender, EventArgs eventArgs)
        {
            mScreenManager.RemoveScreen();
        }

        public EScreen Screen { get; private set; } = EScreen.StatisticsScreen;

        public bool Loaded { get; set; }

        // TODO : USE MEMBER VARIABLE INSTEAD?
        private Vector2 Position { get; }
    }
}
