using System;

namespace Singularity.Exceptions
{
    public sealed class MiniMapProportionsOffException : Exception
    {
        public MiniMapProportionsOffException() : base()
        {
        }

        public MiniMapProportionsOffException(string message) : base(message)
        {
        }
    }
}
