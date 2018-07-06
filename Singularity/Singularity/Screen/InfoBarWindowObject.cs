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
    class InfoBarWindowObject : IDraw, IUpdate, IMousePositionListener, IMouseClickListener
    {
        // list of items in info bar
        private readonly List<IWindowItem> mInfoBarItemList;

        // pause button of info bar
        private Button mPauseButton;

        // refined ressources of info bar
        private Button mRefinedRessourcesButton;

        // raw ressources of info bar
        private Button mRawRessourcesButton;

        // civil units of info bar
        private Button mCivilUnitsButton;

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
        /// It is placed on the top of the screen.
        /// </summary>
        /// <param name="borderColor">the color used for the infoBar's border</param>
        /// <param name="fillColor">the color used to fill the infoBar</param>
        /// <param name="graphics"></param>
        /// <param name="spriteFont">font used for buttons and text</param>
        /// <param name="director"></param>
        /// <param name="screenManager"></param>
        public InfoBarWindowObject(Color borderColor, Color fillColor, GraphicsDeviceManager graphics, SpriteFont spriteFont, Director director, IScreenManager screenManager)
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

            // refined ressources TODO : update button text (texture or text)
            mRefinedRessourcesButton = new Button("refined-Res", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mRefinedRessourcesButton);
            // TODO : ADD DROPDOWN MENU mRefinedRessourcesButton.ButtonReleased += RefinedRessourcesButtonReleased;

            // raw ressources TODO : update button text (texture or text)
            mRawRessourcesButton = new Button("raw", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mRawRessourcesButton);
            // TODO : ADD DROPDOWN MENU mRawRessourcesButton.ButtonReleased += RawRessourcesButtonReleased;

            // civil units TODO : is this a button or what object type is it?
            mCivilUnitsButton = new Button("civil units", mSpriteFont, new Vector2(0, 0)) { Opacity = 1f };
            mInfoBarItemList.Add(mCivilUnitsButton);
            // TODO : ADD DROPDOWN MENU mCivilUnitsButton.ButtonReleased += CivilUnitsButtonReleased;

            // add input manager to prevent other objects from behind the infoBar to get called through the infoBar
            director.GetInputManager.AddMousePositionListener(this);
            director.GetInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);

            // TODO : ADD TINY COLOR RECTANGLES BESIDE THE RESSOURCE BUTTONS


            // pause menu screen
            mGamePauseScreen = new GamePauseScreen(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), mScreenManager, mDirector);


            // TODO :
            // possibly replace graphics device manager with vector of screen size, then replacement of Width with entire screen size is needed.
            // The entire Screen size must then be updated in UI to enable the pause menu to be placed in screen center
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
                spriteBatch.DrawString(mSpriteFont, mDirector.GetStoryManager.GetIngameTime().ToString(), new Vector2(mWidthDivision * 7, 2.5f), new Color(1, 0, 0));
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
                    mWidthBackup = Width;

                    mWidthDivision = Width / 10;

                    Bounds = new Rectangle(0, 0, Width, 25);

                    mPauseButton.Position = new Vector2(Width - 20, 2.5f);
                    mCivilUnitsButton.Position = new Vector2(mWidthDivision * 1, 2.5f);
                    mRefinedRessourcesButton.Position = new Vector2(mWidthDivision * 3, 2.5f);
                    mRawRessourcesButton.Position = new Vector2(mWidthDivision * 5, 2.5f);
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
