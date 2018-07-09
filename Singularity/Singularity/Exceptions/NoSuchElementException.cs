using System;

namespace Singularity.Exceptions
{
    public sealed class NoSuchElementException : Exception
    {
        private const string ErrorMessage = "No value present";

        public NoSuchElementException(string message) : base(message) { }

        public NoSuchElementException() : base(ErrorMessage) { }

    }
}
