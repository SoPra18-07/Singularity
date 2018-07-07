using System.Runtime.Serialization;
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
    //Not sure whether this should be serialized, but I guess...
    [DataContract]
    internal sealed class Tutorial: BasicLevel
    {
        public Tutorial(GraphicsDeviceManager graphics,
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

            //SetUnit
            var map = Map;
            var setUnit = new Settler(new Vector2(1000, 1250), Camera, ref mDirector, ref map, GameScreen, mUi);
            GameScreen.AddObject(setUnit);

            //TESTMETHODS HERE =====================================
        }
    }
}
