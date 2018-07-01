using System.Collections.Generic;

namespace Singularity.Graph
{
    /// <summary>
    /// Provides an interface for nodes, having a set of edges going outwards of it and inwards.
    /// </summary>
    public interface INode
    {
        IEnumerable<INode> GetChilds();

            /// <summary>
        /// Gets all the edges facing outwards of this node.
        /// </summary>
        /// <returns>The edges mentioned</returns>
        IEnumerable<IEdge> GetOutwardsEdges();

        /// <summary>
        /// Gets all the edges facing inwards of this node.
        /// </summary>
        /// <returns>The edges mentioned</returns>
        IEnumerable<IEdge> GetInwardsEdges();
    }
}
