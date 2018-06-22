using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Platform;

namespace Singularity.Map
{
    /// <summary>
    /// A Structure map holds all the structures currently in the game.
    /// </summary>
    public sealed class StructureMap
    {
        /// <summary>
        /// A list of all the platforms currently in the game
        /// </summary>
        private readonly LinkedList<PlatformBlank> mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Road> mRoads;

        private readonly Director mDirector;

        private readonly List<Graph.Graph> mGraphs;

        private int mCurrentGraphIndex;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(Director director)
        {
            mCurrentGraphIndex = 0;

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
            mDirector.GetPathManager().AddGraph(mGraphs[mGraphs.Count - 1]);
        }

    }
}
