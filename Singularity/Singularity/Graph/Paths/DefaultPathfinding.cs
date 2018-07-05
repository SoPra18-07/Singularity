using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Graph.Paths
{
    /// <summary>
    /// The Default pathfinding is meant for "basic" implementations of the algorithms
    /// providied by the IPathfinding interface. Basic basically means the implementation
    /// as stated by wikipedia.
    /// </summary>
    public sealed class DefaultPathfinding : IPathfinding
    {
        public IPath AStar(Graph graph, INode start, INode destination)
        {

            // this is actual copy pasted from wikipedia, this is definitely not optimized
            // since i was too bored to browse all the data structures available in c#
            // and their implementations. From what ive read there are no heap structures
            // though, so no priority queues from .NET :(, if anybody knows more stuff
            // about c# data structure implementations feel free to change them.
            //
            // https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode

            var closedList = new List<INode>();

            var openList = new List<INode> {start};

            var cameFrom = new Dictionary<INode, INode>();

            var gScore = new Dictionary<INode, float>();

            var fScore = new Dictionary<INode, float>();

            foreach (var node in graph.GetNodes())
            {
                gScore[key: node] = int.MaxValue;
                fScore[key: node] = int.MaxValue;
            }

            gScore[key: start] = 0f;

            fScore[key: start] = HeuristicCostEstimate(start: start, destination: destination);

            while (openList.Count > 0)
            {
                float minValue = int.MaxValue;

                INode current = null;

                foreach (var node in openList)
                {
                    if (fScore[key: node] < minValue)
                    {
                        minValue = fScore[key: node];
                        current = node;
                    }

                }

                // current can never be null from my short amount of thinking about it (if actual arguments are given)

                Debug.Assert(condition: current != null, message: "pathFinding failed.");
                if (current.Equals(obj: destination))
                {
                    return ReconstructPath(cameFrom: cameFrom, current: current);
                }

                openList.Remove(item: current);
                closedList.Add(item: current);
                //var edges = new List<IEdge>();
                //edges.AddRange(current.GetOutwardsEdges());
                //edges.AddRange(current.GetInwardsEdges());
                foreach (var outgoing in current.GetOutwardsEdges())
                {
                    var neighbor = outgoing.GetChild();

                    if (neighbor == null)
                    {
                        continue;
                    }

                    if (closedList.Contains(item: neighbor))
                    {
                        continue;
                    }

                    if (!openList.Contains(item: neighbor))
                    {
                        openList.Add(item: neighbor);
                    }

                    var tentativeGScore = gScore[key: current] + outgoing.GetCost();

                    if (tentativeGScore >= gScore[key: neighbor])
                    {
                        continue;
                    }

                    cameFrom[key: neighbor] = current;
                    gScore[key: neighbor] = tentativeGScore;
                    fScore[key: neighbor] = gScore[key: neighbor] + HeuristicCostEstimate(start: neighbor, destination: destination);
                }
                foreach (var outgoing in current.GetInwardsEdges())
                {
                    var neighbor = outgoing.GetParent();

                    if (neighbor == null)
                    {
                        continue;
                    }

                    if (closedList.Contains(item: neighbor))
                    {
                        continue;
                    }

                    if (!openList.Contains(item: neighbor))
                    {
                        openList.Add(item: neighbor);
                    }

                    var tentativeGScore = gScore[key: current] + outgoing.GetCost();

                    if (tentativeGScore >= gScore[key: neighbor])
                    {
                        continue;
                    }

                    cameFrom[key: neighbor] = current;
                    gScore[key: neighbor] = tentativeGScore;
                    fScore[key: neighbor] = gScore[key: neighbor] + HeuristicCostEstimate(start: neighbor, destination: destination);

                }
            }
            return null;

        }

        public IPath Dijkstra(Graph graph, INode start, INode destination)
        {
            throw new NotImplementedException(message: "The Dijkstra algorithm is not yet implemented.");
        }

        /// <summary>
        /// A consistent cost estimation method used for the A* implementation. This is consistent
        /// since HeuristicCostEstimation(x, destination) is always smaller/equal to  distance(x, y) + HeuristicCostEstimation(y, destination)
        /// for every edge (x, y) where x, y are nodes. This means that the straight line distance from a
        /// given node to to the goal is never greater than first taking the distance from x to y and adding the straight line
        /// distance from y to the goal. This is plausible for our game since we operate in an Euclidean space fulfilling
        /// euclidic distance and stuff.
        /// </summary>
        /// <param name="start">The starting node</param>
        /// <param name="destination">The destination node</param>
        /// <returns></returns>
        private static float HeuristicCostEstimate(INode start, INode destination)
        {
            return Vector2.Distance(value1: ((IRevealing) start).Center, value2: ((IRevealing) destination).Center);
        }

        /// <summary>
        /// Reconstructs the path created from the A* algorithm. This aswell is copy pasted from wikipedia.
        /// This is definitely not optimally implemented. This is O(3 * |cameFrom|) = (|cameFrom|) and
        /// could probably be implemented with a factor of 1.
        /// </summary>
        /// <param name="cameFrom">A dictionary holding the record of the best preceding node for the given node</param>
        /// <param name="current">The node from which to reconstruct the path</param>
        /// <returns></returns>
        private static IPath ReconstructPath(IReadOnlyDictionary<INode, INode> cameFrom, INode current)
        {
            var path = new List<INode>();

            var currentNode = current;
            path.Add(item: current);

            while (cameFrom.ContainsKey(key: currentNode))
            {
                currentNode = cameFrom[key: currentNode];
                path.Add(item: currentNode);
            }

            path.Reverse();

            var sortedPath = new SortedPath();

            foreach (var node in path)
            {
                sortedPath.AddNode(node: node);
            }

            return sortedPath;
        }
    }
}
