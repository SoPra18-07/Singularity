using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Graph
{
    public interface IEdge
    {
        INode GetParent();

        INode GetChild();
    }
}
