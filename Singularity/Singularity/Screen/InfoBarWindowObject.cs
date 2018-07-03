using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Screen
{
    /// <summary>
    /// TODO 
    /// </summary>
    sealed class InfoBarWindowObject : IDraw, IUpdate, IMousePositionListener, IMouseClickListener
    {
        // list of items in info bar
        private readonly List<IWindowItem> mInfoBarItemList;

        // pause button of info bar
        private Button mPauseButton;

        // toggle civilUnits window active/deactive via button
        private Button mCivilUnitsWindowButton;

        // toggle resource window active/deactive via button
        private Button mResourceWindowButton;

        // toggle eventLog window active/deactive via button
        private Button mEventLogWindowButton;

        // toggle buildMenu window active/deactive via button
        private Button mBuildMenuWindowButton;

        // toggle selectedPlatform window active/deactive via button
        private Button mSelectedPlatformWindowButton;

        // just a simple division of the infoBar to be able to place it's buttons at one of X possible locations
        private int mWidthDivision;

        // colors for the rectangle
        private Color mBordeColor;
        private Color mFillColor;

        // backup of width to react to resolution changes
        private int mWidthBackup;

        // fonts
        private SpriteFont mSpriteFont;

        // game time to draw in info bar
        private GameTime mGameTime;

        // needed for input management
        private Director mDirector;

        // screen management - needed for pause menu
        private IScreenManager mScreenManager;

        // pause menu screen
        private GamePauseScreen mGamePauseScreen;

        // mouse position - needed to prevent input through the info bar
        private Vector2 mMouse;

        /// <summary>
        /// The infoBar is part of the UI.
        /// It is placed on the top of the screen. TODO
        /// </summary>
        /// <param name="borderColor">the color used for the infoBar's border</param>
        /// <param name="fillColor">the color used to fill the infoBar</param>
        /// <param name="graphics"></param>
        /// <param name="spriteFont">font used for buttons and text</param>
        /// <param name="director"></param>
        /// <param name="screenManager"></param>
        /// <param name="civilUnitsWindow"></param>
        /// <param name="resourceWindow"></param>
        /// <param name="eventLogWindow"></param>
        /// <param name="buildMenuWindow"></param>
        /// <param name="selectedPlatformWindow"></param>
        public InfoBarWindowObject(
            Color borderColor, 
            Color fillColor, 
            GraphicsDeviceManager graphics, 
            SpriteFont spriteFont, 
            Director director, 
            IScreenManager screenManager,
            WindowObject civilUnitsWindow,
            WindowObject resourceWindow,
            WindowObject eventLogWindow,
            WindowObject buildMenuWindow,
            WindowObject selectedPlatformWindow)
        {
            // set member variables - for further commenting see declaration of member variables
            mBordeColor = borderColor;
            mFillColor = fillColor;
            mSpriteFont = spriteFont;
            mDirector = director;
            mScreenManager = screenManager;
            mInfoBarItemList = new List<IWindowItem>();

            // set to zero to force the update 'change-in-res'-call once (because width and backup are different from the beginning)
            mWidthBackup = 0;

            mWidthDivision = Width / 10;

            // set starting values
            Screen = EScreen.UserInterfaceScreen;
            Width = graphics.PreferredBackBufferWidth;
            Active = true;

            // the entire infoBar
            Bounds = new Rectangle(0, 0, Width, 25);

            // NOTICE : all buttons can start with position (0,0) since they will be positioned at the first update-call
            // pause button
            mPauseButton = new Button(" ll ", mSpriteFont, new Vector2(0, 0)) {Opacity = 1f};
            mInfoBarItemList.Add(mPauseButton);
            mPauseButton.ButtonReleased += PauseButtonReleased;

            // refined ressources
            mCivilUnitsWindowButton = new Button("units", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mCivilUnitsWindowButton);

            mResourceWindowButton = new Button("resource", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mResourceWindowButton);

            mEventLogWindowButton = new Button("eventLog", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mEventLogWindowButton);

            mBuildMenuWindowButton = new Button("build", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mBuildMenuWindowButton);

            mSelectedPlatformWindowButton = new Button("platform", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mSelectedPlatformWindowButton);

            // add input manager to prevent other objects from behind the infoBar to get called through the infoBar
            director.GetInputManager.AddMousePositionListener(this);
            director.GetInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);


            // pause menu screen
            mGamePauseScreen = new GamePauseScreen(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), mScreenManager, mDirector);


            // TODO :
            // get graphicsdevman from director as soon as the director has got the gDevice manager
        }

        /// <summary>
        /// standard draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.StrokedRectangle(new Vector2(0, 0), new Vector2(Width, 25), mBordeColor, mFillColor, 1f, 1f);
                spriteBatch.StrokedRectangle(new Vector2(Width - 25, 0), new Vector2(30, 25), mBordeColor, mFillColor, 1f, 1f);

                foreach (var item in mInfoBarItemList)
                {
                    item.Draw(spriteBatch);
                }

                spriteBatch.DrawString(mSpriteFont, DateTime.Now.ToShortTimeString(), new Vector2(mWidthDivision * 9, 2.5f), new Color(0,0,0));
                spriteBatch.DrawString(mSpriteFont, mDirector.GetStoryManager.Time.ToString(), new Vector2(mWidthDivision * 7, 2.5f), new Color(1, 0, 0));
            }
        }

        /// <summary>
        /// standard update
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (Active)
            {
                // changes in resolution result in changes in width of the info bar and the button locations 
                // TODO : SET POSITIONS DUE TO SIZE OF TEXT * PADDING
                if (mWidthBackup != Width)
                {
                    mWidthBackup = Width;

                    mWidthDivision = Width / 10;

                    Bounds = new Rectangle(0, 0, Width, 25);

                    mPauseButton.Position = new Vector2(Width - 20, 2.5f);
                    mCivilUnitsWindowButton.Position = new Vector2(mWidthDivision * 0, 2.5f);
                    mResourceWindowButton.Position = new Vector2(mWidthDivision * 1, 2.5f);
                    mEventLogWindowButton.Position = new Vector2(mWidthDivision * 2, 2.5f);
                    mBuildMenuWindowButton.Position = new Vector2(mWidthDivision * 3, 2.5f);
                    mSelectedPlatformWindowButton.Position = new Vector2(mWidthDivision * 4, 2.5f);
                }
                mGameTime = gametime;
                foreach (var item in mInfoBarItemList)
                {
                    item.Update(gametime);
                }
            }
        }

        // the width of the screen and therefore the width of the infoBar
        public int Width { private get; set; }

        // true, if the infoBar should be updated and drawn
        public bool Active { get; set; } // !KEEP PUBLIC, BECAUSE THE STORY MANAGER WILL PROBABLY USE IT!

        // set screentype
        public EScreen Screen { get; }

        // bounds for input manager
        public Rectangle Bounds { get; private set; }

        // the pause button opens the pause menu screen
        private void PauseButtonReleased(object sender, EventArgs eventArgs)
        {
            mScreenManager.AddScreen(mGamePauseScreen);
        }

        // inputmanagement only prevents the input going through the infoBar
        #region input management

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            return !withinBounds;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return !withinBounds;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return !withinBounds;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouse = new Vector2(screenX, screenY);
        }

        #endregion
    }
}
