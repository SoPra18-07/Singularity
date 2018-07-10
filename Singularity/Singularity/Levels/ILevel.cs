using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Levels
{
    public interface ILevel
    {
        /// <summary>
        /// This method Loads all the data in the Level.
        /// </summary>
        void LoadContent(ContentManager content);

        void ReloadContent(ContentManager content, GraphicsDeviceManager graphics, ref Director director, IScreenManager screenmanager);

        [DataMember]
        GameScreen GameScreen { get; set; }

        [IgnoreDataMember]
        UserInterfaceScreen Ui { get; set; }

        [DataMember]
        Camera Camera { get; set; }
        [DataMember]
        Map.Map Map { get; }
    }
}
