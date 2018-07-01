using System;

namespace Singularity.Exceptions
{
    public sealed class InvalidGenericArgumentException : Exception
    {
        public InvalidGenericArgumentException() { }

        public InvalidGenericArgumentException(string message) : base(message) { }

    }
}
