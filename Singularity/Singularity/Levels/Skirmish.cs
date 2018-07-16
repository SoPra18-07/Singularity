using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Nature;
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
                ref mDirector);
            GameScreen.AddObject(sentinel);

            var settler = new Settler(new Vector2(3000, 3200), Camera, ref mDirector, ref map, GameScreen, Ui);

            GameScreen.AddObject(settler);

            var enemyunit = new EnemyUnit(new Vector2(3500, 3500), Camera, ref mDirector, ref map);
            GameScreen.AddObject(enemyunit);
            // add a puddle
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2500), ref mDirector));
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2700), ref mDirector, false));

            //TESTMETHODS HERE ====================================
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
