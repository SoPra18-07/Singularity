namespace Singularity.Utils
{

    /// <summary>
    /// Provides a generic container for two values, e.g. a 2-Tuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value of the tuple</typeparam>
    /// <typeparam name="T2">The type of the second value of the tuple</typeparam>
    internal sealed class Pair<T1, T2>
    {
        private readonly T1 mFirstValue;
        private readonly T2 mSecondValue;

        /// <summary>
        /// Creates a new 2-Tuple with the given values at their specified location.
        /// </summary>
        /// <param name="firstValue">The first value of the tuple</param>
        /// <param name="secondValue">The second value of the tuple</param>
        public Pair(T1 firstValue, T2 secondValue)
        {
            mFirstValue = firstValue;
            mSecondValue = secondValue;

        }

        /// <summary>
        /// Gets the first value of this pair.
        /// </summary>
        /// <returns>The first value of this pair</returns>
        public T1 GetFirst()
        {
            return mFirstValue;
        }

        /// <summary>
        /// Gets the second value of this pair.
        /// </summary>
        /// <returns>The second value of this pair</returns>
        public T2 GetSecond()
        {
            return mSecondValue;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Pair<T1, T2>))
            {
                return false;
            }

            var objAsPair = (Pair<T1, T2>) obj;

            return objAsPair.GetFirst().Equals(mFirstValue) && objAsPair.GetSecond().Equals(mSecondValue);

        }

        public override int GetHashCode()
        {
            return mFirstValue.GetHashCode() ^ mSecondValue.GetHashCode();
        }
    }
}
