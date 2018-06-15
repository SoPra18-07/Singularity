using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Exceptions
{
    public sealed class InvalidGenericArgumentException : Exception
    {
        public InvalidGenericArgumentException() : base() { }

        public InvalidGenericArgumentException(string message) : base(message) { }

    }
}
