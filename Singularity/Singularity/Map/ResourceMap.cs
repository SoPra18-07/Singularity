using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// The resource map is used to store the amount of resources on a specified location.
    /// </summary>
    public sealed class ResourceMap
    {
        /// <summary>
        /// The internal resource map used to store said resources.
        /// </summary>
        private readonly Dictionary<Vector2, Pair<EResourceType, int>> mResourceMap;

        /// <summary>
        /// Creates a new resource map with the given initial resources.
        /// </summary>
        /// <param name="initialResources">A dictionary holding intial resource values. If left empty it will be null</param>
        internal ResourceMap(IDictionary<Vector2, Pair<EResourceType, int>> initialResources)
        {
            if (initialResources == null)
            {
                return;
            }

            mResourceMap = new Dictionary<Vector2, Pair<EResourceType, int>>(initialResources);
        }

        /// <summary>
        /// Returns an optional value of resources on the given location.
        /// </summary>
        /// <param name="location">The location at which resources are to be</param>
        /// <returns></returns>
        internal Optional<Pair<EResourceType, int>> GetResources(Vector2 location)
        {
            return Optional<Pair<EResourceType, int>>.Of(mResourceMap[location]);
        }
    }
}
