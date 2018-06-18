using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph.Paths;
using Singularity.Input;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized
    class Tutorial
    {

        private GameScreen mGameScreen;
        private InputManager mInput;
        private PathManager mPath;
        private StoryManager.StoryManager mStory;
        private DistributionManager.DistributionManager mDist;
        private GraphicsDevice mGraphics;
        private Map.Map mMap;
        private Camera mCamera;
        private FogOfWar mFow;

        //GameObjects to initialize:
        private CommandCenter mPlatform;

        public Tutorial(GraphicsDevice graphics, InputManager input, PathManager path, StoryManager.StoryManager story, DistributionManager.DistributionManager dist, ContentManager content)
        {
            mInput = input;
            mPath = path;
            mStory = story;
            story.SetLevelType(LevelType.Tutorial);
            story.LoadAchievements();
            mDist = dist;
            mGraphics = graphics;
            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            //Load stuff
            var platformCylTexture = content.Load<Texture2D>("Cylinders");
            var mapBackground = content.Load<Texture2D>("MockUpBackground");

            //Map related stuff
            mMap = new Map.Map(mapBackground, mGraphics.Viewport, mInput, mPath, false);
            mCamera = mMap.GetCamera();
            mFow = new FogOfWar(mCamera, mGraphics);

            //INITIALIZE GAMESCREEN
            mGameScreen = new GameScreen(mGraphics, mInput, mPath, mStory, mDist, mMap, mCamera, mFow);

            //IngameObjects stuff
            mPlatform = new CommandCenter(new Vector2(1000, 500), platformCylTexture, mStory);
            var genUnit = new GeneralUnit(mPlatform, mPath, mDist);
            var genUnit2 = new GeneralUnit(mPlatform, mPath, mDist);
            var genUnit3 = new GeneralUnit(mPlatform, mPath, mDist);
            var genUnit4 = new GeneralUnit(mPlatform, mPath, mDist);
            var genUnit5 = new GeneralUnit(mPlatform, mPath, mDist);

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
