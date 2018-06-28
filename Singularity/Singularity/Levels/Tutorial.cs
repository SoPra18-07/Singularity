using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
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
            dir.GetStoryManager.SetLevelType(LevelType.Tutorial);
            dir.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");

            //Map related stuff
            mMap = new Map.Map(mapBackground, 20, 20, mGraphics.Viewport, ref mDirector);
            mCamera = mMap.GetCamera();
            mFow = new FogOfWar(mCamera, mGraphics);

            //INITIALIZE GAMESCREEN
            mGameScreen = new GameScreen(mGraphics, ref mDirector, mMap, mCamera, mFow);

            //IngameObjects stuff
            mPlatform = new CommandCenter(new Vector2(1000, 500), platformCylTexture, platformBlankTexture, ref mDirector);
            var genUnit = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit2 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit3 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit4 = new GeneralUnit(mPlatform, ref mDirector);
            var genUnit5 = new GeneralUnit(mPlatform, ref mDirector);

            //Finally add the objects
            mFow.AddRevealingObject(mPlatform);

            mMap.AddPlatform(mPlatform);

            mGameScreen.AddObject(mPlatform);
            mGameScreen.AddObject(genUnit);
            mGameScreen.AddObject(genUnit2);
            mGameScreen.AddObject(genUnit3);
            mGameScreen.AddObject(genUnit4);
            mGameScreen.AddObject(genUnit5);
        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
