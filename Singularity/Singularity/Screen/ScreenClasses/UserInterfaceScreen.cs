using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// The UserInterfaceScreen contains everything that is needed for the player's UI
    /// </summary>
    public sealed class UserInterfaceScreen : IScreen, IMouseClickListener
    {
        private bool mActiveUserInterface;

        #region memberVariables

        // most important Bool - sets the UI as initialized
        private bool mInitialized;

        private Button mPauseButtonBeforeUi;

        private readonly GamePauseManagerScreen mGamePauseManagerScreen;

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
        private Texture2D mRoadIcon;
        private Texture2D mBaseIcon;
        private Texture2D mProductionIcon;
        private Texture2D mProcessingIcon;
        private Texture2D mMilitaryIcon;

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

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        // colors
        private readonly Color mWindowColor;
        private readonly Color mBorderColor;

        #endregion

        #region platform placement

        private readonly StructureMap mStructureMap;

        private readonly ResourceMap mResourceMap;

        private Map.Map mMap;

        private readonly Camera mCamera;

        private bool mCanBuildPlatform;

        private StructurePlacer mPlatformToPlace;

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
        private Button mSelectedPlatformActionsButton;

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
        private ResourceIWindowItem mSelectedPlatformTrash;
        
        // lists of items - used to iterate through all items of a specific kind (for example to deactivate all if the corresponding button was toggled)
        private List<ResourceIWindowItem> mSelectedPlatformResourcesList;
        private List<PlatformActionIWindowItem> mSelectedPlatformActionList;

        // save id to reset the scroll-value if the id changes
        private int mSelectedPlatformId;

        #endregion

        #region civilUnitsWindow members

        // civil units window
        private WindowObject mCivilUnitsWindow;

        // slider handler
        private SliderHandler mCivilUnitsSliderHandler;

        // graph ID
        private int mCivilUnitsGraphId;
        private int mCivilUnitsGraphIdToCompare;

        internal Dictionary<int, Graph.Graph> GraphIdToGraphStructureDict { get; set; }

        // sliders for distribution
        private Slider mDefSlider;
        private Slider mConstructionSlider;
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
        // resource window
        private WindowObject mResourceWindow;

        // text to inform the player about how often the resourceWindow is updated
        private TextField mResourceInfoText;

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
        private ResourceIWindowItem mResourceItemTrash;

        // previous clock time
        private int mResourceWindowNextTick;

        // previous resource amount
        private Dictionary<EResourceType, int> mResourceWindowResourceAmountLastTick;

        // X seconds to wait until next update (production/X seconds)
        private readonly int mResourceWindowXSeconds;

        #endregion

        #region eventLog members

        // event log window
        private WindowObject mEventLogWindow;

        #endregion

        #region buildMenuWindow members

        // Build menu window
        private WindowObject mBuildMenuWindow;

        // Horizontal lists
        private HorizontalCollection mListSwitchingList;
        private HorizontalCollection mMainBuildingsList;
        private HorizontalCollection mResourceProductionList;
        private HorizontalCollection mResourceProcessingList;
        private HorizontalCollection mMilitaryBuildingList;

        // ListSwitchingList buttons
        private Button mMainBuildingsListButton;
        private Button mResourceProductionListButton;
        private Button mResourceProcessingButton;
        private Button mMilitaryBuildingsListButton;

        // MainBuildingsList buttons
        private Button mBlankPlatformButton;
        private Button mRoadButton;
        private Button mCommandcenterPlatformButton;

        // ResourceProductionList buttons
        private Button mQuarryPlatformButton;
        private Button mMinePlatformButton;
        private Button mWellPlatformButton;
        private Button mPowerhousePlatformButton;

        // ResourceProcessingList buttons
        private Button mJunkyardPlatformButton;
        private Button mFactoryPlatformButton;
        private Button mStoragePlatformButton;

        // MilitaryBuildingsList buttons
        private Button mKineticTowerPlatformButton;
        private Button mLaserTowerPlatformButton;
        private Button mBarracksPlatformButton;

        // infoBoxes when hovering above the buttons
        private InfoBoxWindow mInfoMainBuildingsList;
        private InfoBoxWindow mInfoResourceProductionList;
        private InfoBoxWindow mInfoResourceProcessingList;
        private InfoBoxWindow mInfoMilitaryList;

        private InfoBoxWindow mInfoBuildBlank;
        private InfoBoxWindow mInfoBuildRoad;
        private InfoBoxWindow mInfoBuildCommandcenter;

        private InfoBoxWindow mInfoBuildQuarry;
        private InfoBoxWindow mInfoBuildMine;
        private InfoBoxWindow mInfoBuildWell;
        private InfoBoxWindow mInfoBuildPowerhouse;

        private InfoBoxWindow mInfoBuildJunkyard;
        private InfoBoxWindow mInfoBuildFactory;
        private InfoBoxWindow mInfoBuildStorage;

        private InfoBoxWindow mInfoBuildKineticTower;
        private InfoBoxWindow mInfoBuildLaserTower;
        private InfoBoxWindow mInfoBuildBarracks;

        // list of infoBoxes
        private List<InfoBoxWindow> mInfoBoxList;

        #endregion

        #region minimap members

        // minimap window
        private WindowObject mMinimapWindow;

        private MiniMap mMinimap;

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
        /// <param name="map"></param>
        /// /// <param name="camera"></param>
        /// <param name="stackScreenManager"></param>
        public UserInterfaceScreen(ref Director director, Map.Map map, Camera camera, IScreenManager stackScreenManager)
        {
            mMap = map;
            mStructureMap = mMap.GetStructureMap();
            mResourceMap = mMap.GetResourceMap();
            mCamera = camera;
            mCanBuildPlatform = true;

            mDirector = director;
            mScreenManager = stackScreenManager;

            // create the windowList
            mWindowList = new List<WindowObject>();

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // set as the controlled UI by the UIController
            mUserInterfaceController = director.GetUserInterfaceController;

            // TODO : BALANCING - CHANGE THIS VALUE TO DECREASE/INCREASE THE SECONDS BETWEEN EACH RESOURCE WINDOW UPDATE
            // resource/X seconds production calc.
            mResourceWindowXSeconds = 5;

            // change color for the border or the filling of all userinterface windows here
            mWindowColor = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            mBorderColor = new Color(0.68f, 0.933f, 0.933f, .8f);

            // set resolution values
            mCurrentScreenWidth = mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth;
            mCurrentScreenHeight = mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight;
            mPrevScreenWidth = mCurrentScreenWidth;
            mPrevScreenHeight = mCurrentScreenHeight;

            // pause menu screen
            mGamePauseManagerScreen = new GamePauseManagerScreen(new Vector2(director.GetGraphicsDeviceManager.PreferredBackBufferWidth, director.GetGraphicsDeviceManager.PreferredBackBufferHeight), mScreenManager, mDirector);
        }

        /// <inheritdoc />
        public void Update(GameTime gametime)
        {
            if (!mActiveUserInterface)
            {
                if (Loaded)
                {
                    mPauseButtonBeforeUi?.Update(gametime);
                }
                return;
            }

            if (!mInitialized) { Initialize(); }

            // update screen size
            mCurrentScreenWidth = mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth;
            mCurrentScreenHeight = mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight;

            // update sliders if there was a change
            if (mCivilUnitsGraphId != mCivilUnitsGraphIdToCompare)
            {
                mCivilUnitsGraphIdToCompare = mCivilUnitsGraphId;

                mCivilUnitsSliderHandler.SetGraphId(mCivilUnitsGraphId);

                mCivilUnitsSliderHandler.Refresh();
                mCivilUnitsSliderHandler.ForceSliderPages();
            }

            if (mStructureMap.GetDictionaryGraphIdToGraph()[mCivilUnitsGraphId] == null)
            {
                var index = 0;

                while (mStructureMap.GetDictionaryGraphIdToGraph().ElementAt(index).Value == null)
                {
                    index += 1;
                }

                mCivilUnitsGraphId = mStructureMap.GetDictionaryGraphIdToGraph().ElementAt(index).Key;
            }

            // if the resolution has changed -> reset windows positions
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

            // update resource window every X seconds to get the "production in the last X seconds amount"
                if (mResourceWindowNextTick == mDirector.GetClock.GetIngameTime().Seconds)
            {
                var currentProducedResourceAmounts = mDirector.GetStoryManager.ResourcesCount;

                // set 'produced resource in past X seconds' amount
                mResourceItemChip.Amount = currentProducedResourceAmounts[EResourceType.Chip] - mResourceWindowResourceAmountLastTick[EResourceType.Chip];
                mResourceItemConcrete.Amount = currentProducedResourceAmounts[EResourceType.Concrete] - mResourceWindowResourceAmountLastTick[EResourceType.Concrete];
                mResourceItemCopper.Amount = currentProducedResourceAmounts[EResourceType.Copper] - mResourceWindowResourceAmountLastTick[EResourceType.Copper];
                mResourceItemFuel.Amount = currentProducedResourceAmounts[EResourceType.Fuel] - mResourceWindowResourceAmountLastTick[EResourceType.Fuel];
                mResourceItemMetal.Amount = currentProducedResourceAmounts[EResourceType.Metal] - mResourceWindowResourceAmountLastTick[EResourceType.Metal];
                mResourceItemOil.Amount = currentProducedResourceAmounts[EResourceType.Oil] - mResourceWindowResourceAmountLastTick[EResourceType.Oil];
                mResourceItemPlastic.Amount = currentProducedResourceAmounts[EResourceType.Plastic] - mResourceWindowResourceAmountLastTick[EResourceType.Plastic];
                mResourceItemSand.Amount = currentProducedResourceAmounts[EResourceType.Sand] - mResourceWindowResourceAmountLastTick[EResourceType.Sand];
                mResourceItemSilicon.Amount = currentProducedResourceAmounts[EResourceType.Silicon] - mResourceWindowResourceAmountLastTick[EResourceType.Silicon];
                mResourceItemSteel.Amount = currentProducedResourceAmounts[EResourceType.Steel] - mResourceWindowResourceAmountLastTick[EResourceType.Steel];
                mResourceItemStone.Amount = currentProducedResourceAmounts[EResourceType.Stone] - mResourceWindowResourceAmountLastTick[EResourceType.Stone];
                mResourceItemWater.Amount = currentProducedResourceAmounts[EResourceType.Water] - mResourceWindowResourceAmountLastTick[EResourceType.Water];
                mResourceItemTrash.Amount = currentProducedResourceAmounts[EResourceType.Trash] - mResourceWindowResourceAmountLastTick[EResourceType.Trash];

                mResourceWindowNextTick = (mDirector.GetClock.GetIngameTime().Seconds + 5) % 60;

                mResourceWindowResourceAmountLastTick = new Dictionary<EResourceType, int>(mDirector.GetStoryManager.ResourcesCount);
            }
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            if (!mActiveUserInterface || !mInitialized)
            {
                mPauseButtonBeforeUi?.Draw(spriteBatch);

                spriteBatch.End();

                return;
            }

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

        /// <inheritdoc />
        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
            mLibSans10 = content.Load<SpriteFont>("LibSans10");

            // Texture Loading
            mBlankPlatformTexture = content.Load<Texture2D>("PlatformBasicSmall");
            mOtherPlatformTexture = content.Load<Texture2D>("PlatformSpriteSheetSmall");
            mBaseIcon = content.Load<Texture2D>("BuildIcons/BaseIcon");
            mProductionIcon = content.Load<Texture2D>("BuildIcons/ProductionIcon");
            mProcessingIcon = content.Load<Texture2D>("BuildIcons/ProcessingIcon");
            mMilitaryIcon = content.Load<Texture2D>("BuildIcons/MilitaryIcon");
            mRoadIcon = content.Load<Texture2D>("BuildIcons/RoadIcon");

            mMinimap = new MiniMap(ref mDirector, content.Load<Texture2D>("minimap"));

            // the button which is displayed before the UI appears to enable the player to exit the game before getting the UI
            mPauseButtonBeforeUi = new Button(" ll ", mLibSans14, new Vector2(
                    mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth - mLibSans14.MeasureString(" ll ").X, 0), true) { Opacity = 1f };
            mPauseButtonBeforeUi.ButtonReleased += PauseMenuBeforeUi;

            //DEACTIVATE EVERYTHING TO ACTIVATE IT LATER

            if (GlobalVariables.DebugState)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }

            Loaded = true;

            // subscribe to input manager
            Bounds = new Rectangle((int)mPauseButtonBeforeUi.Position.X, (int)mPauseButtonBeforeUi.Position.Y, (int)mPauseButtonBeforeUi.Size.X, (int)mPauseButtonBeforeUi.Size.Y);
            mDirector.GetInputManager.FlagForAddition(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
        }

        /// <summary>
        /// Initialize all windows + items + all other stuff the UI consists of
        /// </summary>
        private void Initialize()
        {
            mInitialized = true;

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
            mSelectedPlatformWindow = new WindowObject("No Selection", new Vector2(250, 200), new Vector2(selectedPlatformWidth, selectedPlatformHeight), true, mLibSans14, mDirector);

            // list to add all item to be able to iterate through them
            mSelectedPlatformResourcesList = new List<ResourceIWindowItem>();
            mSelectedPlatformActionList = new List<PlatformActionIWindowItem>();

            // activate / deactivate platform item
            mSelectedPlatformDeactivatePlatformButton = new Button("Deactivate", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformDeactivatePlatformButton.ButtonReleased += SelectedPlatformDeactivate;
            mSelectedPlatformActivatePlatformButton = new Button("Activate", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformActivatePlatformButton.ButtonReleased += SelectedPlatformActivate;
            mSelectedPlatformActiveItem = new ActivationIWindowItem(mSelectedPlatformDeactivatePlatformButton, mSelectedPlatformActivatePlatformButton, selectedPlatformWidth, ref mDirector);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformActiveItem);

            // auto-deactivaed textfield
            mSelectedPlatformIsAutoDeactivatedText = new TextField("FORCE DEACTIVATED", Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X, 0), mLibSans12, Color.Red);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformIsAutoDeactivatedText);
            mSelectedPlatformIsAutoDeactivatedText.InactiveInSelectedPlatformWindow = true;

            // resource-section-title
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
            mSelectedPlatformTrash = new ResourceIWindowItem(EResourceType.Trash, 0, new Vector2(mSelectedPlatformWindow.Size.X - 40, 0), mLibSans10);
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
            mSelectedPlatformResourcesList.Add(mSelectedPlatformTrash);

            // TODO : KEEP ?
            /*
            // unit assignment - section + title
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
            mSelectedPlatformConstructionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans10, ref mDirector);
            mSelectedPlatformUnitAssignmentList.Add(mSelectedPlatformConstructionSlider);
            mSelectedPlatformWindow.AddItem(mSelectedPlatformConstructionSlider);
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
            */

            // actions-section-title
            mSelectedPlatformActionsButton = new Button("Actions", mLibSans12, Vector2.Zero, Color.White) { Opacity = 1f };
            mSelectedPlatformActionsButton.ButtonReleased += CloseActionsInSelectedWindow;
            mSelectedPlatformWindow.AddItem(mSelectedPlatformActionsButton);

            // deactivate all items from selectedPlatformWindow since no platform is selected at the beginning
            mSelectedPlatformActivatePlatformButton.ActiveInWindow = false;
            mSelectedPlatformDeactivatePlatformButton.ActiveInWindow = false;

            foreach (var item in mSelectedPlatformResourcesList)
            {
                item.ActiveInWindow = false;
            }

            mWindowList.Add(mSelectedPlatformWindow);

            #endregion

            #region eventLogWindow

            mEventLogWindow = new WindowObject("// EVENT LOG", new Vector2(0, 0), new Vector2(eventLogWidth, eventLogHeight), true, mLibSans14, mDirector);

            mWindowList.Add(mEventLogWindow);

            #endregion

            #region civilUnitsWindow

            mCivilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(0, 0), new Vector2(civilUnitsWidth, civilUnitsHeight), mBorderColor, mWindowColor, 10, 15, true, mLibSans14, mDirector);

            // create items

            mDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mConstructionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);
            mProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12, Color.White);
            mProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true);

            mIdleUnitsTextAndAmount = new TextAndAmountIWindowItem("Idle", 0, Vector2.Zero, new Vector2(civilUnitsWidth, 0), mLibSans12, Color.White);

            // adding all items
            mCivilUnitsWindow.AddItem(mIdleUnitsTextAndAmount);
            mCivilUnitsWindow.AddItem(mDefTextField);
            mCivilUnitsWindow.AddItem(mDefSlider);
            mCivilUnitsWindow.AddItem(mBuildTextField);
            mCivilUnitsWindow.AddItem(mConstructionSlider);
            mCivilUnitsWindow.AddItem(mLogisticsTextField);
            mCivilUnitsWindow.AddItem(mLogisticsSlider);
            mCivilUnitsWindow.AddItem(mProductionTextField);
            mCivilUnitsWindow.AddItem(mProductionSlider);

            mWindowList.Add(mCivilUnitsWindow);

            mCivilUnitsGraphId = mDirector.GetDistributionDirector.GetSomeId();
            mCivilUnitsGraphIdToCompare = -1;

            #endregion

            #region resourceWindow

            mResourceWindow = new WindowObject("// RESOURCES", new Vector2(0, 0), new Vector2(resourceWidth, resourceHeight), true, mLibSans14, mDirector);

            // create all items (these are simple starting values which will be updated automatically by the UI controller)
            mResourceInfoText = new TextField("         production per " + mResourceWindowXSeconds + " sec", Vector2.Zero, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10, Color.White);

            mResourceItemChip = new ResourceIWindowItem(EResourceType.Chip, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemConcrete = new ResourceIWindowItem(EResourceType.Concrete, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemCopper = new ResourceIWindowItem(EResourceType.Copper, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemFuel = new ResourceIWindowItem(EResourceType.Fuel, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemMetal = new ResourceIWindowItem(EResourceType.Metal, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemOil = new ResourceIWindowItem(EResourceType.Oil, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemPlastic = new ResourceIWindowItem(EResourceType.Plastic, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSand = new ResourceIWindowItem(EResourceType.Sand, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSilicon = new ResourceIWindowItem(EResourceType.Silicon, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemSteel = new ResourceIWindowItem(EResourceType.Steel, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemStone = new ResourceIWindowItem(EResourceType.Stone, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemWater = new ResourceIWindowItem(EResourceType.Water, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);
            mResourceItemTrash = new ResourceIWindowItem(EResourceType.Trash, 0, new Vector2(mResourceWindow.Size.X - 40, mResourceWindow.Size.Y), mLibSans10);

            // add all items to window
            mResourceWindow.AddItem(mResourceInfoText);
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
            mResourceWindow.AddItem(mResourceItemTrash);

            // add the window to windowList
            mWindowList.Add(mResourceWindow);

            mResourceWindowResourceAmountLastTick = mDirector.GetStoryManager.ResourcesCount;

            #endregion

            #region buildMenuWindow

            mBuildMenuWindow = new WindowObject("// BUILDMENU", new Vector2(0, 0), new Vector2(buildMenuWidth, buildMenuHeight), true, mLibSans14, mDirector);

            #region button definitions

            #region open lists

            // Open MainBuildingsList button
            mMainBuildingsListButton = new Button(1, mBaseIcon, Vector2.Zero, false);
            mMainBuildingsListButton.ButtonClicked += OnButtonmMainBuildingsListButtonClick;
            mMainBuildingsListButton.ButtonHovering += OnButtonmMainBuildingsListButtonHovering;
            mMainBuildingsListButton.ButtonHoveringEnd += OnButtonmMainBuildingsListButtonHoveringEnd;

            // Open ResourceProductionList button
            mResourceProductionListButton = new Button(1, mProductionIcon, Vector2.Zero, false);
            mResourceProductionListButton.ButtonClicked += OnButtonmResourceProductionListClick;
            mResourceProductionListButton.ButtonHovering += OnButtonmResourceProductionListHovering;
            mResourceProductionListButton.ButtonHoveringEnd += OnButtonmResourceProductionListHoveringEnd;

            // Open ResourceProcessingList button
            mResourceProcessingButton = new Button(1, mProcessingIcon, Vector2.Zero, false);
            mResourceProcessingButton.ButtonClicked += OnButtonmResourceProcessingClick;
            mResourceProcessingButton.ButtonHovering += OnButtonmResourceProcessingHovering;
            mResourceProcessingButton.ButtonHoveringEnd += OnButtonmResourceProcessingHoveringEnd;

            // Open MilitaryBuildingList button
            mMilitaryBuildingsListButton = new Button(1, mMilitaryIcon, Vector2.Zero, false);
            mMilitaryBuildingsListButton.ButtonClicked += OnButtonmMilitaryBuildingsListClick;
            mMilitaryBuildingsListButton.ButtonHovering += OnButtonmMilitaryBuildingsListHovering;
            mMilitaryBuildingsListButton.ButtonHoveringEnd += OnButtonmMilitaryBuildingsListHoveringEnd;

            #endregion

            #region main buildings

            // blank platform button
            mBlankPlatformButton = new Button(1, mBlankPlatformTexture, Vector2.Zero, false);
            mBlankPlatformButton.ButtonClicked += OnButtonmBlankPlatformClick;
            mBlankPlatformButton.ButtonHovering += OnButtonmBlankPlatformHovering;
            mBlankPlatformButton.ButtonHoveringEnd += OnButtonmBlankPlatformHoveringEnd;

            // road button
            mRoadButton = new Button(1f, mRoadIcon, Vector2.Zero, false);
            mRoadButton.ButtonClicked += OnButtonmRoadButtonClick;
            mRoadButton.ButtonHovering += OnButtonmRoadButtonHovering;
            mRoadButton.ButtonHoveringEnd += OnButtonmRoadButtonHoveringEnd;

            // commandcenter platform button
            mCommandcenterPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 44, 38, 44), Vector2.Zero, false);
            mCommandcenterPlatformButton.ButtonClicked += OnButtonmCommandcenterPlatformClick;
            mCommandcenterPlatformButton.ButtonHovering += OnButtonmCommandcenterPlatformHovering;
            mCommandcenterPlatformButton.ButtonHoveringEnd += OnButtonmCommandcenterPlatformHoveringEnd;

            #endregion

            #region resourceProduction buildings

            // quarry platform button
            mQuarryPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 338, 38, 33), Vector2.Zero, false);
            mQuarryPlatformButton.ButtonClicked += OnButtonmQuarryPlatformClick;
            mQuarryPlatformButton.ButtonHovering += OnButtonmQuarryPlatformHovering;
            mQuarryPlatformButton.ButtonHoveringEnd += OnButtonmQuarryPlatformHoveringEnd;

            // mine platform button
            mMinePlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 272, 38, 33), Vector2.Zero, false);
            mMinePlatformButton.ButtonClicked += OnButtonmMinePlatformClick;
            mMinePlatformButton.ButtonHovering += OnButtonmMinePlatformHovering;
            mMinePlatformButton.ButtonHoveringEnd += OnButtonmMinePlatformHoveringEnd;

            // well platform button
            mWellPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 403, 38, 33), Vector2.Zero, false);
            mWellPlatformButton.ButtonClicked += OnButtonmWellPlatformClick;
            mWellPlatformButton.ButtonHovering += OnButtonmWellPlatformHovering;
            mWellPlatformButton.ButtonHoveringEnd += OnButtonmWellPlatformHoveringEnd;

            // powerhouse platform button
            mPowerhousePlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 87, 38, 33), Vector2.Zero, false);
            mPowerhousePlatformButton.ButtonClicked += OnButtonmPowerhousePlatformClick;
            mPowerhousePlatformButton.ButtonHovering += OnButtonmPowerhousePlatformHovering;
            mPowerhousePlatformButton.ButtonHoveringEnd += OnButtonmPowerhousePlatformHoveringEnd;

            #endregion

            #region resourceProcessing buildings

            // junkyard platform button
            mJunkyardPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 153, 38, 33), Vector2.Zero, false);
            mJunkyardPlatformButton.ButtonClicked += OnButtonmJunkyardPlatformClick;
            mJunkyardPlatformButton.ButtonHovering += OnButtonmJunkyardPlatformHovering;
            mJunkyardPlatformButton.ButtonHoveringEnd += OnButtonmJunkyardPlatformHoveringEnd;

            // factory platform button
            mFactoryPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 120, 38, 33), Vector2.Zero, false);
            mFactoryPlatformButton.ButtonClicked += OnButtonmFactoryPlatformClick;
            mFactoryPlatformButton.ButtonHovering += OnButtonmFactoryPlatformHovering;
            mFactoryPlatformButton.ButtonHoveringEnd += OnButtonmFactoryPlatformHoveringEnd;

            // storage platform button
            mStoragePlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 370, 38, 33), Vector2.Zero, false);
            mStoragePlatformButton.ButtonClicked += OnButtonmStoragePlatformClick;
            mStoragePlatformButton.ButtonHovering += OnButtonmStoragePlatformHovering;
            mStoragePlatformButton.ButtonHoveringEnd += OnButtonmStoragePlatformHoveringEnd;

            #endregion

            #region military buildings

            // kineticTower platform button
            mKineticTowerPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 185, 38, 43), Vector2.Zero, false);
            mKineticTowerPlatformButton.ButtonClicked += OnButtonmKineticTowerPlatformClick;
            mKineticTowerPlatformButton.ButtonHovering += OnButtonmKineticTowerPlatformHovering;
            mKineticTowerPlatformButton.ButtonHoveringEnd += OnButtonmKineticTowerPlatformHoveringEnd;

            // laserTower platform button
            mLaserTowerPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 229, 38, 43), Vector2.Zero, false);
            mLaserTowerPlatformButton.ButtonClicked += OnButtonmLaserTowerPlatformClick;
            mLaserTowerPlatformButton.ButtonHovering += OnButtonmLaserTowerPlatformHovering;
            mLaserTowerPlatformButton.ButtonHoveringEnd += OnButtonmLaserTowerPlatformHoveringEnd;

            // barracks platform button
            mBarracksPlatformButton = new Button(1, mOtherPlatformTexture, new Rectangle(0, 0, 38, 44), Vector2.Zero, false);
            mBarracksPlatformButton.ButtonClicked += OnButtonmBarracksPlatformClick;
            mBarracksPlatformButton.ButtonHovering += OnButtonmBarracksPlatformHovering;
            mBarracksPlatformButton.ButtonHoveringEnd += OnButtonmBarracksPlatformHoveringEnd;

            #endregion

            #endregion

            #region info when hovering over the building menu buttons

            mInfoBoxList = new List<InfoBoxWindow>();
            var infoBoxBorderColor = Color.White;
            var infoBoxCenterColor = Color.Black;

            #region listSwitching

            // Open MainBuildingsList info
            var infoMainBuildingsList = new TextField("Main Buildings",
                Vector2.Zero,
                mLibSans10.MeasureString("Main Buildings"),
                mLibSans10,
                Color.White);

            mInfoMainBuildingsList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMainBuildingsList },
                size: mLibSans10.MeasureString("Main Buildings"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoMainBuildingsList);

            // Open ResourceProductionList info
            var infoResourceProduction = new TextField("Resource Production Buildings",
                Vector2.Zero,
                mLibSans10.MeasureString("Resource Production Buildings"),
                mLibSans10,
                Color.White);

            mInfoResourceProductionList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoResourceProduction },
                size: mLibSans10.MeasureString("Resource Production Buildings"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoResourceProductionList);

            // Open ResourceProcessingList info
            var infoResourceProcessing = new TextField("Resource Processing Buildings",
                Vector2.Zero,
                mLibSans10.MeasureString("Resource Processing Buildings"),
                mLibSans10,
                Color.White);

            mInfoResourceProcessingList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoResourceProcessing },
                size: mLibSans10.MeasureString("Resource Processing Buildings"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoResourceProcessingList);

            // Open MilitaryBuildingsList info
            var infoMilitaryList = new TextField("Military Buildings",
                Vector2.Zero,
                mLibSans10.MeasureString("Military Buildings"),
                mLibSans10,
                Color.White);

            mInfoMilitaryList = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMilitaryList },
                size: mLibSans10.MeasureString("Military Buildings"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoMilitaryList);

            #endregion

            var infoBuildPlacement = new TextField("Place On:", Vector2.Zero,
                mLibSans10.MeasureString("Place On:"),
                mLibSans10,
                Color.White);

            var infoBuildCosts = new TextField("Costs :", Vector2.Zero,
                mLibSans10.MeasureString("Costs :"),
                mLibSans10,
                Color.White);

            #region mainBuildings

            // Build Blank Platform info
            var infoBuildBlank = new TextField("Blank Platform",
                Vector2.Zero,
                mLibSans10.MeasureString("Blank Platform"),
                mLibSans10,
                Color.White);

            var dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Blank);

            var infoBuildBlankMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Blank Platform"),
                mLibSans10);

            mInfoBuildBlank = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoBuildBlank, infoBuildCosts, infoBuildBlankMetal },
                size: mLibSans10.MeasureString("Blank Platform"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildBlank);

            // Build Road info
            var infoRoad = new TextField("Road",
                Vector2.Zero,
                mLibSans10.MeasureString("Road"),
                mLibSans10,
                Color.White);

           var infoRoadStone = new ResourceIWindowItem(
                EResourceType.Stone,
                1,
                mLibSans10.MeasureString("Road"),
                mLibSans10);

            var infoRoadMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                1,
                mLibSans10.MeasureString("Road"),
                mLibSans10);

            mInfoBuildRoad = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoRoad, infoBuildCosts, infoRoadStone, infoRoadMetal },
                size: mLibSans10.MeasureString("Road"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildRoad);

            // Build Commandcenter info
            var infoCommandcenter = new TextField("Commandcenter",
                Vector2.Zero,
                mLibSans10.MeasureString("Commandcenter"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Command);

            var infoCommandConcrete = new ResourceIWindowItem(
                EResourceType.Concrete,
                dictWithRes[EResourceType.Concrete],
                mLibSans10.MeasureString("Commandcenter"),
                mLibSans10);

            var infoCommandChip = new ResourceIWindowItem(
                EResourceType.Chip,
                dictWithRes[EResourceType.Chip],
                mLibSans10.MeasureString("Commandcenter"),
                mLibSans10);

            var infoCommandSteel = new ResourceIWindowItem(
                EResourceType.Steel,
                dictWithRes[EResourceType.Steel],
                mLibSans10.MeasureString("Commandcenter"),
                mLibSans10);

            var infoCommandcenterProduce = new TextField("Produces : General Units/Settlers ",
                Vector2.Zero,
                mLibSans10.MeasureString("Produces : General Units/Settlers"),
                mLibSans10,
                Color.White);



            mInfoBuildCommandcenter = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoCommandcenter, infoBuildCosts, infoCommandConcrete, infoCommandChip, infoCommandSteel,
                    infoCommandcenterProduce
                },
                size: mLibSans10.MeasureString("Commandcenter"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildCommandcenter);

            #endregion

            #region resourceProductionBuildings


            // Build Quarry info
            var infoQuarry = new TextField("Quarry",
                Vector2.Zero,
                mLibSans10.MeasureString("Quarry"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Quarry);

            var infoBuildQuarryStone = new ResourceIWindowItem(
                EResourceType.Stone,
                dictWithRes[EResourceType.Stone],
                mLibSans10.MeasureString("Quarry"),
                mLibSans10);

            var infoBuildQuarryMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Quarry"),
                mLibSans10);


            var infoBuildQuarryPlacementWhere = new TextField("Anywhere = Sand/Stone", Vector2.Zero,
                mLibSans10.MeasureString("Anywhere = Sand/Stone"),
                mLibSans10,
                Color.White);

            mInfoBuildQuarry = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoQuarry, infoBuildCosts, infoBuildQuarryStone, infoBuildQuarryMetal, infoBuildPlacement,
                    infoBuildQuarryPlacementWhere },
                size: mLibSans10.MeasureString("Quarry"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildQuarry);

            // Build Mine info
            var infoMine = new TextField("Mine",
                Vector2.Zero,
                mLibSans10.MeasureString("Mine"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Mine);

            var infoBuildMineStone = new ResourceIWindowItem(
                EResourceType.Stone,
                dictWithRes[EResourceType.Stone],
                mLibSans10.MeasureString("Mine"),
                mLibSans10);

            var infoBuildMineMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Mine"),
                mLibSans10);


            var infoBuildMinePlacementWhere = new TextField("Green = Metal", Vector2.Zero,
                mLibSans10.MeasureString("Green = Metal)"),
                mLibSans10,
                Color.White);

            mInfoBuildMine = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoMine, infoBuildCosts, infoBuildMineStone, infoBuildMineMetal, infoBuildPlacement, infoBuildMinePlacementWhere },
                size: mLibSans10.MeasureString("Mine"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildMine);

            // Build Well info
            var infoWell = new TextField("Well",
                Vector2.Zero,
                mLibSans10.MeasureString("Well"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Well);

            var infoBuildWellStone = new ResourceIWindowItem(
                EResourceType.Stone,
                dictWithRes[EResourceType.Stone],
                mLibSans10.MeasureString("Well"),
                mLibSans10);

            var infoBuildWellMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Well"),
                mLibSans10);

            var infoBuildWellPlacementWhere1 = new TextField("Blue = Water", Vector2.Zero,
                mLibSans10.MeasureString("Blue = Water"),
                mLibSans10,
                Color.White);

            var infoBuildWellPlacementWhere2 = new TextField("Red = Oil", Vector2.Zero,
                mLibSans10.MeasureString("Red = Oil"),
                mLibSans10,
                Color.White);

            mInfoBuildWell = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoWell, infoBuildCosts, infoBuildWellStone, infoBuildWellMetal, infoBuildPlacement, 
                    infoBuildWellPlacementWhere1, infoBuildWellPlacementWhere2
                },
                size: mLibSans10.MeasureString("Well"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildWell);

            // Build Powerhouse info
            var infoPowerhouse = new TextField("Powerhouse",
                Vector2.Zero,
                mLibSans10.MeasureString("Powerhouse"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Energy);

            var infoBuildPowerhouseCopper = new ResourceIWindowItem(
                EResourceType.Copper,
                dictWithRes[EResourceType.Copper],
                mLibSans10.MeasureString("Powerhouse"),
                mLibSans10);

            var infoBuildPowerhouseMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Powerhouse"),
                mLibSans10);

            var infoBuildPowerhouseSilicon = new ResourceIWindowItem(
                EResourceType.Silicon,
                dictWithRes[EResourceType.Silicon],
                mLibSans10.MeasureString("Powerhouse"),
                mLibSans10);

            var infoPowerhouseFunction = new TextField("Produces : Energy",
                Vector2.Zero,
                mLibSans10.MeasureString("Produces : Energy"),
                mLibSans10,
                Color.White);


            mInfoBuildPowerhouse = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoPowerhouse, infoBuildCosts, infoBuildPowerhouseCopper, infoBuildPowerhouseMetal, infoBuildPowerhouseSilicon,
                    infoPowerhouseFunction
                },
                size: mLibSans10.MeasureString("Powerhouse"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildPowerhouse);

            #endregion

            #region resourceProcessingBuildings

            // Build Junkyard info
            var infoJunkyard = new TextField("Junkyard",
                Vector2.Zero,
                mLibSans10.MeasureString("Junkyard"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Junkyard);

            var infoBuildJunkyardStone = new ResourceIWindowItem(
                EResourceType.Stone,
                dictWithRes[EResourceType.Stone],
                mLibSans10.MeasureString("Junkyard"),
                mLibSans10);

            var infoBuildJunkyardMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Junkyard"),
                mLibSans10);

            var infoBuildJunkyardWater = new ResourceIWindowItem(
                EResourceType.Water,
                dictWithRes[EResourceType.Water],
                mLibSans10.MeasureString("Junkyard"),
                mLibSans10);

            var infoJunkyardProduce = new TextField("Disposes of trash",
                Vector2.Zero,
                mLibSans10.MeasureString("Disposes of trash"),
                mLibSans10,
                Color.White);

            mInfoBuildJunkyard = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoJunkyard, infoBuildCosts, infoBuildJunkyardStone, infoBuildJunkyardMetal, infoBuildJunkyardWater,
                    infoJunkyardProduce
                },
                size: mLibSans10.MeasureString("Junkyard"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildJunkyard);

            // Build Factory info
            var infoFactory = new TextField("Factory",
                Vector2.Zero,
                mLibSans10.MeasureString("Factory"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Factory);

            var infoBuildFactoryStone = new ResourceIWindowItem(
                EResourceType.Stone,
                dictWithRes[EResourceType.Stone],
                mLibSans10.MeasureString("Factory"),
                mLibSans10);

            var infoBuildFactoryMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Factory"),
                mLibSans10);

            var infoBuildFactoryWater = new ResourceIWindowItem(
                EResourceType.Water,
                dictWithRes[EResourceType.Water],
                mLibSans10.MeasureString("Factory"),
                mLibSans10);

            var infoFactoryProduce = new TextField("Refines resources into new ones",
                Vector2.Zero,
                mLibSans10.MeasureString("Refines resources into new ones"),
                mLibSans10,
                Color.White);
            var infoFactoryProduce2 = new TextField("Combines resources into new ones",
                Vector2.Zero,
                mLibSans10.MeasureString("Combines resources into new ones"),
                mLibSans10,
                Color.White);


            mInfoBuildFactory = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoFactory, infoBuildCosts, infoBuildFactoryStone, infoBuildFactoryMetal, infoBuildFactoryWater,
                    infoFactoryProduce, infoFactoryProduce2
                },
                size: mLibSans10.MeasureString("Factory"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildFactory);

            // Build Storage info
            var infoStorage = new TextField("Storage",
                Vector2.Zero,
                mLibSans10.MeasureString("Storage"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Storage);

            var infoBuildStorageConcrete = new ResourceIWindowItem(
                EResourceType.Concrete,
                dictWithRes[EResourceType.Concrete],
                mLibSans10.MeasureString("Storage"),
                mLibSans10);

            var infoBuildStorageMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Storage"),
                mLibSans10);

            mInfoBuildStorage = new InfoBoxWindow(
                itemList: new List<IWindowItem> { infoStorage, infoBuildCosts, infoBuildStorageConcrete, infoBuildStorageMetal },
                size: mLibSans10.MeasureString("Storage"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildStorage);

            #endregion

            #region militaryBuildings

            // Build Kinetic Tower info
            var infoKineticTower = new TextField("Kinetic Tower",
                Vector2.Zero,
                mLibSans10.MeasureString("Kinetic Tower"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Kinetic);

            var infoBuildKineticTowerConcrete = new ResourceIWindowItem(
                EResourceType.Concrete,
                dictWithRes[EResourceType.Concrete],
                mLibSans10.MeasureString("Kinetic Tower"),
                mLibSans10);

            var infoBuildKineticTowerMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Kinetic Tower"),
                mLibSans10);

            var infoKineticTowerAssign = new TextField("Assign Units to : Defense",
                Vector2.Zero,
                mLibSans10.MeasureString("Assign Units to : Defense"),
                mLibSans10,
                Color.White);

            mInfoBuildKineticTower = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoKineticTower, infoBuildCosts, infoBuildKineticTowerConcrete, infoBuildKineticTowerMetal,
                    infoKineticTowerAssign
                },
                size: mLibSans10.MeasureString("Kinetic Tower"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);


            mInfoBoxList.Add(mInfoBuildKineticTower);

            // Build Laser Tower info
            var infoLaserTower = new TextField("Laser Tower",
                Vector2.Zero,
                mLibSans10.MeasureString("Laser Tower"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Laser);

            var infoBuildLaserTowerConcrete = new ResourceIWindowItem(
                EResourceType.Concrete,
                dictWithRes[EResourceType.Concrete],
                mLibSans10.MeasureString("Laser Tower"),
                mLibSans10);

            var infoBuildLaserTowerMetal = new ResourceIWindowItem(
                EResourceType.Metal,
                dictWithRes[EResourceType.Metal],
                mLibSans10.MeasureString("Laser Tower"),
                mLibSans10);

            var infoLaserTowerAssign = new TextField("Assign Units to : Defense",
                Vector2.Zero,
                mLibSans10.MeasureString("Assign Units to : Defense"),
                mLibSans10,
                Color.White);

            mInfoBuildLaserTower = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoLaserTower, infoBuildCosts, infoBuildLaserTowerConcrete, infoBuildLaserTowerMetal,
                    infoLaserTowerAssign
                },
                size: mLibSans10.MeasureString("Laser Tower"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildLaserTower);

            // Build Barracks info
            var infoBarracks = new TextField("Barracks",
                Vector2.Zero,
                mLibSans10.MeasureString("Barracks"),
                mLibSans10,
                Color.White);

            dictWithRes = PlatformBlank.GetResourceCosts(EStructureType.Barracks);

            var infoBuildBarracksSteel = new ResourceIWindowItem(
                EResourceType.Steel,
                dictWithRes[EResourceType.Steel],
                mLibSans10.MeasureString("Barracks"),
                mLibSans10);

            var infoBuildBarracksConcrete = new ResourceIWindowItem(
                EResourceType.Concrete,
                dictWithRes[EResourceType.Concrete],
                mLibSans10.MeasureString("Barracks"),
                mLibSans10);

            var infoBuildBarracksChip = new ResourceIWindowItem(
                EResourceType.Chip,
                dictWithRes[EResourceType.Chip],
                mLibSans10.MeasureString("Barracks"),
                mLibSans10);

            var infoBarrackAssign = new TextField("Assign Units to : Logistics",
                Vector2.Zero,
                mLibSans10.MeasureString("Assign Units to : Logistics"),
                mLibSans10,
                Color.White);

            var infoBarrackProduces = new TextField("Produces : Military Units",
                Vector2.Zero,
                mLibSans10.MeasureString("Produces : Military Units"),
                mLibSans10,
                Color.White);

            mInfoBuildBarracks = new InfoBoxWindow(
                itemList: new List<IWindowItem>
                {
                    infoBarracks, infoBuildCosts, infoBuildBarracksSteel, infoBuildBarracksConcrete, infoBuildBarracksChip,
                    infoBarrackAssign, infoBarrackProduces
                },
                size: mLibSans10.MeasureString("Barracks"),
                borderColor: infoBoxBorderColor,
                centerColor: infoBoxCenterColor,
                boxed: true,
                director: mDirector);

            mInfoBoxList.Add(mInfoBuildBarracks);

            #endregion

            #endregion

            #region put buttons -> buttonList -> horizontalCollection -> buildmenu -> windowList

            // create lists each containing all the buttons to place in one row
            var listSwitchingList = new List<IWindowItem> { mMainBuildingsListButton, mResourceProductionListButton, mResourceProcessingButton, mMilitaryBuildingsListButton };
            var mainBuildingsList = new List<IWindowItem> { mBlankPlatformButton, mRoadButton, mCommandcenterPlatformButton };
            var resourceProductionBuildingsList = new List<IWindowItem> { mQuarryPlatformButton, mMinePlatformButton, mWellPlatformButton, mPowerhousePlatformButton };
            var resourceProcessingBuildingsList = new List<IWindowItem> { mJunkyardPlatformButton, mFactoryPlatformButton, mStoragePlatformButton };
            var militaryBuildingsList = new List<IWindowItem> { mKineticTowerPlatformButton, mLaserTowerPlatformButton, mBarracksPlatformButton };
            // create the horizontalCollection objects which process the button-row placement
            mListSwitchingList = new HorizontalCollection(listSwitchingList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mMainBuildingsList = new HorizontalCollection(mainBuildingsList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mResourceProductionList = new HorizontalCollection(resourceProductionBuildingsList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mResourceProcessingList = new HorizontalCollection(resourceProcessingBuildingsList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            mMilitaryBuildingList = new HorizontalCollection(militaryBuildingsList, new Vector2(buildMenuWidth - 30, mBlankPlatformButton.Size.X), Vector2.Zero);
            // add the all horizontalCollection to the build menu window, deactivate all but listswitcher + mainBuildingsList
            mBuildMenuWindow.AddItem(mListSwitchingList);
            mListSwitchingList.ActiveHorizontalCollection = true;
            mBuildMenuWindow.AddItem(mMainBuildingsList);
            mMainBuildingsList.ActiveHorizontalCollection = true;
            mBuildMenuWindow.AddItem(mResourceProductionList);
            mResourceProductionList.ActiveHorizontalCollection = false;
            mBuildMenuWindow.AddItem(mResourceProcessingList);
            mResourceProcessingList.ActiveHorizontalCollection = false;
            mBuildMenuWindow.AddItem(mMilitaryBuildingList);
            mMilitaryBuildingList.ActiveHorizontalCollection = false;

            // add the build menu to the UI's windowList
            mWindowList.Add(mBuildMenuWindow);

            #endregion

            #endregion

            #region MiniMap

            mMinimapWindow = new WindowObject("", new Vector2(0, 0), new Vector2(MapConstants.MiniMapWidth + 20, MapConstants.MiniMapHeight + 20), false, mLibSans12, mDirector);
            mMinimapWindow.AddItem(mMinimap);

            mWindowList.Add(mMinimapWindow);

            #endregion

            #region infoBarWindow

            // NOTICE: this window is the only window which is compeletely created and managed in its own class
            mInfoBar = new InfoBarWindowObject(mBorderColor, mWindowColor, mLibSans14, mScreenManager, mCivilUnitsWindow, mResourceWindow, mEventLogWindow, mBuildMenuWindow, mSelectedPlatformWindow, mMinimapWindow, mDirector);

            #endregion

            // called once to set positions + called everytime the resolution changes
            ResetWindowsToStandardPositon();

            // resource window ticker - needed to get time since last update
            mResourceWindowNextTick = (mDirector.GetClock.GetIngameTime().Seconds + 5 - mDirector.GetClock.GetIngameTime().Seconds % 5) % 60;

            //This instance will handle the comunication between Sliders and DistributionManager.
            mCivilUnitsSliderHandler = new SliderHandler(ref mDirector, mDefSlider, mProductionSlider, mConstructionSlider, mLogisticsSlider, mIdleUnitsTextAndAmount);

            /*
            var id = mDirector.GetDistributionDirector.GetSomeId();
            mCivilUnitsSliderHandler.Initialize(id);
            Debug.WriteLine("id: " + id);
            mCivilUnitsSliderHandler.SetGraphId(0);*/

            // mCivilUnitsSliderHandler.Initialize(mDirector.GetDistributionDirector.GetSomeId());
            mDirector.GetActionManager.AddObject(mCivilUnitsSliderHandler,
                delegate
                {
                    mCivilUnitsSliderHandler.Initialize(mDirector.GetDistributionDirector.GetSomeId());
                    return true;
                });
            // */
        }

        /// <inheritdoc />
        public bool UpdateLower()
        {
            return true;
        }
        /// <inheritdoc />
        public bool DrawLower()
        {
            return true;
        }
        /// <inheritdoc />
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
        /// <param name="isManuallyDeactivated">true, if the platform was manually disabled</param>
        /// <param name="type">the platform's type</param>
        /// <param name="resourceAmountList">list of single resource item's</param>
        /// <param name="actionsList">list of possible actions of the platform</param>
        /// <param name="isActive">true, if the platform is active</param>
        public void SetSelectedPlatformValues(
            int id,
            bool isActive,
            bool isManuallyDeactivated,
            EStructureType type,
            IEnumerable<Resource> resourceAmountList,
            IEnumerable<IPlatformAction> actionsList)
        {
            if (!mActiveUserInterface) { return; }
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
                mSelectedPlatformIsAutoDeactivatedText.InactiveInSelectedPlatformWindow = false;
            }
            else
            {
                mSelectedPlatformIsAutoDeactivatedText.InactiveInSelectedPlatformWindow = true;
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
            mSelectedPlatformTrash.Amount = 0;

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
                    case EResourceType.Trash:
                        mSelectedPlatformTrash.Amount += 1;
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

            /*

// activate defense text/sliders + deactivate production text/sliders if the platform is a defense tower,
// else deactivate defense text/sliders + activate production text/sliders
if (type == EStructureType.Kinetic || type == EStructureType.Laser)
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
            mSelectedPlatformConstructionSlider;
            mSelectedPlatformLogisticsTextField;
            mSelectedPlatformLogisticsSlider;
*/



            #endregion

            #region actions

            // remove all elements from window + list
            foreach (var lastActionItem in mSelectedPlatformActionList)
            {
                mSelectedPlatformWindow.DeleteItem(lastActionItem);
            }
            mSelectedPlatformActionList = new List<PlatformActionIWindowItem>();

            // activate all actions possible on this platform + add them to the window
            foreach (var action in actionsList)
            {
                if (action is MakeStandardMilitaryUnit)
                {
                    var makeStandardMilitaryUnitAction = new PlatformActionIWindowItem(action,
                        mLibSans10,
                        Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y),
                        mDirector);

                    mSelectedPlatformWindow.AddItem(makeStandardMilitaryUnitAction);
                    mSelectedPlatformActionList.Add(makeStandardMilitaryUnitAction);
                }
                else if (action is MakeFastMilitaryUnit)
                {
                    var makeFastMilitaryAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    mSelectedPlatformWindow.AddItem(makeFastMilitaryAction);
                    mSelectedPlatformActionList.Add(makeFastMilitaryAction);
                }
                else if (action is MakeHeavyMilitaryUnit)
                {
                    var makeHeavyMilitaryAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero, new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    mSelectedPlatformWindow.AddItem(makeHeavyMilitaryAction);
                    mSelectedPlatformActionList.Add(makeHeavyMilitaryAction);
                }
                else if (action is ProduceMineResource)
                {
                    var produceMineResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    mSelectedPlatformWindow.AddItem(produceMineResourceAction);
                    mSelectedPlatformActionList.Add(produceMineResourceAction);
                }
                else if (action is ProduceQuarryResource)
                {
                    var produceQuarryResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    mSelectedPlatformWindow.AddItem(produceQuarryResourceAction);
                    mSelectedPlatformActionList.Add(produceQuarryResourceAction);
                }
                else if (action is ProduceWellResource)
                {
                    var produceWellResourceAction = new PlatformActionIWindowItem(action, mLibSans10, Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y), mDirector);

                    mSelectedPlatformWindow.AddItem(produceWellResourceAction);
                    mSelectedPlatformActionList.Add(produceWellResourceAction);
                }
                else if (action is BuildBluePrint)
                {
                    var buildBluePrintAction = new PlatformActionIWindowItem(action,
                        mLibSans10,
                        Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y),
                        mDirector);

                    mSelectedPlatformWindow.AddItem(buildBluePrintAction);
                    mSelectedPlatformActionList.Add(buildBluePrintAction);
                }
                else if (action is RefineResourceAction)
                {
                    var refineResourceAction = new PlatformActionIWindowItem(action,
                        mLibSans10,
                        Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y),
                        mDirector,
                        true);

                    mSelectedPlatformWindow.AddItem(refineResourceAction);
                    mSelectedPlatformActionList.Add(refineResourceAction);
                }
                else if (action is MakeGeneralUnit)
                {
                    var makeGeneralUnitAction = new PlatformActionIWindowItem(action,
                        mLibSans10,
                        Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y),
                        mDirector);

                    mSelectedPlatformWindow.AddItem(makeGeneralUnitAction);
                    mSelectedPlatformActionList.Add(makeGeneralUnitAction);
                }
                else if (action is MakeSettlerUnit)
                {
                    var makeSettlerUnitAction = new PlatformActionIWindowItem(action,
                        mLibSans10,
                        Vector2.Zero,
                        new Vector2(mSelectedPlatformWindow.Size.X - 50, mLibSans10.MeasureString("A").Y),
                        mDirector);

                    mSelectedPlatformWindow.AddItem(makeSettlerUnitAction);
                    mSelectedPlatformActionList.Add(makeSettlerUnitAction);
                }
            }

            #endregion

            // reset the window's scroll value
            if (mSelectedPlatformId != id)
            {
                mSelectedPlatformWindow.ResetScrollValue();
            }

            mSelectedPlatformId = id;
        }

        /// <summary>
        /// Gets called by the UIController when a new event was created by any object
        /// </summary>
        /// <param name="newEvent">the newly created event</param>
        /// <param name="oldEvent">the event which was thrown out of the eventList (if any)</param>
        public void UpdateEventLog(EventLogIWindowItem newEvent, EventLogIWindowItem oldEvent)
        {
            if (!mActiveUserInterface) { return; }

            // used to calculate the height the old event had to reduce the windowheight by this height
            float oldEventSizeY = 0;

            // if an old event is given, delete it + shrink eventLogWindow size by oldEvent height
            if (oldEvent != null)
            {
                mEventLogWindow.DeleteItem(oldEvent);
                oldEventSizeY = oldEvent.Size.Y;
            }

            // add the new event to the eventLogWindow
            mEventLogWindow.AddItem(newEvent);
            mEventLogWindow.AutoScrollToEnd(newEvent.Size.Y, oldEventSizeY);
        }

        /// <summary>
        /// Updates the currently shown graph in civilUnitsWindow in case it was one of the merged ones.
        /// </summary>
        /// <param name="newGraphId">merged graph ID</param>
        /// <param name="oldGraphId1">old graph ID 1</param>
        /// <param name="oldGraphId2">old graph ID 2</param>
        public void MergeGraph(int newGraphId, int oldGraphId1, int oldGraphId2)
        {
            if (!mActiveUserInterface) { return; }

            if (mCivilUnitsGraphId == oldGraphId1 || mCivilUnitsGraphId == oldGraphId2)
            {
                mCivilUnitsSliderHandler.SetGraphId(newGraphId);

                mCivilUnitsSliderHandler.Refresh();
                mCivilUnitsSliderHandler.ForceSliderPages();

                mCivilUnitsGraphId = newGraphId;
            }
        }

        /// <summary>
        /// Gets called when a graph was split. Updates the civilUnitsWindow in case the window was showing the splittedGraph
        /// </summary>
        /// <param name="splittedId"></param>
        public void SplitGraph(int splittedId)
        {
            if (mCivilUnitsGraphId == splittedId)
            {
                if (!mStructureMap.GetDictionaryGraphIdToGraph().ContainsKey(splittedId))
                {
                    mCivilUnitsGraphId = mStructureMap.GetDictionaryGraphIdToGraph().Keys.Min();
                }
                else
                {
                    if (mStructureMap.GetDictionaryGraphIdToGraph()[splittedId] == null)
                    {
                        mCivilUnitsGraphId = mStructureMap.GetDictionaryGraphIdToGraph().Keys.Min();
                    }
                }
                mCivilUnitsSliderHandler.Refresh();
                mCivilUnitsSliderHandler.ForceSliderPages();
            }
        }

        /// <summary>
        /// Automatically change graphSwitcher to get the id of the selectedPlatform and open it's unitAssignment in civilUnitsWindow
        /// </summary>
        /// <param name="graphId"></param>
        public void SelectedPlatformSetsGraphId(int graphId)
        {
            if (!mActiveUserInterface) { return; }

            mCivilUnitsGraphId = graphId;
        }

        public void UpdateSLiderHandler()
        {
            mCivilUnitsSliderHandler?.Refresh();
            mCivilUnitsSliderHandler?.ForceSliderPages();
        }

        /// <summary>
        /// Used to Deactivate the UI to activate it later (used by settler)
        /// </summary>
        private void Deactivate()
        {
            mActiveUserInterface = false;
        }

        /// <summary>
        /// Used to Activate the UI. This was thought to be used by the settler when he spawns the CommandCenter.
        /// </summary>
        public void Activate()
        {
            mActiveUserInterface = true;
        }

        private Button mCurrentlyBuildButton;

        public void BuildingProcessStarted(EStructureType structureType)
        {
            mCurrentlyBuildButton = GetButtonByStructureType(structureType);
            mCurrentlyBuildButton?.AddBorder();
        }

        public void BuildingProcessFinished()
        {
            mCurrentlyBuildButton?.RemoveBorder();
            mCurrentlyBuildButton = null;
        }

        private Button GetButtonByStructureType(EStructureType structureType)
        {
            switch (structureType)
            {
                case EStructureType.Barracks:
                    return mBarracksPlatformButton;

                case EStructureType.Blank:
                    return mBlankPlatformButton;

                case EStructureType.Command:
                    return mCommandcenterPlatformButton;

                case EStructureType.Energy:
                    return mPowerhousePlatformButton;

                case EStructureType.Factory:
                    return mFactoryPlatformButton;

                case EStructureType.Junkyard:
                    return mJunkyardPlatformButton;

                case EStructureType.Kinetic:
                    return mKineticTowerPlatformButton;

                case EStructureType.Laser:
                    return mLaserTowerPlatformButton;

                case EStructureType.Mine:
                    return mMinePlatformButton;

                case EStructureType.Packaging:
                    throw new Exception("packaging shouldn't exists");

                case EStructureType.Quarry:
                    return mQuarryPlatformButton;

                case EStructureType.Road:
                    return mRoadButton;

                case EStructureType.Storage:
                    return mStoragePlatformButton;

                case EStructureType.Well:
                    return mWellPlatformButton;
            }
            throw new Exception("Unknown stucture type");
        }

        #region button management

        #region buildMenu

        #region listSwitchings in buildMenu

        // click on mainBuildings button opens the mainBuildingsList
        private void OnButtonmMainBuildingsListButtonClick(object sender, EventArgs eventArgs)
        {
            // on click open the mainBuildingsList + close all other lists
            mMainBuildingsList.ActiveHorizontalCollection = true;
            mResourceProductionList.ActiveHorizontalCollection = false;
            mResourceProcessingList.ActiveHorizontalCollection = false;
            mMilitaryBuildingList.ActiveHorizontalCollection = false;
        }

        // click on resourceProduction button opens the resourceProductionList
        private void OnButtonmResourceProductionListClick(object sender, EventArgs eventArgs)
        {
            // on click open the resourceProductionList + close all other lists
            mMainBuildingsList.ActiveHorizontalCollection = false;
            mResourceProductionList.ActiveHorizontalCollection = true;
            mResourceProcessingList.ActiveHorizontalCollection = false;
            mMilitaryBuildingList.ActiveHorizontalCollection = false;
        }

        // mouse click on resourceProcessing button opens the resourceProcessingList
        private void OnButtonmResourceProcessingClick(object sender, EventArgs eventArgs)
        {
            // on click open the resourceProcessingList + close all other lists
            mMainBuildingsList.ActiveHorizontalCollection = false;
            mResourceProductionList.ActiveHorizontalCollection = false;
            mResourceProcessingList.ActiveHorizontalCollection = true;
            mMilitaryBuildingList.ActiveHorizontalCollection = false;
        }

        // a click on militaryBuildings button opens the militaryBuildingsList
        private void OnButtonmMilitaryBuildingsListClick(object sender, EventArgs eventArgs)
        {
            // on click open the militaryBuildingsList + close all other lists
            mMainBuildingsList.ActiveHorizontalCollection = false;
            mResourceProductionList.ActiveHorizontalCollection = false;
            mResourceProcessingList.ActiveHorizontalCollection = false;
            mMilitaryBuildingList.ActiveHorizontalCollection = true;
        }

        #endregion

        #region placements

        #region mainBuildings

        private void OnButtonmBlankPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Blank,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector, ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;


        }

        private void OnButtonmRoadButtonClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }
            mPlatformToPlace = new StructurePlacer(EStructureType.Road, EPlacementType.RoadMouseFollowAndRoad, EScreen.UserInterfaceScreen, mCamera, ref mDirector, ref mMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmCommandcenterPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Command,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        #endregion

        #region resourceProductionBuildings

        private void OnButtonmQuarryPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Quarry,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmMinePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Mine,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmWellPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Well,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmPowerhousePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Energy,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        #endregion

        #region resourceProcessingBuidings

        private void OnButtonmJunkyardPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Junkyard,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmFactoryPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Factory,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmStoragePlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Storage,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        #endregion

        #region militaryBuildings

        private void OnButtonmKineticTowerPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Kinetic,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmLaserTowerPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Laser,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        private void OnButtonmBarracksPlatformClick(object sender, EventArgs eventArgs)
        {
            if (!mCanBuildPlatform)
            {
                return;
            }

            mPlatformToPlace = new StructurePlacer(
                EStructureType.Barracks,
                EPlacementType.PlatformMouseFollowAndRoad,
                EScreen.UserInterfaceScreen,
                mCamera,
                ref mDirector,
                ref mMap,
                0f,
                0f,
                mResourceMap);

            mStructureMap.AddPlatformToPlace(mPlatformToPlace);

            mCanBuildPlatform = false;
        }

        #endregion

        #endregion

        // All build buttons show a little info window when hovering
        #region buttonHovering Info

        // NOTICE : all following hovering- or hoveringEnd- methods do basically the same:
        //              - hovering: activate infoBox
        //              - hoveringEnd: deactivate infoBox
        #region listSwitching

        private void OnButtonmMainBuildingsListButtonHovering(object sender, EventArgs eventArgs)
        {
            mInfoMainBuildingsList.Active = true;
        }
        private void OnButtonmMainBuildingsListButtonHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoMainBuildingsList.Active = false;
        }

        private void OnButtonmResourceProductionListHovering(object sender, EventArgs eventArgs)
        {
            mInfoResourceProductionList.Active = true;
        }
        private void OnButtonmResourceProductionListHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoResourceProductionList.Active = false;
        }

        private void OnButtonmResourceProcessingHovering(object sender, EventArgs eventArgs)
        {
            mInfoResourceProcessingList.Active = true;
        }
        private void OnButtonmResourceProcessingHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoResourceProcessingList.Active = false;
        }

        private void OnButtonmMilitaryBuildingsListHovering(object sender, EventArgs eventArgs)
        {
            mInfoMilitaryList.Active = true;
        }
        private void OnButtonmMilitaryBuildingsListHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoMilitaryList.Active = false;
        }

        #endregion

        #region mainBuildings

        private void OnButtonmBlankPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildBlank.Active = true;
        }
        private void OnButtonmBlankPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildBlank.Active = false;
        }

        private void OnButtonmRoadButtonHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildRoad.Active = true;
        }
        private void OnButtonmRoadButtonHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildRoad.Active = false;
        }

        private void OnButtonmCommandcenterPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildCommandcenter.Active = true;
        }
        private void OnButtonmCommandcenterPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildCommandcenter.Active = false;
        }

        #endregion

        #region resourceProduction

        private void OnButtonmQuarryPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildQuarry.Active = true;
        }
        private void OnButtonmQuarryPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildQuarry.Active = false;
        }

        private void OnButtonmMinePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildMine.Active = true;
        }
        private void OnButtonmMinePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildMine.Active = false;
        }

        private void OnButtonmWellPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildWell.Active = true;
        }
        private void OnButtonmWellPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildWell.Active = false;
        }

        private void OnButtonmPowerhousePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildPowerhouse.Active = true;
        }
        private void OnButtonmPowerhousePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildPowerhouse.Active = false;
        }

        #endregion

        #region resourceProcessing

        private void OnButtonmJunkyardPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildJunkyard.Active = true;
        }
        private void OnButtonmJunkyardPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildJunkyard.Active = false;
        }
        private void OnButtonmFactoryPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildFactory.Active = true;
        }
        private void OnButtonmFactoryPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildFactory.Active = false;
        }

        private void OnButtonmStoragePlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildStorage.Active = true;
        }
        private void OnButtonmStoragePlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildStorage.Active = false;
        }

        #endregion

        #region militaryBuildings

        private void OnButtonmKineticTowerPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildKineticTower.Active = true;
        }
        private void OnButtonmKineticTowerPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildKineticTower.Active = false;
        }

        private void OnButtonmLaserTowerPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildLaserTower.Active = true;
        }
        private void OnButtonmLaserTowerPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildLaserTower.Active = false;
        }

        private void OnButtonmBarracksPlatformHovering(object sender, EventArgs eventArgs)
        {
            mInfoBuildBarracks.Active = true;
        }
        private void OnButtonmBarracksPlatformHoveringEnd(object sender, EventArgs eventArgs)
        {
            mInfoBuildBarracks.Active = false;
        }

        #endregion

        #endregion

        #endregion

        #region selectedPlatformWindow

        #region close sections in selectedPlatformWindow

        private void CloseResourcesInSelectedWindow(object sender, EventArgs eventArgs)
        {
            foreach (var resource in mSelectedPlatformResourcesList)
            {
                resource.InactiveInSelectedPlatformWindow = !resource.InactiveInSelectedPlatformWindow;
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

        #region (de)activation of the selected platform

        // Manually deactivate a selected platform
        private void SelectedPlatformDeactivate(object sender, EventArgs eventArgs)
        {
            mUserInterfaceController.DeactivateSelectedPlatform();
        }

        // Manually activate a selected platform
        private void SelectedPlatformActivate(object sender, EventArgs eventArgs)
        {
            mUserInterfaceController.ActivateSelectedPlatform();
        }

        #endregion

        #endregion

        private void PauseMenuBeforeUi(object sender, EventArgs eventArgs)
        {
            GlobalVariables.mGameIsPaused = true;
            mScreenManager.AddScreen(mGamePauseManagerScreen);
        }

        #endregion

        #region InputManagement

        public Rectangle Bounds { get; private set; }

        public EScreen Screen { get; } = EScreen.UserInterfaceScreen;

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            return false;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            //
            return false;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            //
            return false;
        }

        #endregion
    }
}
