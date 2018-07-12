using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    /// <inheritdoc cref="IScreen"/>
    /// <summary>
    /// Shows the pause menu when in the middle of a game. It shows the
    /// following: Resume, Quick Save, Save, Statistics, Quit Game.
    /// </summary>
    class GamePauseScreen : IScreen, IKeyListener, IMouseClickListener, IMouseWheelListener
    {

        // fonts
        private SpriteFont mLibSans20;

        // buttons
        private Button mStatisticsButton;
        private Button mCloseButton;

        // Screens
        private StatisticsScreen mStatisticsScreen;

        // backup of pauseMenu position to update button positions
        private Vector2 mPrevPosition;

        // screen manager to add / close screens
        private IScreenManager mScreenManager;

        private readonly Director mDirector;

        /// <summary>
        /// TODO
        /// </summary>
        public GamePauseScreen(Vector2 screenSize, IScreenManager screenManager, Director director)
        {
            mScreenManager = screenManager;

            // StatisticsScreen
            mStatisticsScreen = new StatisticsScreen(screenSize, screenManager, director);

            Position = new Vector2(screenSize.X / 2 - 150, screenSize.Y / 2 - 200);
            mPrevPosition = Position;

            mDirector = director;
        }

        /// <summary>
        /// Updates the contents of the screen.
        /// </summary>
        /// <param name="gametime">Current gametime. Used for actions
        /// that take place over time </param>
        public void Update(GameTime gametime)
        {
            mStatisticsButton.Update(gametime);
            mCloseButton.Update(gametime);
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <param name="spriteBatch">spriteBatch that this object should draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.StrokedRectangle(Position, new Vector2(300, 400), Color.White, new Color(0.27f, 0.5f, 0.7f, 0.8f), 1f, 1f);
            mStatisticsButton.Draw(spriteBatch);
            mCloseButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads any content specific to this screen.
        /// </summary>
        /// <param name="content">Content Manager that should handle the content loading</param>
        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>("LibSans20");

            mStatisticsButton = new Button("Statistics", mLibSans20, new Vector2(Position.X + 20, Position.Y + 180)) { Opacity = 1f };
            mCloseButton = new Button("Close", mLibSans20, new Vector2(Position.X + 20, Position.Y + 280)) { Opacity = 1f };

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

        private void StatisticsButtonReleased(object sender, EventArgs eventArgs)
        {
            RegisterToInputManager();

            // TODO : ADD PARAMETERS IF NEEDED (FOR EXAMPLE GRAPHICS DEVICE TO CALCULATE POSITIONS/SIZES
            mScreenManager.AddScreen(mStatisticsScreen);
        }

        private void CloseButtonReleased(object sender, EventArgs eventArgs)
        {
            UnregisterFromInputManager();

            mScreenManager.RemoveScreen();
        }

        private void RegisterToInputManager()
        {
            RegisterOrUnregisterFromInputManager(true);
        }

        private void UnregisterFromInputManager()
        {
            RegisterOrUnregisterFromInputManager(false);
        }

        private void RegisterOrUnregisterFromInputManager(bool shouldRegister)
        {
            if (shouldRegister)
            {
                mDirector.GetInputManager.AddKeyListener(this);
                mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
                mDirector.GetInputManager.AddMouseWheelListener(this);
            }
            else
            {
                mDirector.GetInputManager.RemoveKeyListener(this);
                mDirector.GetInputManager.RemoveMouseClickListener(this);
                mDirector.GetInputManager.RemoveMouseWheelListener(this);
            }
        }

        #endregion

        public EScreen Screen { get; } = EScreen.GamePauseScreen;
        public bool Loaded { get; set; }

        // TODO : USE MEMBER VARIABLE INSTEAD?
        private Vector2 Position { get; }

        #region InputManagerDenial

        public Rectangle Bounds => Rectangle.Empty;

        public bool KeyTyped(KeyEvent keyEvent)
        {
            return false;
        }

        public bool KeyPressed(KeyEvent keyEvent)
        {
            return false;
        }

        public bool KeyReleased(KeyEvent keyEvent)
        {
            return false;
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            return false;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return false;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return false;
        }

        public bool MouseWheelValueChanged(EMouseAction mouseAction)
        {
            return false;
        }

        #endregion
    }
}
