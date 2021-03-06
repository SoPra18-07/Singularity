﻿using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    internal class EnemyFast : EnemyUnit
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; Fast type.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        public EnemyFast(Vector2 position, Camera camera, ref Director director)
            : base(position, camera, ref director)
        {
            Speed = MilitaryUnitStats.FastSpeed;
            Health = MilitaryUnitStats.FastHealth;
            Range = MilitaryUnitStats.FastRange;
            mColor = new Color(new Vector3(0.80784f, 0.588235f, 0.176471f));
        }
    }
}
