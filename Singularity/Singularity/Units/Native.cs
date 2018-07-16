using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    internal class Native : EnemyFast
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; Native of the environment.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the Game Director</param>
        public Native(Vector2 position, Camera camera, ref Director director)
            : base(position, camera, ref director)
        {
            Speed = 10;
            Health = 6;
            mColor = Color.OrangeRed;
            mScale = 0.25f;
        }
    }
}
