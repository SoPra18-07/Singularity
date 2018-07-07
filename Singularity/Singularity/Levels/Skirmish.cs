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

        public Camera Camera { get; set; }


        public Map.Map Map { get; set; }



        [DataMember]
        private GraphicsDeviceManager mGraphics;
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
            mDirector.GetStoryManager.SetLevelType(leveltype: LevelType.Skirmish, level: this);
            mDirector.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            mScreenManager = screenmanager;
            LoadContent(content: content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>(assetName: "Cylinders");
            var platformBlankTexture = content.Load<Texture2D>(assetName: "PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>(assetName: "Dome");
            var milUnitSheet = content.Load<Texture2D>(assetName: "UnitSpriteSheet");
            var milGlowSheet = content.Load<Texture2D>(assetName: "UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>(assetName: "backgroundGrid");
            var libSans12 = content.Load<SpriteFont>(assetName: "LibSans12");

            //TODO: have a cone texture
            PlatformFactory.Init(coneSheet: null, cylinderSheet: platformCylTexture, domeSheet: platformDomeTexture, blankSheet: platformBlankTexture, libSans12: libSans12);

            //Map related stuff
            Camera = new Camera(graphics: mGraphics.GraphicsDevice, director: ref mDirector, x: 800, y: 800);
            mFow = new FogOfWar(camera: Camera, graphicsDevice: mGraphics.GraphicsDevice);
            Map = new Map.Map(backgroundTexture: mapBackground, width: 20, height: 20, fow: mFow, camera: Camera, director: ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS AND ADD THEM TO THE SCREENMANAGER
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            var debugScreen = new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);

            mScreenManager.AddScreen(screen: GameScreen);
            mScreenManager.AddScreen(screen: mUi);

            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            mPlatform = new PlatformBlank(position: new Vector2(x: 1000, y: 1000), platformSpriteSheet: null, baseSprite: platformBlankTexture, libSans12Font: libSans12, director: ref mDirector);
            GameScreen.AddObject(toAdd: mPlatform);

            // this is done via the factory to test, so I can instantly see if something is some time off.
            var platform2 = PlatformFactory.Get(type: EPlatformType.Well, director: ref mDirector, x: 800, y: 1000, resourceMap: Map.GetResourceMap());
            GameScreen.AddObject(toAdd: platform2);

            var road1 = new Road(source: mPlatform, destination: platform2, blueprint: false);
            GameScreen.AddObject(toAdd: road1);

            //var platform2 = new Well(new Vector2(800, 1000), platformDomeTexture, platformBlankTexture, mMap.GetResourceMap(), ref mDirector);
            var platform3 = new Quarry(position: new Vector2(x: 1200, y: 1200),
                platformSpriteSheet: platformDomeTexture,
                baseSprite: platformBlankTexture,
                libSans12: libSans12,
                resource: Map.GetResourceMap(),
                director: ref mDirector);
            GameScreen.AddObject(toAdd: platform3);
            var road2 = new Road(source: platform2, destination: platform3, blueprint: false);
            GameScreen.AddObject(toAdd: road2);
            var road3 = new Road(source: platform3, destination: mPlatform, blueprint: false);
            GameScreen.AddObject(toAdd: road3);

            var platform4 = new EnergyFacility(position: new Vector2(x: 1000, y: 800),
                platformSpriteSheet: platformDomeTexture,
                baseSprite: platformBlankTexture, libSans12: libSans12, director: ref mDirector);
            GameScreen.AddObject(toAdd: platform4);
            var road4 = new Road(source: mPlatform, destination: platform4, blueprint: false);
            GameScreen.AddObject(toAdd: road4);

            var road5 = new Road(source: platform4, destination: platform3, blueprint: false);
            GameScreen.AddObject(toAdd: road5);


            //GenUnits
            var genUnit = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit2 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit3 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit4 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit5 = new GeneralUnit(platform: mPlatform, director: ref mDirector);

            //MilUnits
            var map = Map;
            MilitaryUnit.mMilSheet = milUnitSheet;
            MilitaryUnit.mGlowTexture = milGlowSheet;
            var milUnit = new MilitaryUnit(position: new Vector2(x: 2000, y: 700), camera: Camera, director: ref mDirector, map: ref map);

            //SetUnit
            var setUnit = new Settler(position: new Vector2(x: 1000, y: 1250), camera: Camera, director: ref mDirector, map: ref map, gameScreen: GameScreen, ui: mUi);


            // Resources
            var res = new Resource(type: EResourceType.Trash, position: platform2.Center);
            var res4 = new Resource(type: EResourceType.Trash, position: platform2.Center);
            var res5 = new Resource(type: EResourceType.Trash, position: platform2.Center);
            var res2 = new Resource(type: EResourceType.Chip, position: platform3.Center);
            var res3 = new Resource(type: EResourceType.Oil, position: platform4.Center);

            platform2.StoreResource(resource: res);
            platform3.StoreResource(resource: res2);
            platform4.StoreResource(resource: res3);
            platform2.StoreResource(resource: res4);
            platform2.StoreResource(resource: res5);

            GameScreen.AddObject(toAdd: genUnit);
            GameScreen.AddObject(toAdd: genUnit2);
            GameScreen.AddObject(toAdd: genUnit3);
            GameScreen.AddObject(toAdd: genUnit4);
            GameScreen.AddObject(toAdd: genUnit5);
            GameScreen.AddObject(toAdd: milUnit);
            GameScreen.AddObject(toAdd: setUnit);

            //TESTMETHODS HERE ====================================
            mDirector.GetDistributionManager.RequestResource(platform: platform2, resource: EResourceType.Oil, action: null);
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
