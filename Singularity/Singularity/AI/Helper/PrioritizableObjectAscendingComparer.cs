using System;
using System.Collections.Generic;

namespace Singularity.AI.Helper
{
    public sealed class PrioritizableObjectAscendingComparer<T> : IComparer<PrioritizableObject<T>>
    {
        public int Compare(PrioritizableObject<T> x, PrioritizableObject<T> y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("cannot compare against null");
            }

            if (x.GetPrioritization() < y.GetPrioritization())
            {
                return -1;
            }

            return x.GetPrioritization() > y.GetPrioritization() ? 1 : 0;
        }
    }
}
