using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Platform;
using Singularity.Property;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game.
    /// </summary>
    public sealed class StructureMap : IDraw, IUpdate
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Road> mRoads;

        private readonly LinkedList<PlatformPlacement> mPlatformsToPlace;

        private readonly Director mDirector;

        private readonly List<Graph.Graph> mGraphs;

        private readonly FogOfWar mFow;

        private int mCurrentGraphIndex;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(FogOfWar fow, ref Director director)
        {
            mCurrentGraphIndex = 0;

            mFow = fow;

            mGraphs = new List<Graph.Graph>();
            mDirector = director;

            mPlatforms = new LinkedList<PlatformBlank>();
            mRoads = new LinkedList<Road>();
        }

        /// <summary>
        /// A method existing so the DistributionManager has access to all platforms.
        /// </summary>
        /// <returns></returns>
        public LinkedList<PlatformBlank> GetPlatformList()
        {
            return mPlatforms;
        }

        /// <summary>
        /// Adds the specified platform to this map.
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            CreateNewGraph();

            mPlatforms.AddLast(platform);
            mFow.AddRevealingObject(platform);
            mGraphs[mCurrentGraphIndex].AddNode(platform);
        }

        /// <summary>
        /// Removes the specified platform from this map.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            CreateNewGraph();

            mPlatforms.Remove(platform);
            mFow.RemoveRevealingObject(platform);
            mGraphs[mCurrentGraphIndex].RemoveNode(platform);
        }
        public void AddRoad(Road road)
        {
            CreateNewGraph();

            mRoads.AddLast(road);
            mGraphs[mCurrentGraphIndex].AddEdge(road);
        }

        public void RemoveRoad(Road road)
        {
            CreateNewGraph();

            mRoads.Remove(road);
            mGraphs[mCurrentGraphIndex].RemoveEdge(road);

        }

        private void CreateNewGraph()
        {

            if (mGraphs.Count > mCurrentGraphIndex)
            {
                return;
            }

            mGraphs.Add(new Graph.Graph());
            mDirector.GetPathManager.AddGraph(mGraphs[mGraphs.Count - 1]);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var platform in mPlatforms)
            {
                platform.Draw(spriteBatch);
            }

            foreach (var road in mRoads)
            {
                road.Draw(spriteBatch);
            }

            foreach(var platformToAdd in mPlatformsToPlace)
            {
                platformToAdd.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gametime)
        {
            foreach (var platform in mPlatforms)
            {
                platform.Update(gametime);
            }

            foreach (var road in mRoads)
            {
                road.Update(gametime);
            }

            //TODO: set ishovering for the platformtoAdd if there are any.

            foreach (var platformToAdd in mPlatformsToPlace)
            {
                if (!platformToAdd.IsFinished())
                {
                    platformToAdd.Update(gametime);
                }
            }
        }

        public void AddPlatformToPlace(PlatformPlacement platformPlacement)
        {
            mPlatformsToPlace.AddLast(platformPlacement);
        }
    }
}
