using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Utils
{
    /// <summary>
    /// This is probably the default comparator for ints, but might aswell, so I definitely know that it does sort by ascending.
    /// </summary>
    public sealed class AscendingIntegerComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x < y)
            {
                return -1;
            }

            return x > y ? 1 : 0;
        }
    }
}
