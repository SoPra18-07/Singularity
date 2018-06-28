using Microsoft.Xna.Framework;
using Singularity.Map.Properties;

namespace Singularity.Property
{
    internal interface ICollider : ISpatial
    {
        /// <summary>
        /// Creates a collider grid that shows for each grid point in the bounding
        /// box if that grid position is collidable.
        /// </summary>
        /// For example:
        /// [0, 0, 0, 0, 0,
        ///  0, 0, 1, 0, 0,
        ///  0, 1, 1, 1, 0,
        ///  0, 0, 1, 0, 0,]
        ///
        /// is the ColliderGrid for an  diamond shaped object.
        bool[,] ColliderGrid { get; }

        /// <summary>
        /// Absolute bounding box of an object. The object MUST be smaller
        /// than this bounding box.
        /// </summary>
        Rectangle AbsBounds { get; }

        /// <summary>
        /// Indicates if an object has moved since the last update call.
        /// </summary>
        bool Moved { get; }

        /// <summary>
        /// Unique unit ID.
        /// </summary>
        int Id { get; }
    }
}
