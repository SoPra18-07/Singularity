﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Nature;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    /// <inheritdoc cref="BasicLevel"/>
    [DataContract]
    internal sealed class Skirmish : BasicLevel
    {
        public Skirmish(GraphicsDeviceManager graphics,
            ref Director director,
            ContentManager content,
            IScreenManager screenmanager)
            : base(graphics, ref director, content, screenmanager)
        {

            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            var map = Map;
            
            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            var sentinel = new Sentinel(new Vector2(4000, 4000),
                content.Load<Texture2D>("Cones"),
                content.Load<Texture2D>("PlatformBasic"),
                content.Load<SpriteFont>("LibSans12"),
                ref mDirector);;

            GameScreen.AddObject(sentinel);
            var platform1 = PlatformFactory.Get(EStructureType.Blank, ref mDirector, 3000, 3000, Map.GetResourceMap());

            GameScreen.AddObject(platform1);

            var platform2 = PlatformFactory.Get(EStructureType.Well, ref mDirector, 2800, 3000, Map.GetResourceMap());
            GameScreen.AddObject(platform2);

            var road1 = new Road(platform1, platform2, ref mDirector);
            GameScreen.AddObject(road1);

            var platform3 = PlatformFactory.Get(EStructureType.Quarry, ref mDirector, 3200, 3200, Map.GetResourceMap());

            GameScreen.AddObject(platform3);
            var road2 = new Road(platform2, platform3, ref mDirector);
            GameScreen.AddObject(road2);
            var road3 = new Road(platform3, platform1, ref mDirector);
            GameScreen.AddObject(road3);

            var settler = new Settler(new Vector2(3000, 3200), Camera, ref mDirector, ref map, GameScreen, Ui);


            var platform4 = PlatformFactory.Get(EStructureType.Energy, ref mDirector, 3000, 2800, Map.GetResourceMap());

            GameScreen.AddObject(platform4);
            var road4 = new Road(platform1, platform4, ref mDirector);
            GameScreen.AddObject(road4);



            var road5 = new Road(platform4, platform3, ref mDirector);
            GameScreen.AddObject(road5);

            // Enemy Unit
            var enemyUnit = new Target(new Vector2(3200, 3050), Camera, ref mDirector, ref map);
            var enemyUnit2 = new EnemyUnit(new Vector2(3200, 2950), Camera, ref mDirector, ref map);

            var milUnit = new MilitaryUnit(new Vector2(3000, 2900), Camera, ref mDirector, ref map);

            

            var rock1 = new Rock(new Vector2(3500, 2800), ref mDirector);
            //var rock2 = new Rock(new Vector2(3500, 3000));

            GameScreen.AddObject(rock1);
            //GameScreen.AddObject(rock2);

            // GenUnits
            var genUnit = new List<GeneralUnit>(5);
            for (var i = 0; i < 5; i++)
            {
                genUnit.Add(new GeneralUnit(platform1, ref mDirector));
            }

            // Resources
            var res = new Resource(EResourceType.Metal, platform2.Center, mDirector);
            var res4 = new Resource(EResourceType.Metal, platform2.Center, mDirector);
            var res5 = new Resource(EResourceType.Metal, platform2.Center, mDirector);
            var res6 = new Resource(EResourceType.Metal, platform3.Center, mDirector);
            var res7 = new Resource(EResourceType.Metal, platform4.Center, mDirector);
            var res2 = new Resource(EResourceType.Chip, platform3.Center, mDirector);
            var res3 = new Resource(EResourceType.Oil, platform4.Center, mDirector);
            var res8 = new Resource(EResourceType.Metal, platform1.Center, mDirector);
            var res9 = new Resource(EResourceType.Metal, platform1.Center, mDirector);
            var res10 = new Resource(EResourceType.Oil, platform1.Center, mDirector);
            var res11 = new Resource(EResourceType.Oil, platform1.Center, mDirector);
            var res12 = new Resource(EResourceType.Oil, platform2.Center, mDirector);

            platform2.StoreResource(res);
            platform3.StoreResource(res2);
            platform4.StoreResource(res3);
            platform2.StoreResource(res4);
            platform2.StoreResource(res5);
            platform3.StoreResource(res6);
            platform4.StoreResource(res7);
            platform1.StoreResource(res8);
            platform1.StoreResource(res9);
            platform1.StoreResource(res10);
            platform1.StoreResource(res11);
            platform2.StoreResource(res12);

            GameScreen.AddObjects(genUnit);
            GameScreen.AddObject(enemyUnit);
            GameScreen.AddObject(enemyUnit2);

            GameScreen.AddObject(milUnit);
            GameScreen.AddObject(settler);

            // add a puddle
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2500), ref mDirector));
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2700), ref mDirector, false));

            //TESTMETHODS HERE ====================================
            mDirector.GetDistributionDirector.GetManager(platform2.GetGraphIndex()).RequestResource(platform2, EResourceType.Oil, null);
    

        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
