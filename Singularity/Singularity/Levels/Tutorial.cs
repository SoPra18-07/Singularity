using System;
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
    class Tutorial : ILevel
    {
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

        public GameScreen GameScreen { get; set; }

        public Tutorial(GraphicsDevice graphics, ref Director director, ContentManager content)
        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(LevelType.Tutorial, this);
            mDirector.GetStoryManager.LoadAchievements();
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
            mCamera = new Camera(mGraphics, ref mDirector);
            mFow = new FogOfWar(mCamera, mGraphics);
            mMap = new Map.Map(mapBackground, 20, 20, mFow, mGraphics.Viewport, ref mDirector);

            //INITIALIZE GAMESCREEN
            GameScreen = new GameScreen(mGraphics, ref mDirector, mMap, mCamera, mFow);

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

            GameScreen.AddObject(mPlatform);
            GameScreen.AddObject(genUnit);
            GameScreen.AddObject(genUnit2);
            GameScreen.AddObject(genUnit3);
            GameScreen.AddObject(genUnit4);
            GameScreen.AddObject(genUnit5);
        }
    }
}
