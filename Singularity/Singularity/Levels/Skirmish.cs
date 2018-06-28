using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    internal sealed class Skirmish
    {
        [DataMember]
        private GameScreen mGameScreen;
        [DataMember]
        private GraphicsDevice mGraphics;
        [DataMember]
        private Map.Map mMap;
        [DataMember]
        private Camera mCamera;
        [DataMember]
        private FogOfWar mFow;
        [DataMember]
        private Director mDirector;

        //GameObjects to initialize:
        [DataMember]
        private PlatformBlank mPlatform;

        public Skirmish(GraphicsDevice graphics, ref Director director, ContentManager content)
        {
            mDirector = director;
            director.GetStoryManager.SetLevelType(leveltype: LevelType.Skirmish);
            director.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            LoadContent(content: content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>(assetName: "Cylinders");
            var platformBlankTexture = content.Load<Texture2D>(assetName: "PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>(assetName: "Dome");
            var milUnitSheet = content.Load<Texture2D>(assetName: "UnitSpriteSheet");
            var mapBackground = content.Load<Texture2D>(assetName: "backgroundGrid");

            //Map related stuff
            mMap = new Map.Map(backgroundTexture: mapBackground, width: 20, height: 20, viewport: mGraphics.Viewport, director: ref mDirector, neo: true); // NEOLAYOUT (searchmark for @fkarg)
            mCamera = mMap.GetCamera();
            mFow = new FogOfWar(camera: mCamera, graphicsDevice: mGraphics);

            //INITIALIZE GAMESCREEN
            mGameScreen = new GameScreen(graphicsDevice: mGraphics, director: ref mDirector, map: mMap, camera: mCamera, fow: mFow);

            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            mPlatform = new PlatformBlank(position: new Vector2(x: 1000, y: 1000), platformSpriteSheet: null, baseSprite: platformBlankTexture, director: ref mDirector);
            var platform2 = new Well(position: new Vector2(x: 800, y: 1000), platformSpriteSheet: platformDomeTexture, baseSprite: platformBlankTexture, resource: mMap.GetResourceMap(), director: ref mDirector);
            var platform3 = new Quarry(position: new Vector2(x: 1200, y: 1200),
                platformSpriteSheet: platformDomeTexture,
                baseSprite: platformBlankTexture,
                resource: mMap.GetResourceMap(),
                director: ref mDirector);
            var platform4 = new EnergyFacility(position: new Vector2(x: 1000, y: 800),
                platformSpriteSheet: platformDomeTexture,
                baseSprite: platformBlankTexture,
                director: ref mDirector);

            //GenUnits
            var genUnit = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit2 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit3 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit4 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit5 = new GeneralUnit(platform: mPlatform, director: ref mDirector);

            //MilUnits
            var milUnit = new MilitaryUnit(position: new Vector2(x: 2000, y: 700), spriteSheet: milUnitSheet, camera: mMap.GetCamera(), director: ref mDirector, map: ref mMap);

            //Roads
            var road1 = new Road(source: mPlatform, destination: platform2, blueprint: false);
            var road2 = new Road(source: platform2, destination: platform3, blueprint: false);
            var road3 = new Road(source: platform3, destination: mPlatform, blueprint: false);
            var road4 = new Road(source: mPlatform, destination: platform4, blueprint: false);
            var road5 = new Road(source: platform4, destination: platform3, blueprint: false);

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

            //Finally add the objects

            //GAMESCREEN=====================
            mGameScreen.AddObject(toAdd: mPlatform);
            mGameScreen.AddObject(toAdd: platform2);
            mGameScreen.AddObject(toAdd: platform3);
            mGameScreen.AddObject(toAdd: platform4);
            mGameScreen.AddObject(toAdd: road1);
            mGameScreen.AddObject(toAdd: road2);
            mGameScreen.AddObject(toAdd: road3);
            mGameScreen.AddObject(toAdd: road4);
            mGameScreen.AddObject(toAdd: road5);
            mGameScreen.AddObject(toAdd: genUnit);
            mGameScreen.AddObject(toAdd: genUnit2);
            mGameScreen.AddObject(toAdd: genUnit3);
            mGameScreen.AddObject(toAdd: genUnit4);
            mGameScreen.AddObject(toAdd: genUnit5);
            mGameScreen.AddObject(toAdd: milUnit);

            //TESTMETHODS HERE =====================================
            mDirector.GetDistributionManager.DistributeJobs(oldj: JobType.Idle, newj: JobType.Production, amount: 3);
            mDirector.GetDistributionManager.TestAttributes();
        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
