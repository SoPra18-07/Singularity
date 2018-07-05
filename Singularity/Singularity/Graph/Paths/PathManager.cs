using System.Collections.Generic;
using Singularity.Exceptions;
using Singularity.Units;

namespace Singularity.Graph.Paths
{

    /// <summary>
    /// The path manager holds all the graphs currently in the game and can
    /// get paths for general and military units, given a destination.
    /// </summary>
    public sealed class PathManager
    {
        /// <summary>
        /// All the graphs currently in the game
        /// </summary>
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
        /// <returns></returns>
        public IPath GetPath<T>(T unit, INode destination, int GraphIndex)
        {
            // the basic idea for military and general units to use the same method and the distinuishing
            // between the two is handled here.

            var asGeneralUnit = unit as GeneralUnit;

            if (asGeneralUnit != null)
            {
                return GetPathForGeneralUnits(asGeneralUnit, destination, GraphIndex);
            }

            var asMilitaryUnit = unit as MilitaryUnit;

            if (asMilitaryUnit != null)
            {
                return GetPathForMilitaryUnits(asMilitaryUnit, destination, GraphIndex);
            }

            throw new InvalidGenericArgumentException(
                "The given argument was not one for which paths are meant to be calculated. The following are" +
                "supported: MilitaryUnit and GeneralUnit.");

        }

        private IPath GetPathForGeneralUnits(GeneralUnit unit, INode destination, int graphIndex)
        {
            //TODO: implement distribution on multiple graphs, then the following boolean expression can be removed

            return PathfindingFactory.GetPathfinding().AStar(mGraphs[graphIndex], unit.CurrentNode, destination);
        }

        private IPath GetPathForMilitaryUnits(MilitaryUnit unit, INode destination, int graphIndex)
        {
            //todo: implement
            var pathfinding = PathfindingFactory.GetPathfinding();

            return new SortedPath();
        }

    }
}
