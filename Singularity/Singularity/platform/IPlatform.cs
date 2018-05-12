using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.platform
{
    /// <inheritdoc cref="Singularity.property.IDraw"/>
    /// <inheritdoc cref="Singularity.property.IUpdate"/>
    /// <remarks>
    /// An abstraction for "Platforms".
    /// Examples of IPlatforms would be the basic platform, the quarry etc.
    /// </remarks>
    internal interface IPlatforms: IUpdate, IDraw
    {
        /// <summary>
        /// Get the assigned Units of this platform.
        /// </summary>
        /// <returns> a list containing the IDs of the units</returns>
        List<CUnit> GetAssignedUnitsIDs();

        /// <summary>
        /// Get the Position of the platform as a 2dimensional vector.
        /// </summary>
        /// <returns>a Vector2 containing the position</returns>
        Vector2 GetPosition();

        /// <summary>
        /// Get the special actions you can perform on this platform.
        /// </summary>
        /// <returns> an array with the available actions.</returns>
        Action[] GetSpecialActions();

        /// <summary>
        /// Perform the given action on the platform.
        /// </summary>
        /// <param name="action"> The action to be performed </param>
        /// <returns> true if it was succesfull</returns>
        bool DoSpecialAction(Action action);

        /// <summary>
        /// Get the requirements of resources to build this platform.
        /// </summary>
        /// <returns> a dictionary of the resources with a number telling how much of it is required</returns>
        Dictionary<IResources, int> ResourcesRequired();

        /// <summary>
        /// Get the Resources on the platform.
        /// </summary>
        /// <returns> a List containing the references to the resource-objects</returns>
        List<IResources> GetPlatformResources();

        /// <summary>
        /// Get the health points of the platform
        /// </summary>
        /// <returns> the health points as integer</returns>
        int GetHealth();

        /// <summary>
        /// Set the health points of the platform to a new value.
        /// </summary>
        /// <param name="newHealth"> the new health points of the platform</param>
        void SetHealth(int newHealth);

        /// <summary>
        /// Add a new resource to the platform.
        /// </summary>
        /// <param name="resource"> the resource to be added to the platform </param>
        void Store(IResources resource);

        /// <summary>
        /// Remove the given resource from the platform.
        /// </summary>
        /// <param name="resource"> the resource to be removed </param>
        void Remove(IResources resource);

        /// <summary>
        /// Used to draw content onto the screen with the given SpriteBatch and the given spritesheet.
        /// </summary>
        /// <param name="spritebatch"> The SpriteBatch to which the platform texture should be added</param>
        /// <param name="spritesheet"> The texture of the spritesheet that contains the texture for the platform</param>
        void Draw(SpriteBatch spritebatch, Texture2D spritesheet);
    }
}
