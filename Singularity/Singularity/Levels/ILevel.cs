using Microsoft.Xna.Framework.Content;
using Singularity.Screen.ScreenClasses;
using System.Runtime.Serialization;
using Singularity.Map;

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
        Camera Camera { get; set; }

        Map.Map Map { get; }
    }
}
