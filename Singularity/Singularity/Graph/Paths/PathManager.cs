using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Exceptions;
using Singularity.Units;

namespace Singularity.Graph.Paths
{

    /// <summary>
    /// The path manager holds all the graphs currently in the game and can
    /// get paths for general and military units, given a destination.
    /// </summary>
    [DataContract]
    public sealed class PathManager
    {
        /// <summary>
        /// All the graphs currently in the game
        /// </summary>
        [DataMember]
        private readonly Dictionary<int, Graph> mGraphs;

        public PathManager()
        {
            mGraphs = new Dictionary<int, Graph>();
        }

        public void AddGraph(int id, Graph graph)
        {

            if (mGraphs.ContainsKey(id))
            {
                mGraphs[id] = graph;
                return;
            }
            mGraphs.Add(id, graph);
        }

        public void RemoveGraph(int id)
        {
            mGraphs.Remove(id);
        }


        /// <summary>
        /// Gets a path for the given unit and its destination.
        /// </summary>
        /// <typeparam name="T">The type of the object wanting to request a path</typeparam>
        /// <param name="unit">The unit which requests a path</param>
        /// <param name="destination">The destination to which the path should lead</param>
        /// <param name="graphIndex">The index of the Graph to get a Path on</param>
        /// <returns></returns>
        public IPath GetPath<T>(T unit, INode destination, int graphIndex)
        {

            var asGeneralUnit = unit as GeneralUnit;

            if (asGeneralUnit != null)
            {
                return GetPathForGeneralUnits(asGeneralUnit, destination, graphIndex);
            }

            throw new InvalidGenericArgumentException(
                "The given argument was not one for which paths are meant to be calculated. The following are" +
                "supported: GeneralUnit.");

        }

        private IPath GetPathForGeneralUnits(GeneralUnit unit, INode destination, int graphIndex)
        {
            IPath path;
            try
            {
                path = PathfindingFactory.GetPathfinding().AStar(mGraphs[graphIndex], unit.CurrentNode, destination);
            }
            catch (KeyNotFoundException e)
            {
                path = new SortedPath();
            }

            return path;
        }


    }
}
