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
            mResourceMap = new List<MapResource>(initialResources);
        }

        public MapResource MapResource
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public Optional<Resource> GetWellResource(Vector2 location)
        {
            var resourcesWell = GetResources(location).Where(r => r.Type == EResourceType.Water || r.Type == EResourceType.Oil).ToList();
            if (resourcesWell.Count() <= 0)
            {
                return Optional<Resource>.Of(null);
            }
            return resourcesWell[0].Get(location);
        }

        public Optional<Resource> GetQuarryResource(Vector2 location) {
            return Optional<Resource>.Of(new Resource(EResourceType.Stone, location));
            // this is reference-based and totally fine, since there'll be only references then ... we don't care about that, and as soon as the references are all gone, the GC will take care of it. :)
            // (but yes, actually this could break, since we rely heavily on how c# handles references and stuff.)
        }

        public Optional<Resource> GetMineResource(Vector2 location) {
            var resourcesMine = GetResources(location).Where(r => r.Type == EResourceType.Metal).ToList();
            if (resourcesMine.Count() <= 0) {
                return Optional<Resource>.Of(null);
            }
            return resourcesMine[0].Get(location);
        }

        // TODO
        public Optional<Resource> GetAmmoResource(Vector2 location)
        {
            var resourceAmmo = GetResources(location).Where(r => r.Type == EResourceType.Metal).ToList();
            if (!resourceAmmo.Any())
            {
                return Optional<Resource>.Of(null);
            }
            return resourceAmmo[0].Get(location);
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

            if (mLocationCache[location] != null)
            {
                return mLocationCache[location];
            }

            var foundResources = new List<MapResource>();

            foreach (var resource in mResourceMap)
            {
                if (new Rectangle((int) resource.RelativePosition.X,
                    (int)resource.RelativePosition.Y,
                    (int)resource.RelativeSize.X,
                    (int)resource.RelativeSize.Y).Intersects(new Rectangle((int)location.X, (int)location.Y, 1, 1)))
                {
                    foundResources.Add(resource);
                }
            }

            mLocationCache[location] = foundResources;

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
            mResourceMap.Remove(toRemove);
            mLocationCache.Clear();
        }
    }
}

