using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Content;
using Singularity.Map;
using Singularity.Screen.ScreenClasses;

namespace Singularity.Levels
{
    public interface ILevel
    {
        /// <summary>
        /// This method Loads all the data in the Level.
        /// </summary>
        void LoadContent(ContentManager content);

        [DataMember]
        GameScreen GameScreen { get; set; }
        [DataMember]
        UserInterfaceScreen Ui { get; set; }

        [DataMember]
        Camera Camera { get; set; }
        Map.Map Map { get; }
    }
}
