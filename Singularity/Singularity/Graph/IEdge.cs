namespace Singularity.Graph
{
    /// <summary>
    /// Provides an interface for egdes, having a parent, child, and a cost. This can also be used for undirected graphs or graphs with no
    /// cast function for their edges.
    /// </summary>
    public interface IEdge
    {
        EEdgeFacing EEdgeFacing { get; set; }

        /// <summary>
        /// Gets the parent node this edge.
        /// </summary>
        /// <returns>The node mentioned</returns>
        INode GetParent();

        /// <summary>
        /// Gets the child node for this edge.
        /// </summary>
        /// <returns>The node mentioned</returns>
        INode GetChild();

        /// <summary>
        /// Gets the cost for this edge.
        /// </summary>
        /// <returns>The cost mentioned</returns>
        float GetCost();
    }
}
