using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Graph
{
    public interface INode
    {
        List<IEdge> GetOutwardsEdges();

        List<IEdge> GetInwardsEdges();
    }
}
