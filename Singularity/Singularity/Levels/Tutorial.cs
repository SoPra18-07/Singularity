using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Screen;
using Singularity.Platforms;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    class Tutorial: ILevel
    {
        [DataMember]
        public GameScreen GameScreen { get; set; }
        [DataMember]
        private GraphicsDeviceManager mGraphics;
        private Map.Map mMap;
        [DataMember]
        private Camera mCamera;
        [DataMember]
        private FogOfWar mFow;
        [DataMember]
        private Director mDirector;

        [DataMember]
        public UserInterfaceScreen Ui { get; set; }
        [DataMember]
        private IScreenManager mScreenManager;

        //GameObjects to initialize:
        [DataMember]
        private CommandCenter mPlatform;

        public Tutorial(GraphicsDeviceManager graphics, ref Director director, ContentManager content, IScreenManager screenmanager)
        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(LevelType.Tutorial, this);
            mDirector.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            mScreenManager = screenmanager;
            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>("Dome");
            var milUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");

            //TODO: have a cone texture 
            PlatformFactory.Init(null, platformCylTexture, platformDomeTexture, platformBlankTexture);

            //Map related stuff
            mCamera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 800, 800);
            mFow = new FogOfWar(mCamera, mGraphics.GraphicsDevice);
            mMap = new Map.Map(mapBackground, 20, 20, mFow, mGraphics.GraphicsDevice.Viewport, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, mMap, mCamera, mFow);
            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            Ui.LoadContent(content);


            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var setUnit = new Settler(new Vector2(1000, 1250), mCamera, ref mDirector, ref mMap, GameScreen, Ui);
            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
