
using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    /// <summary>
    /// Represents any object that can shoot
    /// </summary>
    internal interface IShooting : IDamageable
    {
        /// <summary>
        /// The range of the shooting unit.
        /// </summary>
        int Range { get; }

        /// <summary>
        /// Sets the target to be shot at.
        /// </summary>
        /// <param name="target">The target to be shot.</param>
        void SetShootingTarget(Vector2 target);
    }
}
