using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Sound;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    public class EnemyUnit : MilitaryUnit
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; standard type.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        public EnemyUnit(Vector2 position, Camera camera, ref Director director)
            : base(position, camera, ref director, false)
        {
            mColor = Color.Maroon;
            mShootColor = Color.Red;
        }

        public ICollider GetShootingTarget()
        {
            return mShootingTarget;
        }

        public void SetMovementTarget(Vector2 goal)
        {
            // let the flocker take care of it
            throw new NotImplementedException();
        }
    }
}
