using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shown after the "New Game" button on the main
    /// menu has been clicked. It shows the option to either
    /// play a new campaign or a new skirmish. It uses two text buttons
    /// and a back button.
    /// </summary>

    class GameModeSelectScreen : IScreen
    {

        private string mStoryString;
        private string mFreePlayString;
        private string mBackString;
        private string mWindowTitleString;

        private List<Button> mButtonList;

        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        private Button mStoryButton;
        private Button mFreePlayButton;
        private Button mBackButton;

        private Vector2 mMenuBoxPosition;

        public GameModeSelectScreen(Vector2 screenResolution)
        {
            mStoryString = "Campaign Mode";
            mFreePlayString = "Skirmish";
            mBackString = "Back";
            mWindowTitleString = "New Game";

            mButtonList = new List<Button>(3);

            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            foreach (Button button in mButtonList)
            {
                button.Update(gametime);
            }
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (Button button in mButtonList)
            {
                button.Draw(spriteBatch);
            }


            // Draw menu window
            spriteBatch.StrokedRectangle(mMenuBoxPosition,
                new Vector2(300, 350),
                Color.White,
                Color.White,
                .5f,
                .20f);
            spriteBatch.DrawString(mLibSans36,
                mWindowTitleString,
                new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 10), Color.White);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mStoryButton = new Button(mStoryString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 90));
            mFreePlayButton = new Button(mFreePlayString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 140));
            mBackButton = new Button(mBackString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 190));
            mButtonList.Add(mStoryButton);
            mButtonList.Add(mFreePlayButton);
            mButtonList.Add(mBackButton);


            mStoryButton.ButtonReleased += MainMenuManagerScreen.OnStoryButtonReleased;
            mFreePlayButton.ButtonReleased += MainMenuManagerScreen.OnFreePlayButtonReleased;
            mBackButton.ButtonReleased += MainMenuManagerScreen.OnBackButtonReleased;
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
            return true;
        }
    }
}
