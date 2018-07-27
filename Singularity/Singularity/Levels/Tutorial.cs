using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Manager;
using Singularity.Nature;
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
            : base(graphics, ref director, content, screenmanager, level, null)
        {
            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            //INGAME OBJECTS INITIALIZATION ===================================================

            //SetUnit
            var settler = new Settler(new Vector2(2900, 3200), Camera, ref mDirector, GameScreen, Ui);
            
            Camera.CenterOn(settler.AbsolutePosition);

            GameScreen.AddObject(settler);

            GameScreen.AddObject(new Puddle(new Vector2(3300, 2500), ref mDirector));
            GameScreen.AddObject(new Puddle(new Vector2(4500, 2700), ref mDirector, false));
        }
    }
}
