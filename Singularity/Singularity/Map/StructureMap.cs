using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game.
    /// </summary>
    internal sealed class StructureMap
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Pair<PlatformBlank, PlatformBlank>> mRoads;


        private readonly FogOfWar mFow;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(FogOfWar fow)
        {
            mFow = fow;

            mPlatforms = new LinkedList<PlatformBlank>();
            mRoads = new LinkedList<Pair<PlatformBlank, PlatformBlank>>();
        }

        /// <summary>
        /// Adds the specified platform to this map.
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(platform);
            mFow.AddRevealingObject(platform);
        }

        /// <summary>
        /// Removes the specified platform from this map.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.Remove(platform);
            mFow.RemoveRevealingObject(platform);
        }

        public LinkedList<PlatformBlank> GetPlatforms()
        {
            return mPlatforms;
        }

    }
}
