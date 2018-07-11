using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
    internal abstract class BasicLevel : ILevel
    {
        [DataMember]
        public GameScreen GameScreen { get; set; }

        public Camera Camera { get; set; }


        public Map.Map Map { get; set; }


        [DataMember]
        protected GraphicsDeviceManager mGraphics;

        [DataMember]
        protected FogOfWar mFow;

        [DataMember]
        protected Director mDirector;

        [DataMember]
        protected UserInterfaceScreen mUi;

        [DataMember]
        protected IScreenManager mScreenManager;

        protected BasicLevel(GraphicsDeviceManager graphics,
            ref Director director,
            ContentManager content,
            IScreenManager screenmanager)

        {
            mDirector = director;
            mDirector.GetStoryManager.SetLevelType(LevelType.Skirmish, this);
            mDirector.GetStoryManager.LoadAchievements();
            mGraphics = graphics;
            mScreenManager = screenmanager;

            StandardInitialization(content);
        }

        private void StandardInitialization(ContentManager content)
        {
            //Load stuff
            var platformConeTexture = content.Load<Texture2D>("Cones");
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var platformBlankTexture = content.Load<Texture2D>("PlatformBasic");
            var platformDomeTexture = content.Load<Texture2D>("Dome");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");
            var libSans12 = content.Load<SpriteFont>("LibSans12");

            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture, libSans12);

            //Map related stuff
            Camera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 2800, 2800);
            mFow = new FogOfWar(Camera, mGraphics.GraphicsDevice);

            var map = new Map.Map(mapBackground, 60, 60, mFow, Camera, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)
            Map = map;
            var milUnitSheet = content.Load<Texture2D>("UnitSpriteSheet");
            var milGlowSheet = content.Load<Texture2D>("UnitGlowSprite");

            MilitaryUnit.mMilSheet = milUnitSheet;
            MilitaryUnit.mGlowTexture = milGlowSheet;
            mDirector.GetMilitaryManager.SetMap(ref map);

            //INITIALIZE SCREENS AND ADD THEM TO THE SCREENMANAGER
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);
            mDirector.GetUserInterfaceController.ControlledUserInterface = mUi; // the UI needs to be added to the controller

            // the input manager keeps this from not getting collected by the GC
            new DebugScreen((StackScreenManager)mScreenManager, Camera, Map, ref mDirector);

            mScreenManager.AddScreen(GameScreen);
            mScreenManager.AddScreen(mUi);
        }

        public abstract void LoadContent(ContentManager content);
    }
}
