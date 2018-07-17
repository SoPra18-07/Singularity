using System.Collections.Generic;
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
            var settler = new Settler(new Vector2(3000, 3200), Camera, ref mDirector, GameScreen, Ui);
            GameScreen.AddObject(settler);

            var mil = new MilitaryUnit(new Vector2(3500, 3000), Camera, ref mDirector);
            GameScreen.AddObject(mil);
            var mil2 = new MilitaryUnit(new Vector2(3400, 3000), Camera, ref mDirector);
            GameScreen.AddObject(mil2);
            var mil3 = new MilitaryUnit(new Vector2(3600, 3000), Camera, ref mDirector);
            GameScreen.AddObject(mil3);
            var mil4 = new MilitaryUnit(new Vector2(3500, 3100), Camera, ref mDirector);
            GameScreen.AddObject(mil4);
            var mil5 = new MilitaryUnit(new Vector2(3400, 3100), Camera, ref mDirector);
            GameScreen.AddObject(mil5);
            var mil6 = new MilitaryUnit(new Vector2(3600, 3100), Camera, ref mDirector);
            GameScreen.AddObject(mil6);

            // add a puddle
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2500), ref mDirector));
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2700), ref mDirector, false));
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
