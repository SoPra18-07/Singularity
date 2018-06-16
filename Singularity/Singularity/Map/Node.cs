using Singularity.Exceptions;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    class Node
    {
        private readonly Optional<ICollider> mContents;

        public Node(int x, int y, Optional<ICollider> contents)
        {
            mContents = contents;
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// X coordinate of the node
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y coordinate of the node
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Walkability of the node
        /// </summary>
        /// <returns>True if the node is walkable</returns>
        public bool IsWalkable()
        {
            return !mContents.IsPresent();
        }

        /// <summary>
        /// The collider that is covering this node
        /// </summary>
        public ICollider Get()
        {
            if (!mContents.IsPresent())
            {
                throw new NoSuchElementException();
            }
            return mContents.Get();
        }
    }
}
