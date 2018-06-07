using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Resources
{

    /// <inheritdoc cref="Singularity.property.IDraw"/>
    /// <inheritdoc cref="Singularity.property.IUpdate"/>
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
        /// Accelarets the resource in such a fashion that it moves with a unit if held.
        /// </summary>
        /// <param name="vector">The vector with which to accelerate this resource</param>
        void Accelerate(Vector2 vector);
    }
}
