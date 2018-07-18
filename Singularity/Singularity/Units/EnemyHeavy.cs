using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    internal class EnemyHeavy : EnemyUnit
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; Heavy type.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        public EnemyHeavy(Vector2 position, Camera camera, ref Director director)
            : base(position, camera, ref director)
        {
            Speed = MilitaryUnitStats.HeavySpeed;
            Health = MilitaryUnitStats.HeavyHealth;
            Range = MilitaryUnitStats.HeavyRange;
            mColor = new Color(new Vector3(0.75682f, .247058f, 0.054902f));
        }
    }
}
