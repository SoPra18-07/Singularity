using Singularity.Property;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Units;

namespace Singularity.AI.Behavior
{
    /// <summary>
    /// An interface for all possible AI behaviors.
    /// </summary>
    interface IAiBehavior
    {
        /// <summary>
        /// Moves the units in a specific way specified by the implementation
        /// </summary>
        /// <param name="gametime">A snapshot of timing values</param>
        void Move(GameTime gametime);

        /// <summary>
        /// Spawns units in a specific way specified by the implementation
        /// </summary>
        /// <param name="gametime">A snapshot of timing values</param>
        void Spawn(GameTime gametime);

        /// <summary>
        /// Creates a new base in a specific way specified by the implementation
        /// </summary>
        /// <param name="gametime">A snapshot of timing values</param>
        void CreateNewBase(GameTime gametime);

        /// <summary>
        /// Needed for reloading the references etc. after deserializing
        /// </summary>
        /// <param name="dir"></param>
        void ReloadContent(ref Director dir);

        void Kill(EnemyUnit unit);

        void Shooting(MilitaryUnit sender, ICollider shootingAt, GameTime gametime);
    }
}
