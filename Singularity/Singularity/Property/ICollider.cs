using Microsoft.Xna.Framework;

namespace Singularity.Property
{
    internal interface ICollider : ISpatial
    {
        /// <summary>
        /// Provides a lookup table for which spaces have a collider in them and which don't within the bounding box
        /// </summary>
        bool[,] ColliderGrid { get; }

        /// <summary>
        /// Indicates the absolute bounds of a collider. The entire object must be within this bounds
        /// </summary>
        Rectangle AbsBounds { get; }


        bool Moved { get; }

        int Id { get; }
    }
}
