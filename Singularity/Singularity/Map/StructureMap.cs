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
        private readonly LinkedList<PlatformBlank> _mPlatforms;

        /// <summary>
        /// A list of all the roads in the game, identified by a source and destination platform.
        /// </summary>
        private readonly LinkedList<Road> _mRoads;

        private readonly Director _mDirector;

        private readonly List<Graph.Graph> _mGraphs;

        private int _mCurrentGraphIndex;

        /// <summary>
        /// Creates a new structure map which holds all the structures currently in the game.
        /// </summary>
        public StructureMap(Director director)
        {
            _mCurrentGraphIndex = 0;

            _mGraphs = new List<Graph.Graph>();
            _mDirector = director;

            _mPlatforms = new LinkedList<PlatformBlank>();
            _mRoads = new LinkedList<Road>();
        }

        /// <summary>
        /// A method existing so the DistributionManager has access to all platforms.
        /// </summary>
        /// <returns></returns>
        public LinkedList<PlatformBlank> GetPlatformList()
        {
            return _mPlatforms;
        }

        /// <summary>
        /// Adds the specified platform to this map.
        /// </summary>
        /// <param name="platform">The platform to be added</param>
        public void AddPlatform(PlatformBlank platform)
        {
            CreateNewGraph();

            _mPlatforms.AddLast(platform);
            _mGraphs[_mCurrentGraphIndex].AddNode(platform);
        }

        /// <summary>
        /// Removes the specified platform from this map.
        /// </summary>
        /// <param name="platform">The platform to be removed</param>
        public void RemovePlatform(PlatformBlank platform)
        {
            CreateNewGraph();

            _mPlatforms.Remove(platform);
            _mGraphs[_mCurrentGraphIndex].RemoveNode(platform);
        }
        public void AddRoad(Road road)
        {
            CreateNewGraph();

            _mRoads.AddLast(road);
            _mGraphs[_mCurrentGraphIndex].AddEdge(road);
        }

        public void RemoveRoad(Road road)
        {
            CreateNewGraph();

            _mRoads.Remove(road);
            _mGraphs[_mCurrentGraphIndex].RemoveEdge(road);

        }

        private void CreateNewGraph() 
        {

            if (_mGraphs.Count > _mCurrentGraphIndex)
            {
                return;
            }

            _mGraphs.Add(new Graph.Graph());
            _mDirector.GetPathManager.AddGraph(_mGraphs[_mGraphs.Count - 1]);
        }

    }
}
