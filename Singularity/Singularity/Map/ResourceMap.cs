using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Map
{
    internal sealed class ResourceMap
    {
        private readonly Dictionary<Vector2, Pair<EResourceType, int>> mResourceMap;

        public ResourceMap(IDictionary<Vector2, Pair<EResourceType, int>> initialResources)
        {
            if (initialResources == null)
            {
                return;
            }

            mResourceMap = new Dictionary<Vector2, Pair<EResourceType, int>>(initialResources);
        }

        public Optional<Pair<EResourceType, int>> GetResources(Vector2 location)
        {
            return Optional<Pair<EResourceType, int>>.Of(mResourceMap[location]);
        }
    }
}
