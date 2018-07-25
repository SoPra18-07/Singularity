using System;

namespace Singularity.Exceptions
{
    public sealed class InvalidGenericArgumentException : Exception
    {
        // ReSharper disable once UnusedMember.Global
        public InvalidGenericArgumentException() { }

        public InvalidGenericArgumentException(string message) : base(message) { }

    }
}
