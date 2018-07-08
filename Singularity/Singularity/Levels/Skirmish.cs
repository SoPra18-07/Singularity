using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    /// <inheritdoc cref="BasicLevel"/>
    //Not sure whether this should be serialized, but I guess...
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
            //INGAME OBJECTS INITIALIZATION ===================================================
            //Platforms
            var platform1 = PlatformFactory.Get(EPlatformType.Blank, ref mDirector, 1000, 1000, Map.GetResourceMap());

            GameScreen.AddObject(platform1);

            // this is done via the factory to test, so I can instantly see if something is some time off.
            var platform2 = PlatformFactory.Get(EPlatformType.Well, ref mDirector, 800, 1000, Map.GetResourceMap());
            GameScreen.AddObject(platform2);

            var road1 = new Road(platform1, platform2, false);
            GameScreen.AddObject(road1);

            //var platform2 = new Well(new Vector2(800, 1000), platformDomeTexture, platformBlankTexture, mMap.GetResourceMap(), ref mDirector);
            var platform3 = PlatformFactory.Get(EPlatformType.Quarry, ref mDirector, 1200, 1200, Map.GetResourceMap());
            GameScreen.AddObject(platform3);
            var road2 = new Road(platform2, platform3, false);
            GameScreen.AddObject(road2);
            var road3 = new Road(platform3, platform1, false);
            GameScreen.AddObject(road3);

            var platform4 = PlatformFactory.Get(EPlatformType.Energy, ref mDirector, 1000, 800, Map.GetResourceMap());

            GameScreen.AddObject(platform4);
            var road4 = new Road(platform1, platform4, false);
            GameScreen.AddObject(road4);

            var road5 = new Road(platform4, platform3, false);
            GameScreen.AddObject(road5);


            //GenUnits
            var genUnit = new List<GeneralUnit>(5);
            for (var i = 0; i < 5; i++)
            {
                genUnit.Add(new GeneralUnit(platform1, ref mDirector));
            }

            //MilUnits
            var map = Map;
            var milUnit = new MilitaryUnit(new Vector2(2000, 700), Camera, ref mDirector, ref map);

            //SetUnit
            var setUnit = new Settler(new Vector2(1000, 1250), Camera, ref mDirector, ref map, GameScreen, mUi);


            // Resources
            var res = new Resource(EResourceType.Metal, platform2.Center);
            var res4 = new Resource(EResourceType.Metal, platform2.Center);
            var res5 = new Resource(EResourceType.Metal, platform2.Center);
            var res2 = new Resource(EResourceType.Chip, platform3.Center);
            var res3 = new Resource(EResourceType.Oil, platform4.Center);

            platform2.StoreResource(res);
            platform3.StoreResource(res2);
            platform4.StoreResource(res3);
            platform2.StoreResource(res4);
            platform2.StoreResource(res5);

            GameScreen.AddObjects(genUnit);
            GameScreen.AddObject(milUnit);
            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE ====================================
            mDirector.GetDistributionManager.RequestResource(platform2, EResourceType.Oil, null);
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
