using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// The UserInterfaceScreen contains everything that is needed for the player's UI
    /// </summary>
    public sealed class UserInterfaceScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        #region memberVariables

        // list of windows to show on the UI
        private readonly List<WindowObject> mWindowList;

        // fonts used for the texts
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans10;
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

        // user interface controller - updates the event log + selected platform window + unit distribution
        private UserInterfaceController mUserInterfaceController;

        // needed to calculate screen-sizes
        private readonly GraphicsDeviceManager mGraphics;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        private readonly StructureMap mStructureMap;

        private readonly ResourceMap mResourceMap;

        private readonly Camera mCamera;

        private bool mCanBuildPlatform;

        private PlatformPlacement mPlatformToPlace;

        #region selectedPlatformWindow members

        // selected platform window
        private WindowObject mSelectedPlatformWindow;

        // selectedPlatform standard position
        private Vector2 mSelectedPlatformWindowStandardPos;

        // textFields of selectedPlatformWindow titles
        private TextField mSelectedPlatformResources;
        private TextField mSelectedPlatformUnitAssignment;
        private TextField mSelectedPlatformActions;

        // textFields of selectedPlatformWindow unit assignment
        private TextField mSelectedPlatformDefTextField;
        private TextField mSelectedPlatformBuildTextField;
        private TextField mSelectedPlatformLogisticsTextField;
        private TextField mSelectedPlatformProductionTextField;

        // TODO : ? CREATE ENERGY ITEM ?


        // resourceItems of selectedPlatformWindow
        private ResourceIWindowItem mSelectedPlatformChips;
        private ResourceIWindowItem mSelectedPlatformConcrete;
        private ResourceIWindowItem mSelectedPlatformCopper;
        private ResourceIWindowItem mSelectedPlatformFuel;
        private ResourceIWindowItem mSelectedPlatformMetal;
        private ResourceIWindowItem mSelectedPlatformOil;
        private ResourceIWindowItem mSelectedPlatformPlastic;
        private ResourceIWindowItem mSelectedPlatformSand;
        private ResourceIWindowItem mSelectedPlatformSilicon;
        private ResourceIWindowItem mSelectedPlatformSteel;
        private ResourceIWindowItem mSelectedPlatformStone;
        private ResourceIWindowItem mSelectedPlatformWater;

        // unitSliders of selectedPlatformWindow
        private Slider mSelectedPlatformDefSlider;
        private Slider mSelectedPlatformBuildSlider;
        private Slider mSelectedPlatformLogisticsSlider;
        private Slider mSelectedPlatformProductionSlider;

        // resourceList
        private List<ResourceIWindowItem> mSelectedPlatformResourceList;

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

        // resourceItems
        private ResourceIWindowItem mResourceItemChip;
        private ResourceIWindowItem mResourceItemConcrete;
        private ResourceIWindowItem mResourceItemCopper;
        private ResourceIWindowItem mResourceItemFuel;
        private ResourceIWindowItem mResourceItemMetal;
        private ResourceIWindowItem mResourceItemOil;
        private ResourceIWindowItem mResourceItemPlastic;
        private ResourceIWindowItem mResourceItemSand;
        private ResourceIWindowItem mResourceItemSilicon;
        private ResourceIWindowItem mResourceItemSteel;
        private ResourceIWindowItem mResourceItemStone;
        private ResourceIWindowItem mResourceItemWater;

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
        public UserInterfaceScreen(ref Director director, GraphicsDeviceManager mgraphics, GameScreen gameScreen)
        {
            mStructureMap = gameScreen.GetMap().GetStructureMap();
            mResourceMap = gameScreen.GetMap().GetResourceMap();
            mCamera = gameScreen.GetCamera();
            mCanBuildPlatform = true;

            mDirector = director;
            mInputManager = director.GetInputManager;
            mGraphics = mgraphics;

            // initialize input manager
            mInputManager.AddMousePositionListener(this);
            mInputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            Bounds = new Rectangle(0,0, mgraphics.PreferredBackBufferWidth, mgraphics.PreferredBackBufferHeight);

            // create the windowList
            mWindowList = new List<WindowObject>();

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // subscribe to user interface controller
            mUserInterfaceController = director.GetUserInterfaceController;
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

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
            mLibSans10 = content.Load<SpriteFont>("LibSans10");

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

            // size of selected platform window
            const float selectedPlatformWidth = 240;
            const float selectedPlatformHeight = 180;

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

            #region selectedPlatformWindow

            mSelectedPlatformWindow = new WindowObject("None", new Vector2(250, 200), new Vector2(selectedPlatformWidth, selectedPlatformHeight), true, mLibSans14, mInputManager, mGraphics);

            // list to add all resource item to be able to iterate through them in the set-method
            mSelectedPlatformResourceList = new List<ResourceIWindowItem>();

            // resource-section-title
            mSelectedPlatformResources = new TextField("Resources", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformResources);
            // resource items
            mSelectedPlatformChips = new ResourceIWindowItem(EResourceType.Chip, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformChips);
            mSelectedPlatformConcrete = new ResourceIWindowItem(EResourceType.Concrete, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformConcrete);
            mSelectedPlatformCopper = new ResourceIWindowItem(EResourceType.Copper, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformCopper);
            mSelectedPlatformFuel = new ResourceIWindowItem(EResourceType.Fuel, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformFuel);
            mSelectedPlatformMetal = new ResourceIWindowItem(EResourceType.Metal, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformMetal);
            mSelectedPlatformOil = new ResourceIWindowItem(EResourceType.Oil, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformOil);
            mSelectedPlatformPlastic = new ResourceIWindowItem(EResourceType.Plastic, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformPlastic);
            mSelectedPlatformSand = new ResourceIWindowItem(EResourceType.Sand, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSand);
            mSelectedPlatformSilicon = new ResourceIWindowItem(EResourceType.Silicon, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSilicon);
            mSelectedPlatformSteel = new ResourceIWindowItem(EResourceType.Steel, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSteel);
            mSelectedPlatformStone = new ResourceIWindowItem(EResourceType.Stone, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformStone);
            mSelectedPlatformWater = new ResourceIWindowItem(EResourceType.Water, 0, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformWater);
            // add all resourcItems to list - 
            mSelectedPlatformResourceList.Add(mSelectedPlatformChips);
            mSelectedPlatformResourceList.Add(mSelectedPlatformConcrete);
            mSelectedPlatformResourceList.Add(mSelectedPlatformCopper);
            mSelectedPlatformResourceList.Add(mSelectedPlatformFuel);
            mSelectedPlatformResourceList.Add(mSelectedPlatformMetal);
            mSelectedPlatformResourceList.Add(mSelectedPlatformOil);
            mSelectedPlatformResourceList.Add(mSelectedPlatformPlastic);
            mSelectedPlatformResourceList.Add(mSelectedPlatformSand);
            mSelectedPlatformResourceList.Add(mSelectedPlatformSilicon);
            mSelectedPlatformResourceList.Add(mSelectedPlatformSteel);
            mSelectedPlatformResourceList.Add(mSelectedPlatformStone);
            mSelectedPlatformResourceList.Add(mSelectedPlatformWater);

            // unit - assignment - section - title
            mSelectedPlatformUnitAssignment = new TextField("Unit Assignments", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformUnitAssignment);
            // unit assignment text + slider
            mSelectedPlatformDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformDefTextField);
            mSelectedPlatformDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformDefSlider);
            mSelectedPlatformBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformBuildTextField);
            mSelectedPlatformBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformBuildSlider);
            mSelectedPlatformLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformLogisticsTextField);
            mSelectedPlatformLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformLogisticsSlider);
            mSelectedPlatformProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformProductionTextField);
            mSelectedPlatformProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformProductionSlider);

            // actions-section-title
            mSelectedPlatformActions = new TextField("Actions", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.White);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformActions);

            // actions TODO

            mWindowList.Add(mSelectedPlatformWindow);

            #endregion

            // TODO
            #region topBarWindow

            // var topBarWindow = new WindowObject("", new Vector2(0, 0), new Vector2(topBarWidth, topBarHeight), borderColor, windowColor, 10f, 10f, false, mLibSans20, mInputManager, mGraphics);

            // create items

            // add all items

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
            mDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);


            //Subscribe Distr to sliders
            //Still need to be implemented. A Container for all the sliders would be very useful!
            //mDefSlider.SliderMoving += mDirector.GetDistributionManager.DefSlider();
            //mBuildSlider.SliderMoving += mDirector.GetDistributionManager.BuildSlider();
            //mLogisticsSlider.SliderMoving += mDirector.GetDistributionManager.LogisticsSlider();
            //mProductionSlider.SliderMoving += mDirector.GetDistributionManager.ProductionSlider();

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

            // create all items (these are simple starting values which will be updated automatically by the UI controller)
            mResourceItemChip = new ResourceIWindowItem(EResourceType.Chip, 10, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemConcrete = new ResourceIWindowItem(EResourceType.Concrete, 20, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemCopper = new ResourceIWindowItem(EResourceType.Copper, 30, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemFuel = new ResourceIWindowItem(EResourceType.Fuel, 5000, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemMetal = new ResourceIWindowItem(EResourceType.Metal, 100, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemOil = new ResourceIWindowItem(EResourceType.Oil, 2343, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemPlastic = new ResourceIWindowItem(EResourceType.Plastic, 4, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSand = new ResourceIWindowItem(EResourceType.Sand, 32, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSilicon = new ResourceIWindowItem(EResourceType.Silicon, 543, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSteel = new ResourceIWindowItem(EResourceType.Steel, 0, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemStone = new ResourceIWindowItem(EResourceType.Stone, 4365, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemWater = new ResourceIWindowItem(EResourceType.Water, 99, new Vector2(mResourceWindow.Size.X - 50, mResourceWindow.Size.Y), mLibSans10);

            // add all items to window
            mResourceWindow.AddItem(mResourceItemWater);
            mResourceWindow.AddItem(mResourceItemSand);
            mResourceWindow.AddItem(mResourceItemOil);
            mResourceWindow.AddItem(mResourceItemMetal);
            mResourceWindow.AddItem(mResourceItemStone);
            mResourceWindow.AddItem(mResourceItemConcrete);
            mResourceWindow.AddItem(mResourceItemSilicon);
            mResourceWindow.AddItem(mResourceItemPlastic);
            mResourceWindow.AddItem(mResourceItemFuel);
            mResourceWindow.AddItem(mResourceItemSteel);
            mResourceWindow.AddItem(mResourceItemCopper);
            mResourceWindow.AddItem(mResourceItemChip);

            // add the window to windowList
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

            var infoBoxBorderColor = new Color(1, 1, 1);
            var infoBoxCenterColor = new Color(0, 0, 0);

            // Build Blank Platform info
            var infoBuildBlank = new TextField("Build Blank Platform",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Blank Platform"),
                mLibSans10, 
                Color.White);
            
            var infoBuildBlankResource1 = new ResourceIWindowItem(EResourceType.Chip, 2, mLibSans10.MeasureString("Build Blank Platform"), mLibSans10);

            mInfoBuildBlank = new InfoBoxWindow(
                itemList: new List<IWindowItem> {infoBuildBlank, infoBuildBlankResource1 },
                size: mLibSans10.MeasureString("Build Blank Platform"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int) mBlankPlatformButton.Position.X,
                    (int) mBlankPlatformButton.Position.Y,
                    (int) mBlankPlatformButton.Size.X,
                    (int) mBlankPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildBlank);

            // Open Basic List info
            var infoBasicList = new TextField("Basic Building Menu",
                Vector2.Zero,
                mLibSans10.MeasureString("Basic Building Menu"),
                mLibSans10,
                Color.White);

            mInfoBasicList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBasicList },
                size: mLibSans10.MeasureString("Basic Building Menu"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mBasicListButton.Position.X,
                    (int)mBasicListButton.Position.Y,
                    (int)mBasicListButton.Size.X,
                    (int)mBasicListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBasicList);

            // Open Special List info
            var infoSpecialList = new TextField("Special Building Menu",
                Vector2.Zero,
                mLibSans10.MeasureString("Special Building Menu"),
                mLibSans10, 
                Color.White);

            mInfoSpecialList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoSpecialList },
                size: mLibSans10.MeasureString("Special Building Menu"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mSpecialListButton.Position.X,
                    (int)mSpecialListButton.Position.Y,
                    (int)mSpecialListButton.Size.X,
                    (int)mSpecialListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoSpecialList);

            // Open Military List info
            var infoMilitaryList = new TextField("Military Building Menu",
                Vector2.Zero,
                mLibSans10.MeasureString("Military Building Menu"),
                mLibSans10, 
                Color.White);

            mInfoMilitaryList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMilitaryList },
                size: mLibSans10.MeasureString("Military Building Menu"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mMilitaryListButton.Position.X,
                    (int)mMilitaryListButton.Position.Y,
                    (int)mMilitaryListButton.Size.X,
                    (int)mMilitaryListButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoMilitaryList);

            // Build Junkyard info
            var infoJunkyard = new TextField("Build Junkyard",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Junkyard"),
                mLibSans10, 
                Color.White);

            mInfoBuildJunkyard = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoJunkyard },
                size: mLibSans10.MeasureString("Build Junkyard"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mJunkyardPlatformButton.Position.X,
                    (int)mJunkyardPlatformButton.Position.Y,
                    (int)mJunkyardPlatformButton.Size.X,
                    (int)mJunkyardPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildJunkyard);

            // Build Quarry info
            var infoQuarry = new TextField("Build Quarry",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Quarry"),
                mLibSans10, 
                Color.White);

            mInfoBuildQuarry = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoQuarry },
                size: mLibSans10.MeasureString("Build Quarry"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mQuarryPlatformButton.Position.X,
                    (int)mQuarryPlatformButton.Position.Y,
                    (int)mQuarryPlatformButton.Size.X,
                    (int)mQuarryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildQuarry);

            // Build Mine info
            var infoMine = new TextField("Build Mine",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Mine"),
                mLibSans10, 
                Color.White);

            mInfoBuildMine = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMine },
                size: mLibSans10.MeasureString("Build Mine"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mMinePlatformButton.Position.X,
                    (int)mMinePlatformButton.Position.Y,
                    (int)mMinePlatformButton.Size.X,
                    (int)mMinePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildMine);

            // Build Well info
            var infoWell = new TextField("Build Well",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Well"),
                mLibSans10, 
                Color.White);

            mInfoBuildWell = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoWell },
                size: mLibSans10.MeasureString("Build Well"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mWellPlatformButton.Position.X,
                    (int)mWellPlatformButton.Position.Y,
                    (int)mWellPlatformButton.Size.X,
                    (int)mWellPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildWell);

            // Build Factory info
            var infoFactory = new TextField("Build Factory",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Factory"),
                mLibSans10,
                Color.White);

            mInfoBuildFactory = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoFactory },
                size: mLibSans10.MeasureString("Build Factory"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mFactoryPlatformButton.Position.X,
                    (int)mFactoryPlatformButton.Position.Y,
                    (int)mFactoryPlatformButton.Size.X,
                    (int)mFactoryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildFactory);

            // Build Storage info
            var infoStorage = new TextField("Build Storage",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Storage"),
                mLibSans10, 
                Color.White);

            mInfoBuildStorage = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoStorage },
                size: mLibSans10.MeasureString("Build Storage"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mStoragePlatformButton.Position.X,
                    (int)mStoragePlatformButton.Position.Y,
                    (int)mStoragePlatformButton.Size.X,
                    (int)mStoragePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildStorage);

            // Build Powerhouse info
            var infoPowerhouse = new TextField("Build Powerhouse",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Powerhouse"),
                mLibSans10, 
                Color.White);

            mInfoBuildPowerhouse = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoPowerhouse },
                size: mLibSans10.MeasureString("Build Powerhouse"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mPowerhousePlatformButton.Position.X,
                    (int)mPowerhousePlatformButton.Position.Y,
                    (int)mPowerhousePlatformButton.Size.X,
                    (int)mPowerhousePlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildPowerhouse);

            // Build Commandcenter info
            var infoCommandcenter = new TextField("Build Commandcenter",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Commandcenter"),
                mLibSans10, 
                Color.White);

            mInfoBuildCommandcenter = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoCommandcenter },
                size: mLibSans10.MeasureString("Build Commandcenter"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mCommandcenterPlatformButton.Position.X,
                    (int)mCommandcenterPlatformButton.Position.Y,
                    (int)mCommandcenterPlatformButton.Size.X,
                    (int)mCommandcenterPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildCommandcenter);

            // Build Armory info
            var infoArmory = new TextField("Build Armory",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Armory"),
                mLibSans10, 
                Color.White);

            mInfoBuildArmory = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoArmory },
                size: mLibSans10.MeasureString("Build Armory"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mArmoryPlatformButton.Position.X,
                    (int)mArmoryPlatformButton.Position.Y,
                    (int)mArmoryPlatformButton.Size.X,
                    (int)mArmoryPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildArmory);

            // Build Kinetic Tower info
            var infoKineticTower = new TextField("Build Kinetic Tower",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Kinetic Tower"),
                mLibSans10, 
                Color.White);

            mInfoBuildKineticTower = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoKineticTower },
                size: mLibSans10.MeasureString("Build Kinetic Tower"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mKineticTowerPlatformButton.Position.X,
                    (int)mKineticTowerPlatformButton.Position.Y,
                    (int)mKineticTowerPlatformButton.Size.X,
                    (int)mKineticTowerPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildKineticTower);

            // Build Laser Tower info
            var infoLaserTower = new TextField("Build Laser Tower",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Laser Tower"),
                mLibSans10, 
                Color.White);

            mInfoBuildLaserTower = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoLaserTower },
                size: mLibSans10.MeasureString("Build Laser Tower"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mLaserTowerPlatformButton.Position.X,
                    (int)mLaserTowerPlatformButton.Position.Y,
                    (int)mLaserTowerPlatformButton.Size.X,
                    (int)mLaserTowerPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildLaserTower);

            // Build Barracks info
            var infoBarracks = new TextField("Build Barracks",
                Vector2.Zero,
                mLibSans10.MeasureString("Build Barracks"),
                mLibSans10, 
                Color.White);

            mInfoBuildBarracks = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBarracks },
                size: mLibSans10.MeasureString("Build Barracks"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boundsRectangle: new Rectangle(
                    (int)mBarracksPlatformButton.Position.X,
                    (int)mBarracksPlatformButton.Position.Y,
                    (int)mBarracksPlatformButton.Size.X,
                    (int)mBarracksPlatformButton.Size.Y),
                boxed: true,
                director: mDirector);

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

            // called once to set positions
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

            mTextFieldTestPopup = new TextField(testText2, Vector2.One, new Vector2(resourceWidth - 50, resourceHeight), mLibSans12, Color.White);

            mTestPopupWindow.AddItem(mTextFieldTestPopup);

            mPopupWindowList.Add(mTestPopupWindow);

            #endregion
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

        /// <summary>
        /// resets defined windows to standard position
        /// </summary>
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
        /// Set the platform's values in the selectedPlatformWindow
        /// </summary>
        /// <param name="type">the platform's type</param>
        /// <param name="resourceAmountList"></param>
        /// <param name="unitAssignmentList"></param>
        /// <param name="actionsList"></param>
        public void SetSelectedPlatformValues(EPlatformType type,
            IReadOnlyList<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentList,
            IPlatformAction[] actionsList)
        {
            // set window type
            mSelectedPlatformWindow.WindowName = type.ToString();

            #region resources

            foreach (var resource in resourceAmountList)
            {
                switch (resource.Type)
                {
                    case EResourceType.Chip:
                        mSelectedPlatformChips.Amount += 1;
                        break;
                    case EResourceType.Concrete:
                        mSelectedPlatformConcrete.Amount += 1;
                        break;
                    case EResourceType.Copper:
                        mSelectedPlatformCopper.Amount += 1;
                        break;
                    case EResourceType.Fuel:
                        mSelectedPlatformFuel.Amount += 1;
                        break;
                    case EResourceType.Metal:
                        mSelectedPlatformMetal.Amount += 1;
                        break;
                    case EResourceType.Oil:
                        mSelectedPlatformOil.Amount += 1;
                        break;
                    case EResourceType.Plastic:
                        mSelectedPlatformPlastic.Amount += 1;
                        break;
                    case EResourceType.Sand:
                        mSelectedPlatformSand.Amount += 1;
                        break;
                    case EResourceType.Silicon:
                        mSelectedPlatformSilicon.Amount += 1;
                        break;
                    case EResourceType.Steel:
                        mSelectedPlatformSteel.Amount += 1;
                        break;
                    case EResourceType.Stone:
                        mSelectedPlatformStone.Amount += 1;
                        break;
                    case EResourceType.Water:
                        mSelectedPlatformWater.Amount += 1;
                        break;
                }
            }

            // deactivate (do not draw) a resource if the platform does not contain any
            foreach (var resource in mSelectedPlatformResourceList)
            {
                if (resource.Amount <= 0)
                {
                    resource.ActiveWindow = false;
                }
                else
                {
                    resource.ActiveWindow = true;
                }
            }

            #endregion

            #region unitAssignments

            // activate defense text/sliders + deactivate production text/sliders if the platform is a defense tower,
            // else deactivate defense text/sliders + activate production text/sliders
            if (type == EPlatformType.Kinetic || type == EPlatformType.Laser)
            {
                mSelectedPlatformDefTextField.ActiveWindow = true;
                mSelectedPlatformDefSlider.ActiveWindow = true;
                mSelectedPlatformProductionTextField.ActiveWindow = false;
                mSelectedPlatformProductionSlider.ActiveWindow = false;

                // TODO : CONNECT SLIDER (MAX + CURRENTVAL) WITH DISTRIBUTION MANAGER
            }
            else
            {
                mSelectedPlatformDefTextField.ActiveWindow = false;
                mSelectedPlatformDefSlider.ActiveWindow = false;
                mSelectedPlatformProductionTextField.ActiveWindow = true;
                mSelectedPlatformProductionSlider.ActiveWindow = true;

                // TODO : CONNECT SLIDER (MAX + CURRENTVAL) WITH DISTRIBUTION MANAGER
            }

            // TODO : CONNECT SLIDER (MAX + CURRENTVAL) WITH DISTRIBUTION MANAGER
            /*
                        mSelectedPlatformBuildTextField;
                        mSelectedPlatformBuildSlider;
                        mSelectedPlatformLogisticsTextField;
                        mSelectedPlatformLogisticsSlider;
            */



            #endregion

            // TODO : SET ACTIONS FOR THE ACTIONS MENU
            #region actions

            foreach (var action in actionsList)
            {
                
            }

            #endregion
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
            // TODO : ? enable deselection of SelectedPlatform when clicking on a part of the screen that is not part of the UI and not a platform ?
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
