using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;

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
            mInputManager.AddMousePositionListener(iMouseListener: this);
            mInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.InBoundsOnly, rightClickType: EClickType.InBoundsOnly);
            Bounds = new Rectangle(x: 0,y: 0, width: mgraphics.PreferredBackBufferWidth, height: mgraphics.PreferredBackBufferHeight);

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
                window.Update(gametime: gametime);
            }

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

            // update screen size TODO : UPDATE POSITIONS OF THE WINDOWS WHEN RES-CHANGE IN PAUSE MENU
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;
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

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            // load all spritefonts
            mLibSans20 = content.Load<SpriteFont>(assetName: "LibSans20");
            mLibSans14 = content.Load<SpriteFont>(assetName: "LibSans14");
            mLibSans12 = content.Load<SpriteFont>(assetName: "LibSans12");

            // resolution
            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;

            #region windows pos+size calculation

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 0.8f);
            var borderColor = new Color(r: 0.68f, g: 0.933f, b: 0.933f, alpha: .8f);

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

            var eventLogWindow = new WindowObject(windowName: "// EVENT LOG", position: new Vector2(x: eventLogX, y: eventLogY), size: new Vector2(x: eventLogWidth, y: eventLogHeight), colorBorder: borderColor, colorFill: windowColor, borderPadding: 10f, objectPadding: 10f, minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

            // create items

            // add all items

            mWindowList.Add(item: eventLogWindow);

            #endregion

            #region civilUnitsWindow

            var civilUnitsWindow = new WindowObject(windowName: "// CIVIL UNITS", position: new Vector2(x: civilUnitsX, y: civilUnitsY), size: new Vector2(x: civilUnitsWidth, y: civilUnitsHeight), colorBorder: borderColor, colorFill: windowColor, borderPadding: 10, objectPadding: 20, minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

            // create items
            //TODO: Create an object representing the Idle units at the moment. Something like "Idle: 24" should be enough
            mDefTextField = new TextField(text: "Defense", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mDefSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector, withValueBox: true, withPages: true, pages: 5);
            mBuildTextField = new TextField(text: "Build", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mBuildSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector);
            mLogisticsTextField = new TextField(text: "Logistics", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mLogisticsSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector);
            mProductionTextField = new TextField(text: "Production", position: Vector2.Zero, size: new Vector2(x: civilUnitsWidth, y: civilUnitsWidth), spriteFont: mLibSans12);
            mProductionSlider = new Slider(position: Vector2.Zero, length: 150, sliderSize: 10, font: mLibSans12, director: ref mDirector);


            //Subscribe Distr to sliders
            //Still need to be implemented. A Container for all the sliders would be very useful!
            //mDefSlider.SliderMoving += mDirector.GetDistributionManager.DefSlider();
            //mBuildSlider.SliderMoving += mDirector.GetDistributionManager.BuildSlider();
            //mLogisticsSlider.SliderMoving += mDirector.GetDistributionManager.LogisticsSlider();
            //mProductionSlider.SliderMoving += mDirector.GetDistributionManager.ProductionSlider();

            // adding all items
            civilUnitsWindow.AddItem(item: mDefTextField);
            civilUnitsWindow.AddItem(item: mDefSlider);
            civilUnitsWindow.AddItem(item: mBuildTextField);
            civilUnitsWindow.AddItem(item: mBuildSlider);
            civilUnitsWindow.AddItem(item: mLogisticsTextField);
            civilUnitsWindow.AddItem(item: mLogisticsSlider);
            civilUnitsWindow.AddItem(item: mProductionTextField);
            civilUnitsWindow.AddItem(item: mProductionSlider);

            mWindowList.Add(item: civilUnitsWindow);

            #endregion

            // TODO
            #region resourceWindow

            var resourceWindow = new WindowObject(windowName: "// RESOURCES", position: new Vector2(x: resourceX, y: resourceY), size: new Vector2(x: resourceWidth, y: resourceHeight), minimizable: true, spriteFont: mLibSans14, inputManager: mInputManager, graphics: mGraphics);

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

            resourceWindow.AddItem(item: mTextFieldTest);

            #endregion

            mWindowList.Add(item: resourceWindow);

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
            mInputManager.RemoveMousePositionListener(iMouseListener: mTestPopupWindow);
            mInputManager.RemoveMouseWheelListener(iMouseWheelListener: mTestPopupWindow);
        }

        #region InputManagement

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
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
