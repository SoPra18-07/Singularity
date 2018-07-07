using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// The resource map is used to store the amount of resources on a specified location.
    /// We removed the whole Dictionary stuff, since felix mentioned we could just use the Resource
    /// itself. So we literally obtain all our desired information from the resource directly.
    /// </summary>
    public sealed class ResourceMap
    {

        private readonly Dictionary<Vector2, List<MapResource>> mLocationCache;

        /// <summary>
        /// The internal resource map used to store said resources.
        /// </summary>
        private readonly List<MapResource> mResourceMap;

        /// <summary>
        /// Creates a new resource map with the given initial resources.
        /// </summary>
        /// <param name="initialResources">A list holding intial resource values. If left empty it will be null</param>
        internal ResourceMap(IEnumerable<MapResource> initialResources)
        {
            if (initialResources == null)
            {
                return;
            }

            mLocationCache = new Dictionary<Vector2, List<MapResource>>();
            mResourceMap = new List<MapResource>(collection: initialResources);
        }


        public Optional<Resource> GetWellResource(Vector2 location)
        {
            var resourcesWell = GetResources(location: location).Where(predicate: r => r.Type == EResourceType.Water || r.Type == EResourceType.Oil).ToList();
            if (resourcesWell.Count() <= 0)
            {
                return Optional<Resource>.Of(value: null);
            }
            return resourcesWell[index: 0].Get(location: location);
        }

        public Optional<Resource> GetQuarryResource(Vector2 location) {
            return Optional<Resource>.Of(value: new Resource(type: EResourceType.Stone, position: location));
            // this is reference-based and totally fine, since there'll be only references then ... we don't care about that, and as soon as the references are all gone, the GC will take care of it. :)
            // (but yes, actually this could break, since we rely heavily on how c# handles references and stuff.)
        }

        public Optional<Resource> GetMineResource(Vector2 location) {
            var resourcesMine = GetResources(location: location).Where(predicate: r => r.Type == EResourceType.Metal).ToList();
            if (resourcesMine.Count() <= 0) {
                return Optional<Resource>.Of(value: null);
            }
            return resourcesMine[index: 0].Get(location: location);
        }

        /// <summary>
        /// Returns an optional value of resources on the given location.
        /// </summary>
        /// <param name="location">The location at which resources are to be</param>
        /// <returns></returns>
        internal List<MapResource> GetResources(Vector2 location)
        {
            // note, the location cache is probably reason number 1 if bugs occur with resources being there even though they shouldn't be,
            // we need to take care, that the resources are getting properly removed.

            if (mLocationCache[key: location] != null)
            {
                return mLocationCache[key: location];
            }

            var foundResources = new List<MapResource>();

            foreach (var resource in mResourceMap)
            {
                if (new Rectangle(x: (int) resource.RelativePosition.X,
                    y: (int)resource.RelativePosition.Y,
                    width: (int)resource.RelativeSize.X,
                    height: (int)resource.RelativeSize.Y).Intersects(value: new Rectangle(x: (int)location.X, y: (int)location.Y, width: 1, height: 1)))
                {
                    foundResources.Add(item: resource);
                }
            }

            mLocationCache[key: location] = foundResources;

            return foundResources;
        }

        /// <summary>
        /// Gets the list of all the resources currently in the game
        /// </summary>
        /// <returns>The list mentioned</returns>
        public List<MapResource> GetAllResources()
        {
            return mResourceMap;
        }

        /// <summary>
        /// Removes the given resource from the resource map.
        /// </summary>
        /// <param name="toRemove">The resource to remove</param>
        public void RemoveResource(MapResource toRemove)
        {
            mResourceMap.Remove(item: toRemove);
            mLocationCache.Clear();
        }
    }
}

