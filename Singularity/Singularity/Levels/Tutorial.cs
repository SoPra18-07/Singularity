using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    class Tutorial: ILevel
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
        private CommandCenter mPlatform;

        public Tutorial(GraphicsDeviceManager graphics, ref Director dir, ContentManager content, IScreenManager screenmanager)
        {
            mDirector = dir;
            dir.GetStoryManager.SetLevelType(LevelType.Tutorial);
            dir.GetStoryManager.LoadAchievements();
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

            //INITIALIZE SCREENS AND ADD THEM
            mGameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, mMap, mCamera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, mGameScreen, mScreenManager);

            mScreenManager.AddScreen(mGameScreen);
            mScreenManager.AddScreen(mUi);

            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var setUnit = new Settler(new Vector2(1000, 1250), mCamera, ref mDirector, ref mMap, mGameScreen, mUi);
            mGameScreen.AddObject(setUnit);


            //TESTMETHODS HERE =====================================
        }

        public GameScreen GetGameScreen()
        {
            return mGameScreen;
        }
    }
}
