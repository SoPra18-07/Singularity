using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Graph.Paths
{
    /// <summary>
    /// No documentation needed, this was solely used for debugging purposes.
    /// </summary>
    public sealed class DummyPathfinding : IPathfinding
    {
        public IPath Dijkstra(Graph graph, INode start, INode destination)
        {
            var path = new SortedPath();

            foreach (var node in graph.GetNodes())
            {
                path.AddNode(node);
            }

            return path;
        }

        public IPath AStar(Graph graph, INode start, INode destination)
        {
            var path = new SortedPath();

            foreach (var node in graph.GetNodes())
            {
                path.AddNode(node);
            }

            return path;
        }
    }
}
