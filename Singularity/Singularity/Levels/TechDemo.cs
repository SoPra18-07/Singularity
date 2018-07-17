using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity.Levels
{
    class TechDemo : BasicLevel
    {

        public TechDemo(GraphicsDeviceManager graphics, ref Director director, ContentManager content, IScreenManager screenmanager) : base(graphics, ref director, content, screenmanager)
        {
            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            var map = Map;

            GlobalVariables.mFowEnabled = false;

            var platform = PlatformFactory.Get(EStructureType.Barracks, ref mDirector, 3000, 3000);

            var rnd = new Random();

            for (var i = 0; i < 1000; i++)
            {
                var x = rnd.Next(MapConstants.MapWidth);
                var y = rnd.Next(MapConstants.MapHeight);

                if (!Singularity.Map.Map.IsOnTop(new Vector2(x, y)))
                {
                    i--;
                    continue;
                }
                GameScreen.AddObject(new MilitaryUnit(new Vector2(x, y), Camera, ref mDirector, ref map));

            }

            GameScreen.AddObject(platform);
        }
    }
}
