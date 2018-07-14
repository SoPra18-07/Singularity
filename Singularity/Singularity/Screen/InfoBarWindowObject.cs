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
    /// The InfoBar is the UI's top bar.
    /// It includes the time, mission time, the pause button and buttons to close the given 6 windows
    /// </summary>
    sealed class InfoBarWindowObject : IDraw, IUpdate, IMouseClickListener
    {
        #region member variables

        // list of items in info bar
        private readonly List<IWindowItem> mInfoBarItemList = new List<IWindowItem>();

        // pause button of info bar
        private readonly Button mPauseButton;

        // toggle civilUnits window active/deactive via button
        private readonly Button mCivilUnitsButton;

        // toggle resource window active/deactive via button
        private readonly Button mResourceButton;

        // toggle eventLog window active/deactive via button
        private readonly Button mEventLogButton;

        // toggle buildMenu window active/deactive via button
        private readonly Button mBuildMenuButton;

        // toggle selectedPlatform window active/deactive via button
        private readonly Button mSelectedPlatformButton;

        // toggle minimap window active/deactive via button
        private readonly Button mMinimapButton;

        // the 6 windows do toggle via buttons
        private readonly WindowObject mCivilUnitsWindow;
        private readonly WindowObject mResourceWindow;
        private readonly WindowObject mEventLogWindow;
        private readonly WindowObject mBuildMenuWindow;
        private readonly WindowObject mSelectedPlatformWindow;
        private readonly WindowObject mMinimapWindow;

        // just a simple division of the infoBar to be able to place it's buttons at one of X possible locations
        private int mWidthPadding;

        // colors for the rectangle
        private readonly Color mBordeColor;
        private readonly Color mFillColor;

        // backup of width to react to resolution changes
        private int mWidthBackup;

        // fonts
        private readonly SpriteFont mSpriteFont;

        // needed for input management
        private readonly Director mDirector;

        // screen management - needed for pause menu
        private readonly IScreenManager mScreenManager;

        // pause menu screen
        private readonly GamePauseScreen mGamePauseScreen;

        #endregion

        /// <summary>
        /// The infoBar is part of the UI. It is placed on the top of the screen
        /// and includes the time, a pause button and buttons to disable the WindowObject it gets in constructor
        /// </summary>
        /// <param name="borderColor">the color used for the infoBar's border</param>
        /// <param name="fillColor">the color used to fill the infoBar</param>
        /// <param name="spriteFont">font used for buttons and text</param>
        /// <param name="screenManager">the games screenManager</param>
        /// <param name="civilUnitsWindow">the civilUnitsWindow to close/open</param>
        /// <param name="resourceWindow">the resourceWindow to close/open</param>
        /// <param name="eventLogWindow">the eventLogWindow to close/open</param>
        /// <param name="buildMenuWindow">the buildMenuWindow to close/open</param>
        /// <param name="selectedPlatformWindow">the selectedPlatformWindow to close/open</param>
        /// <param name="minimapWindow">the minimapWindow to close/open</param>
        /// <param name="director">director</param>
        public InfoBarWindowObject(
            Color borderColor,
            Color fillColor,
            SpriteFont spriteFont,
            IScreenManager screenManager,
            WindowObject civilUnitsWindow,
            WindowObject resourceWindow,
            WindowObject eventLogWindow,
            WindowObject buildMenuWindow,
            WindowObject selectedPlatformWindow,
            WindowObject minimapWindow,
            Director director)
        {
            // set member variables - for further commenting see declaration of member variables
            mBordeColor = borderColor;
            mFillColor = fillColor;
            mSpriteFont = spriteFont;
            mScreenManager = screenManager;
            mDirector = director;
            mCivilUnitsWindow = civilUnitsWindow;
            mResourceWindow = resourceWindow;
            mEventLogWindow = eventLogWindow;
            mBuildMenuWindow = buildMenuWindow;
            mSelectedPlatformWindow = selectedPlatformWindow;
            mMinimapWindow = minimapWindow;

            // set to zero to force the update 'change-in-res'-call once (because width and backup are different from the beginning)
            mWidthBackup = 0;

            // divide the width to get a padding between the buttons
            mWidthPadding = Width / 25;

            // set starting values
            Screen = EScreen.UserInterfaceScreen;
            Width = director.GetGraphicsDeviceManager.PreferredBackBufferWidth;
            Active = true;

            // the entire infoBar
            Bounds = new Rectangle(0, 0, Width, 25);

            // NOTICE : all buttons can start with position (0,0) since they will be positioned at the first update-call
            // pause button
            mPauseButton = new Button(" ll ", mSpriteFont, new Vector2(0, 0)) {Opacity = 1f};
            mInfoBarItemList.Add(mPauseButton);
            mPauseButton.ButtonReleased += PauseButtonReleased;

            // toggle windows buttons + adding to the itemList
            mCivilUnitsButton = new Button("units", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mCivilUnitsButton);
            mCivilUnitsButton.ButtonReleased += TogglerCivilUnits;

            mResourceButton = new Button("resource", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mResourceButton);
            mResourceButton.ButtonReleased += TogglerResource;

            mEventLogButton = new Button("eventLog", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mEventLogButton);
            mEventLogButton.ButtonReleased += TogglerEventLog;

            mBuildMenuButton = new Button("build", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mBuildMenuButton);
            mBuildMenuButton.ButtonReleased += TogglerBuildMenu;

            mSelectedPlatformButton = new Button("platform", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mSelectedPlatformButton);
            mSelectedPlatformButton.ButtonReleased += TogglerSelectedPlatform;

            mMinimapButton = new Button("minimap", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mMinimapButton);
            mMinimapButton.ButtonReleased += TogglerMinimap;

            // add input manager to prevent other objects from behind the infoBar to get input through the infoBar
            director.GetInputManager.FlagForAddition(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);

            // pause menu screen
            mGamePauseScreen = new GamePauseScreen(new Vector2(director.GetGraphicsDeviceManager.PreferredBackBufferWidth, director.GetGraphicsDeviceManager.PreferredBackBufferHeight), mScreenManager, mDirector);
        }

        /// <summary>
        /// standard draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                // draw the infoBar rectangle
                spriteBatch.StrokedRectangle(new Vector2(0, 0), new Vector2(Width, 25), mBordeColor, mFillColor, 1f, 1f);

                // draw the pause button rectangle
                spriteBatch.StrokedRectangle(new Vector2(Width - 25, 0), new Vector2(30, 25), mBordeColor, mFillColor, 1f, 1f);

                // draw all buttons + times
                foreach (var item in mInfoBarItemList)
                {
                    item.Draw(spriteBatch);
                }

                // draw the current time
                spriteBatch.DrawString(mSpriteFont, DateTime.Now.ToShortTimeString(), new Vector2(Width - 30 - 80, 2.5f), new Color(0,0,0));

                // draw the mission time
                spriteBatch.DrawString(mSpriteFont, mDirector.GetClock.GetIngameTime().ToString(), new Vector2(Width - 30 - 300, 2.5f), new Color(1, 0, 0));
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
                if (mWidthBackup != Width)
                {
                    // update backup
                    mWidthBackup = Width;

                    // set padding between the buttons
                    mWidthPadding = Width / 25;

                    // update Bounds
                    Bounds = new Rectangle(0, 0, Width, 25);

                    // update pause button position
                    mPauseButton.Position = new Vector2(Width - 20, 2.5f);

                    // update all other button positions
                    mCivilUnitsButton.Position = new Vector2(mWidthPadding, 2.5f);
                    mResourceButton.Position = new Vector2(mCivilUnitsButton.Position.X + mCivilUnitsButton.Size.X + mWidthPadding, 2.5f);
                    mEventLogButton.Position = new Vector2(mResourceButton.Position.X + mResourceButton.Size.X + mWidthPadding, 2.5f);
                    mBuildMenuButton.Position = new Vector2(mEventLogButton.Position.X + mEventLogButton.Size.X + mWidthPadding, 2.5f);
                    mSelectedPlatformButton.Position = new Vector2(mBuildMenuButton.Position.X + mBuildMenuButton.Size.X + mWidthPadding, 2.5f);
                    mMinimapButton.Position = new Vector2(mSelectedPlatformButton.Position.X + mSelectedPlatformButton.Size.X + mWidthPadding, 2.5f);
                }

                // update all buttons
                foreach (var item in mInfoBarItemList)
                {
                    item.Update(gametime);
                }
            }
        }

        // the width of the screen and therefore the width of the infoBar
        public int Width { private get; set; }

        // true, if the infoBar should be updated and drawn
        public bool Active { private get; set; }

        // set screentype
        public EScreen Screen { get; }

        // bounds for input manager
        public Rectangle Bounds { get; private set; }

        #region button management

        // the pause button opens the pause menu screen
        private void PauseButtonReleased(object sender, EventArgs eventArgs)
        {
            mScreenManager.AddScreen(mGamePauseScreen);
        }

        // toggles the civilUnits window opened/closed
        private void TogglerCivilUnits(object sender, EventArgs eventArgs)
        {
            mCivilUnitsWindow.Active = !mCivilUnitsWindow.Active;
        }

        // toggles the resource window opened/closed
        private void TogglerResource(object sender, EventArgs eventArgs)
        {
            mResourceWindow.Active = !mResourceWindow.Active;
        }

        // toggles the eventLog window opened/closed
        private void TogglerEventLog(object sender, EventArgs eventArgs)
        {
            mEventLogWindow.Active = !mEventLogWindow.Active;
        }

        // toggles the buildMenu window opened/closed
        private void TogglerBuildMenu(object sender, EventArgs eventArgs)
        {
            mBuildMenuWindow.Active = !mBuildMenuWindow.Active;
        }

        // toggles the selectedPlatform window opened/closed
        private void TogglerSelectedPlatform(object sender, EventArgs eventArgs)
        {
            mSelectedPlatformWindow.Active = !mSelectedPlatformWindow.Active;
        }

        // toggles the minimap window opened/closed
        private void TogglerMinimap(object sender, EventArgs eventArgs)
        {
            mMinimapWindow.Active = !mMinimapWindow.Active;
        }

        #endregion

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

        #endregion
    }
}
