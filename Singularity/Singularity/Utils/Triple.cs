namespace Singularity.Utils
{

    /// <summary>
    /// Provides a generic container for three values, e.g. a 3-Tuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value of the tuple</typeparam>
    /// <typeparam name="T2">The type of the second value of the tuple</typeparam>
    /// <typeparam name="T3">The type of the third value of the tuple</typeparam>
    internal sealed class Triple<T1, T2, T3>
    {
        private readonly T1 mFirstValue;
        private readonly T2 mSecondValue;
        private readonly T3 mThirdValue;

        /// <summary>
        /// Creates a new 2-Tuple with the given values at their specified location.
        /// </summary>
        /// <param name="firstValue">The first value of the Triple</param>
        /// <param name="secondValue">The second value of the Triple</param>
        /// <param name="thirdValue">The third value of the Triple </param>
        public Triple(T1 firstValue, T2 secondValue, T3 thirdValue)
        {
            mFirstValue = firstValue;
            mSecondValue = secondValue;
            mThirdValue = thirdValue;

        }

        /// <summary>
        /// Gets the first value of this triple.
        /// </summary>
        /// <returns>The first value of this triple</returns>
        public T1 GetFirst()
        {
            return mFirstValue;
        }

        /// <summary>
        /// Gets the second value of this triple.
        /// </summary>
        /// <returns>The second value of this triple</returns>
        public T2 GetSecond()
        {
            return mSecondValue;
        }

        /// <summary>
        /// Gets the third value of this triple.
        /// </summary>
        /// <returns>The third value of this triple</returns>
        public T3 GetThird()
        {
            return mThirdValue;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is Triple<T1, T2, T3>))
            {
                return false;
            }

            var objAsTriple = (Triple<T1, T2, T3>)obj;

            return objAsTriple.GetFirst().Equals(obj: mFirstValue) && objAsTriple.GetSecond().Equals(obj: mSecondValue) && objAsTriple.GetThird().Equals(obj: mThirdValue);

        }

        public override int GetHashCode()
        {
            return mThirdValue.GetHashCode() * mFirstValue.GetHashCode() ^ mSecondValue.GetHashCode();
        }
    }
}
