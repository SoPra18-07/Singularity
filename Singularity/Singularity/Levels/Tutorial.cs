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
        private UserInterfaceScreen mUi;
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
            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var map = Map;
            var setUnit = new Settler(new Vector2(1000, 1250), Camera, ref mDirector, ref map, GameScreen, mUi);
            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
