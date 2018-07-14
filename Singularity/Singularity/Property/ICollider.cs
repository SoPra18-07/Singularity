using Microsoft.Xna.Framework;
using Singularity.Utils;

namespace Singularity.Property
{
    /// <inheritdoc cref="ISpatial"/>
    /// <inheritdoc cref="IDamageable"/>
    /// <inheritdoc cref="IId"/>
    public interface ICollider : ISpatial, IDamageable, IId
    {
        /// <summary>
        /// Provides a lookup table for which spaces have a collider in them and which don't within the bounding box
        /// </summary>
        bool[,] ColliderGrid { get; }

        /// <summary>
        /// Indicates the absolute bounds of a collider. The entire object must be within this bounds
        /// </summary>
        Rectangle AbsBounds { get; }

        /// <summary>
        /// Indicates the center of the object
        /// </summary>
        Vector2 Center { get; }


        bool Moved { get; }
    }
}
