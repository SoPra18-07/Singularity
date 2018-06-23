using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    class CollisionNode
    {
        public int X { get; }

        public int Y { get; }

        public Optional<ICollider> Collider { get; }

        public CollisionNode(int x, int y, Optional<ICollider> iCollider)
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
