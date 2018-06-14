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
    public sealed class DummyPathfinding : IPathfinding
    {

        public IPath Dijkstra(Graph graph)
        {
            var path = new SortedPath();

            foreach (var node in graph.GetNodes())
            {
                path.AddNewPosition(((IRevealing)node).Center);
            }

            return path;
        }
    }
}
