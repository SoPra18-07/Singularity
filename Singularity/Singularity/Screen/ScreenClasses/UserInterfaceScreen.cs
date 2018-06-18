using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Map;
using Singularity.Platform;

namespace Singularity.Screen.ScreenClasses
{
    class UserInterfaceScreen : IScreen, IMousePositionListener, IMouseClickListener
    {
        private List<WindowObject> mWindowList;
        private SpriteFont mLibSans20;
        private InputManager mInputManager;
        private GameScreen mGameScreen;

        // TODO: DELETE TESTING
        private Button mBlankPlatformButton;
        private Button mJunkyardButton;
        private Button mEnergyFacilityButton;
        private Texture2D mPlatformBlankTexture;
        private Texture2D mPlatformDomeTexture;

        // TODO: BETTER WAY SOMEHOW?
        private float mMouseX;
        private float mMouseY;

        // TODO: UPDATE - ADDED PLATFORMS
        private Texture2D mCurrentPlatform;

        // TODO: DELETE TESTING
        private Button mSprintButton1;
        private Button mSprintButton2;
        private Button mSprintButton3;
        private Button mSprintButton4;
        private Button mSprintButton5;
        private Button mSprintButton6;
        private Button mSprintButton7;
        private Button mSprintButton8;
        private Button mSprintButton9;

        public UserInterfaceScreen(InputManager inputManager, GameScreen gameScreen)
        {
            mInputManager = inputManager;
            mGameScreen = gameScreen;

            inputManager.AddMousePositionListener(this);
            inputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);

            mCurrentPlatform = null;
        }

        public void Update(GameTime gametime)
        {
            foreach (var window in mWindowList)
            {
                window.Update(gametime);
            }

            mBlankPlatformButton.Update(gametime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var window in mWindowList)
            {
                window.Draw(spriteBatch);
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

            // test windows object
            mWindowList = new List<WindowObject>();

            var currentScreenWidth = 1080;
            var currentScreenHeight = 720;

            // change color for the border or the filling of all userinterface windows here
            var windowColor = new Color(255, 0, 0, .8f);
            var borderColor = new Color(50, 50, 50, .8f);

            // set position- and size-values for all windows of the userinterface
            var topBarHeight = currentScreenHeight / 30;
            var topBarWidth = currentScreenWidth;

            var civilUnitsX = topBarHeight / 2;
            var civilUnitsY = topBarHeight / 2 + topBarHeight;
            var civilUnitsWidth = (int)(currentScreenWidth / 4.5);
            var civilUnitsHeight = (int)(currentScreenHeight / 1.8);

            var resourceX = topBarHeight / 2;
            var resourceY = 2 * (topBarHeight / 2) + topBarHeight + civilUnitsHeight;
            var resourceWidth = civilUnitsWidth;
            var resourceHeight = (int)(currentScreenHeight / 2.75);

            var eventLogX = civilUnitsX + civilUnitsWidth + 50; // TODO
            var eventLogY = civilUnitsY;
            var eventLogWidth = civilUnitsWidth;
            var eventLogHeight = (int)(currentScreenHeight / 2.5);

            // create windowObjects for all windows of the userinterface
            // INFO: parameters are: NAME, POSITION-vector, SIZE-vector, COLOR of border, COLOR of filling, opacity of everything that gets drawn, borderPadding, objectPadding, minimizable, fontForText, inputmanager)
            var civilUnitsWindow = new WindowObject("// CIVIL UNITS", new Vector2(civilUnitsX, civilUnitsY), new Vector2(civilUnitsWidth, civilUnitsHeight), borderColor, windowColor, 10f, 10f, true, mLibSans20, mInputManager);
            var topBarWindow = new WindowObject("", new Vector2(0, 0), new Vector2(topBarWidth, topBarHeight), borderColor, windowColor, 10f, 10f, false, mLibSans20, mInputManager);
            var resourceWindow = new WindowObject("// RESOURCES", new Vector2(resourceX, resourceY), new Vector2(resourceWidth, resourceHeight), borderColor, windowColor, 10f, 10f, true, mLibSans20, mInputManager);
            var eventLogWindow = new WindowObject("// EVENT LOG", new Vector2(eventLogX, eventLogY), new Vector2(eventLogWidth, eventLogHeight), borderColor, windowColor, 10f, 10f, true, mLibSans20, mInputManager);

            // platform textures
            mPlatformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            mPlatformDomeTexture = content.Load<Texture2D>("Dome");


            // buttons PlatformWindow
            //mBlankPlatformButton = new Button(0.2f, mPlatformBlankTexture, Vector2.Zero, true) {Opacity = 1f}; // opacity is crucial or it won't be drawn since it's default value seems to be 0
            mBlankPlatformButton = new Button("Blank", mLibSans20, Vector2.Zero) {Opacity = 1f};
            mJunkyardButton = new Button("Junkyard", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mEnergyFacilityButton = new Button("EnergyFacility", mLibSans20, Vector2.Zero) { Opacity = 1f };

            // TODO: FIX AFTER FURTHER IMPLEMENTATION IF NEEDED
            /*            mBlankPlatformButton.ButtonReleased += OnButtonReleaseBlank;
                        mJunkyardButton.ButtonReleased += OnButtonReleaseJunkyard;
                        mEnergyFacilityButton.ButtonReleased += OnButtonReleaseEnergyFacility;*/

            mBlankPlatformButton.ButtonClicked += OnButtonClickBlank;
            mJunkyardButton.ButtonClicked += OnButtonClickJunkyard;
            mEnergyFacilityButton.ButtonClicked += OnButtonClickEnergyFacility;


            civilUnitsWindow.AddItem(mBlankPlatformButton);
            civilUnitsWindow.AddItem(mJunkyardButton);
            civilUnitsWindow.AddItem(mEnergyFacilityButton);

            // TODO: DELETE TESTING - JUST FOR SPRINT
            mSprintButton1 = new Button("button1", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton2 = new Button("button2", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton3 = new Button("button3", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton4 = new Button("button4", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton5 = new Button("button5", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton6 = new Button("button6", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton7 = new Button("button7", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton8 = new Button("button8", mLibSans20, Vector2.Zero) { Opacity = 1f };
            mSprintButton9 = new Button("button9", mLibSans20, Vector2.Zero) { Opacity = 1f };

            resourceWindow.AddItem(mSprintButton1);
            resourceWindow.AddItem(mSprintButton2);
            resourceWindow.AddItem(mSprintButton3);
            resourceWindow.AddItem(mSprintButton4);
            resourceWindow.AddItem(mSprintButton5);
            resourceWindow.AddItem(mSprintButton6);
            resourceWindow.AddItem(mSprintButton7);
            resourceWindow.AddItem(mSprintButton8);
            resourceWindow.AddItem(mSprintButton9);

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

        /*        private void OnButtonReleaseBlank(object sender, EventArgs eventArgs)
                {
                    mPlatform = new PlatformBlank(new Vector2(mMouseX, mMouseY), mPlatformBlankTexture);
                    mGameScreen.AddObject(mPlatform);
                }

                private void OnButtonReleaseJunkyard(object sender, EventArgs eventArgs)
                {
                    mPlatform = new Junkyard(new Vector2(mMouseX, mMouseY), mPlatformBlankTexture);
                    mGameScreen.AddObject(mPlatform);
                }

                private void OnButtonReleaseEnergyFacility(object sender, EventArgs eventArgs) 
                {
                    mPlatform = new EnergyFacility(new Vector2(mMouseX, mMouseY), mPlatformBlankTexture);
                    mGameScreen.AddObject(mPlatform);
                }*/

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
