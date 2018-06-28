using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    class Tutorial
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
        private CommandCenter mPlatform;

        public Tutorial(GraphicsDevice graphics, ref Director dir, ContentManager content)
        {
            mDirector = dir;
            dir.GetStoryManager.SetLevelType(leveltype: LevelType.Tutorial);
            dir.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            LoadContent(content: content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>(assetName: "Cylinders");
            var platformBlankTexture = content.Load<Texture2D>(assetName: "PlatformBasic");
            var mapBackground = content.Load<Texture2D>(assetName: "backgroundGrid");

            //Map related stuff
            mMap = new Map.Map(backgroundTexture: mapBackground, width: 20, height: 20, viewport: mGraphics.Viewport, director: ref mDirector, debug: true);
            mCamera = mMap.GetCamera();
            mFow = new FogOfWar(camera: mCamera, graphicsDevice: mGraphics);

            //INITIALIZE GAMESCREEN
            mGameScreen = new GameScreen(graphicsDevice: mGraphics, director: ref mDirector, map: mMap, camera: mCamera, fow: mFow);

            //IngameObjects stuff
            mPlatform = new CommandCenter(position: new Vector2(x: 1000, y: 500), spriteSheet: platformCylTexture, baseSprite: platformBlankTexture, director: ref mDirector);
            var genUnit = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit2 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit3 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit4 = new GeneralUnit(platform: mPlatform, director: ref mDirector);
            var genUnit5 = new GeneralUnit(platform: mPlatform, director: ref mDirector);

            //Finally add the objects
            mFow.AddRevealingObject(revealingObject: mPlatform);

            mMap.AddPlatform(platform: mPlatform);

            mGameScreen.AddObject(toAdd: mPlatform);
            mGameScreen.AddObject(toAdd: genUnit);
            mGameScreen.AddObject(toAdd: genUnit2);
            mGameScreen.AddObject(toAdd: genUnit3);
            mGameScreen.AddObject(toAdd: genUnit4);
            mGameScreen.AddObject(toAdd: genUnit5);
        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
