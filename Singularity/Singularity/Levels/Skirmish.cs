using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    internal sealed class Skirmish : ILevel
    {
        [DataMember]
        public GameScreen GameScreen { get; set; }
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

        public Skirmish(GraphicsDeviceManager graphics, ref Director director, ContentManager content, IScreenManager screenmanager)

        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(LevelType.Skirmish, this);
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

            //INITIALIZE SCREENS AND ADD THEM TO THE SCREENMANAGER
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, mMap, mCamera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);

            mScreenManager.AddScreen(GameScreen);
            mScreenManager.AddScreen(mUi);

            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            mPlatform = new PlatformBlank(new Vector2(1000, 1000), null, platformBlankTexture, ref mDirector);
            GameScreen.AddObject(mPlatform);

            // this is done via the factory to test, so I can instantly see if something is some time off.
            var platform2 = PlatformFactory.Get(EPlatformType.Well, ref mDirector, 800, 1000, mMap.GetResourceMap());
            GameScreen.AddObject(platform2);

            var road1 = new Road(mPlatform, platform2, false);
            GameScreen.AddObject(road1);

            //var platform2 = new Well(new Vector2(800, 1000), platformDomeTexture, platformBlankTexture, mMap.GetResourceMap(), ref mDirector);
            var platform3 = new Quarry(new Vector2(1200, 1200),
                platformDomeTexture,
                platformBlankTexture,
                mMap.GetResourceMap(),
                ref mDirector);
            GameScreen.AddObject(platform3);
            var road2 = new Road(platform2, platform3, false);
            GameScreen.AddObject(road2);
            var road3 = new Road(platform3, mPlatform, false);
            GameScreen.AddObject(road3);

            var platform4 = new EnergyFacility(new Vector2(1000, 800),
                platformDomeTexture,
                platformBlankTexture, ref mDirector);
            GameScreen.AddObject(platform4);
            var road4 = new Road(mPlatform, platform4, false);
            GameScreen.AddObject(road4);

            var road5 = new Road(platform4, platform3, false);
            GameScreen.AddObject(road5);


            //GenUnits
            var genUnit = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit2 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit3 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit4 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit5 = new GeneralUnit(mPlatform, ref mDirector);

            //MilUnits
            var milUnit = new MilitaryUnit(new Vector2(2000, 700), milUnitSheet, mCamera, ref mDirector, ref mMap);

            //SetUnit
            var setUnit = new Settler(new Vector2(1000, 1250), mCamera, ref mDirector, ref mMap, GameScreen, mUi);
            

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

            GameScreen.AddObject(genUnit);
            GameScreen.AddObject(genUnit2);
            GameScreen.AddObject(genUnit3);
            GameScreen.AddObject(genUnit4);
            GameScreen.AddObject(genUnit5);
            GameScreen.AddObject(milUnit);
            GameScreen.AddObject(setUnit);


            //TESTMETHODS HERE ====================================
            mDirector.GetDistributionManager.RequestResource(platform2, EResourceType.Oil, null);
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
