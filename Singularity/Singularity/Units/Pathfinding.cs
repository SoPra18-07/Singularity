using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Resources;

namespace Singularity.Units
{
    class Pathfinding
    {
        /// <summary>
        /// Searches for the closest resource of a given type using breadth first search.
        /// </summary>
        /// <param name="currentPositionId">ID of the current platform the unit is on</param>
        /// <param name="resourceType">Type of resource that is being searched for</param>
        /// <returns></returns>
        public int? Bfs(int currentPositionId, EResourceType resourceType)
        {
            // TODO
            // does a breadth first search until it finds the resource
            return null;
        }

        /// <summary>
        /// Uses Dijkstra's algorithm to find the fastest path between two points
        /// </summary>
        /// <param name="startPlatformId">ID of the starting platform where the algorithm will start its search</param>
        /// <param name="targetPlatformId">ID of the target platform</param>
        /// <returns></returns>
        public Stack<int> Dijkstra(int startPlatformId, int targetPlatformId)
        {
            var pathQueue = new Stack<int>();
            // TODO
            return pathQueue;
        }

    }
}
