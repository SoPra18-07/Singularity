using System;
using Microsoft.Xna.Framework;
using Singularity.Property;
using Singularity.Units;

namespace Singularity.Resources
{

    /// <inheritdoc cref="Singularity.Property.IDraw"/>
    /// <inheritdoc cref="Singularity.Property.IUpdate"/>
    /// <remarks>
    /// An interface providing necessary signatures for resources.
    /// </remarks>
    internal interface IResource : IDraw, IUpdate
    {

        /// <summary>
        /// Gets the ID for this resource.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The type of this resource. Used to differ between different resources.
        /// Also see: <see cref="EResourceType"/>.
        /// </summary>
        EResourceType Type { get; }

        //TODO: remove when ISpatial is there. Since it provides the position.

        /// <summary>
        /// Gets the current position of this resource on the map.
        /// </summary>
        /// <returns>The position as a vector 2</returns>
        Vector2 GetPosition();

        /// <summary>
        /// Uses this resource. Ends this resources life cycle.
        /// </summary>
        void Use();

        /// <summary>
        /// Follows the unit which holds this resource.
        /// </summary>
        /// <param name="unitToFollow">The unit which holds this resource, thus this resource needs to follow the unit</param>
        void Follow(GeneralUnit unitToFollow);
    }
}
