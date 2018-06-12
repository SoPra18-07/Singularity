using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Microsoft.Xna.Framework.Content;


namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shows the main menu screen with 5 options:
    /// New Game, Load Game, Achievements, Options, and Quit Game.
    /// </summary>
    internal sealed class MainMenuScreen : ITransitionableMenu
    {
        private readonly Vector2 mMenuBoxPosition;
        private EScreen mScreenState;

        // Fonts
        private SpriteFont mLibSans36;
        private SpriteFont mLibSans20;

        // all text is stored as string variables to allow for easy changes
        private readonly string mPlayString;
        private readonly string mLoadSelectString;
        private readonly string mAchievementsString;
        private readonly string mOptionsString;
        private readonly string mQuitString;
        private readonly string mTitle;

        // Buttons on the main menu
        private Button mPlayButton;
        private Button mLoadButton;
        private Button mAchievementsButton;
        private Button mOptionsButton;
        private Button mQuitButton;
        private readonly List<Button> mButtonList;

        // Transition properties
        public bool TransitionRunning { get; private set; }

        public void TransitionTo(EScreen eScreen, GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Used to construct a new instance of the main menu screen
        /// </summary>
        /// <param name="screenResolution">Screen resolution of the game</param>
        public MainMenuScreen(Vector2 screenResolution)
        {
            mMenuBoxPosition = new Vector2(screenResolution.X / 2 - 150, screenResolution.Y / 2 - 175);

            mPlayString = "New Game";
            mLoadSelectString = "Load Game";
            mAchievementsString = "Achivements";
            mOptionsString = "Options";
            mQuitString = "Quit";
            mTitle = "Singularity";
            mButtonList = new List<Button>(5);
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans36 = content.Load<SpriteFont>("LibSans36");
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mPlayButton = new Button(mPlayString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 90));
            mLoadButton = new Button(mLoadSelectString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 140));
            mOptionsButton = new Button(mOptionsString, mLibSans20, new Vector2(mMenuBoxPosition.X+30, mMenuBoxPosition.Y + 190));
            mAchievementsButton = new Button(mAchievementsString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 240));
            mQuitButton = new Button(mQuitString, mLibSans20, new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 290));
            mButtonList.Add(mPlayButton);
            mButtonList.Add(mLoadButton);
            mButtonList.Add(mOptionsButton);
            mButtonList.Add(mAchievementsButton);
            mButtonList.Add(mQuitButton);

            mPlayButton.ButtonReleased += MainMenuManagerScreen.OnPlayButtonReleased;
            mLoadButton.ButtonReleased += MainMenuManagerScreen.OnLoadButtonReleased;
            mOptionsButton.ButtonReleased += MainMenuManagerScreen.OnOptionsButtonReleased;
            mAchievementsButton.ButtonReleased += MainMenuManagerScreen.OnAchievementsButtonReleased;
            mQuitButton.ButtonReleased += MainMenuManagerScreen.OnQuitButtonReleased;

        }


        /// <summary>
        /// Updates the buttons within the MainMenuScreen.
        /// </summary>
        /// <param name="gametime"></param>
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
                mTitle,
                new Vector2(mMenuBoxPosition.X + 30, mMenuBoxPosition.Y + 10), Color.White);

            spriteBatch.End();
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
