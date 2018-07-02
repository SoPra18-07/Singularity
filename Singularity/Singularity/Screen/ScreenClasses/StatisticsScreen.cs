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
        private readonly string mStatistics;
        private readonly string mTime;
        private readonly string mUnitsCreated;
        private readonly string mUnitsLost;
        private readonly string mUnitsKilled;
        private readonly string mResources;
        private readonly string mPlatformsCreated;
        private readonly string mPlatformsLost;
        private readonly string mPlatformsDestroyed;        

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
            
            // assignments actually happen here
            mStatistics = "Statistics";
            mTime = "Time:";
            mUnitsCreated = "Units created:";
            mUnitsLost = "Units lost:";
            mUnitsKilled = "Units killed:";
            mResources = "Resources created:";
            mPlatformsCreated = "Platforms created";
            mPlatformsLost = "Platforms lost:";
            mPlatformsDestroyed = "Platforms destroyed:";
            

            Position = new Vector2(screenSize.X / 2 - 205, screenSize.Y / 2 - 225);
            mPrevPosition = Position;

        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            mCloseButton.Update(gametime);
        }

        /// <summary>
        /// Loads any content that this screen might need.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");

            mCloseButton = new Button("Close", mLibSans20, new Vector2(Position.X + 167, Position.Y + 410)) { Opacity = 1f };

            mCloseButton.ButtonReleased += CloseButtonReleased;
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.FillRectangle(Position, new Vector2(400, 450), new Color(0.27f, 0.5f, 0.7f, 1f), 0f);
            mCloseButton.Draw(spriteBatch);
            // Draw the window name and stats names.
            spriteBatch.DrawString(mLibSans20, mStatistics, new Vector2(Position.X + 145, Position.Y + 10), Color.Black);
            spriteBatch.DrawString(mLibSans14, mTime, new Vector2(Position.X + 60, Position.Y + 60), Color.Black);
            spriteBatch.DrawString(mLibSans14, mUnitsCreated, new Vector2(Position.X + 60, Position.Y + 110), Color.Black);
            spriteBatch.DrawString(mLibSans14, mUnitsLost, new Vector2(Position.X + 60, Position.Y + 150), Color.Black);
            spriteBatch.DrawString(mLibSans14, mUnitsKilled, new Vector2(Position.X + 60, Position.Y + 190), Color.Black);
            spriteBatch.DrawString(mLibSans14, mResources, new Vector2(Position.X + 60, Position.Y + 240), Color.Black);
            spriteBatch.DrawString(mLibSans14, mPlatformsCreated, new Vector2(Position.X + 60, Position.Y + 290), Color.Black);
            spriteBatch.DrawString(mLibSans14, mPlatformsLost, new Vector2(Position.X + 60, Position.Y + 370), Color.Black);
            spriteBatch.DrawString(mLibSans14, mPlatformsDestroyed, new Vector2(Position.X + 60, Position.Y + 330), Color.Black);
            // Draw the stats behind their names, live from the StoryManager.
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.Time.ToString(), new Vector2(Position.X + 110, Position.Y + 60), Color.Black);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mUnits["created"].ToString(), new Vector2(Position.X + 180, Position.Y + 110), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mUnits["lost"].ToString(), new Vector2(Position.X + 180, Position.Y + 150), Color.Red);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mUnits["killed"].ToString(), new Vector2(Position.X + 180, Position.Y + 190), Color.Black);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mResources.Sum(x => x.Value).ToString(), new Vector2(Position.X + 245, Position.Y + 240), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mPlatforms["created"].ToString(), new Vector2(Position.X + 240, Position.Y + 290), Color.LawnGreen);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mPlatforms["lost"].ToString(), new Vector2(Position.X + 240, Position.Y + 370), Color.Red);
            spriteBatch.DrawString(mLibSans14, mDirector.GetStoryManager.mPlatforms["destroyed"].ToString(), new Vector2(Position.X + 240, Position.Y + 330), Color.Black);

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
