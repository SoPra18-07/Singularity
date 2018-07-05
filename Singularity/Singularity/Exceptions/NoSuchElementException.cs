using System;

namespace Singularity.Exceptions
{
    internal sealed class NoSuchElementException : Exception
    {
        private const string ErrorMessage = "No value present";

        public NoSuchElementException(string message) : base(message: message) { }

        public NoSuchElementException() : base(message: ErrorMessage) { }

    }
}
