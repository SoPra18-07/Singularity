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
        private readonly T1 _mFirstValue;
        private readonly T2 _mSecondValue;
        private readonly T3 _mThirdValue;

        /// <summary>
        /// Creates a new 2-Tuple with the given values at their specified location.
        /// </summary>
        /// <param name="firstValue">The first value of the Triple</param>
        /// <param name="secondValue">The second value of the Triple</param>
        /// <param name="thirdValue">The third value of the Triple </param>
        public Triple(T1 firstValue, T2 secondValue, T3 thirdValue)
        {
            _mFirstValue = firstValue;
            _mSecondValue = secondValue;
            _mThirdValue = thirdValue;

        }

        /// <summary>
        /// Gets the first value of this triple.
        /// </summary>
        /// <returns>The first value of this triple</returns>
        public T1 GetFirst()
        {
            return _mFirstValue;
        }

        /// <summary>
        /// Gets the second value of this triple.
        /// </summary>
        /// <returns>The second value of this triple</returns>
        public T2 GetSecond()
        {
            return _mSecondValue;
        }

        /// <summary>
        /// Gets the third value of this triple.
        /// </summary>
        /// <returns>The third value of this triple</returns>
        public T3 GetThird()
        {
            return _mThirdValue;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is Triple<T1, T2, T3>))
            {
                return false;
            }

            var objAsTriple = (Triple<T1, T2, T3>)obj;

            return (objAsTriple.GetFirst().Equals(_mFirstValue) && objAsTriple.GetSecond().Equals(_mSecondValue) && objAsTriple.GetThird().Equals(_mThirdValue));

        }

        public override int GetHashCode()
        {
            return _mThirdValue.GetHashCode() * _mFirstValue.GetHashCode() ^ _mSecondValue.GetHashCode();
        }
    }
}
