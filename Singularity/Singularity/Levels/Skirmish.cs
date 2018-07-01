using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    internal sealed class Skirmish: ILevel
    {
        [DataMember]
        private GameScreen mGameScreen;
        [DataMember]
        private GraphicsDeviceManager mGraphics;
        [DataMember]
        private Map.Map mMap;
        [DataMember]
        private Camera mCamera;
        [DataMember]
        private FogOfWar mFow;
        [DataMember]
        private Director mDirector;
        [DataMember]
        private UserInterfaceScreen mUi;

        [DataMember]
        private IScreenManager mScreenManager;

        //GameObjects to initialize:
        [DataMember]
        private PlatformBlank mPlatform;

        public Skirmish(GraphicsDeviceManager graphics, ref Director dir, ContentManager content, IScreenManager screenmanager)
        {
            mDirector = dir;
            dir.GetStoryManager.SetLevelType(LevelType.Skirmish);
            dir.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            LoadContent(content);
            mScreenManager = screenmanager;
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

            //INITIALIZE SCREENS AND ADD THEM TO THE SCREENMANAGER
            mGameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, mMap, mCamera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, mGameScreen, mScreenManager);

            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mUi);

            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            mPlatform = new PlatformBlank(new Vector2(1000, 1000), null, platformBlankTexture, ref mDirector);
            mGameScreen.AddObject(mPlatform);

            // this is done via the factory to test, so I can instantly see if something is some time off.
            var platform2 = PlatformFactory.Get(EPlatformType.Well, ref mDirector, 800, 1000, mMap.GetResourceMap());
            mGameScreen.AddObject(platform2);

            var road1 = new Road(mPlatform, platform2, false);
            mGameScreen.AddObject(road1);

            //var platform2 = new Well(new Vector2(800, 1000), platformDomeTexture, platformBlankTexture, mMap.GetResourceMap(), ref mDirector);
            var platform3 = new Quarry(new Vector2(1200, 1200),
                platformDomeTexture,
                platformBlankTexture,
                mMap.GetResourceMap(),
                ref mDirector);
            mGameScreen.AddObject(platform3);
            var road2 = new Road(platform2, platform3, false);
            mGameScreen.AddObject(road2);
            var road3 = new Road(platform3, mPlatform, false);
            mGameScreen.AddObject(road3);

            var platform4 = new EnergyFacility(new Vector2(1000, 800),
                platformDomeTexture,
                platformBlankTexture, ref mDirector);
            mGameScreen.AddObject(platform4);
            var road4 = new Road(mPlatform, platform4, false);
            mGameScreen.AddObject(road4);

            var road5 = new Road(platform4, platform3, false);
            mGameScreen.AddObject(road5);


            //GenUnits
            var genUnit = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit2 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit3 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit4 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit5 = new GeneralUnit(mPlatform, ref mDirector);

            //MilUnits
            var milUnit = new MilitaryUnit(new Vector2(2000, 700), milUnitSheet, mCamera, ref mDirector, ref mMap);

            //SetUnit
            var setUnit = new Settler(new Vector2(1000, 1250), mCamera, ref mDirector, ref mMap, mGameScreen, mUi);
            

            // Resources
            var res = new Resource(EResourceType.Trash, platform2.Center);
            var res4 = new Resource(EResourceType.Trash, platform2.Center);
            var res5 = new Resource(EResourceType.Trash, platform2.Center);
            var res2 = new Resource(EResourceType.Chip, platform3.Center);
            var res3 = new Resource(EResourceType.Oil, platform4.Center);

            platform2.StoreResource(res);
            platform3.StoreResource(res2);
            platform4.StoreResource(res3);
            platform2.StoreResource(res4);
            platform2.StoreResource(res5);

            mGameScreen.AddObject(genUnit);
            mGameScreen.AddObject(genUnit2);
            mGameScreen.AddObject(genUnit3);
            mGameScreen.AddObject(genUnit4);
            mGameScreen.AddObject(genUnit5);
            mGameScreen.AddObject(milUnit);
            mGameScreen.AddObject(setUnit);


            //TESTMETHODS HERE =====================================
            mDirector.GetDistributionManager.DistributeJobs(JobType.Idle, JobType.Production, 5);
            mDirector.GetDistributionManager.TestAttributes();

        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
