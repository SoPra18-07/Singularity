using System.Runtime.Serialization;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    [DataContract]
    public class CollisionNode
    {
        [DataMember]
        public int X { get; }
        [DataMember]
        public int Y { get; }
        [DataMember]
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
