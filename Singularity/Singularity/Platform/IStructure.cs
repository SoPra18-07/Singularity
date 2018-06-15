using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Platform
{
    interface IStructure
    {
        bool IsPlaced { get; set; }

        bool IsAdded { get; set; }

        bool IsSemiPlaced { get; set; }

    }
}
