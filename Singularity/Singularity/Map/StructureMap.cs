using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Utils;
using Singularity.platform;

namespace Singularity.Map
{
    /// <inheritdoc cref="IUpdate"/>
    /// <inheritdoc cref="IDraw"/>
    /// <summary>
    /// A Structure map holds all the structures currently in the game.
    /// </summary>
    internal sealed class StructureMap : IUpdate, IDraw
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Pair<PlatformBlank, PlatformBlank>> mRoads;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap()
        {
            mPlatforms = new LinkedList<PlatformBlank>();
            mRoads = new LinkedList<Pair<PlatformBlank, PlatformBlank>>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var platform in mPlatforms)
            {
                platform.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var platform in mPlatforms)
            {
                platform.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Adds the specified platform to this map.
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            mPlatforms.AddLast(platform);
        }

        /// <summary>
        /// Removes the specified platform from this map.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            mPlatforms.Remove(platform);
        }

        public LinkedList<PlatformBlank> GetPlatforms()
        {
            return mPlatforms;
        }

    }
}
