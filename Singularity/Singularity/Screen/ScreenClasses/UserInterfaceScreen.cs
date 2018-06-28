using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Sound;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// the userInterface screen
    /// </summary>
    internal sealed class UserInterfaceScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        // list of windows to show on the UI
        private List<WindowObject> mWindowList;

        // fonts used for the texts
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;

        // manage input
        private readonly InputManager mInputManager;

        // mouse position
        private float mMouseX;
        private float mMouseY;

        // used to calculate the positions of the windows at the beginning
        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        // needed to calculate screen-sizes
        private readonly GraphicsDeviceManager mGraphics;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        #region civilUnits members

        private Slider mDefSlider;
        private Slider mBuildSlider;
        private Slider mLogisticsSlider;
        private Slider mProductionSlider;

        private Director mDirector;

        private TextField mDefTextField;
        private TextField mBuildTextField;
        private TextField mLogisticsTextField;
        private TextField mProductionTextField;

        #endregion

        #region resourceWindow members
        // TODO : ALL WITH RESOURCE-IWindowItems
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

        /// <summary>
        /// Creates a UserInterface with it's windows
        /// </summary>
        /// <param name="director"></param>
        /// <param name="mgraphics"></param>
        public UserInterfaceScreen(ref Director director, GraphicsDeviceManager mgraphics)
        {
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
        }

        public void Update(GameTime gametime)
        {
            // update all windows
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
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

            // update screen size TODO : UPDATE POSITIONS OF THE WINDOWS WHEN RES-CHANGE IN PAUSE MENU
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;
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

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");

            // resolution
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;

            #region windows pos+size calculation

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            var borderColor = new Color(0.68f, 0.933f, 0.933f, .8f);

            // position + size of topBar
            var topBarHeight = mCurrentScreenHeight / 30;
            var topBarWidth = mCurrentScreenWidth;

            // position + size of civilUnits window
            var civilUnitsX = topBarHeight / 2;
            var civilUnitsY = topBarHeight / 2 + topBarHeight;
            var civilUnitsWidth = (int)(mCurrentScreenWidth / 4.5);
            var civilUnitsHeight = (int)(mCurrentScreenHeight / 1.8);

            // position + size of resource window
            var resourceX = topBarHeight / 2;
            var resourceY = 2 * (topBarHeight / 2) + topBarHeight + civilUnitsHeight;
            var resourceWidth = civilUnitsWidth;
            var resourceHeight = (int)(mCurrentScreenHeight / 2.75);

            // position + size of eventLog window
            var eventLogWidth = civilUnitsWidth;
            var eventLogHeight = (int)(mCurrentScreenHeight / 2.5);
            var eventLogX = mCurrentScreenWidth - eventLogWidth - topBarHeight / 2; //civilUnitsX + civilUnitsWidth + 50; // TODO
            var eventLogY = civilUnitsY;

            #endregion

            // TODO
            #region topBarWindow

            // var topBarWindow = new WindowObject("", new Vector2(0, 0), new Vector2(topBarWidth, topBarHeight), borderColor, windowColor, 10f, 10f, false, mLibSans20, mInputManager, mGraphics);

            // create items

            // add all items

            #endregion

            // TODO
            #region eventLogWindow

            var eventLogWindow = new WindowObject("// EVENT LOG", new Vector2(eventLogX, eventLogY), new Vector2(eventLogWidth, eventLogHeight), borderColor, windowColor, 10f, 10f, true, mLibSans14, mInputManager, mGraphics);

            // create items

            // add all items

            mWindowList.Add(eventLogWindow);

            #endregion

            #region civilUnitsWindow

            var civilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(civilUnitsX, civilUnitsY), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, windowColor, 10, 20, true, mLibSans14, mInputManager, mGraphics);

            // create items
            //TODO: Create an object representing the Idle units at the moment. Something like "Idle: 24" should be enough
            mDefTextField = new TextField("Defense", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mDefSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector, true, true, 5);
            mBuildTextField = new TextField("Build", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mBuildSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mLogisticsTextField = new TextField("Logistics", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mLogisticsSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);
            mProductionTextField = new TextField("Production", Vector2.Zero, new Vector2(civilUnitsWidth, civilUnitsWidth), mLibSans12);
            mProductionSlider = new Slider(Vector2.Zero, 150, 10, mLibSans12, ref mDirector);


            //Subscribe Distr to sliders
            //Still need to be implemented. A Container for all the sliders would be very useful!
            //mDefSlider.SliderMoving += mDirector.GetDistributionManager.DefSlider();
            //mBuildSlider.SliderMoving += mDirector.GetDistributionManager.BuildSlider();
            //mLogisticsSlider.SliderMoving += mDirector.GetDistributionManager.LogisticsSlider();
            //mProductionSlider.SliderMoving += mDirector.GetDistributionManager.ProductionSlider();

            // adding all items
            civilUnitsWindow.AddItem(mDefTextField);
            civilUnitsWindow.AddItem(mDefSlider);
            civilUnitsWindow.AddItem(mBuildTextField);
            civilUnitsWindow.AddItem(mBuildSlider);
            civilUnitsWindow.AddItem(mLogisticsTextField);
            civilUnitsWindow.AddItem(mLogisticsSlider);
            civilUnitsWindow.AddItem(mProductionTextField);
            civilUnitsWindow.AddItem(mProductionSlider);

            mWindowList.Add(civilUnitsWindow);

            #endregion

            // TODO
            #region resourceWindow

            var resourceWindow = new WindowObject("// RESOURCES", new Vector2(resourceX, resourceY), new Vector2(resourceWidth, resourceHeight), true, mLibSans14, mInputManager, mGraphics);

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

            resourceWindow.AddItem(mTextFieldTest);

            #endregion

            mWindowList.Add(resourceWindow);

            #endregion

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

        private void OnButtonClickOkayButton(object sender, EventArgs eventArgs)
        {
            mActiveWindow = false;
            mInputManager.RemoveMousePositionListener(mTestPopupWindow);
            mInputManager.RemoveMouseWheelListener(mTestPopupWindow);
        }

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
