using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.Units;

namespace Singularity.DistributionManager
{
    [DataContract()]
    class DistributionManager
    {
        [DataMember()]
        private List<GeneralUnit> mUnits;
    }
}
