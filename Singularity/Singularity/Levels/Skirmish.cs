using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.AI;
using Singularity.Manager;
using Singularity.Nature;
using Singularity.Property;
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
            IScreenManager screenmanager,
            LevelType level)
            : base(graphics, ref director, content, screenmanager, level, new BasicAi(GlobalVariables.Difficulty, ref director))
        {

            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            var settler = new Settler(new Vector2(2900, 3200), Camera, ref mDirector, GameScreen, Ui);
            GameScreen.AddObject(settler);

            var milunitList = new List<MilitaryFast>(6);
            for (var i = 0; i < 6; i++)
            {
                milunitList.Add(new MilitaryFast(new Vector2(3000 + (i > 2 ? 100 : 0), 3000 + (i % 3) * 50), Camera, ref mDirector));
            }

            GameScreen.AddObjects(milunitList);

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
