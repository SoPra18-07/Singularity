using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// The resource map is used to store the amount of resources on a specified location.
    /// We removed the whole Dictionary stuff, since felix mentioned we could just use the Resource
    /// itself. So we literally obtain all our desired information from the resource directly.
    /// </summary>
    [DataContract]
    public sealed class ResourceMap : IDraw
    {
        /// <summary>
        /// The director for the game
        /// </summary>
        private Director mDirector;

        [DataMember]
        private readonly Dictionary<Vector2, List<MapResource>> mLocationCache;

        /// <summary>
        /// The internal resource map used to store said resources.
        /// </summary>
        [DataMember]
        private readonly List<MapResource> mResourceMap;

        /// <summary>
        /// Creates a new resource map with the given initial resources.
        /// </summary>
        /// <param name="initialResources">A list holding intial resource values. If left empty it will be null</param>
        internal ResourceMap(IEnumerable<MapResource> initialResources, Director director)
        {
            mLocationCache = new Dictionary<Vector2, List<MapResource>>();
            if (initialResources == null)
            {
                return;
            }

            mResourceMap = new List<MapResource>(initialResources);

            mDirector = director;
        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
            foreach (var resource in mResourceMap)
            {
                resource.ReloadContent(ref dir);
            }
        }

        public Optional<Resource> GetWellResource(Vector2 location)
        {
            var resourcesWell = GetResources(location).Where(r => r.Type == EResourceType.Water || r.Type == EResourceType.Oil).ToList();
            return !resourcesWell.Any() ? Optional<Resource>.Of(null) : resourcesWell[0].Get(location);
        }

        public Optional<Resource> GetQuarryResource(Vector2 location)
        {
            var rnd = new Random();
            return Optional<Resource>.Of(rnd.Next(2) == 0 ? new Resource(EResourceType.Stone, location, mDirector) : new Resource(EResourceType.Sand, location, mDirector));
            // this is reference-based and totally fine, since there'll be only references then ... we don't care about that, and as soon as the references are all gone, the GC will take care of it. :)
            // (but yes, actually this could break, since we rely heavily on how c# handles references and stuff.)
        }

        public Optional<Resource> GetMineResource(Vector2 location) {
            var resourcesMine = GetResources(location).Where(r => r.Type == EResourceType.Metal).ToList();
            return !resourcesMine.Any() ? Optional<Resource>.Of(null) : resourcesMine[0].Get(location);
        }

        // TODO
        public Optional<Resource> GetAmmoResource(Vector2 location)
        {
            var resourceAmmo = GetResources(location).Where(r => r.Type == EResourceType.Metal).ToList();
            return !resourceAmmo.Any() ? Optional<Resource>.Of(null) : resourceAmmo[0].Get(location);
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

            if (mLocationCache.ContainsKey(location))
            {
                return mLocationCache[location];
            }

            var foundResources = mResourceMap.Where(resource => new Rectangle((int) resource.AbsolutePosition.X,
                    (int) resource.AbsolutePosition.Y,
                    (int) resource.AbsoluteSize.X,
                    (int) resource.AbsoluteSize.Y).Intersects(new Rectangle((int) location.X, (int) location.Y, 1, 1)))
                .ToList();

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

        public void Draw(SpriteBatch spriteBatch)
        {
            mResourceMap.ForEach(r => r.Draw(spriteBatch));
        }
    }
}

