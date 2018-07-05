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
            mInputManager.AddMousePositionListener(iMouseListener: this);
            mInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.InBoundsOnly, rightClickType: EClickType.InBoundsOnly);
            Bounds = new Rectangle(x: 0,y: 0, width: mgraphics.PreferredBackBufferWidth, height: mgraphics.PreferredBackBufferHeight);

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
                window.Update(gametime: gametime);
            }

            foreach (var infoBox in mInfoBoxList)
            {
                if (infoBox.Active)
                {
                    infoBox.Update(gametime: gametime);
                }
            }

            // TODO : JUST FOR TESTING
            mInfoBar.Update(gametime: gametime);

            #region testing

            // TODO - DELETE TESTING OF POPUPWINDOW
            if (!mActiveWindow)
            {
                if (mPopupWindowList.Contains(item: mTestPopupWindow))
                {
                    mPopupWindowList.Remove(item: mTestPopupWindow);
                }
            }
            else
            {
                foreach (var window in mPopupWindowList)
                {
                    window.Update(gametime: gametime);
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
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: null, depthStencilState: null, rasterizerState: mRasterizerState);

            // draw all windows
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch: spriteBatch);
            }

            // TODO - DELETE TESTING OF POPUPWINDOW
            foreach (var popupWindow in mPopupWindowList)
            {
                popupWindow.Draw(spriteBatch: spriteBatch);
            }

            foreach (var infoBox in mInfoBoxList)
            {
                if (infoBox.Active)
                {
                    infoBox.Draw(spriteBatch: spriteBatch);
                }
            }

            // TODO : JUST FOR TESTING
            mInfoBar.Draw(spriteBatch: spriteBatch);

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans20 = content.Load<SpriteFont>(assetName: "LibSans20");
            mLibSans14 = content.Load<SpriteFont>(assetName: "LibSans14");
            mLibSans12 = content.Load<SpriteFont>(assetName: "LibSans12");

            // Texture Loading
            mBlankPlatformTexture = content.Load<Texture2D>(assetName: "PlatformBasic");
            mOtherPlatformTexture = content.Load<Texture2D>(assetName: "PlatformSpriteSheet");

            // resolution
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;
            mPrevScreenWidth = mCurrentScreenWidth;
            mPrevScreenHeight = mCurrentScreenHeight;

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 0.8f);
            var borderColor = new Color(r: 0.68f, g: 0.933f, b: 0.933f, alpha: .8f);

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
            mInfoBar = new InfoBarWindowObject(borderColor: borderColor, fillColor: windowColor, graphics: mGraphics, spriteFont: mLibSans14, director: mDirector, screenManager: mScreenManager);

            #endregion

            // TODO
            #region eventLogWindow

            mEventLogWindow = new WindowObject(windowName: "// EVENT LOG", position: new Vector2(x: 0, y: 0), size: new Vector2(x: eventLogWidth, y: eventLogHeight), minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

                // create items

                // add all items

                mWindowList.Add(item: mEventLogWindow);

                #endregion

            #region civilUnitsWindow

            mCivilUnitsWindow = new WindowObject(windowName: "// CIVIL UNITS", position: new Vector2(x: 0, y: 0), size: new Vector2(x: civilUnitsWidth, y: civilUnitsHeight), colorBorder: borderColor, colorFill: windowColor, borderPadding: 10, objectPadding: 20, minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

            // create items
            //TODO: Create an object representing the Idle units at the moment. Something like "Idle: 24" should be enough
            mDefTextField = new TextField(text: "Defense", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mDefSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector, withValueBox: true, withPages: true, pages: 5);
            mBuildTextField = new TextField(text: "Build", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mBuildSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector, withValueBox: true, withPages: true, pages: 5);
            mLogisticsTextField = new TextField(text: "Logistics", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mLogisticsSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector, withValueBox: true, withPages: true, pages: 5);
            mProductionTextField = new TextField(text: "Production", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mProductionSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector, withValueBox: true, withPages: true, pages: 5);

            //This instance will handle the comunication between Sliders and DistributionManager.
            var handler = new SliderHandler(director: ref mDirector, def: mDefSlider, prod: mProductionSlider, constr: mBuildSlider, logi: mLogisticsSlider);

            // adding all items
            mCivilUnitsWindow.AddItem(item: mDefTextField);
            mCivilUnitsWindow.AddItem(item: mDefSlider);
            mCivilUnitsWindow.AddItem(item: mBuildTextField);
            mCivilUnitsWindow.AddItem(item: mBuildSlider);
            mCivilUnitsWindow.AddItem(item: mLogisticsTextField);
            mCivilUnitsWindow.AddItem(item: mLogisticsSlider);
            mCivilUnitsWindow.AddItem(item: mProductionTextField);
            mCivilUnitsWindow.AddItem(item: mProductionSlider);

            mWindowList.Add(item: mCivilUnitsWindow);

            #endregion

            // TODO
            #region resourceWindow

            mResourceWindow = new WindowObject(windowName: "// RESOURCES", position: new Vector2(x: 0, y: 0), size: new Vector2(x: resourceWidth, y: resourceHeight), minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

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

            mTextFieldTest = new TextField(text: testText, position: Vector2.One, size: new Vector2(x: resourceWidth - 50, y: resourceHeight), spriteFont: mLibSans12);

            mResourceWindow.AddItem(item: mTextFieldTest);

            #endregion

            mWindowList.Add(item: mResourceWindow);

            #endregion

            // TODO
            #region buildMenuWindow

            mBuildMenuWindow = new WindowObject(windowName: "// BUILDMENU", position: new Vector2(x: 0, y: 0), size: new Vector2(x: buildMenuWidth, y: buildMenuHeight), minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

            #region button definitions

            // blank platform button
            mBlankPlatformButton = new Button(scale: 0.25f, buttonTexture: mBlankPlatformTexture, position: Vector2.Zero, withBorder: true);
            mBlankPlatformButton.ButtonClicked += OnButtonmBlankPlatformClick;
            mBlankPlatformButton.ButtonReleased += OnButtonmBlankPlatformReleased;
            mBlankPlatformButton.ButtonHovering += OnButtonmBlankPlatformHovering;
            mBlankPlatformButton.ButtonHoveringEnd += OnButtonmBlankPlatformHoveringEnd;

            // open basic list button
            mBasicListButton = new Button(scale: 0.25f, buttonTexture: mBlankPlatformTexture, position: Vector2.Zero, withBorder: true);
            mBasicListButton.ButtonClicked += OnButtonmBasicListClick;
            mBasicListButton.ButtonHovering += OnButtonmBasicListHovering;
            mBasicListButton.ButtonHoveringEnd += OnButtonmBasicListHoveringEnd;

            // open special list button
            mSpecialListButton = new Button(scale: 0.25f, buttonTexture: mBlankPlatformTexture, position: Vector2.Zero, withBorder: true);
            mSpecialListButton.ButtonClicked += OnButtonmSpecialListClick;
            mSpecialListButton.ButtonHovering += OnButtonmSpecialListHovering;
            mSpecialListButton.ButtonHoveringEnd += OnButtonmSpecialListHoveringEnd;

            // open military list button
            mMilitaryListButton = new Button(scale: 0.25f, buttonTexture: mBlankPlatformTexture, position: Vector2.Zero, withBorder: true);
            mMilitaryListButton.ButtonClicked += OnButtonmMilitaryListClick;
            mMilitaryListButton.ButtonHovering += OnButtonmMilitaryListHovering;
            mMilitaryListButton.ButtonHoveringEnd += OnButtonmMilitaryListHoveringEnd;


            // junkyard platform button
            mJunkyardPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 2 * 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mJunkyardPlatformButton.ButtonClicked += OnButtonmJunkyardPlatformClick;
            mJunkyardPlatformButton.ButtonReleased += OnButtonmJunkyardPlatformReleased;
            mJunkyardPlatformButton.ButtonHovering += OnButtonmJunkyardPlatformHovering;
            mJunkyardPlatformButton.ButtonHoveringEnd += OnButtonmJunkyardPlatformHoveringEnd;

            // quarry platform button
            mQuarryPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 2 * 175 + 2 * 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mQuarryPlatformButton.ButtonClicked += OnButtonmQuarryPlatformClick;
            mQuarryPlatformButton.ButtonReleased += OnButtonmQuarryPlatformReleased;
            mQuarryPlatformButton.ButtonHovering += OnButtonmQuarryPlatformHovering;
            mQuarryPlatformButton.ButtonHoveringEnd += OnButtonmQuarryPlatformHoveringEnd;

            // mine platform button
            mMinePlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 2 * 175, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mMinePlatformButton.ButtonClicked += OnButtonmMinePlatformClick;
            mMinePlatformButton.ButtonReleased += OnButtonmMinePlatformReleased;
            mMinePlatformButton.ButtonHovering += OnButtonmMinePlatformHovering;
            mMinePlatformButton.ButtonHoveringEnd += OnButtonmMinePlatformHoveringEnd;

            // well platform button
            mWellPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 2 * 175 + 4 * 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mWellPlatformButton.ButtonClicked += OnButtonmWellPlatformClick;
            mWellPlatformButton.ButtonReleased += OnButtonmWellPlatformReleased;
            mWellPlatformButton.ButtonHovering += OnButtonmWellPlatformHovering;
            mWellPlatformButton.ButtonHoveringEnd += OnButtonmWellPlatformHoveringEnd;


            // factory platform button
            mFactoryPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mFactoryPlatformButton.ButtonClicked += OnButtonmFactoryPlatformClick;
            mFactoryPlatformButton.ButtonReleased += OnButtonmFactoryPlatformReleased;
            mFactoryPlatformButton.ButtonHovering += OnButtonmFactoryPlatformHovering;
            mFactoryPlatformButton.ButtonHoveringEnd += OnButtonmFactoryPlatformHoveringEnd;

            // storage platform button
            mStoragePlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 2 * 175 + 3 * 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mStoragePlatformButton.ButtonClicked += OnButtonmStoragePlatformClick;
            mStoragePlatformButton.ButtonReleased += OnButtonmStoragePlatformReleased;
            mStoragePlatformButton.ButtonHovering += OnButtonmStoragePlatformHovering;
            mStoragePlatformButton.ButtonHoveringEnd += OnButtonmStoragePlatformHoveringEnd;

            // powerhouse platform button
            mPowerhousePlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mPowerhousePlatformButton.ButtonClicked += OnButtonmPowerhousePlatformClick;
            mPowerhousePlatformButton.ButtonReleased += OnButtonmPowerhousePlatformReleased;
            mPowerhousePlatformButton.ButtonHovering += OnButtonmPowerhousePlatformHovering;
            mPowerhousePlatformButton.ButtonHoveringEnd += OnButtonmPowerhousePlatformHoveringEnd;

            // commandcenter platform button
            mCommandcenterPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 1 * 175, width: 150, height: 175), position: Vector2.Zero, withBorder: true);
            mCommandcenterPlatformButton.ButtonClicked += OnButtonmCommandcenterPlatformClick;
            mCommandcenterPlatformButton.ButtonReleased += OnButtonmCommandcenterPlatformReleased;
            mCommandcenterPlatformButton.ButtonHovering += OnButtonmCommandcenterPlatformHovering;
            mCommandcenterPlatformButton.ButtonHoveringEnd += OnButtonmCommandcenterPlatformHoveringEnd;


            // armory platform button
            mArmoryPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 2 * 175 + 1 * 130, width: 150, height: 130), position: Vector2.Zero, withBorder: true);
            mArmoryPlatformButton.ButtonClicked += OnButtonmArmoryPlatformClick;
            mArmoryPlatformButton.ButtonReleased += OnButtonmArmoryPlatformReleased;
            mArmoryPlatformButton.ButtonHovering += OnButtonmArmoryPlatformHovering;
            mArmoryPlatformButton.ButtonHoveringEnd += OnButtonmArmoryPlatformHoveringEnd;

            // kineticTower platform button
            mKineticTowerPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130, width: 150, height: 170), position: Vector2.Zero, withBorder: true);
            mKineticTowerPlatformButton.ButtonClicked += OnButtonmKineticTowerPlatformClick;
            mKineticTowerPlatformButton.ButtonReleased += OnButtonmKineticTowerPlatformReleased;
            mKineticTowerPlatformButton.ButtonHovering += OnButtonmKineticTowerPlatformHovering;
            mKineticTowerPlatformButton.ButtonHoveringEnd += OnButtonmKineticTowerPlatformHoveringEnd;

            // laserTower platform button
            mLaserTowerPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 2 * 175 + 3 * 130 + 1 * 175, width: 150, height: 170), position: Vector2.Zero, withBorder: true);
            mLaserTowerPlatformButton.ButtonClicked += OnButtonmLaserTowerPlatformClick;
            mLaserTowerPlatformButton.ButtonReleased += OnButtonmLaserTowerPlatformReleased;
            mLaserTowerPlatformButton.ButtonHovering += OnButtonmLaserTowerPlatformHovering;
            mLaserTowerPlatformButton.ButtonHoveringEnd += OnButtonmLaserTowerPlatformHoveringEnd;

            // barracks platform button
            mBarracksPlatformButton = new Button(scale: 0.25f, buttonTexture: mOtherPlatformTexture, sourceRectangle: new Rectangle(x: 0, y: 0 * 175, width: 150, height: 175), position: Vector2.Zero, withBorder: true);
            mBarracksPlatformButton.ButtonClicked += OnButtonmBarracksPlatformClick;
            mBarracksPlatformButton.ButtonReleased += OnButtonmBarracksPlatformReleased;
            mBarracksPlatformButton.ButtonHovering += OnButtonmBarracksPlatformHovering;
            mBarracksPlatformButton.ButtonHoveringEnd += OnButtonmBarracksPlatformHoveringEnd;

            #endregion

            // TODO : ADD RESSOURCES TO ALL BUILD BUTTONS (ressources need to be created as IWindowItems first)
            #region info when hovering over the building menu buttons

            mInfoBoxList = new List<InfoBoxWindow>();

            // Build Blank Platform info
            var infoBuildBlank = new TextField(text: "Blank Platform",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Blank Platform"),
                spriteFont: mLibSans12);

            mInfoBuildBlank = new InfoBoxWindow(
                itemList: new List<IWindowItem> {infoBuildBlank},
                size: mLibSans12.MeasureString(text: "Blank Platform"),
                borderColor: new Color(r: 0.86f, g: 0.85f, b: 0.86f),
                centerColor: new Color(r: 1f, g: 1f, b: 1f),//(0.75f, 0.75f, 0.75f),
                boundsRectangle: new Rectangle(
                    x: (int) mBlankPlatformButton.Position.X,
                    y: (int) mBlankPlatformButton.Position.Y,
                    width: (int) mBlankPlatformButton.Size.X,
                    height: (int) mBlankPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildBlank);

            // Open Basic List info
            var infoBasicList = new TextField(text: "Basic Building Menu",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Basic Building Menu"),
                spriteFont: mLibSans12);

            mInfoBasicList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBasicList },
                size: mLibSans12.MeasureString(text: "Basic Building Menu"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mBasicListButton.Position.X,
                    y: (int)mBasicListButton.Position.Y,
                    width: (int)mBasicListButton.Size.X,
                    height: (int)mBasicListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBasicList);

            // Open Special List info
            var infoSpecialList = new TextField(text: "Special Building Menu",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Special Building Menu"),
                spriteFont: mLibSans12);

            mInfoSpecialList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoSpecialList },
                size: mLibSans12.MeasureString(text: "Special Building Menu"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mSpecialListButton.Position.X,
                    y: (int)mSpecialListButton.Position.Y,
                    width: (int)mSpecialListButton.Size.X,
                    height: (int)mSpecialListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoSpecialList);

            // Open Military List info
            var infoMilitaryList = new TextField(text: "Military Building Menu",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Military Building Menu"),
                spriteFont: mLibSans12);

            mInfoMilitaryList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMilitaryList },
                size: mLibSans12.MeasureString(text: "Military Building Menu"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mMilitaryListButton.Position.X,
                    y: (int)mMilitaryListButton.Position.Y,
                    width: (int)mMilitaryListButton.Size.X,
                    height: (int)mMilitaryListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoMilitaryList);

            // Build Junkyard info
            var infoJunkyard = new TextField(text: "Build Junkyard",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Junkyard"),
                spriteFont: mLibSans12);

            mInfoBuildJunkyard = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoJunkyard },
                size: mLibSans12.MeasureString(text: "Build Junkyard"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mJunkyardPlatformButton.Position.X,
                    y: (int)mJunkyardPlatformButton.Position.Y,
                    width: (int)mJunkyardPlatformButton.Size.X,
                    height: (int)mJunkyardPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildJunkyard);

            // Build Quarry info
            var infoQuarry = new TextField(text: "Build Quarry",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Quarry"),
                spriteFont: mLibSans12);

            mInfoBuildQuarry = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoQuarry },
                size: mLibSans12.MeasureString(text: "Build Quarry"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mQuarryPlatformButton.Position.X,
                    y: (int)mQuarryPlatformButton.Position.Y,
                    width: (int)mQuarryPlatformButton.Size.X,
                    height: (int)mQuarryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildQuarry);

            // Build Mine info
            var infoMine = new TextField(text: "Build Mine",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Mine"),
                spriteFont: mLibSans12);

            mInfoBuildMine = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMine },
                size: mLibSans12.MeasureString(text: "Build Mine"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mMinePlatformButton.Position.X,
                    y: (int)mMinePlatformButton.Position.Y,
                    width: (int)mMinePlatformButton.Size.X,
                    height: (int)mMinePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildMine);

            // Build Well info
            var infoWell = new TextField(text: "Build Well",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Well"),
                spriteFont: mLibSans12);

            mInfoBuildWell = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoWell },
                size: mLibSans12.MeasureString(text: "Build Well"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mWellPlatformButton.Position.X,
                    y: (int)mWellPlatformButton.Position.Y,
                    width: (int)mWellPlatformButton.Size.X,
                    height: (int)mWellPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildWell);

            // Build Factory info
            var infoFactory = new TextField(text: "Build Factory",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Factory"),
                spriteFont: mLibSans12);

            mInfoBuildFactory = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoFactory },
                size: mLibSans12.MeasureString(text: "Build Factory"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mFactoryPlatformButton.Position.X,
                    y: (int)mFactoryPlatformButton.Position.Y,
                    width: (int)mFactoryPlatformButton.Size.X,
                    height: (int)mFactoryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildFactory);

            // Build Storage info
            var infoStorage = new TextField(text: "Build Storage",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Storage"),
                spriteFont: mLibSans12);

            mInfoBuildStorage = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoStorage },
                size: mLibSans12.MeasureString(text: "Build Storage"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mStoragePlatformButton.Position.X,
                    y: (int)mStoragePlatformButton.Position.Y,
                    width: (int)mStoragePlatformButton.Size.X,
                    height: (int)mStoragePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildStorage);

            // Build Powerhouse info
            var infoPowerhouse = new TextField(text: "Build Powerhouse",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Powerhouse"),
                spriteFont: mLibSans12);

            mInfoBuildPowerhouse = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoPowerhouse },
                size: mLibSans12.MeasureString(text: "Build Powerhouse"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mPowerhousePlatformButton.Position.X,
                    y: (int)mPowerhousePlatformButton.Position.Y,
                    width: (int)mPowerhousePlatformButton.Size.X,
                    height: (int)mPowerhousePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildPowerhouse);

            // Build Commandcenter info
            var infoCommandcenter = new TextField(text: "Build Commandcenter",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Commandcenter"),
                spriteFont: mLibSans12);

            mInfoBuildCommandcenter = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoCommandcenter },
                size: mLibSans12.MeasureString(text: "Build Commandcenter"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mCommandcenterPlatformButton.Position.X,
                    y: (int)mCommandcenterPlatformButton.Position.Y,
                    width: (int)mCommandcenterPlatformButton.Size.X,
                    height: (int)mCommandcenterPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildCommandcenter);

            // Build Armory info
            var infoArmory = new TextField(text: "Build Armory",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Armory"),
                spriteFont: mLibSans12);

            mInfoBuildArmory = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoArmory },
                size: mLibSans12.MeasureString(text: "Build Armory"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mArmoryPlatformButton.Position.X,
                    y: (int)mArmoryPlatformButton.Position.Y,
                    width: (int)mArmoryPlatformButton.Size.X,
                    height: (int)mArmoryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildArmory);

            // Build Kinetic Tower info
            var infoKineticTower = new TextField(text: "Build Kinetic Tower",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Kinetic Tower"),
                spriteFont: mLibSans12);

            mInfoBuildKineticTower = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoKineticTower },
                size: mLibSans12.MeasureString(text: "Build Kinetic Tower"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mKineticTowerPlatformButton.Position.X,
                    y: (int)mKineticTowerPlatformButton.Position.Y,
                    width: (int)mKineticTowerPlatformButton.Size.X,
                    height: (int)mKineticTowerPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildKineticTower);

            // Build Laser Tower info
            var infoLaserTower = new TextField(text: "Build Laser Tower",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Laser Tower"),
                spriteFont: mLibSans12);

            mInfoBuildLaserTower = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoLaserTower },
                size: mLibSans12.MeasureString(text: "Build Laser Tower"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mLaserTowerPlatformButton.Position.X,
                    y: (int)mLaserTowerPlatformButton.Position.Y,
                    width: (int)mLaserTowerPlatformButton.Size.X,
                    height: (int)mLaserTowerPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildLaserTower);

            // Build Barracks info
            var infoBarracks = new TextField(text: "Build Barracks",
                position: Vector2.Zero,
                size: mLibSans12.MeasureString(text: "Build Barracks"),
                spriteFont: mLibSans12);

            mInfoBuildBarracks = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBarracks },
                size: mLibSans12.MeasureString(text: "Build Barracks"),
                borderColor: new Color(r: 0, g: 0, b: 0),
                centerColor: new Color(r: 1f, g: 0.96f, b: 0.9f),
                boundsRectangle: new Rectangle(
                    x: (int)mBarracksPlatformButton.Position.X,
                    y: (int)mBarracksPlatformButton.Position.Y,
                    width: (int)mBarracksPlatformButton.Size.X,
                    height: (int)mBarracksPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(item: mInfoBuildBarracks);

            #endregion

            #region put buttons -> buttonList -> horizontalCollection -> buildmenu -> windowList

            // create lists each containing all the buttons to place in one row
            var topList = new List<IWindowItem> { mBlankPlatformButton, mBasicListButton, mSpecialListButton, mMilitaryListButton };
            var basicList = new List<IWindowItem> { mFactoryPlatformButton, mStoragePlatformButton, mPowerhousePlatformButton, mCommandcenterPlatformButton };
            var specialList = new List<IWindowItem> { mJunkyardPlatformButton, mQuarryPlatformButton, mMinePlatformButton, mWellPlatformButton };
            var militaryList = new List<IWindowItem> { mArmoryPlatformButton, mKineticTowerPlatformButton, mLaserTowerPlatformButton, mBarracksPlatformButton };
            // create the horizontalCollection objects which process the button-row placement
            mButtonTopList = new HorizontalCollection(itemList: topList, size: new Vector2(x: buildMenuWidth - 30, y: mBlankPlatformButton.Size.X), position: Vector2.Zero);
            mButtonBasicList = new HorizontalCollection(itemList: basicList, size: new Vector2(x: buildMenuWidth - 30, y: mBlankPlatformButton.Size.X), position: Vector2.Zero);
            mButtonSpecialList = new HorizontalCollection(itemList: specialList, size: new Vector2(x: buildMenuWidth - 30, y: mBlankPlatformButton.Size.X), position: Vector2.Zero);
            mButtonMilitaryList = new HorizontalCollection(itemList: militaryList, size: new Vector2(x: buildMenuWidth - 30, y: mBlankPlatformButton.Size.X), position: Vector2.Zero);
            // add the all horizontalCollection to the build menu window, but deactivate all but topList
            // (they get activated if the corresponding button is pressed)
            mBuildMenuWindow.AddItem(item: mButtonTopList);
            mBuildMenuWindow.AddItem(item: mButtonBasicList);
            mButtonBasicList.ActiveHorizontalCollection = true;
            mBuildMenuWindow.AddItem(item: mButtonSpecialList);
            mButtonSpecialList.ActiveHorizontalCollection = false;
            mBuildMenuWindow.AddItem(item: mButtonMilitaryList);
            mButtonMilitaryList.ActiveHorizontalCollection = false;

            // add the build menu to the UI's windowList
            mWindowList.Add(item: mBuildMenuWindow);

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

            var testWindowColor = new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 0.8f);
            var testBorderColor = new Color(r: 0.68f, g: 0.933f, b: 0.933f, alpha: 0.8f);

            mOkayButton = new Button(buttonText: "Okay", font: mLibSans12, position: Vector2.Zero, color: borderColor) { Opacity = 1f };

            mActiveWindow = true;

            mOkayButton.ButtonClicked += OnButtonClickOkayButton;

            mTestPopupWindow = new PopupWindow(windowName: "// POPUP", button: mOkayButton , position: new Vector2(x: mCurrentScreenWidth / 2 - 250 / 2, y: mCurrentScreenHeight / 2 - 250 / 2), size: new Vector2(x: 250, y: 250), colorBorder: testBorderColor, colorFill: testWindowColor, spriteFontTitle: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

            mTextFieldTestPopup = new TextField(text: testText2, position: Vector2.One, size: new Vector2(x: resourceWidth - 50, y: resourceHeight), spriteFont: mLibSans12);

            mTestPopupWindow.AddItem(item: mTextFieldTestPopup);

            mPopupWindowList.Add(item: mTestPopupWindow);

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
            mResourceWindow.Position = new Vector2(x: 12, y: mCurrentScreenHeight - 12 - mResourceWindow.Size.Y);

            // civil units position
            mCivilUnitsWindow.Position = new Vector2(x: 12, y: 12 + 25);

            // build menu position
            mBuildMenuWindow.Position = new Vector2(x: mCurrentScreenWidth - 12 - mBuildMenuWindow.Size.X, y: mCurrentScreenHeight - 12 - mBuildMenuWindow.Size.Y);

            // event log position
            mEventLogWindow.Position = new Vector2(x: mCurrentScreenWidth - 12 - mEventLogWindow.Size.X, y: 12 + 25);
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
            mInputManager.RemoveMousePositionListener(iMouseListener: mTestPopupWindow);
            mInputManager.RemoveMouseWheelListener(iMouseWheelListener: mTestPopupWindow);
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
                platformType: EPlatformType.Blank,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Junkyard,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Quarry,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Mine,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Well,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Factory,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Storage,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Energy,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Command,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Kinetic,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Laser,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
                platformType: EPlatformType.Barracks,
                placementType: EPlacementType.MouseFollowAndRoad,
                screen: EScreen.UserInterfaceScreen,
                camera: mCamera,
                director: ref mDirector,
                x: 0f,
                y: 0f,
                resourceMap: mResourceMap);

            mStructureMap.AddPlatformToPlace(platformPlacement: mPlatformToPlace);

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
            mInfoBuildBlank.BoundRectangle = new Rectangle(x: (int)mBlankPlatformButton.Position.X, y: (int)mBlankPlatformButton.Position.Y, width: (int)mBlankPlatformButton.Size.X, height: (int)mBlankPlatformButton.Size.Y);
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
            mInfoBasicList.BoundRectangle = new Rectangle(x: (int)mBasicListButton.Position.X, y: (int)mBasicListButton.Position.Y, width: (int)mBasicListButton.Size.X, height: (int)mBasicListButton.Size.Y);
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
            mInfoSpecialList.BoundRectangle = new Rectangle(x: (int)mSpecialListButton.Position.X, y: (int)mSpecialListButton.Position.Y, width: (int)mSpecialListButton.Size.X, height: (int)mSpecialListButton.Size.Y);
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
            mInfoMilitaryList.BoundRectangle = new Rectangle(x: (int)mMilitaryListButton.Position.X, y: (int)mMilitaryListButton.Position.Y, width: (int)mMilitaryListButton.Size.X, height: (int)mMilitaryListButton.Size.Y);
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
            mInfoBuildJunkyard.BoundRectangle = new Rectangle(x: (int)mJunkyardPlatformButton.Position.X, y: (int)mJunkyardPlatformButton.Position.Y, width: (int)mJunkyardPlatformButton.Size.X, height: (int)mJunkyardPlatformButton.Size.Y);
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
            mInfoBuildQuarry.BoundRectangle = new Rectangle(x: (int)mQuarryPlatformButton.Position.X, y: (int)mQuarryPlatformButton.Position.Y, width: (int)mQuarryPlatformButton.Size.X, height: (int)mQuarryPlatformButton.Size.Y);
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
            mInfoBuildMine.BoundRectangle = new Rectangle(x: (int)mMinePlatformButton.Position.X, y: (int)mMinePlatformButton.Position.Y, width: (int)mMinePlatformButton.Size.X, height: (int)mMinePlatformButton.Size.Y);
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
            mInfoBuildWell.BoundRectangle = new Rectangle(x: (int)mWellPlatformButton.Position.X, y: (int)mWellPlatformButton.Position.Y, width: (int)mWellPlatformButton.Size.X, height: (int)mWellPlatformButton.Size.Y);
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
            mInfoBuildFactory.BoundRectangle = new Rectangle(x: (int)mFactoryPlatformButton.Position.X, y: (int)mFactoryPlatformButton.Position.Y, width: (int)mFactoryPlatformButton.Size.X, height: (int)mFactoryPlatformButton.Size.Y);
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
            mInfoBuildStorage.BoundRectangle = new Rectangle(x: (int)mStoragePlatformButton.Position.X, y: (int)mStoragePlatformButton.Position.Y, width: (int)mStoragePlatformButton.Size.X, height: (int)mStoragePlatformButton.Size.Y);
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
            mInfoBuildPowerhouse.BoundRectangle = new Rectangle(x: (int)mPowerhousePlatformButton.Position.X, y: (int)mPowerhousePlatformButton.Position.Y, width: (int)mPowerhousePlatformButton.Size.X, height: (int)mPowerhousePlatformButton.Size.Y);
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
            mInfoBuildCommandcenter.BoundRectangle = new Rectangle(x: (int)mCommandcenterPlatformButton.Position.X, y: (int)mCommandcenterPlatformButton.Position.Y, width: (int)mCommandcenterPlatformButton.Size.X, height: (int)mCommandcenterPlatformButton.Size.Y);
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
            mInfoBuildArmory.BoundRectangle = new Rectangle(x: (int)mArmoryPlatformButton.Position.X, y: (int)mArmoryPlatformButton.Position.Y, width: (int)mArmoryPlatformButton.Size.X, height: (int)mArmoryPlatformButton.Size.Y);
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
            mInfoBuildKineticTower.BoundRectangle = new Rectangle(x: (int)mKineticTowerPlatformButton.Position.X, y: (int)mKineticTowerPlatformButton.Position.Y, width: (int)mKineticTowerPlatformButton.Size.X, height: (int)mKineticTowerPlatformButton.Size.Y);
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
            mInfoBuildLaserTower.BoundRectangle = new Rectangle(x: (int)mLaserTowerPlatformButton.Position.X, y: (int)mLaserTowerPlatformButton.Position.Y, width: (int)mLaserTowerPlatformButton.Size.X, height: (int)mLaserTowerPlatformButton.Size.Y);
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
            mInfoBuildBarracks.BoundRectangle = new Rectangle(x: (int)mBarracksPlatformButton.Position.X, y: (int)mBarracksPlatformButton.Position.Y, width: (int)mBarracksPlatformButton.Size.X, height: (int)mBarracksPlatformButton.Size.Y);
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
