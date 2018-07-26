using System;

namespace Singularity.Exceptions
{
    public sealed class NoSuchElementException : Exception
    {
        private const string ErrorMessage = "No value present";

        // ReSharper disable once UnusedMember.Global
        public NoSuchElementException(string message) : base(message) { }

        public NoSuchElementException() : base(ErrorMessage) { }

    }
}
