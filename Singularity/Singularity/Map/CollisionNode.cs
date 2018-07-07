using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    public class CollisionNode
    {
        public int X { get; }

        public int Y { get; }

        internal Optional<ICollider> Collider { get; }

        internal CollisionNode(int x, int y, Optional<ICollider> iCollider)
        {
            X = x;
            Y = y;
            Collider = iCollider;
        }

        public bool IsWalkable()
        {
            return !Collider.IsPresent();
        }
    }
}
