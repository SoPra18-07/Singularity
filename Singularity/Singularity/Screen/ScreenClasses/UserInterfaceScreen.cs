using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// The UserInterfaceScreen contains everything of the player's UI
    /// </summary>
    internal sealed class UserInterfaceScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        #region memberVariables

        // list of windows to show on the UI
        private readonly List<WindowObject> mWindowList;

        // fonts used for the texts
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;

        // textures
        private Texture2D mBlankPlatformTexture;
        private Texture2D mOtherPlatformTexture;

        // manage input
        private readonly InputManager mInputManager;

        // mouse position
        private float mMouseX;
        private float mMouseY;

        // used to calculate the positions of the windows at the beginning
        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        // save the last resolution -> needed to update the window position when changes in res ingame
        private int mPrevScreenWidth;
        private int mPrevScreenHeight;

        // director
        private Director mDirector;

        // screen manager -- needed for pause menu
        private IScreenManager mScreenManager;

        // needed to calculate screen-sizes
        private readonly GraphicsDeviceManager mGraphics;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        private readonly StructureMap mStructureMap;

        private readonly ResourceMap mResourceMap;

        private readonly Camera mCamera;

        private bool mCanBuildPlatform;

        private PlatformPlacement mPlatformToPlace;

        #region infoBar members

        // info bar
        private InfoBarWindowObject mInfoBar;



        // units of info bar
        // TODO : ??

        #endregion

        #region civilUnitsWindow members

        // civil units window
        private WindowObject mCivilUnitsWindow;

        // standard position
        private Vector2 mCivilUnitsWindowStandardPos;

        // sliders for distribution
        private Slider mDefSlider;
        private Slider mBuildSlider;
        private Slider mLogisticsSlider;
        private Slider mProductionSlider;

        // text for sliders
        private TextField mDefTextField;
        private TextField mBuildTextField;
        private TextField mLogisticsTextField;
        private TextField mProductionTextField;

        #endregion

        #region resourceWindow members
        // TODO : ALL WITH RESOURCE-IWindowItems

        // resource window
        private WindowObject mResourceWindow;

        // standard position
        private Vector2 mResourceWindowStandardPos;

        #endregion

        #region eventLog members
        // TODO : IMPLEMENT EVENT LOG

        // event log window
        private WindowObject mEventLogWindow;

        // standard position
        private Vector2 mEventLogWindowStandardPos;

        #endregion

        #region buildMenuWindow members

        // build menu window
        private WindowObject mBuildMenuWindow;

        // vertical lists
        private HorizontalCollection mButtonTopList;
        private HorizontalCollection mButtonBasicList;
        private HorizontalCollection mButtonSpecialList;
        private HorizontalCollection mButtonMilitaryList;

        // Top Bar buttons
        private Button mBlankPlatformButton;
        private Button mBasicListButton;
        private Button mSpecialListButton;
        private Button mMilitaryListButton;

        // basic list buttons
        private Button mJunkyardPlatformButton;
        private Button mQuarryPlatformButton;
        private Button mMinePlatformButton;
        private Button mWellPlatformButton;

        // special list buttons
        private Button mFactoryPlatformButton;
        private Button mStoragePlatformButton;
        private Button mPowerhousePlatformButton;
        private Button mCommandcenterPlatformButton;

        // military list buttons
        private Button mArmoryPlatformButton;
        private Button mKineticTowerPlatformButton;
        private Button mLaserTowerPlatformButton;
        private Button mBarracksPlatformButton;

        // infoBox of buttons TODO : ADD COSTS
        private InfoBoxWindow mInfoBuildBlank;
        private InfoBoxWindow mInfoBasicList;
        private InfoBoxWindow mInfoSpecialList;
        private InfoBoxWindow mInfoMilitaryList;
        private InfoBoxWindow mInfoBuildJunkyard;
        private InfoBoxWindow mInfoBuildQuarry;
        private InfoBoxWindow mInfoBuildMine;
        private InfoBoxWindow mInfoBuildWell;
        private InfoBoxWindow mInfoBuildFactory;
        private InfoBoxWindow mInfoBuildStorage;
        private InfoBoxWindow mInfoBuildPowerhouse;
        private InfoBoxWindow mInfoBuildCommandcenter;
        private InfoBoxWindow mInfoBuildArmory;
        private InfoBoxWindow mInfoBuildKineticTower;
        private InfoBoxWindow mInfoBuildLaserTower;
        private InfoBoxWindow mInfoBuildBarracks;

        // list of infoBoxes
        private List<InfoBoxWindow> mInfoBoxList;

        #endregion

        #region testing members

        // TODO: DELETE TESTING

        private Button mOkayButton;

        private PopupWindow mTestPopupWindow;
        private bool mActiveWindow;

        private List<PopupWindow> mPopupWindowList;

        private TextField mTextFieldTest;
        private TextField mTextFieldTestPopup;

        #endregion

        #endregion

        /// <summary>
        /// Creates a UserInterface with it's windows
        /// </summary>
        /// <param name="director"></param>
        /// <param name="mgraphics"></param>
        /// <param name="gameScreen"></param>
        /// <param name="stackScreenManager"></param>
        public UserInterfaceScreen(ref Director director, GraphicsDeviceManager mgraphics, GameScreen gameScreen, IScreenManager stackScreenManager)
        {
            mStructureMap = gameScreen.GetMap().GetStructureMap();
            mResourceMap = gameScreen.GetMap().GetResourceMap();
            mCamera = gameScreen.GetCamera();
            mCanBuildPlatform = true;

            mDirector = director;
            mScreenManager = stackScreenManager;
            mInputManager = director.GetInputManager;
            mGraphics = mgraphics;

            // initialize input manager
            mInputManager.AddMousePositionListener(this);
            mInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            Bounds = new Rectangle(0,0, mgraphics.PreferredBackBufferWidth, mgraphics.PreferredBackBufferHeight);

            // create the windowList
            mWindowList = new List<WindowObject>();

            // Initialize scissor window
            mRasterizerState = new RasterizerState { ScissorTestEnable = true };
        }

        public void Update(GameTime gametime)
        {
            // update screen size
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;

            // if the resolution has changed -> reset windows to standard positions
            if (mCurrentScreenWidth != mPrevScreenWidth || mCurrentScreenHeight != mPrevScreenHeight)
            {
                mPrevScreenWidth = mCurrentScreenWidth;
                mPrevScreenHeight = mCurrentScreenHeight;

                // reset position to standard position
                ResetWindowsToStandardPositon();

                // update infoBar width to fit the new resolution
                mInfoBar.Width = mCurrentScreenWidth;
            }

            // update all windows
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
            }

            foreach (var infoBox in mInfoBoxList)
            {
                if (infoBox.Active)
                {
                    infoBox.Update(gametime);
                }
            }

            // TODO : JUST FOR TESTING
            mInfoBar.Update(gametime);

            #region testing

            // TODO - DELETE TESTING OF POPUPWINDOW
            if (!mActiveWindow)
            {
                if (mPopupWindowList.Contains(mTestPopupWindow))
                {
                    mPopupWindowList.Remove(mTestPopupWindow);
                }
            }
            else
            {
                foreach (var window in mPopupWindowList)
                {
                    window.Update(gametime);
                }
            }

            #endregion

            if (mPlatformToPlace != null && mPlatformToPlace.IsFinished())
            {
                mCanBuildPlatform = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            // draw all windows
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch);
            }

            // TODO - DELETE TESTING OF POPUPWINDOW
            foreach (var popupWindow in mPopupWindowList)
            {
                popupWindow.Draw(spriteBatch);
            }

            foreach (var infoBox in mInfoBoxList)
            {
                if (infoBox.Active)
                {
                    infoBox.Draw(spriteBatch);
                }
            }

            // TODO : JUST FOR TESTING
            mInfoBar.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");

            // Texture Loading
            mBlankPlatformTexture = content.Load<Texture2D>("PlatformBasic");
            mOtherPlatformTexture = content.Load<Texture2D>("PlatformSpriteSheet");

            // resolution
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;
            mPrevScreenWidth = mCurrentScreenWidth;
            mPrevScreenHeight = mCurrentScreenHeight;

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            var borderColor = new Color(0.68f, 0.933f, 0.933f, .8f);

            #region windows size calculation

            // position + size of topBar
            const float topBarHeight = 25;
            var topBarWidth = mCurrentScreenWidth;

            // size of resource window
            const float resourceWidth = 240;
            const float resourceHeight = 262;

            // size of civilUnits window
            const float civilUnitsWidth = 240;
            const float civilUnitsHeight = 400;

            // size of buildMenu window
            const float buildMenuWidth = 240;
            const float buildMenuHeight = 170;

            // size of eventLog window
            const float eventLogWidth = 240;
            const float eventLogHeight = 288;

            #endregion

            // TODO
            #region infoBarWindow

            // NOTICE: this window is the only window which is compeletely created and managed in its own class due to very different tasks
            mInfoBar = new InfoBarWindowObject(borderColor, windowColor, mGraphics, mLibSans14, mDirector, mScreenManager);

            #endregion

            // TODO
            #region eventLogWindow

            mEventLogWindow = new WindowObject("// EVENT LOG", new Vector2(0, 0), new Vector2(eventLogWidth, eventLogHeight), true, mLibSans14, mInputManager, mGraphics);

                // create items

                // add all items

                mWindowList.Add(mEventLogWindow);

                #endregion

            #region civilUnitsWindow

            mCivilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(0, 0), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, windowColor, 10, 20, true, mLibSans14, mInputManager, mGraphics);

            // create items
            //TODO: Create an object representing the Idle units at the moment. Something like "Idle: 24" should be enough
            mDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);

            //This instance will handle the comunication between Sliders and DistributionManager.
            var handler = new SliderHandler(ref mDirector, mDefSlider, mProductionSlider, mBuildSlider, mLogisticsSlider);

            // adding all items
            mCivilUnitsWindow.AddItem(mDefTextField);
            mCivilUnitsWindow.AddItem(mDefSlider);
            mCivilUnitsWindow.AddItem(mBuildTextField);
            mCivilUnitsWindow.AddItem(mBuildSlider);
            mCivilUnitsWindow.AddItem(mLogisticsTextField);
            mCivilUnitsWindow.AddItem(mLogisticsSlider);
            mCivilUnitsWindow.AddItem(mProductionTextField);
            mCivilUnitsWindow.AddItem(mProductionSlider);

            mWindowList.Add(mCivilUnitsWindow);

            #endregion

            // TODO
            #region resourceWindow

            mResourceWindow = new WindowObject("// RESOURCES", new Vector2(0, 0), new Vector2(resourceWidth, resourceHeight), true, mLibSans14, mInputManager, mGraphics);

            // create items

            // add all items

            #region testing resourceWindow

            var testText =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In sed nisi venenatis massa vehicula suscipit. " +
                "Morbi scelerisque urna et feugiat sodales. Sed tristique arcu a odio faucibus, sit amet pharetra ligula accumsan. " +
                "Curabitur volutpat, lectus nec lacinia eleifend, erat magna maximus dui, vitae ultrices purus dui at velit. " +
                "Etiam nisl nisl, ultricies ut odio at, pharetra pharetra augue. Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Aenean ante leo, porttitor sed cursus in, rhoncus a risus. Morbi urna tortor, luctus id tincidunt ac, bibendum nec magna. " +
                "Sed fringilla posuere felis, eu blandit lacus posuere nec. Nunc et laoreet lacus. Maecenas ac laoreet lectus. " +
                "Quisque semper feugiat diam, eu posuere mi posuere ac. Vestibulum posuere posuere semper. " +
                "Pellentesque nec lacinia nulla, sit amet scelerisque justo.";

            mTextFieldTest = new TextField(testText, Vector2.One, new Vector2(resourceWidth - 50, resourceHeight), mLibSans12);

            mResourceWindow.AddItem(mTextFieldTest);

            #endregion

            mWindowList.Add(mResourceWindow);

            #endregion

            // TODO
            #region buildMenuWindow

            mBuildMenuWindow = new WindowObject("// BUILDMENU", new Vector2(0, 0), new Vector2(buildMenuWidth, buildMenuHeight), true, mLibSans14, mInputManager, mGraphics);

            #region button definitions

            // blank platform button
            mBlankPlatformButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, true);
            mBlankPlatformButton.ButtonClicked += OnButtonmBlankPlatformClick;
            mBlankPlatformButton.ButtonReleased += OnButtonmBlankPlatformReleased;
            mBlankPlatformButton.ButtonHovering += OnButtonmBlankPlatformHovering;
            mBlankPlatformButton.ButtonHoveringEnd += OnButtonmBlankPlatformHoveringEnd;

            // open basic list button
            mBasicListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, true);
            mBasicListButton.ButtonClicked += OnButtonmBasicListClick;
            mBasicListButton.ButtonHovering += OnButtonmBasicListHovering;
            mBasicListButton.ButtonHoveringEnd += OnButtonmBasicListHoveringEnd;

            // open special list button
            mSpecialListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, true);
            mSpecialListButton.ButtonClicked += OnButtonmSpecialListClick;
            mSpecialListButton.ButtonHovering += OnButtonmSpecialListHovering;
            mSpecialListButton.ButtonHoveringEnd += OnButtonmSpecialListHoveringEnd;

            // open military list button
            mMilitaryListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, true);
            mMilitaryListButton.ButtonClicked += OnButtonmMilitaryListClick;
            mMilitaryListButton.ButtonHovering += OnButtonmMilitaryListHovering;
            mMilitaryListButton.ButtonHoveringEnd += OnButtonmMilitaryListHoveringEnd;


            // junkyard platform button
            mJunkyardPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 2 * 130, 150, 130), Vector2.Zero, true);
            mJunkyardPlatformButton.ButtonClicked += OnButtonmJunkyardPlatformClick;
            mJunkyardPlatformButton.ButtonReleased += OnButtonmJunkyardPlatformReleased;
            mJunkyardPlatformButton.ButtonHovering += OnButtonmJunkyardPlatformHovering;
            mJunkyardPlatformButton.ButtonHoveringEnd += OnButtonmJunkyardPlatformHoveringEnd;

            // quarry platform button
            mQuarryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 2 * 130, 150, 130), Vector2.Zero, true);
            mQuarryPlatformButton.ButtonClicked += OnButtonmQuarryPlatformClick;
            mQuarryPlatformButton.ButtonReleased += OnButtonmQuarryPlatformReleased;
            mQuarryPlatformButton.ButtonHovering += OnButtonmQuarryPlatformHovering;
            mQuarryPlatformButton.ButtonHoveringEnd += OnButtonmQuarryPlatformHoveringEnd;

            // mine platform button
            mMinePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175, 150, 130), Vector2.Zero, true);
            mMinePlatformButton.ButtonClicked += OnButtonmMinePlatformClick;
            mMinePlatformButton.ButtonReleased += OnButtonmMinePlatformReleased;
            mMinePlatformButton.ButtonHovering += OnButtonmMinePlatformHovering;
            mMinePlatformButton.ButtonHoveringEnd += OnButtonmMinePlatformHoveringEnd;

            // well platform button
            mWellPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 4 * 130, 150, 130), Vector2.Zero, true);
            mWellPlatformButton.ButtonClicked += OnButtonmWellPlatformClick;
            mWellPlatformButton.ButtonReleased += OnButtonmWellPlatformReleased;
            mWellPlatformButton.ButtonHovering += OnButtonmWellPlatformHovering;
            mWellPlatformButton.ButtonHoveringEnd += OnButtonmWellPlatformHoveringEnd;


            // factory platform button
            mFactoryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 130, 150, 130), Vector2.Zero, true);
            mFactoryPlatformButton.ButtonClicked += OnButtonmFactoryPlatformClick;
            mFactoryPlatformButton.ButtonReleased += OnButtonmFactoryPlatformReleased;
            mFactoryPlatformButton.ButtonHovering += OnButtonmFactoryPlatformHovering;
            mFactoryPlatformButton.ButtonHoveringEnd += OnButtonmFactoryPlatformHoveringEnd;

            // storage platform button
            mStoragePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 3 * 130, 150, 130), Vector2.Zero, true);
            mStoragePlatformButton.ButtonClicked += OnButtonmStoragePlatformClick;
            mStoragePlatformButton.ButtonReleased += OnButtonmStoragePlatformReleased;
            mStoragePlatformButton.ButtonHovering += OnButtonmStoragePlatformHovering;
            mStoragePlatformButton.ButtonHoveringEnd += OnButtonmStoragePlatformHoveringEnd;

            // powerhouse platform button
            mPowerhousePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175, 150, 130), Vector2.Zero, true);
            mPowerhousePlatformButton.ButtonClicked += OnButtonmPowerhousePlatformClick;
            mPowerhousePlatformButton.ButtonReleased += OnButtonmPowerhousePlatformReleased;
            mPowerhousePlatformButton.ButtonHovering += OnButtonmPowerhousePlatformHovering;
            mPowerhousePlatformButton.ButtonHoveringEnd += OnButtonmPowerhousePlatformHoveringEnd;

            // commandcenter platform button
            mCommandcenterPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 1 * 175, 150, 175), Vector2.Zero, true);
            mCommandcenterPlatformButton.ButtonClicked += OnButtonmCommandcenterPlatformClick;
            mCommandcenterPlatformButton.ButtonReleased += OnButtonmCommandcenterPlatformReleased;
            mCommandcenterPlatformButton.ButtonHovering += OnButtonmCommandcenterPlatformHovering;
            mCommandcenterPlatformButton.ButtonHoveringEnd += OnButtonmCommandcenterPlatformHoveringEnd;


            // armory platform button
            mArmoryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 1 * 130, 150, 130), Vector2.Zero, true);
            mArmoryPlatformButton.ButtonClicked += OnButtonmArmoryPlatformClick;
            mArmoryPlatformButton.ButtonReleased += OnButtonmArmoryPlatformReleased;
            mArmoryPlatformButton.ButtonHovering += OnButtonmArmoryPlatformHovering;
            mArmoryPlatformButton.ButtonHoveringEnd += OnButtonmArmoryPlatformHoveringEnd;

            // kineticTower platform button
            mKineticTowerPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130, 150, 170), Vector2.Zero, true);
            mKineticTowerPlatformButton.ButtonClicked += OnButtonmKineticTowerPlatformClick;
            mKineticTowerPlatformButton.ButtonReleased += OnButtonmKineticTowerPlatformReleased;
            mKineticTowerPlatformButton.ButtonHovering += OnButtonmKineticTowerPlatformHovering;
            mKineticTowerPlatformButton.ButtonHoveringEnd += OnButtonmKineticTowerPlatformHoveringEnd;

            // laserTower platform button
            mLaserTowerPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 1 * 175, 150, 170), Vector2.Zero, true);
            mLaserTowerPlatformButton.ButtonClicked += OnButtonmLaserTowerPlatformClick;
            mLaserTowerPlatformButton.ButtonReleased += OnButtonmLaserTowerPlatformReleased;
            mLaserTowerPlatformButton.ButtonHovering += OnButtonmLaserTowerPlatformHovering;
            mLaserTowerPlatformButton.ButtonHoveringEnd += OnButtonmLaserTowerPlatformHoveringEnd;

            // barracks platform button
            mBarracksPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 0 * 175, 150, 175), Vector2.Zero, true);
            mBarracksPlatformButton.ButtonClicked += OnButtonmBarracksPlatformClick;
            mBarracksPlatformButton.ButtonReleased += OnButtonmBarracksPlatformReleased;
            mBarracksPlatformButton.ButtonHovering += OnButtonmBarracksPlatformHovering;
            mBarracksPlatformButton.ButtonHoveringEnd += OnButtonmBarracksPlatformHoveringEnd;

            #endregion

            // TODO : ADD RESSOURCES TO ALL BUILD BUTTONS (ressources need to be created as IWindowItems first)
            #region info when hovering over the building menu buttons

            mInfoBoxList = new List<InfoBoxWindow>();

            // Build Blank Platform info
            var infoBuildBlank = new TextField("Blank Platform",
                Vector2.Zero,
                mLibSans12.MeasureString("Blank Platform"),
                mLibSans12);

            mInfoBuildBlank = new InfoBoxWindow(
                new List<IWindowItem> {infoBuildBlank},
                mLibSans12.MeasureString("Blank Platform"),
                new Color(0.86f, 0.85f, 0.86f),
                new Color(1f, 1f, 1f),//(0.75f, 0.75f, 0.75f),
                new Rectangle(
                    (int) mBlankPlatformButton.Position.X,
                    (int) mBlankPlatformButton.Position.Y,
                    (int) mBlankPlatformButton.Size.X,
                    (int) mBlankPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildBlank);

            // Open Basic List info
            var infoBasicList = new TextField("Basic Building Menu",
                Vector2.Zero,
                mLibSans12.MeasureString("Basic Building Menu"),
                mLibSans12);

            mInfoBasicList = new InfoBoxWindow(
                new List<IWindowItem> { infoBasicList },
                mLibSans12.MeasureString("Basic Building Menu"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mBasicListButton.Position.X,
                    (int)mBasicListButton.Position.Y,
                    (int)mBasicListButton.Size.X,
                    (int)mBasicListButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBasicList);

            // Open Special List info
            var infoSpecialList = new TextField("Special Building Menu",
                Vector2.Zero,
                mLibSans12.MeasureString("Special Building Menu"),
                mLibSans12);

            mInfoSpecialList = new InfoBoxWindow(
                new List<IWindowItem> { infoSpecialList },
                mLibSans12.MeasureString("Special Building Menu"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mSpecialListButton.Position.X,
                    (int)mSpecialListButton.Position.Y,
                    (int)mSpecialListButton.Size.X,
                    (int)mSpecialListButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoSpecialList);

            // Open Military List info
            var infoMilitaryList = new TextField("Military Building Menu",
                Vector2.Zero,
                mLibSans12.MeasureString("Military Building Menu"),
                mLibSans12);

            mInfoMilitaryList = new InfoBoxWindow(
                new List<IWindowItem> { infoMilitaryList },
                mLibSans12.MeasureString("Military Building Menu"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mMilitaryListButton.Position.X,
                    (int)mMilitaryListButton.Position.Y,
                    (int)mMilitaryListButton.Size.X,
                    (int)mMilitaryListButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoMilitaryList);

            // Build Junkyard info
            var infoJunkyard = new TextField("Build Junkyard",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Junkyard"),
                mLibSans12);

            mInfoBuildJunkyard = new InfoBoxWindow(
                new List<IWindowItem> { infoJunkyard },
                mLibSans12.MeasureString("Build Junkyard"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mJunkyardPlatformButton.Position.X,
                    (int)mJunkyardPlatformButton.Position.Y,
                    (int)mJunkyardPlatformButton.Size.X,
                    (int)mJunkyardPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildJunkyard);

            // Build Quarry info
            var infoQuarry = new TextField("Build Quarry",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Quarry"),
                mLibSans12);

            mInfoBuildQuarry = new InfoBoxWindow(
                new List<IWindowItem> { infoQuarry },
                mLibSans12.MeasureString("Build Quarry"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mQuarryPlatformButton.Position.X,
                    (int)mQuarryPlatformButton.Position.Y,
                    (int)mQuarryPlatformButton.Size.X,
                    (int)mQuarryPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildQuarry);

            // Build Mine info
            var infoMine = new TextField("Build Mine",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Mine"),
                mLibSans12);

            mInfoBuildMine = new InfoBoxWindow(
                new List<IWindowItem> { infoMine },
                mLibSans12.MeasureString("Build Mine"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mMinePlatformButton.Position.X,
                    (int)mMinePlatformButton.Position.Y,
                    (int)mMinePlatformButton.Size.X,
                    (int)mMinePlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildMine);

            // Build Well info
            var infoWell = new TextField("Build Well",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Well"),
                mLibSans12);

            mInfoBuildWell = new InfoBoxWindow(
                new List<IWindowItem> { infoWell },
                mLibSans12.MeasureString("Build Well"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mWellPlatformButton.Position.X,
                    (int)mWellPlatformButton.Position.Y,
                    (int)mWellPlatformButton.Size.X,
                    (int)mWellPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildWell);

            // Build Factory info
            var infoFactory = new TextField("Build Factory",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Factory"),
                mLibSans12);

            mInfoBuildFactory = new InfoBoxWindow(
                new List<IWindowItem> { infoFactory },
                mLibSans12.MeasureString("Build Factory"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mFactoryPlatformButton.Position.X,
                    (int)mFactoryPlatformButton.Position.Y,
                    (int)mFactoryPlatformButton.Size.X,
                    (int)mFactoryPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildFactory);

            // Build Storage info
            var infoStorage = new TextField("Build Storage",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Storage"),
                mLibSans12);

            mInfoBuildStorage = new InfoBoxWindow(
                new List<IWindowItem> { infoStorage },
                mLibSans12.MeasureString("Build Storage"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mStoragePlatformButton.Position.X,
                    (int)mStoragePlatformButton.Position.Y,
                    (int)mStoragePlatformButton.Size.X,
                    (int)mStoragePlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildStorage);

            // Build Powerhouse info
            var infoPowerhouse = new TextField("Build Powerhouse",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Powerhouse"),
                mLibSans12);

            mInfoBuildPowerhouse = new InfoBoxWindow(
                new List<IWindowItem> { infoPowerhouse },
                mLibSans12.MeasureString("Build Powerhouse"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mPowerhousePlatformButton.Position.X,
                    (int)mPowerhousePlatformButton.Position.Y,
                    (int)mPowerhousePlatformButton.Size.X,
                    (int)mPowerhousePlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildPowerhouse);

            // Build Commandcenter info
            var infoCommandcenter = new TextField("Build Commandcenter",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Commandcenter"),
                mLibSans12);

            mInfoBuildCommandcenter = new InfoBoxWindow(
                new List<IWindowItem> { infoCommandcenter },
                mLibSans12.MeasureString("Build Commandcenter"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mCommandcenterPlatformButton.Position.X,
                    (int)mCommandcenterPlatformButton.Position.Y,
                    (int)mCommandcenterPlatformButton.Size.X,
                    (int)mCommandcenterPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildCommandcenter);

            // Build Armory info
            var infoArmory = new TextField("Build Armory",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Armory"),
                mLibSans12);

            mInfoBuildArmory = new InfoBoxWindow(
                new List<IWindowItem> { infoArmory },
                mLibSans12.MeasureString("Build Armory"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mArmoryPlatformButton.Position.X,
                    (int)mArmoryPlatformButton.Position.Y,
                    (int)mArmoryPlatformButton.Size.X,
                    (int)mArmoryPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildArmory);

            // Build Kinetic Tower info
            var infoKineticTower = new TextField("Build Kinetic Tower",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Kinetic Tower"),
                mLibSans12);

            mInfoBuildKineticTower = new InfoBoxWindow(
                new List<IWindowItem> { infoKineticTower },
                mLibSans12.MeasureString("Build Kinetic Tower"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mKineticTowerPlatformButton.Position.X,
                    (int)mKineticTowerPlatformButton.Position.Y,
                    (int)mKineticTowerPlatformButton.Size.X,
                    (int)mKineticTowerPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildKineticTower);

            // Build Laser Tower info
            var infoLaserTower = new TextField("Build Laser Tower",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Laser Tower"),
                mLibSans12);

            mInfoBuildLaserTower = new InfoBoxWindow(
                new List<IWindowItem> { infoLaserTower },
                mLibSans12.MeasureString("Build Laser Tower"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mLaserTowerPlatformButton.Position.X,
                    (int)mLaserTowerPlatformButton.Position.Y,
                    (int)mLaserTowerPlatformButton.Size.X,
                    (int)mLaserTowerPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildLaserTower);

            // Build Barracks info
            var infoBarracks = new TextField("Build Barracks",
                Vector2.Zero,
                mLibSans12.MeasureString("Build Barracks"),
                mLibSans12);

            mInfoBuildBarracks = new InfoBoxWindow(
                new List<IWindowItem> { infoBarracks },
                mLibSans12.MeasureString("Build Barracks"),
                new Color(0, 0, 0),
                new Color(1f, 0.96f, 0.9f),
                new Rectangle(
                    (int)mBarracksPlatformButton.Position.X,
                    (int)mBarracksPlatformButton.Position.Y,
                    (int)mBarracksPlatformButton.Size.X,
                    (int)mBarracksPlatformButton.Size.Y),
                true,
                mDirector);

            mInfoBoxList.Add(mInfoBuildBarracks);

            #endregion

            #region put buttons -> buttonList -> horizontalCollection -> buildmenu -> windowList

            // create lists each containing all the buttons to place in one row
            var topList = new List<IWindowItem> { mBlankPlatformButton, mBasicListButton, mSpecialListButton, mMilitaryListButton };
            var basicList = new List<IWindowItem> { mFactoryPlatformButton, mStoragePlatformButton, mPowerhousePlatformButton, mCommandcenterPlatformButton };
            var specialList = new List<IWindowItem> { mJunkyardPlatformButton, mQuarryPlatformButton, mMinePlatformButton, mWellPlatformButton };
            var militaryList = new List<IWindowItem> { mArmoryPlatformButton, mKineticTowerPlatformButton, mLaserTowerPlatformButton, mBarracksPlatformButton };
            // create the horizontalCollection objects which process the button-row placement
            mButtonTopList = new HorizontalCollection(topList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mButtonBasicList = new HorizontalCollection(basicList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mButtonSpecialList = new HorizontalCollection(specialList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mButtonMilitaryList = new HorizontalCollection(militaryList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            // add the all horizontalCollection to the build menu window, but deactivate all but topList
            // (they get activated if the corresponding button is pressed)
            mBuildMenuWindow.AddItem(mButtonTopList);
            mBuildMenuWindow.AddItem(mButtonBasicList);
            mButtonBasicList.ActiveHorizontalCollection = true;
            mBuildMenuWindow.AddItem(mButtonSpecialList);
            mButtonSpecialList.ActiveHorizontalCollection = false;
            mBuildMenuWindow.AddItem(mButtonMilitaryList);
            mButtonMilitaryList.ActiveHorizontalCollection = false;

            // add the build menu to the UI's windowList
            mWindowList.Add(mBuildMenuWindow);

            #endregion

            #endregion

            ResetWindowsToStandardPositon();

            // TODO : DELETE AFTER SPRINT
            #region testing popup window
            // TODO

            mPopupWindowList = new List<PopupWindow>();

            var testText2 =
                "Neuronale Methoden werden vor allem dann eingesetzt, wenn es darum geht, " +
                "aus schlechten oder verrauschten Daten Informationen zu gewinnen, aber " +
                "auch Algorithmen, die sich neuen Situationen anpassen, also lernen, " +
                "sind typisch fuer die Neuroinformatik. Dabei unterscheidet man grundsaetzlich " +
                "ueberwachtes Lernen und unueberwachtes Lernen, ein Kompromiss zwischen beiden " +
                "Techniken ist das Reinforcement-Lernen. Assoziativspeicher sind eine besondere " +
                "Anwendung neuronaler Methoden, und damit oft Forschungsgegenstand der Neuroinformatik. " +
                "Viele Anwendungen fuer kuenstliche neuronale Netze finden sich auch in der Mustererkennung und vor allem im Bildverstehen.";

            var testWindowColor = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            var testBorderColor = new Color(0.68f, 0.933f, 0.933f, 0.8f);

            mOkayButton = new Button("Okay", mLibSans12, Vector2.Zero, borderColor) { Opacity = 1f };

            mActiveWindow = true;

            mOkayButton.ButtonClicked += OnButtonClickOkayButton;

            mTestPopupWindow = new PopupWindow("// POPUP", mOkayButton , new Vector2(mCurrentScreenWidth / 2 - 250 / 2, mCurrentScreenHeight / 2 - 250 / 2), new Vector2(250, 250), testBorderColor, testWindowColor, mLibSans14, mInputManager, mGraphics);

            mTextFieldTestPopup = new TextField(testText2, Vector2.One, new Vector2(resourceWidth - 50, resourceHeight), mLibSans12);

            mTestPopupWindow.AddItem(mTextFieldTestPopup);

            mPopupWindowList.Add(mTestPopupWindow);

            #endregion

            //DEACTIVATE EVERYTHING TO ACTIVATE IT LATER

            if (GlobalVariables.DebugState)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }

        public bool Loaded { get; set; }

        private void ResetWindowsToStandardPositon()
        {
            // resource window position
            mResourceWindow.Position = new Vector2(12, mCurrentScreenHeight - 12 - mResourceWindow.Size.Y);

            // civil units position
            mCivilUnitsWindow.Position = new Vector2(12, 12 + 25);

            // build menu position
            mBuildMenuWindow.Position = new Vector2(mCurrentScreenWidth - 12 - mBuildMenuWindow.Size.X, mCurrentScreenHeight - 12 - mBuildMenuWindow.Size.Y);

            // event log position
            mEventLogWindow.Position = new Vector2(mCurrentScreenWidth - 12 - mEventLogWindow.Size.X, 12 + 25);
        }

        /// <summary>
        /// Used to Deactivate the UI to activate it later (used by settler)
        /// </summary>
        public void Deactivate()
        {
            foreach (var window in mWindowList)
            {
                window.Active = false;
            }
            //Treat our special snowflake
            mInfoBar.Active = false;
        }

        /// <summary>
        /// Used to Activate the UI. This was thought to be used by the settler when he spawns the CommandCenter.
        /// </summary>
        public void Activate()
        {
            foreach (var window in mWindowList)
            {
                window.Active = true;
            }
            mInfoBar.Active = true;
        }

        // TODO : ADD ALL BUILD PLATFORM ACTIONS
        #region button management

        // TODO : DELETE - POPUP WINDOW TEST
        private void OnButtonClickOkayButton(object sender, EventArgs eventArgs)
        {
            mActiveWindow = false;
            mInputManager.RemoveMousePositionListener(mTestPopupWindow);
            mInputManager.RemoveMouseWheelListener(mTestPopupWindow);
        }

        // mouse click on basic list button opens the basic platform build menu
        private void OnButtonmBasicListClick(object sender, EventArgs eventArgs)
        {
            // on click open the basic platform list + close all open lists
            mButtonBasicList.ActiveHorizontalCollection = true;
            mButtonSpecialList.ActiveHorizontalCollection = false;
            mButtonMilitaryList.ActiveHorizontalCollection = false;
        }

        // mouse click on special list button opens the special platform build menu
        private void OnButtonmSpecialListClick(object sender, EventArgs eventArgs)
        {
            // on click open the basic platform list + close all open lists
            mButtonBasicList.ActiveHorizontalCollection = false;
            mButtonSpecialList.ActiveHorizontalCollection = true;
            mButtonMilitaryList.ActiveHorizontalCollection = false;
        }

        // a click on military button opens the military build menu
        private void OnButtonmMilitaryListClick(object sender, EventArgs eventArgs)
        {
            // on click open the basic platform list + close all open lists
            mButtonBasicList.ActiveHorizontalCollection = false;
            mButtonSpecialList.ActiveHorizontalCollection = false;
            mButtonMilitaryList.ActiveHorizontalCollection = true;
        }

        // TODO : Add Platform placement
        #region platform placement

        private void OnButtonmBlankPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Blank,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;


        }
        private void OnButtonmBlankPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmJunkyardPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Junkyard,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmJunkyardPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmQuarryPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Quarry,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmQuarryPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmMinePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Mine,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmMinePlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmWellPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Well,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmWellPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmFactoryPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Factory,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmFactoryPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmStoragePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Storage,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmStoragePlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmPowerhousePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Energy,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmPowerhousePlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmCommandcenterPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Command,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmCommandcenterPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmArmoryPlatformClick(object sender, EventArgs eventArgs)
        {
            //TODO: implement armory?
        }
        private void OnButtonmArmoryPlatformReleased(object sender, EventArgs eventArgs)
        {
            //TODO: see above
        }

        private void OnButtonmKineticTowerPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Kinetic,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmKineticTowerPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmLaserTowerPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Laser,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmLaserTowerPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        private void OnButtonmBarracksPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new PlatformPlacement(
                EPlatformType.Barracks,
                EPlacementType.MouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }
        private void OnButtonmBarracksPlatformReleased(object sender, EventArgs eventArgs)
        {

        }

        #endregion

        // All build buttons show a little info window mouse is hovering of costs + name of what gets build
        #region buttonHovering Info

        // NOTICE : all following hovering- or hoveringEnd- methods do basically the same:
        //              - hovering: activate infoBox and set Rectangle
        //              - hoveringEnd: deactivate ALL infoBoxes
        private void OnButtonmBlankPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildBlank.Active = true;
            mInfoBuildBlank.BoundRectangle = new Rectangle((int)mBlankPlatformButton.Position.X, (int)mBlankPlatformButton.Position.Y, (int)mBlankPlatformButton.Size.X, (int)mBlankPlatformButton.Size.Y);
        }
        private void OnButtonmBlankPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                    infoBox.Active = false;
            }
        }

        private void OnButtonmBasicListHovering(object sender, EventArgs eventArgs)
        {
            mInfoBasicList.Active = true;
            mInfoBasicList.BoundRectangle = new Rectangle((int)mBasicListButton.Position.X, (int)mBasicListButton.Position.Y, (int)mBasicListButton.Size.X, (int)mBasicListButton.Size.Y);
        }
        private void OnButtonmBasicListHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmSpecialListHovering(object sender, EventArgs eventArgs)
        {
            mInfoSpecialList.Active = true;
            mInfoSpecialList.BoundRectangle = new Rectangle((int)mSpecialListButton.Position.X, (int)mSpecialListButton.Position.Y, (int)mSpecialListButton.Size.X, (int)mSpecialListButton.Size.Y);
        }
        private void OnButtonmSpecialListHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmMilitaryListHovering(object sender, EventArgs eventArgs)
        {
            mInfoMilitaryList.Active = true;
            mInfoMilitaryList.BoundRectangle = new Rectangle((int)mMilitaryListButton.Position.X, (int)mMilitaryListButton.Position.Y, (int)mMilitaryListButton.Size.X, (int)mMilitaryListButton.Size.Y);
        }
        private void OnButtonmMilitaryListHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmJunkyardPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildJunkyard.Active = true;
            mInfoBuildJunkyard.BoundRectangle = new Rectangle((int)mJunkyardPlatformButton.Position.X, (int)mJunkyardPlatformButton.Position.Y, (int)mJunkyardPlatformButton.Size.X, (int)mJunkyardPlatformButton.Size.Y);
        }
        private void OnButtonmJunkyardPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmQuarryPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildQuarry.Active = true;
            mInfoBuildQuarry.BoundRectangle = new Rectangle((int)mQuarryPlatformButton.Position.X, (int)mQuarryPlatformButton.Position.Y, (int)mQuarryPlatformButton.Size.X, (int)mQuarryPlatformButton.Size.Y);
        }
        private void OnButtonmQuarryPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmMinePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildMine.Active = true;
            mInfoBuildMine.BoundRectangle = new Rectangle((int)mMinePlatformButton.Position.X, (int)mMinePlatformButton.Position.Y, (int)mMinePlatformButton.Size.X, (int)mMinePlatformButton.Size.Y);
        }
        private void OnButtonmMinePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmWellPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildWell.Active = true;
            mInfoBuildWell.BoundRectangle = new Rectangle((int)mWellPlatformButton.Position.X, (int)mWellPlatformButton.Position.Y, (int)mWellPlatformButton.Size.X, (int)mWellPlatformButton.Size.Y);
        }
        private void OnButtonmWellPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmFactoryPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildFactory.Active = true;
            mInfoBuildFactory.BoundRectangle = new Rectangle((int)mFactoryPlatformButton.Position.X, (int)mFactoryPlatformButton.Position.Y, (int)mFactoryPlatformButton.Size.X, (int)mFactoryPlatformButton.Size.Y);
        }
        private void OnButtonmFactoryPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmStoragePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildStorage.Active = true;
            mInfoBuildStorage.BoundRectangle = new Rectangle((int)mStoragePlatformButton.Position.X, (int)mStoragePlatformButton.Position.Y, (int)mStoragePlatformButton.Size.X, (int)mStoragePlatformButton.Size.Y);
        }
        private void OnButtonmStoragePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmPowerhousePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildPowerhouse.Active = true;
            mInfoBuildPowerhouse.BoundRectangle = new Rectangle((int)mPowerhousePlatformButton.Position.X, (int)mPowerhousePlatformButton.Position.Y, (int)mPowerhousePlatformButton.Size.X, (int)mPowerhousePlatformButton.Size.Y);
        }
        private void OnButtonmPowerhousePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmCommandcenterPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildCommandcenter.Active = true;
            mInfoBuildCommandcenter.BoundRectangle = new Rectangle((int)mCommandcenterPlatformButton.Position.X, (int)mCommandcenterPlatformButton.Position.Y, (int)mCommandcenterPlatformButton.Size.X, (int)mCommandcenterPlatformButton.Size.Y);
        }
        private void OnButtonmCommandcenterPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmArmoryPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildArmory.Active = true;
            mInfoBuildArmory.BoundRectangle = new Rectangle((int)mArmoryPlatformButton.Position.X, (int)mArmoryPlatformButton.Position.Y, (int)mArmoryPlatformButton.Size.X, (int)mArmoryPlatformButton.Size.Y);
        }
        private void OnButtonmArmoryPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmKineticTowerPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildKineticTower.Active = true;
            mInfoBuildKineticTower.BoundRectangle = new Rectangle((int)mKineticTowerPlatformButton.Position.X, (int)mKineticTowerPlatformButton.Position.Y, (int)mKineticTowerPlatformButton.Size.X, (int)mKineticTowerPlatformButton.Size.Y);
        }
        private void OnButtonmKineticTowerPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmLaserTowerPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildLaserTower.Active = true;
            mInfoBuildLaserTower.BoundRectangle = new Rectangle((int)mLaserTowerPlatformButton.Position.X, (int)mLaserTowerPlatformButton.Position.Y, (int)mLaserTowerPlatformButton.Size.X, (int)mLaserTowerPlatformButton.Size.Y);
        }
        private void OnButtonmLaserTowerPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        private void OnButtonmBarracksPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildBarracks.Active = true;
            mInfoBuildBarracks.BoundRectangle = new Rectangle((int)mBarracksPlatformButton.Position.X, (int)mBarracksPlatformButton.Position.Y, (int)mBarracksPlatformButton.Size.X, (int)mBarracksPlatformButton.Size.Y);
        }
        private void OnButtonmBarracksPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        #endregion

        #endregion

        #region InputManagement

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;
        }

        public Rectangle Bounds { get; }

        public EScreen Screen { get; } = EScreen.UserInterfaceScreen;

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            //
            return true;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            //
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            //
            return true;
        }

        #endregion
    }
}
