﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Manager;
using Singularity.Nature;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity.Levels
{
    class PlatformDestructionTestLevel : BasicLevel
    {
        public PlatformDestructionTestLevel(GraphicsDeviceManager graphics, ref Director director, ContentManager content, IScreenManager screenmanager) : base(graphics, ref director, content, screenmanager)
        {
            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            var map = Map;

            var platform = PlatformFactory.Get(EPlatformType.Blank, ref mDirector, 3000, 3000);
            GameScreen.AddObject(platform);

            var platform2 = PlatformFactory.Get(EPlatformType.Blank, ref mDirector, 3000, 2500);
            GameScreen.AddObject(platform2);

            var road = new Road(platform, platform2, ref mDirector);
            GameScreen.AddObject(road);

            var platform3 = PlatformFactory.Get(EPlatformType.Blank, ref mDirector, 2700, 2700);
            GameScreen.AddObject(platform3);

            var road2 = new Road(platform2, platform3, ref mDirector);
            GameScreen.AddObject(road2);

            var road3 = new Road(platform, platform3, ref mDirector);
            GameScreen.AddObject(road3);


            var settler = new Settler(new Vector2(3000, 3200), Camera, ref mDirector, ref map, GameScreen, mUi);
            GameScreen.AddObject(settler);
        }
    }
}