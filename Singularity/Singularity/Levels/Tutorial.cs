using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
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
        private UserInterfaceScreen mUi;
        [DataMember]
        private IScreenManager mScreenManager;

        //GameObjects to initialize:
        [DataMember]
        private CommandCenter mPlatform;

        public Tutorial(GraphicsDeviceManager graphics, ref Director director, ContentManager content, IScreenManager screenmanager)
        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(leveltype: LevelType.Tutorial, level: this);
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
            MilitaryUnit.mMilSheet = content.Load<Texture2D>(assetName: "UnitSpriteSheet");
            MilitaryUnit.mGlowTexture = content.Load<Texture2D>(assetName: "UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>(assetName: "backgroundGrid");
            var libSans12 = content.Load<SpriteFont>(assetName: "LibSans12");

            //TODO: have a cone texture
            PlatformFactory.Init(coneSheet: null, cylinderSheet: platformCylTexture, domeSheet: platformDomeTexture, blankSheet: platformBlankTexture, libSans12: libSans12);

            //Map related stuff
            Camera = new Camera(graphics: mGraphics.GraphicsDevice, director: ref mDirector, x: 800, y: 800);
            mFow = new FogOfWar(camera: Camera, graphicsDevice: mGraphics.GraphicsDevice);
            Map = new Map.Map(backgroundTexture: mapBackground, width: 20, height: 20, fow: mFow, camera: Camera, director: ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS AND ADD THEM
            GameScreen = new GameScreen(graphicsDevice: mGraphics.GraphicsDevice, director: ref mDirector, map: Map, camera: Camera, fow: mFow);
            mUi = new UserInterfaceScreen(director: ref mDirector, mgraphics: mGraphics, gameScreen: GameScreen, stackScreenManager: mScreenManager);

            mScreenManager.AddScreen(screen: GameScreen);
            mScreenManager.AddScreen(screen: mUi);


            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var map = Map;
            var setUnit = new Settler(position: new Vector2(x: 1000, y: 1250), camera: Camera, director: ref mDirector, map: ref map, gameScreen: GameScreen, ui: mUi);
            GameScreen.AddObject(toAdd: setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
