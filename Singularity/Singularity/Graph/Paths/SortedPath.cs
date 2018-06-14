using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Property;

namespace Singularity.Graph.Paths
{
    /// <summary>
    /// An implementation of IPath featuring sorted paths. Sorted in the sense, that the first object of the queue is the starting
    /// node of the path, and the last object of the queue is the destination node of the path. That way we have a sorting
    /// on our nodes such that the path can be properly traversed by calling Queue.Dequeue sequently.
    /// 
    /// TODO: maybe make sure the nodes are sorted when GetVectorPath() or GetNodePath() gets called, because right now
    /// TODO: we actually rely on whoever creates this to add the path in a sorted fashion
    /// 
    /// </summary>
    public sealed class SortedPath : IPath
    {

        private readonly Queue<Vector2> mPath;

        private readonly Queue<INode> mNodePath;


        public SortedPath()
        {
            mNodePath = new Queue<INode>();
            mPath = new Queue<Vector2>();

        }

        public Queue<Vector2> GetVectorPath()
        {
            return mPath;
        }

        public Queue<INode> GetNodePath()
        {
            return mNodePath;
        }

        /// <summary>
        /// Adds a node to this path.
        /// </summary>
        /// <param name="node">The node to add</param>
        public void AddNode(INode node)
        {
            mNodePath.Enqueue(node);
            mPath.Enqueue(((IRevealing)node).Center);
        }
    }
}
