using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Exceptions;

namespace Singularity.Utils
{
    /// <summary>
    /// A container object which may or may not contain a non-null
    /// value. If a value is present, isPresent() will return true
    /// and Get() will return the value.
    /// </summary>
    /// <typeparam name="T">The type of this optional</typeparam>
    internal sealed class Optional<T>
    {
        private readonly T mValue;

        /// <summary>
        /// Returns a new Optinal for the given value.
        /// </summary>
        /// <param name="value">The value this Optional is holding</param>
        /// <returns>A new Optional with the value given</returns>
        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Gets the current value of the Optional if its not null. Otherwise an
        /// exception is thrown. Always check with IsPresent() if the value is
        /// not null.
        /// </summary>
        /// <returns>The value if not null</returns>
        public T Get()
        {
            if (mValue == null)
            {
                throw new NoSuchElementException();
            }
            return mValue;

        }

        /// <summary>
        /// Returns whether the value is null or not.
        /// </summary>
        /// <returns>True if the value is not null, false otherwise</returns>
        public bool IsPresent()
        {
            return mValue != null;
        }

        private Optional(T value)
        {
            mValue = value;

        }
    }
}
