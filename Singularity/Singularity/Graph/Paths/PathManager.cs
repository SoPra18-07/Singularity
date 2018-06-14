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
    public class PathManager
    {
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


        public IPath GetPath<T>(T unit)
        {
            var asGeneralUnit = unit as GeneralUnit;

            if (asGeneralUnit != null)
            {
                return GetPathForGeneralUnits(asGeneralUnit);
            }

            var asMilitaryUnit = unit as MilitaryUnit;

            if (asMilitaryUnit != null)
            {
                return GetPathForMilitaryUnits(asMilitaryUnit);
            }

            throw new InvalidGenericArgumentException(
                "The given argument was not one for which pathes are meant to be calculated. The following are" +
                "supported: MilitaryUnit and GeneralUnit.");

        }

        private IPath GetPathForGeneralUnits(GeneralUnit unit)
        {
            var pathfinding = PathfindingFactory.GetPathfinding();

            //todo: know which units are on which graph
            return pathfinding.Dijkstra(mGraphs[0]);
        }

        private IPath GetPathForMilitaryUnits(MilitaryUnit unit)
        {
            var pathfinding = PathfindingFactory.GetPathfinding();

            return new SortedPath();
        }

    }
}
