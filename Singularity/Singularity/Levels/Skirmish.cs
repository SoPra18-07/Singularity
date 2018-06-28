using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
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

        public Skirmish(GraphicsDevice graphics, ref Director dir, ContentManager content)
        {
            mDirector = dir;
            dir.GetStoryManager.SetLevelType(LevelType.Skirmish);
            dir.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
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

            //Map related stuff
            mMap = new Map.Map(mapBackground, 20, 20, mGraphics.Viewport, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)
            mCamera = mMap.GetCamera();
            mFow = new FogOfWar(mCamera, mGraphics);

            //INITIALIZE GAMESCREEN
            mGameScreen = new GameScreen(mGraphics, ref mDirector, mMap, mCamera, mFow);

            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            mPlatform = new PlatformBlank(new Vector2(1000, 1000), null, platformBlankTexture);
            var platform2 = new Well(new Vector2(800, 1000), platformDomeTexture, platformBlankTexture, mMap.GetResourceMap(), ref mDirector);
            var platform3 = new Quarry(new Vector2(1200, 1200),
                platformDomeTexture,
                platformBlankTexture,
                mMap.GetResourceMap(),
                ref mDirector);
            var platform4 = new EnergyFacility(new Vector2(1000, 800),
                platformDomeTexture,
                platformBlankTexture);

            //GenUnits
            var genUnit = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit2 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit3 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit4 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit5 = new GeneralUnit(mPlatform, ref mDirector);

            //MilUnits
            var milUnit = new MilitaryUnit(new Vector2(2000, 700), milUnitSheet, mMap.GetCamera(), ref mDirector, ref mMap);

            //Roads
            var road1 = new Road(mPlatform, platform2, false);
            var road2 = new Road(platform2, platform3, false);
            var road3 = new Road(platform3, mPlatform, false);
            var road4 = new Road(mPlatform, platform4, false);
            var road5 = new Road(platform4, platform3, false);

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

            //Finally add the objects
            //FOG OF WAR =====================
            mFow.AddRevealingObject(mPlatform);
            mFow.AddRevealingObject(platform2);
            mFow.AddRevealingObject(platform3);
            mFow.AddRevealingObject(platform4);

            mFow.AddRevealingObject(milUnit);

            //MAP============================
            mMap.AddPlatform(mPlatform);
            mMap.AddPlatform(platform2);
            mMap.AddPlatform(platform3);
            mMap.AddPlatform(platform4);

            mMap.AddRoad(road1);
            mMap.AddRoad(road2);
            mMap.AddRoad(road3);
            mMap.AddRoad(road4);
            mMap.AddRoad(road5);


            //GAMESCREEN=====================
            mGameScreen.AddObject(mPlatform);
            mGameScreen.AddObject(platform2);
            mGameScreen.AddObject(platform3);
            mGameScreen.AddObject(platform4);
            mGameScreen.AddObject(road1);
            mGameScreen.AddObject(road2);
            mGameScreen.AddObject(road3);
            mGameScreen.AddObject(road4);
            mGameScreen.AddObject(road5);
            mGameScreen.AddObject(genUnit);
            mGameScreen.AddObject(genUnit2);
            mGameScreen.AddObject(genUnit3);
            mGameScreen.AddObject(genUnit4);
            mGameScreen.AddObject(genUnit5);
            mGameScreen.AddObject(milUnit);
        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
