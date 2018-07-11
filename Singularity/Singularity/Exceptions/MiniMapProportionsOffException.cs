using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
