using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Singularity.Graph.Paths
{
    /// <summary>
    /// Describes a path holding the path as a Coordinate Queue and as a Node Queue.
    /// The sorting of these queues is up to the implementation
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Gets all the coordinates (center) from the nodes which to travel.
        /// </summary>
        /// <returns>A Queue of Vector2</returns>
        Queue<Vector2> GetVectorPath();

        /// <summary>
        /// Gets all the nodes which to travel.
        /// </summary>
        /// <returns>A Queue of nodes</returns>
        Queue<INode> GetNodePath();

    }
}
