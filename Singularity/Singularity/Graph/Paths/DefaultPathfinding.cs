﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using C5;
using Microsoft.Xna.Framework;
using Singularity.Platforms;
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

            // this is more or less copy pasted from wikipedia, this is definitely not optimized
            //
            // now there's actually a PriorityQueue used, however I doubt it to be actually performant.
            //
            // reference:
            // https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode

            var closedList = new List<INode>();

            // var openList = new List<INode> {start};

            // openList.Comparer = null;

            var cameFrom = new Dictionary<INode, INode>();

            var gScore = new Dictionary<INode, float>();

            var fScore = new Dictionary<INode, float>();

            // ReSharper disable once ConvertToLocalFunction
            // converting to local function does not work
            Func<INode, INode, int> compareFunc = (a, b) => (int) fScore[a] > (int) fScore[b] ? 1 : (int) fScore[a] < (int) fScore[b] ? -1 : 0;

            var openList = new IntervalHeap<INode>(ComparerFactory<INode>.CreateComparer(compareFunc)) { start };

            foreach (var node in graph.GetNodes())
            {
                gScore[node] = int.MaxValue;
                fScore[node] = int.MaxValue;
            }

            gScore[start] = 0f;

            fScore[start] = HeuristicCostEstimate(start, destination);

            while (openList.Count > 0)
            {

                var current = openList.DeleteMin();

                // current can never be null from my short amount of thinking about it (if actual arguments are given)

                Debug.Assert(current != null, "pathFinding failed.");
                if (current.Equals(destination))
                {
                    return ReconstructPath(cameFrom, current);
                }

                closedList.Add(current);
                foreach (var outgoing in current.GetOutwardsEdges())
                {
                    var neighbor = outgoing.GetChild();

                    if (neighbor == null || ((Road) outgoing).Blueprint)
                    {
                        continue;
                    }

                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }

                    var tentativeGScore = gScore[current] + outgoing.GetCost();

                    if (tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, destination);
                }
                foreach (var outgoing in current.GetInwardsEdges())
                {
                    var neighbor = outgoing.GetParent();

                    if (neighbor == null || ((PlatformBlank) neighbor).mBlueprint)
                    {
                        continue;
                    }

                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }

                    var tentativeGScore = gScore[current] + outgoing.GetCost();

                    if (tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, destination);

                }
            }
            return null;

        }

        public IPath Dijkstra(Graph graph, INode start, INode destination)
        {
            throw new NotImplementedException("The Dijkstra algorithm is not yet implemented.");
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
            return Vector2.Distance(((IRevealing) start).Center, ((IRevealing) destination).Center);
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
            path.Add(current);

            while (cameFrom.ContainsKey(currentNode))
            {
                currentNode = cameFrom[currentNode];
                path.Add(currentNode);
            }

            path.Reverse();

            var sortedPath = new SortedPath();

            foreach (var node in path)
            {
                sortedPath.AddNode(node);
            }

            return sortedPath;
        }
    }
}
