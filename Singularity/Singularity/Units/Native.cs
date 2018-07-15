using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    internal class Native : MilitaryFast
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; Native of the environment.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        public Native(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
            : base(position, camera, ref director, ref map, false)
        {
            mColor = Color.OrangeRed;
            mScale = 0.25f;
        }
    }
}
