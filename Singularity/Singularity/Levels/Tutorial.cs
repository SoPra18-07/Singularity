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
        public Camera Camera { get; set; }

        [DataMember]
        public Map.Map Map { get; private set; }


        [DataMember]
        private GraphicsDeviceManager mGraphics;
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
            MilitaryUnit.mMilSheet = content.Load<Texture2D>("UnitSpriteSheet");
            MilitaryUnit.mGlowTexture = content.Load<Texture2D>("UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");

            //TODO: have a cone texture 
            PlatformFactory.Init(null, platformCylTexture, platformDomeTexture, platformBlankTexture);

            //Map related stuff
            Camera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 800, 800);
            mFow = new FogOfWar(Camera, mGraphics.GraphicsDevice);
            Map = new Map.Map(mapBackground, 20, 20, mFow, Camera, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS AND ADD THEM
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            Ui = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            Ui.LoadContent(content);

            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var map = Map;
            var setUnit = new Settler(new Vector2(1000, 1250), Camera, ref mDirector, ref map, GameScreen, Ui);

            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
