﻿using System;
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

        protected Texture2D mPlatformBlankTexture;

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
            MilitaryUnit.mMilSheet = content.Load<Texture2D>("UnitSpriteSheet");
            MilitaryUnit.mGlowTexture = content.Load<Texture2D>("UnitGlowSprite");
            var mapBackground = content.Load<Texture2D>("backgroundGrid");

            //TODO: have a cone texture
            PlatformFactory.Init(platformConeTexture, platformCylTexture, platformDomeTexture, platformBlankTexture);

            //Map related stuff
            Camera = new Camera(mGraphics.GraphicsDevice, ref mDirector, 800, 800);
            mFow = new FogOfWar(Camera, mGraphics.GraphicsDevice);
            Map = new Map.Map(mapBackground, 20, 20, mFow, mGraphics.GraphicsDevice.Viewport, ref mDirector); // NEOLAYOUT (searchmark for @fkarg)

            //INITIALIZE SCREENS AND ADD THEM TO THE SCREENMANAGER
            GameScreen = new GameScreen(mGraphics.GraphicsDevice, ref mDirector, Map, Camera, mFow);
            mUi = new UserInterfaceScreen(ref mDirector, mGraphics, GameScreen, mScreenManager);

            mScreenManager.AddScreen(GameScreen);
            mScreenManager.AddScreen(mUi);
        }

        public abstract void LoadContent(ContentManager content);
    }
}