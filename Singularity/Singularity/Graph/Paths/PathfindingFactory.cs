using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Graph.Paths
{

    /// <summary>
    /// Provides a singleton for the current pathfinding instance. This is useful since we
    /// can access the pathfinding from everywhere now and can change the implementation
    /// of the pathfining object here.
    /// </summary>
    public static class PathfindingFactory
    {
        private static IPathfinding sPathfinding;

        public static IPathfinding GetPathfinding()
        {
            return sPathfinding ?? (sPathfinding = new DefaultPathfinding());
        }

    }
}
