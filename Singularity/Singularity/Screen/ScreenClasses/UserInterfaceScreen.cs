using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.PlatformActions;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// The UserInterfaceScreen contains everything that is needed for the player's UI
    /// </summary>
    public sealed class UserInterfaceScreen : IScreen, IMouseClickListener
    {
        #region memberVariables

        #region members used by several windows

        // list of windows to show on the UI
        private readonly List<WindowObject> mWindowList;

        // fonts used for the texts
        private SpriteFont mLibSans10;
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;

        // textures
        private Texture2D mBlankPlatformTexture;
        private Texture2D mOtherPlatformTexture;

        // manage input
        private readonly InputManager mInputManager;

        // used to calculate the positions of the windows at the beginning
        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        // save the last resolution -> needed to update the window position when changes in res ingame
        private int mPrevScreenWidth;
        private int mPrevScreenHeight;

        // director
        private Director mDirector;

        // screen manager -- needed for pause menu
        private readonly IScreenManager mScreenManager;

        // user interface controller - updates the event log + selected platform window + unit distribution
        private readonly UserInterfaceController mUserInterfaceController;

        // TODO : REPLACE GRAPHICS WITH DIRECTOR EVERYWHERE
        // needed to calculate screen-sizes
        private readonly GraphicsDeviceManager mGraphics;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        #endregion

        #region platform placement

        private readonly StructureMap mStructureMap;

        private readonly ResourceMap mResourceMap;

        private readonly Map.Map mMap;

        private readonly Camera mCamera;

        private bool mCanBuildPlatform;

        private PlatformPlacement mPlatformToPlace;

        #endregion

        #region selectedPlatformWindow members

        // selected platform window
        private WindowObject mSelectedPlatformWindow;

        // button + rightAlignedIWindowItem for activation/deactivation of selectedPlatform
        private Button mSelectedPlatformDeactivatePlatformButton;
        private Button mSelectedPlatformActivatePlatformButton;
        private ActivationIWindowItem mSelectedPlatformActiveItem;

        // auto deactivated platform textfield
        private TextField mSelectedPlatformIsAutoDeactivatedText;

        // textFields of selectedPlatformWindow titles
        private Button mSelectedPlatformResourcesButton;
        private Button mSelectedPlatformUnitAssignmentButton;
        private Button mSelectedPlatformActionsButton;

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

        // lists of items - used to iterate through all items of a specific kind (for example to deactivate all if the corresponding button was toggled)
        private List<ResourceIWindowItem> mSelectedPlatformResourcesList;
        private List<IWindowItem> mSelectedPlatformUnitAssignmentList;
        private List<PlatformActionIWindowItem> mSelectedPlatformActionList;

        // actions of selectedPlatformWindow
        private PlatformActionIWindowItem mMakeFastMilitaryAction;
        private PlatformActionIWindowItem mMakeStrongMilitaryAction;
        private PlatformActionIWindowItem mProduceWellResourceAction;
        private PlatformActionIWindowItem mProduceQuarryResourceAction;
        private PlatformActionIWindowItem mProduceMineResourceAction;

        // bools if the platformactions have already been added to the selectedplatformwindow
        private bool mFastMilitaryAdded;
        private bool mStronggMilitaryAdded;
        private bool mProduceWellResourceAdded;
        private bool mProduceQuarryResourceAdded;
        private bool mProduceMineResourceAdded;

        // save id to reset the scroll-value if the id changes
        private int selectedPlatformId;

        #endregion

        #region civilUnitsWindow members

        // civil units window
        private WindowObject mCivilUnitsWindow;

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

        // text and amount item for idle units
        private TextAndAmountIWindowItem mIdleUnitsTextAndAmount;

        #endregion

        #region resourceWindow members
        // TODO : ALL WITH RESOURCE-IWindowItems

        // resource window
        private WindowObject mResourceWindow;

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

        // TODO : IMPLEMENT EVENT LOG
        #region eventLog members

        // event log window
        private WindowObject mEventLogWindow;

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

        #region minimap members

        // minimap window
        private WindowObject mMinimapWindow;

        #endregion

        #region infoBar members

        // info bar - the info bar is entirely managed in it's own class, therefore there's no need to define members here
        private InfoBarWindowObject mInfoBar;

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
            mMap = gameScreen.GetMap();
            mStructureMap = gameScreen.GetMap().GetStructureMap();
            mResourceMap = gameScreen.GetMap().GetResourceMap();
            mCamera = gameScreen.GetCamera();
            mCanBuildPlatform = true;

            mDirector = director;
            mScreenManager = stackScreenManager;
            mInputManager = director.GetInputManager;
            mGraphics = mgraphics;

            // initialize input manager
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
            mCurrentScreenWidth = mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth;
            mCurrentScreenHeight = mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight;

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

            // update all infoBoxes
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Update(gametime);
            }

            // update all windows
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
            }

            // update the infoBar
            mInfoBar.Update(gametime);

            if (mPlatformToPlace != null && mPlatformToPlace.IsFinished())
            {
                mCanBuildPlatform = true;
            }

            // update the idle units amount
            mIdleUnitsTextAndAmount.Amount = mUserInterfaceController.GetIdleUnits();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            // draw all windows
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch);
            }

            foreach (var infoBox in mInfoBoxList)
            {
                if (infoBox.Active)
                {
                    infoBox.Draw(spriteBatch);
                }
            }

            // draw the info bar
            mInfoBar.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
            mLibSans10 = content.Load<SpriteFont>("LibSans10");

            // Texture Loading
            mBlankPlatformTexture = content.Load<Texture2D>("PlatformBasic");
            mOtherPlatformTexture = content.Load<Texture2D>("PlatformSpriteSheet");

            // set resolution values TODO : GET GRAPHICS FROM DIRECTOR
            mCurrentScreenWidth = mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth;
            mCurrentScreenHeight = mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight;
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

            // create the window object
            mSelectedPlatformWindow = new WindowObject("No Selection", new Vector2(250, 200), new Vector2(selectedPlatformWidth, selectedPlatformHeight), true, mLibSans14, mInputManager, mGraphics);

            // list to add all item to be able to iterate through them
            mSelectedPlatformResourcesList = new List<ResourceIWindowItem>();
            mSelectedPlatformUnitAssignmentList = new List<IWindowItem>();
            mSelectedPlatformActionList = new List<PlatformActionIWindowItem>();

            // activate / deactivate platform item
            mSelectedPlatformDeactivatePlatformButton = new Button("Deactivate", mLibSans12, Vector2.Zero, Color.White) {Opacity = 1f};
            mSelectedPlatformDeactivatePlatformButton.ButtonReleased += SelectedPlatformDeactivate;
            mSelectedPlatformActivatePlatformButton = new Button("Activate", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformActivatePlatformButton.ButtonReleased += SelectedPlatformActivate;
            mSelectedPlatformActiveItem = new ActivationIWindowItem(mSelectedPlatformDeactivatePlatformButton, mSelectedPlatformActivatePlatformButton,selectedPlatformWidth, ref mDirector);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformActiveItem);

            // auto-deactivaed textfield
            mSelectedPlatformIsAutoDeactivatedText = new TextField("FORCE DEACTIVATED", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.Red);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformIsAutoDeactivatedText);
            mSelectedPlatformIsAutoDeactivatedText.ActiveInWindow = false;

            // resource-section-title
            //mSelectedPlatformResourcesButton = new TextField("Resources", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, 0), mLibSans12, Color.White);
            mSelectedPlatformResourcesButton = new Button("Resources", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformResourcesButton.ButtonReleased += CloseResourcesInSelectedWindow;
            mSelectedPlatformWindow.AddItem(mSelectedPlatformResourcesButton);
            // resource items
            mSelectedPlatformChips = new ResourceIWindowItem(EResourceType.Chip, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformChips);
            mSelectedPlatformConcrete = new ResourceIWindowItem(EResourceType.Concrete, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformConcrete);
            mSelectedPlatformCopper = new ResourceIWindowItem(EResourceType.Copper, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformCopper);
            mSelectedPlatformFuel = new ResourceIWindowItem(EResourceType.Fuel, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformFuel);
            mSelectedPlatformMetal = new ResourceIWindowItem(EResourceType.Metal, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformMetal);
            mSelectedPlatformOil = new ResourceIWindowItem(EResourceType.Oil, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformOil);
            mSelectedPlatformPlastic = new ResourceIWindowItem(EResourceType.Plastic, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformPlastic);
            mSelectedPlatformSand = new ResourceIWindowItem(EResourceType.Sand, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSand);
            mSelectedPlatformSilicon = new ResourceIWindowItem(EResourceType.Silicon, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSilicon);
            mSelectedPlatformSteel = new ResourceIWindowItem(EResourceType.Steel, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformSteel);
            mSelectedPlatformStone = new ResourceIWindowItem(EResourceType.Stone, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformStone);
            mSelectedPlatformWater = new ResourceIWindowItem(EResourceType.Water, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformWater);
            // add all resourcItems to resourceList - used to iterate through all items
            mSelectedPlatformResourcesList.Add(mSelectedPlatformChips);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformConcrete);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformCopper);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformFuel);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformMetal);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformOil);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformPlastic);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformSand);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformSilicon);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformSteel);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformStone);
            mSelectedPlatformResourcesList.Add(mSelectedPlatformWater);

            // unit assignment - section + title
            //mSelectedPlatformUnitAssignmentButton = new TextField("Unit Assignments", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.White);
            mSelectedPlatformUnitAssignmentButton = new Button("Unit Assignments", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformUnitAssignmentButton.ButtonReleased += CloseUnitAssignmentsInSelectedWindow;
            mSelectedPlatformWindow.AddItem(mSelectedPlatformUnitAssignmentButton);
            // unit assignment text + slider
            mSelectedPlatformDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformDefTextField);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformDefTextField);
            mSelectedPlatformDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector, true, true, 5);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformDefSlider);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformDefSlider);
            mSelectedPlatformBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformBuildTextField);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformBuildTextField);
            mSelectedPlatformBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformBuildSlider);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformBuildSlider);
            mSelectedPlatformLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformLogisticsTextField);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformLogisticsTextField);
            mSelectedPlatformLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformLogisticsSlider);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformLogisticsSlider);
            mSelectedPlatformProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans10, Color.White);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformProductionTextField);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformProductionTextField);
            mSelectedPlatformProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformProductionSlider);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformProductionSlider);

            // actions-section-title
            //mSelectedPlatformActionsButton = new TextField("Actions", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.White);
            mSelectedPlatformActionsButton = new Button("Actions", mLibSans12, Vector2.Zero, Color.White) {Opacity = 1f};
            mSelectedPlatformActionsButton.ButtonReleased += CloseActionsInSelectedWindow;
            mSelectedPlatformWindow.AddItem(mSelectedPlatformActionsButton);

            // deactivate all items from selectedPlatformWindow since no platform is selected
            mSelectedPlatformActivatePlatformButton.ActiveInWindow = false;
            mSelectedPlatformDeactivatePlatformButton.ActiveInWindow = false;
            foreach (var item in mSelectedPlatformActionList)
            {
                item.ActiveInWindow = false;
            }

            foreach (var item in mSelectedPlatformResourcesList)
            {
                item.ActiveInWindow = false;
            }

            foreach (var item in mSelectedPlatformUnitAssignmentList)
            {
                item.ActiveInWindow = false;
            }

            mWindowList.Add(mSelectedPlatformWindow);

            #endregion

            // TODO : IMPLEMENT THE EVENT LOG
            #region eventLogWindow

            mEventLogWindow = new WindowObject("// EVENT LOG", new Vector2(0, 0), new Vector2(eventLogWidth, eventLogHeight), true, mLibSans14, mInputManager, mGraphics);
            // create items

            // add all items

            mWindowList.Add(mEventLogWindow);

            #endregion

            #region civilUnitsWindow

            mCivilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(0, 0), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, windowColor, 10, 20, true, mLibSans14, mInputManager, mGraphics);

            // create items
            mDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);

            mIdleUnitsTextAndAmount = new TextAndAmountIWindowItem("Idle", 0, Vector2.Zero, new Vector2(civilUnitsWidth, 0), mLibSans12, Color.White );

            //This instance will handle the comunication between Sliders and DistributionManager.
            var handler = new SliderHandler(ref mDirector, mDefSlider, mProductionSlider, mBuildSlider, mLogisticsSlider);

            // adding all items
            mCivilUnitsWindow.AddItem(mIdleUnitsTextAndAmount);
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

            // TODO : WHAT IS THE RESOURCE WINDOW SUPPOSED TO SHOW ? - IMPLEMENT IT
            #region resourceWindow

            mResourceWindow = new WindowObject("// RESOURCES", new Vector2(0, 0), new Vector2(resourceWidth, resourceHeight), true, mLibSans14, mInputManager, mGraphics);

            // create all items (these are simple starting values which will be updated automatically by the UI controller)
            mResourceItemChip = new ResourceIWindowItem(EResourceType.Chip, 10, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemConcrete = new ResourceIWindowItem(EResourceType.Concrete, 20, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemCopper = new ResourceIWindowItem(EResourceType.Copper, 30, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemFuel = new ResourceIWindowItem(EResourceType.Fuel, 5000, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemMetal = new ResourceIWindowItem(EResourceType.Metal, 100, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemOil = new ResourceIWindowItem(EResourceType.Oil, 2343, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemPlastic = new ResourceIWindowItem(EResourceType.Plastic, 4, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSand = new ResourceIWindowItem(EResourceType.Sand, 32, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSilicon = new ResourceIWindowItem(EResourceType.Silicon, 543, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSteel = new ResourceIWindowItem(EResourceType.Steel, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemStone = new ResourceIWindowItem(EResourceType.Stone, 4365, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemWater = new ResourceIWindowItem(EResourceType.Water, 99, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);

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

            #region buildMenuWindow

            mBuildMenuWindow = new WindowObject("// BUILDMENU", new Vector2(0, 0), new Vector2(buildMenuWidth, buildMenuHeight), true, mLibSans14, mInputManager, mGraphics);

            #region button definitions

            // blank platform button
            mBlankPlatformButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, false);
            mBlankPlatformButton.ButtonClicked += OnButtonmBlankPlatformClick;
            mBlankPlatformButton.ButtonReleased += OnButtonmBlankPlatformReleased;
            mBlankPlatformButton.ButtonHovering += OnButtonmBlankPlatformHovering;
            mBlankPlatformButton.ButtonHoveringEnd += OnButtonmBlankPlatformHoveringEnd;

            // open basic list button
            mBasicListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, false);
            mBasicListButton.ButtonClicked += OnButtonmBasicListClick;
            mBasicListButton.ButtonHovering += OnButtonmBasicListHovering;
            mBasicListButton.ButtonHoveringEnd += OnButtonmBasicListHoveringEnd;

            // open special list button
            mSpecialListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, false);
            mSpecialListButton.ButtonClicked += OnButtonmSpecialListClick;
            mSpecialListButton.ButtonHovering += OnButtonmSpecialListHovering;
            mSpecialListButton.ButtonHoveringEnd += OnButtonmSpecialListHoveringEnd;

            // open military list button
            mMilitaryListButton = new Button(0.25f, mBlankPlatformTexture, Vector2.Zero, false);
            mMilitaryListButton.ButtonClicked += OnButtonmMilitaryListClick;
            mMilitaryListButton.ButtonHovering += OnButtonmMilitaryListHovering;
            mMilitaryListButton.ButtonHoveringEnd += OnButtonmMilitaryListHoveringEnd;


            // junkyard platform button
            mJunkyardPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 2 * 130, 150, 130), Vector2.Zero, false);
            mJunkyardPlatformButton.ButtonClicked += OnButtonmJunkyardPlatformClick;
            mJunkyardPlatformButton.ButtonReleased += OnButtonmJunkyardPlatformReleased;
            mJunkyardPlatformButton.ButtonHovering += OnButtonmJunkyardPlatformHovering;
            mJunkyardPlatformButton.ButtonHoveringEnd += OnButtonmJunkyardPlatformHoveringEnd;

            // quarry platform button
            mQuarryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 2 * 130, 150, 130), Vector2.Zero, false);
            mQuarryPlatformButton.ButtonClicked += OnButtonmQuarryPlatformClick;
            mQuarryPlatformButton.ButtonReleased += OnButtonmQuarryPlatformReleased;
            mQuarryPlatformButton.ButtonHovering += OnButtonmQuarryPlatformHovering;
            mQuarryPlatformButton.ButtonHoveringEnd += OnButtonmQuarryPlatformHoveringEnd;

            // mine platform button
            mMinePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175, 150, 130), Vector2.Zero, false);
            mMinePlatformButton.ButtonClicked += OnButtonmMinePlatformClick;
            mMinePlatformButton.ButtonReleased += OnButtonmMinePlatformReleased;
            mMinePlatformButton.ButtonHovering += OnButtonmMinePlatformHovering;
            mMinePlatformButton.ButtonHoveringEnd += OnButtonmMinePlatformHoveringEnd;

            // well platform button
            mWellPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 4 * 130, 150, 130), Vector2.Zero, false);
            mWellPlatformButton.ButtonClicked += OnButtonmWellPlatformClick;
            mWellPlatformButton.ButtonReleased += OnButtonmWellPlatformReleased;
            mWellPlatformButton.ButtonHovering += OnButtonmWellPlatformHovering;
            mWellPlatformButton.ButtonHoveringEnd += OnButtonmWellPlatformHoveringEnd;


            // factory platform button
            mFactoryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 130, 150, 130), Vector2.Zero, false);
            mFactoryPlatformButton.ButtonClicked += OnButtonmFactoryPlatformClick;
            mFactoryPlatformButton.ButtonReleased += OnButtonmFactoryPlatformReleased;
            mFactoryPlatformButton.ButtonHovering += OnButtonmFactoryPlatformHovering;
            mFactoryPlatformButton.ButtonHoveringEnd += OnButtonmFactoryPlatformHoveringEnd;

            // storage platform button
            mStoragePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 3 * 130, 150, 130), Vector2.Zero, false);
            mStoragePlatformButton.ButtonClicked += OnButtonmStoragePlatformClick;
            mStoragePlatformButton.ButtonReleased += OnButtonmStoragePlatformReleased;
            mStoragePlatformButton.ButtonHovering += OnButtonmStoragePlatformHovering;
            mStoragePlatformButton.ButtonHoveringEnd += OnButtonmStoragePlatformHoveringEnd;

            // powerhouse platform button
            mPowerhousePlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175, 150, 130), Vector2.Zero, false);
            mPowerhousePlatformButton.ButtonClicked += OnButtonmPowerhousePlatformClick;
            mPowerhousePlatformButton.ButtonReleased += OnButtonmPowerhousePlatformReleased;
            mPowerhousePlatformButton.ButtonHovering += OnButtonmPowerhousePlatformHovering;
            mPowerhousePlatformButton.ButtonHoveringEnd += OnButtonmPowerhousePlatformHoveringEnd;

            // commandcenter platform button
            mCommandcenterPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 1 * 175, 150, 175), Vector2.Zero, false);
            mCommandcenterPlatformButton.ButtonClicked += OnButtonmCommandcenterPlatformClick;
            mCommandcenterPlatformButton.ButtonReleased += OnButtonmCommandcenterPlatformReleased;
            mCommandcenterPlatformButton.ButtonHovering += OnButtonmCommandcenterPlatformHovering;
            mCommandcenterPlatformButton.ButtonHoveringEnd += OnButtonmCommandcenterPlatformHoveringEnd;


            // armory platform button
            mArmoryPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 2 * 175 + 1 * 130, 150, 130), Vector2.Zero, false);
            mArmoryPlatformButton.ButtonClicked += OnButtonmArmoryPlatformClick;
            mArmoryPlatformButton.ButtonReleased += OnButtonmArmoryPlatformReleased;
            mArmoryPlatformButton.ButtonHovering += OnButtonmArmoryPlatformHovering;
            mArmoryPlatformButton.ButtonHoveringEnd += OnButtonmArmoryPlatformHoveringEnd;

            // kineticTower platform button
            mKineticTowerPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130, 150, 170), Vector2.Zero, false);
            mKineticTowerPlatformButton.ButtonClicked += OnButtonmKineticTowerPlatformClick;
            mKineticTowerPlatformButton.ButtonReleased += OnButtonmKineticTowerPlatformReleased;
            mKineticTowerPlatformButton.ButtonHovering += OnButtonmKineticTowerPlatformHovering;
            mKineticTowerPlatformButton.ButtonHoveringEnd += OnButtonmKineticTowerPlatformHoveringEnd;

            // laserTower platform button
            mLaserTowerPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 2 * 175 + 3 * 130 + 1 * 175, 150, 170), Vector2.Zero, false);
            mLaserTowerPlatformButton.ButtonClicked += OnButtonmLaserTowerPlatformClick;
            mLaserTowerPlatformButton.ButtonReleased += OnButtonmLaserTowerPlatformReleased;
            mLaserTowerPlatformButton.ButtonHovering += OnButtonmLaserTowerPlatformHovering;
            mLaserTowerPlatformButton.ButtonHoveringEnd += OnButtonmLaserTowerPlatformHoveringEnd;

            // barracks platform button
            mBarracksPlatformButton = new Button(0.25f, mOtherPlatformTexture, new Rectangle(0, 0 * 175, 150, 175), Vector2.Zero, false);
            mBarracksPlatformButton.ButtonClicked += OnButtonmBarracksPlatformClick;
            mBarracksPlatformButton.ButtonReleased += OnButtonmBarracksPlatformReleased;
            mBarracksPlatformButton.ButtonHovering += OnButtonmBarracksPlatformHovering;
            mBarracksPlatformButton.ButtonHoveringEnd += OnButtonmBarracksPlatformHoveringEnd;

            #endregion

            // TODO : ADD RESSOURCES TO ALL BUILD BUTTONS
            #region info when hovering over the building menu buttons

            mInfoBoxList = new List<InfoBoxWindow>();

            var infoBoxBorderColor = Color.White;
            var infoBoxCenterColor = Color.Black;

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

            #region MiniMap

            // TODO: properly place the minimapObject to better fit with the rest. Don't change the size
            // TODO: other than changing it in MapConstants, the +20 is for the padding left and right.
            var minimap = new MiniMap(mMap, mCamera, content.Load<Texture2D>("minimap"));
            mMinimapWindow = new WindowObject("", new Vector2(0, 0), new Vector2(MapConstants.MiniMapWidth + 20, MapConstants.MiniMapHeight + 20), false, mLibSans12, mDirector.GetInputManager, mGraphics);
            mMinimapWindow.AddItem(minimap);

            mWindowList.Add(mMinimapWindow);

            #endregion

            #region infoBarWindow

            // NOTICE: this window is the only window which is compeletely created and managed in its own class
            mInfoBar = new InfoBarWindowObject(borderColor, windowColor, mLibSans14, mScreenManager, mCivilUnitsWindow, mResourceWindow, mEventLogWindow, mBuildMenuWindow, mSelectedPlatformWindow, mMinimapWindow, mDirector);

            #endregion

            // called once to set positions + called everytime the resolution changes
            ResetWindowsToStandardPositon();

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

        /// <summary>
        /// resets defined windows to standard position
        /// </summary>
        private void ResetWindowsToStandardPositon()
        {
            // civil units position
            mCivilUnitsWindow.Position = new Vector2(12, 12 + 25);

            // resource window position
            mResourceWindow.Position = new Vector2(12, mCurrentScreenHeight - 12 - mResourceWindow.Size.Y);

            // event log position
            mEventLogWindow.Position = new Vector2(mCurrentScreenWidth - 12 - mEventLogWindow.Size.X, 12 + 25);

            // build menu position
            mBuildMenuWindow.Position = new Vector2(mCurrentScreenWidth - 12 - mBuildMenuWindow.Size.X, mEventLogWindow.Position.Y + mEventLogWindow.Size.Y + 12);

            // selected platform window position
            mSelectedPlatformWindow.Position = new Vector2(mCurrentScreenWidth / 2f - mSelectedPlatformWindow.Size.X / 2, mCurrentScreenHeight - mSelectedPlatformWindow.Size.Y - 12);

            // minimap position (-32 due to 12 padding + the window size is 20 bigger than the map
            mMinimapWindow.Position = new Vector2(
                mCurrentScreenWidth - 32 - MapConstants.MiniMapWidth,
                mCurrentScreenHeight - 32 - MapConstants.MiniMapHeight
            );
        }

        /// <summary>
        /// Set the platform's values in the selectedPlatformWindow
        /// </summary>
        /// <param name="id">the platform's id</param>
        /// <param name="type">the platform's type</param>
        /// <param name="resourceAmountList"></param>
        /// <param name="unitAssignmentList"></param>
        /// <param name="actionsList"></param>
        public void SetSelectedPlatformValues(
            int id,
            bool isActive,
            bool isManuallyDeactivated,
            EPlatformType type,
            IEnumerable<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentList,
            IEnumerable<IPlatformAction> actionsList)
        {
            // set window type
            mSelectedPlatformWindow.WindowName = type.ToString();

            #region active/deactive

            // manage activate/deactivate
            if (isManuallyDeactivated)
            {
                mSelectedPlatformDeactivatePlatformButton.ActiveInWindow = false;
                mSelectedPlatformActivatePlatformButton.ActiveInWindow = true;
            }
            else
            {
                mSelectedPlatformDeactivatePlatformButton.ActiveInWindow = true;
                mSelectedPlatformActivatePlatformButton.ActiveInWindow = false;
            }

            if (!isActive && !isManuallyDeactivated)
            {
                mSelectedPlatformIsAutoDeactivatedText.ActiveInWindow = true;
            }
            else
            {
                mSelectedPlatformIsAutoDeactivatedText.ActiveInWindow = false;
            }

            #endregion

            #region resources

            // reset all resource-amounts
            mSelectedPlatformChips.Amount = 0;
            mSelectedPlatformConcrete.Amount = 0;
            mSelectedPlatformCopper.Amount = 0;
            mSelectedPlatformFuel.Amount = 0;
            mSelectedPlatformMetal.Amount = 0;
            mSelectedPlatformOil.Amount = 0;
            mSelectedPlatformPlastic.Amount = 0;
            mSelectedPlatformSand.Amount = 0;
            mSelectedPlatformSilicon.Amount = 0;
            mSelectedPlatformSteel.Amount = 0;
            mSelectedPlatformStone.Amount = 0;
            mSelectedPlatformWater.Amount = 0;

            // go through each resource from the list and add it to the corresponding amount
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
            foreach (var resource in mSelectedPlatformResourcesList)
            {
                resource.ActiveInWindow = resource.Amount > 0;
            }

            #endregion

            // TODO : COMBINE UNIT ASSIGNMENTS AND DISTRIBUTIONMANAGER
            #region unitAssignments

            // activate defense text/sliders + deactivate production text/sliders if the platform is a defense tower,
            // else deactivate defense text/sliders + activate production text/sliders
            if (type == EPlatformType.Kinetic || type == EPlatformType.Laser)
            {
                mSelectedPlatformDefTextField.ActiveInWindow = true;
                mSelectedPlatformDefSlider.ActiveInWindow = true;
                mSelectedPlatformProductionTextField.ActiveInWindow = false;
                mSelectedPlatformProductionSlider.ActiveInWindow = false;
            }
            else
            {
                mSelectedPlatformDefTextField.ActiveInWindow = false;
                mSelectedPlatformDefSlider.ActiveInWindow = false;
                mSelectedPlatformProductionTextField.ActiveInWindow = true;
                mSelectedPlatformProductionSlider.ActiveInWindow = true;
            }

            // TODO : UPDATE TRUE/FALSE VALUES FOR EVERY PLATFORM TO SHOW ONLY POSSIBLE ACTIONS (FOR EXAMPLE COMMANDCENTER PROBABLY DOESN'T NEED PRODUCTION UNITS)
            /*
                        mSelectedPlatformBuildTextField;
                        mSelectedPlatformBuildSlider;
                        mSelectedPlatformLogisticsTextField;
                        mSelectedPlatformLogisticsSlider;
            */



            #endregion

            #region actions

            // deactivate all actions
            if (mMakeFastMilitaryAction != null)
            {
                mMakeFastMilitaryAction.ActiveInWindow = false;
            }
            if (mMakeStrongMilitaryAction != null)
            {
                mMakeStrongMilitaryAction.ActiveInWindow = false;
            }
            if (mProduceMineResourceAction != null)
            {
                mProduceMineResourceAction.ActiveInWindow = false;
            }
            if (mProduceQuarryResourceAction != null)
            {
                mProduceQuarryResourceAction.ActiveInWindow = false;
            }
            if (mProduceWellResourceAction != null)
            {
                mProduceWellResourceAction.ActiveInWindow = false;
            }

            // activate all actions possible on this platform + add them to the window if they haven't been added yet
            foreach (var action in actionsList)
            {
                if (action is MakeFastMilitaryUnit)
                {
                    mMakeFastMilitaryAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    if (!mFastMilitaryAdded)
                    {
                        mSelectedPlatformWindow.AddItem(mMakeFastMilitaryAction);
                        mSelectedPlatformActionList.Add(mMakeFastMilitaryAction);
                    }
                }
                else if (action is MakeStrongMilitrayUnit)
                {
                    mMakeStrongMilitaryAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    if (!mStronggMilitaryAdded)
                    {
                        mSelectedPlatformWindow.AddItem(mMakeStrongMilitaryAction);
                        mSelectedPlatformActionList.Add(mMakeFastMilitaryAction);
                    }
                }
                else if (action is ProduceMineResource)
                {
                    mProduceMineResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    if (!mProduceMineResourceAdded)
                    {
                        mSelectedPlatformWindow.AddItem(mProduceMineResourceAction);
                        mSelectedPlatformActionList.Add(mMakeFastMilitaryAction);
                    }
                }
                else if (action is ProduceQuarryResource)
                {
                    mProduceQuarryResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    if (!mProduceQuarryResourceAdded)
                    {
                        mSelectedPlatformWindow.AddItem(mProduceQuarryResourceAction);
                        mSelectedPlatformActionList.Add(mProduceQuarryResourceAction);
                    }
                }
                else if (action is ProduceWellResource)
                {
                    mProduceWellResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    if (!mProduceWellResourceAdded)
                    {
                        mSelectedPlatformWindow.AddItem(mProduceWellResourceAction);
                        mSelectedPlatformActionList.Add(mProduceWellResourceAction);
                    }
                }
            }

            #endregion

            // reset the window's scroll value + open all lists in selectedPlatformWindow if the id changes
            if (selectedPlatformId != id)
            {
                mSelectedPlatformWindow.ResetScrollValue();
            }
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

        #region button management

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
        }
        private void OnButtonmBarracksPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            foreach (var infoBox in mInfoBoxList)
            {
                infoBox.Active = false;
            }
        }

        #endregion

        #region close sections in selectedPlatformWindow

        private void CloseResourcesInSelectedWindow(object sender, EventArgs eventArgs)
        {
            foreach (var resource in mSelectedPlatformResourcesList)
            {
                resource.InactiveInSelectedPlatformWindow = !resource.InactiveInSelectedPlatformWindow;
            }
        }

        private void CloseUnitAssignmentsInSelectedWindow(object sender, EventArgs eventArgs)
        {
            foreach (var unitPart in mSelectedPlatformUnitAssignmentList)
            {
                unitPart.InactiveInSelectedPlatformWindow = !unitPart.InactiveInSelectedPlatformWindow;
            }
        }

        private void CloseActionsInSelectedWindow(object sender, EventArgs eventArgs)
        {
            foreach (var action in mSelectedPlatformActionList)
            {
                action.InactiveInSelectedPlatformWindow = !action.InactiveInSelectedPlatformWindow;
            }
        }

        #endregion

        public void SelectedPlatformDeactivate(object sender, EventArgs eventArgs)
        {
/*            mSelectedPlatformDeactivatePlatformButton.InactiveInSelectedPlatformWindow = true;
            mSelectedPlatformActivatePlatformButton.InactiveInSelectedPlatformWindow = false;*/
            mUserInterfaceController.DeactivateSelectedPlatform();
        }

        public void SelectedPlatformActivate(object sender, EventArgs eventArgs)
        {
/*            mSelectedPlatformDeactivatePlatformButton.InactiveInSelectedPlatformWindow = false;
            mSelectedPlatformActivatePlatformButton.InactiveInSelectedPlatformWindow = true;*/
            mUserInterfaceController.ActivateSelectedPlatform();
        }

        #endregion

        #region InputManagement

        public Rectangle Bounds { get; }

        public EScreen Screen { get; } = EScreen.UserInterfaceScreen;

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
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
