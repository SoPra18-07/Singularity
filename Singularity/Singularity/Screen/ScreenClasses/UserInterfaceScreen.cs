using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// TODO
    /// </summary>
    internal sealed class UserInterfaceScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        private List<WindowObject> mWindowList;
        private SpriteFont mLibSans20;
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;
        private readonly InputManager mInputManager;

        // mouse position
        private float mMouseX;
        private float mMouseY;

        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        private readonly GraphicsDeviceManager mGraphics;

        #region testing

        // TODO: DELETE TESTING
        private Texture2D mCurrentPlatform;

        private Slider mSprintSlider;
        private Button mSprintButton2;
        private Button mSprintButton3;
        private Button mSprintButton4;
        private Button mSprintButton5;
        private Button mSprintButton6;
        private Button mSprintButton7;
        private Button mSprintButton8;
        private Button mSprintButton9;
        private Button mOkayButton;

        private Button mBlankPlatformButton;
        private Button mJunkyardButton;
        private Button mEnergyFacilityButton;
        private Texture2D mPlatformBlankTexture;
        private Texture2D mPlatformDomeTexture;

        private PopupWindow mTestWindow;
        private bool mActiveWindow;

        private List<PopupWindow> mPopupWindowList;

        private TextField mTextFieldTest;
        private TextField mTextFieldTestPopup;

        #endregion

        public UserInterfaceScreen(InputManager inputManager, GraphicsDeviceManager mgraphics)
        {
            mInputManager = inputManager;
            mGraphics = mgraphics;

            inputManager.AddMousePositionListener(this);
            inputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);

            mCurrentPlatform = null;

            Bounds = new Rectangle(0,0, mgraphics.PreferredBackBufferWidth, mgraphics.PreferredBackBufferHeight);
        }

        public void Update(GameTime gametime)
        {
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
            }

            // TODO
            if (!mActiveWindow)
            {
                if (mPopupWindowList.Contains(mTestWindow))
                {
                    mPopupWindowList.Remove(mTestWindow);
                }
            }
            else
            {
                foreach (var window in mPopupWindowList)
                {
                    window.Update(gametime);
                }
            }

            mBlankPlatformButton.Update(gametime);

            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch);
            }

            // TODO
            foreach (var popupWindow in mPopupWindowList)
            {
                popupWindow.Draw(spriteBatch);
            }

            if (mCurrentPlatform != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(mCurrentPlatform, new Vector2(mMouseX, mMouseY), new Color(255, 255, 255));
                spriteBatch.End();
            }

        }

        public void LoadContent(ContentManager content)
        {
            mLibSans20 = content.Load<SpriteFont>("LibSans20");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");
            mLibSans12 = content.Load<SpriteFont>("LibSans12");

            // test windows object
            mWindowList = new List<WindowObject>();

            mCurrentScreenWidth = mGraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mGraphics.PreferredBackBufferHeight;

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            var borderColor = new Color(0.68f, 0.933f, 0.933f, .8f);

            // set position- and size-values for all windows of the userinterface
            var topBarHeight = mCurrentScreenHeight / 30;
            var topBarWidth = mCurrentScreenWidth;

            var civilUnitsX = topBarHeight / 2;
            var civilUnitsY = topBarHeight / 2 + topBarHeight;
            var civilUnitsWidth = (int)(mCurrentScreenWidth / 4.5);
            var civilUnitsHeight = (int)(mCurrentScreenHeight / 1.8);

            var resourceX = topBarHeight / 2;
            var resourceY = 2 * (topBarHeight / 2) + topBarHeight + civilUnitsHeight;
            var resourceWidth = civilUnitsWidth;
            var resourceHeight = (int)(mCurrentScreenHeight / 2.75);

            var eventLogX = civilUnitsX + civilUnitsWidth + 50; // TODO
            var eventLogY = civilUnitsY;
            var eventLogWidth = civilUnitsWidth;
            var eventLogHeight = (int)(mCurrentScreenHeight / 2.5);

            // create windowObjects for all windows of the userinterface
            var civilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(civilUnitsX, civilUnitsY), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, 10f, 10f, true, mLibSans14, mInputManager, mGraphics);
            var topBarWindow = new WindowObject("", new Vector2(0, 0), new Vector2(topBarWidth, topBarHeight), borderColor, windowColor, 10f, 10f, false, mLibSans20, mInputManager, mGraphics);
            var resourceWindow = new WindowObject("// RESOURCES", new Vector2(resourceX, resourceY), new Vector2(resourceWidth, resourceHeight), true, mLibSans14, mInputManager, mGraphics);
            var eventLogWindow = new WindowObject("// EVENT LOG", new Vector2(eventLogX, eventLogY), new Vector2(eventLogWidth, eventLogHeight), borderColor, windowColor, 10f, 10f, true, mLibSans14, mInputManager, mGraphics);


            #region testing1
            // TODO

            mPopupWindowList = new List<PopupWindow>();

            var testText =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In sed nisi venenatis massa vehicula suscipit. " +
                "Morbi scelerisque urna et feugiat sodales. Sed tristique arcu a odio faucibus, sit amet pharetra ligula accumsan. " +
                "Curabitur volutpat, lectus nec lacinia eleifend, erat magna maximus dui, vitae ultrices purus dui at velit. " +
                "Etiam nisl nisl, ultricies ut odio at, pharetra pharetra augue. Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Aenean ante leo, porttitor sed cursus in, rhoncus a risus. Morbi urna tortor, luctus id tincidunt ac, bibendum nec magna. " +
                "Sed fringilla posuere felis, eu blandit lacus posuere nec. Nunc et laoreet lacus. Maecenas ac laoreet lectus. " +
                "Quisque semper feugiat diam, eu posuere mi posuere ac. Vestibulum posuere posuere semper. " +
                "Pellentesque nec lacinia nulla, sit amet scelerisque justo.";

            var testText2 =
                "Neuronale Methoden werden vor allem dann eingesetzt, wenn es darum geht, " +
                "aus schlechten oder verrauschten Daten Informationen zu gewinnen, aber " +
                "auch Algorithmen, die sich neuen Situationen anpassen, also lernen, " +
                "sind typisch fuer die Neuroinformatik. Dabei unterscheidet man grundsaetzlich " +
                "ueberwachtes Lernen und unueberwachtes Lernen, ein Kompromiss zwischen beiden " +
                "Techniken ist das Reinforcement-Lernen. Assoziativspeicher sind eine besondere " +
                "Anwendung neuronaler Methoden, und damit oft Forschungsgegenstand der Neuroinformatik. " +
                "Viele Anwendungen fuer kuenstliche neuronale Netze finden sich auch in der Mustererkennung und vor allem im Bildverstehen.";

            var testWindowColor = new Color(0.27f, 0.5f, 0.7f, 1f);
            var testBorderColor = new Color(0.68f, 0.933f, 0.933f, 1f);

            mOkayButton = new Button("Okay", mLibSans12, Vector2.Zero, borderColor) { Opacity = 1f };

            mActiveWindow = true;

            mOkayButton.ButtonClicked += OnButtonClickOkayButton;

            mTestWindow = new PopupWindow("// POPUP", mOkayButton , new Vector2(500, 400), new Vector2(250, 250), testBorderColor, testWindowColor, mLibSans14, mLibSans12, mInputManager, mGraphics);

            // platform textures
            mPlatformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            mPlatformDomeTexture = content.Load<Texture2D>("Dome");


            // buttons PlatformWindow
            //mBlankPlatformButton = new Button(0.2f, mPlatformBlankTexture, Vector2.Zero, true) {Opacity = 1f}; // opacity is crucial or it won't be drawn since it's default value seems to be 0
            mBlankPlatformButton = new Button("Blank", mLibSans20, Vector2.Zero) {Opacity = 1f};
            mJunkyardButton = new Button("Junkyard", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mEnergyFacilityButton = new Button("EnergyFacility", mLibSans20, Vector2.Zero) { Opacity = 1f };

            mBlankPlatformButton.ButtonClicked += OnButtonClickBlank;
            mJunkyardButton.ButtonClicked += OnButtonClickJunkyard;
            mEnergyFacilityButton.ButtonClicked += OnButtonClickEnergyFacility;

            civilUnitsWindow.AddItem(mBlankPlatformButton);
            civilUnitsWindow.AddItem(mJunkyardButton);
            civilUnitsWindow.AddItem(mEnergyFacilityButton);

            mSprintSlider = new Slider(Vector2.Zero, 100, 15, mLibSans12);
            mTextFieldTest = new TextField(testText, Vector2.One, new Vector2(resourceWidth - 50, resourceHeight), mLibSans12);
            mTextFieldTestPopup = new TextField(testText2, Vector2.One, new Vector2(resourceWidth - 50, resourceHeight), mLibSans12);
            mSprintButton2 = new Button("button2", mLibSans12, Vector2.Zero) { Opacity = 1f };
            mSprintButton3 = new Button("button3", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton4 = new Button("button4", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton5 = new Button("button5", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton6 = new Button("button6", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton7 = new Button("button7", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton8 = new Button("button8", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton9 = new Button("button9", mLibSans20, Vector2.Zero) { Opacity = 1f };

            //resourceWindow.AddItem(mSprintSlider);
            resourceWindow.AddItem(mTextFieldTest);
            civilUnitsWindow.AddItem(mSprintButton2);
            civilUnitsWindow.AddItem(mSprintButton3);
            civilUnitsWindow.AddItem(mSprintButton4);
            civilUnitsWindow.AddItem(mSprintButton5);
            civilUnitsWindow.AddItem(mSprintButton6);
            civilUnitsWindow.AddItem(mSprintButton7);
            civilUnitsWindow.AddItem(mSprintButton8);
            civilUnitsWindow.AddItem(mSprintButton9);

            mTestWindow.AddItem(mTextFieldTestPopup);

            mPopupWindowList.Add(mTestWindow);

            #endregion

            // add all windowObjects of the userinterface
            mWindowList.Add(civilUnitsWindow);
            //mWindowList.Add(topBarWindow);
            mWindowList.Add(resourceWindow);
            mWindowList.Add(eventLogWindow);
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

        private void OnButtonClickBlank(object sender, EventArgs eventArgs)
        {
            mCurrentPlatform = mPlatformBlankTexture;
        }

        private void OnButtonClickJunkyard(object sender, EventArgs eventArgs)
        {
            mCurrentPlatform = mPlatformDomeTexture;
        }

        private void OnButtonClickEnergyFacility(object sender, EventArgs eventArgs)
        {
            mCurrentPlatform = mPlatformDomeTexture;
        }

        private void OnButtonClickOkayButton(object sender, EventArgs eventArgs)
        {
            mActiveWindow = false;
        }

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }

        public Rectangle Bounds { get; }
        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.RightClick)
            {
                mCurrentPlatform = null;
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            // 
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            //
        }
    }
}
