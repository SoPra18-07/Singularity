using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    public class TutorialScreen: IScreen, IMousePositionListener, IMouseClickListener, IMouseWheelListener
    {
        public string TutorialState { get; set; }

        private PopupWindow mTutorialWindow;
        private TextField mTutorialText;
        private ResourceIWindowItem mResourceExample;
        private Button mOkayButton;

        private int mTextWidth;
        private int mTextWidthScrolling;

        // textfonts
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;

        private bool mLowerUpdates;

        private bool mPopupOpen;

        private Director mDirector;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        public TutorialScreen(Director director)
        {
            mDirector = director;

            mTextWidth = 250;

            mTextWidthScrolling = 200;

            mLowerUpdates = false;

            mPopupOpen = true;

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };
        }

        public void Update(GameTime gametime)
        {
            switch (TutorialState)
            {
                case "AwaitingUserAction":
                    break;
                case "Beginning":
                    if (!mPopupOpen)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.DeleteItem(mTutorialText);

                        mTutorialText = new TextField(
                            "This tiny rectangle is your settler. \n " +
                            "The settler can be turned into a CommandCenter by pressing b after selecting it. But before doing so, select the settler (left click) and explore your surroundings \n " +
                            "(left click to move, right click to deselect) \n " +
                            "We need to find a good place to settle - look out for the three basic resources: Water (blue), Metal (teal) and Oil (brown) and settle near them.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();

                        mPopupOpen = true;
                    }
                    break;
                case "UI_FirstPlatform":
                    if (!mPopupOpen)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.DeleteItem(mTutorialText);

                        mTutorialText = new TextField(
                            "Now that you've built your CommandCenter it's time to get warm with the Userinterface.\n" + 
                            "On the bottom right you can see the minimap. Use it to quickly scroll around the map and keep an overview. \n" + 
                            "\n" + 
                            "Above the minimap you can see the build menu, which can be used to extend your graph." + 
                            "Try it out! Build a blank platform by left clicking on the blankPlatform button and then place the platform near your CommandCenter by left clicking on the ground. Finish the building process by connecting your blank Platform with your CommandCenter.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();

                        mPopupOpen = true;
                    }
                    break;
                case "UI_SecondPlatform":
                    if (!mPopupOpen)
                    {
                        mTutorialWindow.DeleteItem(mTutorialText);

                        mTutorialText = new TextField(
                            "Great job! \n" +
                            "Now that you have learned how to expand your graph it's time to produce some resources. Resources are produced by the resource production platforms in the build menu. \n" +
                            "We need to start by building a production platform on top of the corresponding resource.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mPopupOpen = true;
                    }
                    break;
                case "ProducePlatform":
                    if (!mPopupOpen)
                    {
                        TutorialState = "UserInterface_BuildMenu";
                        mTutorialWindow.DeleteItem(mTutorialText);

                        mTutorialText = new TextField(
                            "Great job! \n" +
                            "Now that you have learned how to expand your graph it's time to produce some resources. Resources are produced by the resource production platforms in the build menu. \n" +
                            "We need to start by building a production platform on top of the corresponding resource." ,
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mPopupOpen = true;
                    }
                    break;
                default:
                    break;
            }

            if (mPopupOpen)
            {
                mTutorialWindow?.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mPopupOpen)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

                mTutorialWindow?.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

        public void LoadContent(ContentManager content)
        {
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");

            mOkayButton = new Button("Close", mLibSans12, Vector2.Zero) {Opacity = 1f};
            mOkayButton.ButtonReleased += OkayButtonReleased;

            var position = new Vector2(mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth / 2 - 100, mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight / 2 - 100);

            mTutorialWindow = new PopupWindow("// Tutorial", mOkayButton, position, new Vector2(250, 300), Color.YellowGreen, Color.Orange, mLibSans14, mDirector.GetInputManager, EScreen.TutorialScreen);

            mTutorialText = new TextField("Welcome! \n This short tutorial will show you a little bit around the game. \n\n Have fun!", Vector2.Zero, new Vector2(mTextWidth, 0), mLibSans12, Color.White);

            mTutorialWindow.AddItem(mTutorialText);

            Loaded = true;
        }

        public bool Loaded { get; set; }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }

        private void OkayButtonReleased(object sender, EventArgs eventArgs)
        {
            mLowerUpdates = true;
            mPopupOpen = false;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            throw new System.NotImplementedException();
        }

        public Rectangle Bounds { get; }

        public EScreen Screen { get; } = EScreen.TutorialScreen;

        public bool MouseWheelValueChanged(EMouseAction mouseAction)
        {
            throw new System.NotImplementedException();
        }
        
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            throw new System.NotImplementedException();
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            throw new System.NotImplementedException();
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            throw new System.NotImplementedException();
        }
    }
}
