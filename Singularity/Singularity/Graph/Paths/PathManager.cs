using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly List<Graph> mGraphs;
        
        public PathManager()
        {
            mGraphs = new List<Graph>();
        }

        public void AddGraph(Graph graph)
        {
            mGraphs.Add(graph);
        }

        public void RemoveGraph(Graph graph)
        {
            mGraphs.Remove(graph);
        }


        /// <summary>
        /// Gets a path for the given unit and its destination.
        /// </summary>
        /// <typeparam name="T">The type of the object wanting to request a path</typeparam>
        /// <param name="unit">The unit which requests a path</param>
        /// <param name="destination">The destination to which the path should lead</param>
        /// <returns></returns>
        public IPath GetPath<T>(T unit, INode destination)
        {
            // the basic idea for military and general units to use the same method and the distinuishing
            // between the two is handled here.

            var asGeneralUnit = unit as GeneralUnit;

            if (asGeneralUnit != null)
            {
                return GetPathForGeneralUnits(asGeneralUnit, destination);
            }

            var asMilitaryUnit = unit as MilitaryUnit;

            if (asMilitaryUnit != null)
            {
                return GetPathForMilitaryUnits(asMilitaryUnit, destination);
            }

            throw new InvalidGenericArgumentException(
                "The given argument was not one for which pathes are meant to be calculated. The following are" +
                "supported: MilitaryUnit and GeneralUnit.");

        }
        
        private IPath GetPathForGeneralUnits(GeneralUnit unit, INode destination)
        {
            //todo: know which units are on which graph
            return PathfindingFactory.GetPathfinding().AStar(mGraphs[0], unit.CurrentNode, destination);
        }

        private IPath GetPathForMilitaryUnits(MilitaryUnit unit, INode destination)
        {
            //todo: implement
            var pathfinding = PathfindingFactory.GetPathfinding();

            return new SortedPath();
        }

    }
}
