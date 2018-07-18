using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Manager;
using Singularity.Screen;
using Singularity.Units;

namespace Singularity.Levels
{
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    internal sealed class Tutorial: BasicLevel
    {
        public Tutorial(GraphicsDeviceManager graphics,
            ref Director director,
            ContentManager content,
            IScreenManager screenmanager, 
            LevelType level)
            : base(graphics, ref director, content, screenmanager, level)
        {
            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var map = Map;
            var setUnit = new Settler(new Vector2(1000, 1250), Camera, ref mDirector, ref map, GameScreen, Ui);

            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
