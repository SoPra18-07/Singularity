namespace Singularity.Graph.Paths
{
    /// <summary>
    /// Provides an interface for numerous different path finding algorithms. This allows for different
    /// implementations for these.
    /// </summary>
    public interface IPathfinding
    {
        /// <summary>
        /// The Dijkstra algorithm. For further information refer to:
        /// https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        /// </summary>
        /// <param name="graph">The graph on which to operate the algorithm</param>
        /// <param name="start">The starting node</param>
        /// <param name="destination">The destination node</param>
        /// <returns></returns>
        IPath Dijkstra(Graph graph, INode start, INode destination);


        /// <summary>
        /// The A* algorithm. For further information refer to:
        /// https://en.wikipedia.org/wiki/A*_search_algorithm
        /// </summary>
        /// <param name="graph">The graph on which to operate the algorithm</param>
        /// <param name="start">The starting node</param>
        /// <param name="destination">The destination node</param>
        /// <returns></returns>
        IPath AStar(Graph graph, INode start, INode destination);
    }
}
