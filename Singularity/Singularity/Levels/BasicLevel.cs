using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Serialization;
using Singularity.Units;

namespace Singularity.Levels
{
    [DataContract]
    internal abstract class BasicLevel : ILevel, IKeyListener
    {

        [DataMember]
        public EScreen Screen { get; private set; }

        private DebugScreen mDebugscreen;

        [DataMember]
        public GameScreen GameScreen { get; set; }

        [DataMember]
        public Camera Camera { get; set; }

        public UserInterfaceScreen Ui { get; set; }

        [DataMember]
        public Map.Map Map { get; set; }

        protected GraphicsDeviceManager mGraphics;

        [DataMember]
        protected FogOfWar mFow;

        [DataMember]
        protected Director mDirector;

        protected IScreenManager mScreenManager;

        private ContentManager mContent;

        protected BasicLevel(GraphicsDeviceManager graphics,
            ref Director director,
            ContentManager content,
            IScreenManager screenmanager)

        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(LevelType.Skirmish, this);
            mDirector.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            mScreenManager = screenmanager;
            mContent = content;
            Screen = EScreen.GameScreen;
            StandardInitialization(content);
        }

        private void StandardInitialization(ContentManager content)
        {
            //Load stuff
            var platformConeTexture = content.Load<Texture2D>("Cones");
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>("Dome");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");
            var libSans12 = content.Load<SpriteFont>("LibSans12");

            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture, libSans12);
            //Map related stuff
            Camera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 2800, 2800);
            mFow = new FogOfWar(Camera, mGraphics.GraphicsDevice);

            var map = new Map.Map(mapBackground, 60, 60, mFow, Camera, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)
            Map = map;
            var milUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");
            var milGlowSheet = content.Load<Texture2D>("UnitGlowSprite");

            MilitaryUnit.mMilSheet = milUnitSheet;
            MilitaryUnit.mGlowTexture = milGlowSheet;
            mDirector.GetMilitaryManager.SetMap(ref map);

            //INITIALIZE SCREENS
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, Map, Camera, mScreenManager);
            mDirector.GetUserInterfaceController.ControlledUserInterface = Ui; // the UI needs to be added to the controller

            // the input manager keeps this from not getting collected by the GC
            mDebugscreen = new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);

            mDirector.GetInputManager.FlagForAddition(this);
        }

        public void ReloadContent(ContentManager content, GraphicsDeviceManager graphics, ref Director director, IScreenManager screenmanager)
        {
            mScreenManager = screenmanager;
            mGraphics = graphics;
            //Load stuff
            var platformConeTexture = content.Load<Texture2D>("Cones");
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>("Dome");
            MilitaryUnit.mMilSheet = content.Load<Texture2D>("UnitSpriteSheet");
            MilitaryUnit.mGlowTexture = content.Load<Texture2D>("UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");
            var libSans12 = content.Load<SpriteFont>("LibSans12");

            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture, libSans12);
            PlatformBlank.mLibSans12 = libSans12;
            director.ReloadContent(mDirector, Map.GetMeasurements());
            mDirector = director;

            //Map related stuff
            Camera.ReloadContent(mGraphics, ref mDirector);
            mFow.ReloadContent(mGraphics, Camera);
            Map.ReloadContent(mapBackground, Camera, mFow, ref mDirector, content);
            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, Map, Camera, mScreenManager);
            Ui.LoadContent(content);
            Ui.Loaded = true;
            GameScreen.ReloadContent(content, graphics, Map, mFow, Camera, ref mDirector, Ui);
            mDirector.GetUserInterfaceController.ReloadContent(ref mDirector);
            mDirector.GetUserInterfaceController.ControlledUserInterface = Ui; // the UI needs to be added to the controller

            // Reload map for the military manager
            var map = Map;
            director.GetMilitaryManager.ReloadSetMap(ref map);
            // the input manager keeps this from not getting collected by the GC
            mDebugscreen = new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);
            mDirector.GetInputManager.FlagForAddition(this);
        }

        public abstract void LoadContent(ContentManager content);

        public bool KeyTyped(KeyEvent keyevent)
        {
            // b key is used to convert the settler unit into a command center
            var keyArray = keyevent.CurrentKeys;
            foreach (var key in keyArray)
            {
                // if key b has been pressed and the settler unit is selected and its not moving
                // --> send out event that deletes settler and adds a command center
                if (key == Keys.Q)
                {
                    mDirector.GetStoryManager.SaveAchievements();
                    XSerializer.Save(this, "xXxSwaglordofYolonessxXx.xml", false);
                    return false;
                }
            }

            return true;
        }


        public bool KeyPressed(KeyEvent keyEvent)
        {
            return true;
        }


        public bool KeyReleased(KeyEvent keyEvent)
        {
            return true;
        }
    }
}
