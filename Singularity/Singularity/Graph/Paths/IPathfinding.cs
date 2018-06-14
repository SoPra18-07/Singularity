using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Graph.Paths
{
    public interface IPathfinding
    {
        IPath Dijkstra(Graph graph);
    }
}
