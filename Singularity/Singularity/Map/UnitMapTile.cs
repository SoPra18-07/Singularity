using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Property;

namespace Singularity.Map
{
    [DataContract]
    internal sealed class UnitMapTile
    {
        [DataMember]
        public List<ICollider> UnitList { get; private set; } = new List<ICollider>();
    }
}
