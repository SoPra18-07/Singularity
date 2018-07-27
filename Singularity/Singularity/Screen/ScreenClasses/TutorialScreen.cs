using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class TutorialScreen: IScreen
    {
        public string TutorialState { get; set; }

        private PopupWindow mTutorialWindow;
        private TextField mTutorialText;
        private Button mOkayButton;

        private readonly int mTextWidthScrolling;

        // textfonts
        private SpriteFont mLibSans12;
        private SpriteFont mLibSans14;

        private readonly Director mDirector;

        private Button mReopenPopup;

        private bool mLowerUpdates;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        public TutorialScreen(Director director)
        {
            mDirector = director;

            mTextWidthScrolling = 350;

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
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "This tiny rectangle is your settler. \n " +
                            "The settler can be turned into a CommandCenter by pressing b after selecting it. But before doing so, select the settler (left click) and explore your surroundings " +
                            "(left click to move, right click to deselect) \n " +
                            "We need to find a good place to settle - look out for Metal (teal) and settle near it since you will need it to expand your graph. \n" +
                            "You can move the camera by pressing w,a,s or d",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();

                        mTutorialWindow.Active = true;
                    }
                    break;
                case "UI_FirstPlatform":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Now that you've built your CommandCenter it's time to get warm with the Userinterface.\n" + 
                            "On the bottom right you can see the minimap. Use it to quickly scroll around the map and keep an overview. \n" + 
                            "\n" + 
                            "Above the minimap you can see the build menu, which can be used to extend your graph." + 
                            "Try it out! Build a blank platform by left clicking on the blankPlatform button and then place the " +
                            "platform near your CommandCenter (not on resources) by left clicking on the map. Finish the building " +
                            "process by connecting your blank Platform with your CommandCenter. \n" +
                            "You can always cancel by right clicking anywhere on the map",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();

                        mTutorialWindow.Active = true;
                    }
                    break;
                case "UI_SecondPlatform":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "You may have noticed that the platform appeared immediately, but that's just the case if the required " +
                            "resources are on the adjacent platform. Let's look at the other possibility - " +
                            "build another blank platform and connect it to the just placed platform.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "CivilUnits_Build":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "The new platform looks different than the first - it's still in blueprint mode. " +
                            "This means we have to assign units to build the platform. If you take a look at the '// CivilUnits' " +
                            "window you can see four sliders plus the number of idle units. \n" +
                            "Move the sliders to (un)assign units to the respective task: \n\n" +
                            "Defense units can be assigned to defend your graph in kinetic towers \n" +
                            "Build units build platforms, which are still in blueprint mode, by bringing required resources to adjacent platforms \n" +
                            "Logistic units carry resources between platforms \n" +
                            "Production units work on platforms and produce or refine resources \n\n" +
                            "Now assign a unit to build, so we can build the new platform",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "UserInterface_ProducePlatform":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Great job! \n" +
                            "Now that you have learned how to expand your graph it's time to produce some resources. " +
                            "Resources are produced by the resource production platforms in the build menu. \n" +
                            "We need to start by building a production platform on top of the respective resource. " +
                            "Let's begin with building a mine (under 'produce resources' in build menu) on top of the metal resource (teal). \n" +
                            "If the resource is too far away, use blank platforms to bridge the gap.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);
                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "Factory":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Wait until the mine is not in blueprint mode anymore, then assign one unit to 'production'. " +
                            "The unit will go to the mine and start producing recources. \n\n " +
                            "This will take some time, so let's use the time to continue expanding our graph. For example by building a factory. \n" +
                            "You will find the factory under 'refine resources' in the build menu.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "SelectedPlatform_ActionAssignment":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Wait until the Factory is built, then assign one idle unit to 'production'. " +
                            "The assigned unit should go into the factory and wait for further assigment. \n\n" +
                            "If you click on a platform, the selected platform menu is shown in the window at the middle at the bottom." +
                            "There you can find a list of possible platform actions. \n\n" +
                            "We want to let the factory produce steel, so just click on (de)activate below 'refining to steel' and verify that the " +
                            "action was activated by checking the activity-state beside the (de)activate button. \n\n" +
                            "Lastly, we don't need any build units right now, so just unassign it by sliding the slider to the left",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "CivilUnits_Logistics":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Now the factory is set to produce steel and a production unit is assigned, but there is no metal " +
                            "to be refined to steel, so we need a logistics unit to bring the missing metal to the factory. Assign the " +
                            "idle unit to logistics.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "BuildGeneralUnit":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "The three currently assigned units produce steel by mining metal, carrying it to the factory and refining it to steel." +
                            "Weirdly that's exactly the resource we need to produce general units - the working units. \n\n" +
                            "You can produce general units in the CommandCenter(CC). Just click on your CC and then " +
                            "click on 'create' under 'MakeGeneralUnit." +
                            "As soon as enough steel is on the CC platform an unit will be created.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "BuildDefenseBuilding":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Now that we are so far to be able to build production and refining platforms and produce general units, " +
                            "we should probably take a look at the military part of this tutorial. Let us start with the defense - a defense tower: " +
                            "- a kinetic tower, which needs general units to run or \n" +
                            "- a laser tower, which only needs energy to run. \n\n" +
                            "Let's build a laser tower. \n" +
                            "Remember that you need to assign an unit to 'build' the tower",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "Force-Deactivation":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Have you noticed how all your platforms went green? That means they were force-deactivated, " +
                            "since the graph is lacking energy..\n\n" +
                            "You can manually deactivate all platforms. Since the laser tower needs a lot of energy we can't " +
                            "provide yet - how about we deactivate it for now? \n\n" +
                            "Look into the selected platform window after clicking on the tower and choose 'deactivate'. You will notice" +
                            " that all other platforms are automatically re-activated - since enough is provided again - , " +
                            "but the unit assignments have changed." +
                            "Take care of that by assigning all units to a task you see fit." ,
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "Powerhouse":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "You've deactivated the laser tower to enable everything else to run, but " +
                            "of course we also want the laser tower to run. So we need energy. A lot of energy. " +
                            "The CC brings some energy with it, but it's obviously not enough energy to run a laser tower, " +
                            "so let's build a powerhouse to power the laser tower. \n\n " +
                            "After building the powerhouse, don't forget to activate the laser tower again and assign one unit to 'defend'.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "Barracks":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Of course both towers are only defense, so let's take a look into your offensive. \n " +
                            "We start by building a barracks. (Remember - you need build units) \n\n" + 
                            "Then re-assign at least three units to logistics after finishing the barracks",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
                case "create_MilitaryUnit":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "Great Job! \n " +
                            "We're getting really close to the end of this tutorial.. The only thing left to do is to create a " +
                            "military unit and the victory is yours! \n\n" +
                            "A little info in between: Have you seen the eventLog already? A lot of important events are shown there, like" +
                            "the newly created barracks. The eventLog does not only give you a nice overview above what's currently happening, " +
                            "you can also use it to quickly jump to the object, which raised the event. Just click on " +
                            "the first line of the eventmessage. \n\n" +
                            "But back to your task: you can create military units in the barracks just like you created the general units in the CC. " +
                            "You could, for example, build a FastMilitary unit. \n" + 
                            "Remember that you need units assigned to logistics if you want resources on a specific platform.",
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;

                case "finalPopup":
                    if (!mTutorialWindow.Active)
                    {
                        TutorialState = "AwaitingUserAction";
                        mTutorialWindow.RemoveAllItems();

                        mTutorialText = new TextField(
                            "You're almost done and you've done a fantastic job! \n\n\n" +
                            "Some tips for your next games: \n" +
                            "- minimize all windows by clicking on the 'minimize-button' in the top right corner. \n" +
                            "- move all windows by dragging it around after clicking on it's title \n" +
                            "- disable all windows by clicking on the corresponding name in the top bar \n" +
                            "- remove a building with right click \n" +
                            "- units are automatically assigned to a job somewhere on their graph, but you can force a redistribution " +
                            "by manually deactivating certain platforms \n" + 
                            "- you can build roads between two platforms \n" +
                            "- the resource window shows you your resource production in the past 10 seconds. But it gets updates only every 10 seconds \n\n\n" + 
                            "How about we relax at the end of our adventure? Unassing all units and let them run around as they wish."
                            ,
                            Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

                        mTutorialWindow.AddItem(mTutorialText);
                        mTutorialWindow.ResetScrollValue();
                        mTutorialWindow.Active = true;
                    }
                    break;
            }

            if (mTutorialWindow.Active)
            {
                mTutorialWindow?.Update(gametime);
            }
            else
            {
                mReopenPopup.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mTutorialWindow.Active)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

                mTutorialWindow?.Draw(spriteBatch);

                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                mReopenPopup.Draw(spriteBatch);
                spriteBatch.End();
            }
        }

        public EScreen Screen { get; } = EScreen.TutorialScreen;

        public void LoadContent(ContentManager content)
        {
            mLibSans12 = content.Load<SpriteFont>("LibSans12");
            mLibSans14 = content.Load<SpriteFont>("LibSans14");

            mOkayButton = new Button("Close", mLibSans12, Vector2.Zero) {Opacity = 1f};
            mOkayButton.ButtonReleased += OkayButtonReleased;

            mReopenPopup = new Button("Task", mLibSans12, Vector2.Zero, true) {Opacity = 1f};
            mReopenPopup.Position = new Vector2((mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth - mReopenPopup.Size.X) / 2, 30);
            mReopenPopup.ButtonReleased += ReopenReleased;

            var position = new Vector2(mDirector.GetGraphicsDeviceManager.PreferredBackBufferWidth / 2 - 200, mDirector.GetGraphicsDeviceManager.PreferredBackBufferHeight / 2 - 150);

            mTutorialWindow = new PopupWindow("// Tutorial", mOkayButton, position, new Vector2(400, 300), Color.YellowGreen, Color.Orange, mLibSans14, mDirector.GetInputManager, EScreen.TutorialScreen);

            mTutorialText = new TextField("Welcome! \n\n This short tutorial will show you a little bit around the game. \n\n " +
                                          " Have fun!", Vector2.Zero, new Vector2(mTextWidthScrolling, 0), mLibSans12, Color.White);

            mTutorialWindow.AddItem(mTutorialText);

            mTutorialWindow.Active = true;

            Loaded = true;
        }

        public bool Loaded { get; set; }

        public bool UpdateLower()
        {
            return mLowerUpdates;
        }

        public bool DrawLower()
        {
            return true;
        }

        private void OkayButtonReleased(object sender, EventArgs eventArgs)
        {
            mLowerUpdates = true;
            mTutorialWindow.Active = false;
        }

        private void ReopenReleased(object sender, EventArgs eventArgs)
        {
            mTutorialWindow.Active = true;
        }
    }
}
