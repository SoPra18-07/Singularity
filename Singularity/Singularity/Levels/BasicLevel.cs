using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    [DataContract]
    internal abstract class BasicLevel : ILevel
    {
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

        protected Texture2D mPlatformBlankTexture;

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

            StandardInitialization(content);
        }

        private void StandardInitialization(ContentManager content)
        {
            //Load stuff
            var platformConeTexture = content.Load<Texture2D>("Cones");
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>("Dome");
            MilitaryUnit.mMilSheet = content.Load<Texture2D>("UnitSpriteSheet");
            MilitaryUnit.mGlowTexture = content.Load<Texture2D>("UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");

            //TODO: have a cone texture
            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture);

            //Map related stuff
            Camera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 2800, 2800);
            mFow = new FogOfWar(Camera, mGraphics.GraphicsDevice);
            Map = new Map.Map(mapBackground, 60, 60, mFow, Camera, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            Ui.LoadContent(content);
            mDirector.GetUserInterfaceController.ControlledUserInterface = Ui; // the UI needs to be added to the controller

            // the input manager keeps this from not getting collected by the GC
            mDebugscreen = new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);
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

            //TODO: have a cone texture
            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture);

            director.ReloadContent(mDirector);
            mDirector = director;

            //Map related stuff
            Camera.ReloadContent(mGraphics, ref mDirector);
            mFow.ReloadContent(mGraphics);
            Map.ReloadContent(mapBackground, Camera, mFow, ref mDirector);

            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            Ui.LoadContent(content);

            GameScreen.ReloadContent(content, graphics, Map, mFow, Camera, ref mDirector, Ui);
            mDirector.GetUserInterfaceController.ControlledUserInterface = Ui; // the UI needs to be added to the controller

            // the input manager keeps this from not getting collected by the GC
            mDebugscreen = new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);
        }

        public abstract void LoadContent(ContentManager content);
    }
}
