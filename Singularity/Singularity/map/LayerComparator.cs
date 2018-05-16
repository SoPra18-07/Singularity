using System.Collections.Generic;

namespace Singularity.map
{
    internal sealed class LayerComparator<T> : IComparer<T> where T : ILayerable
    {
        public int Compare(T x, T y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            return x.Layer.CompareTo(y.Layer);

        }
    }
}
