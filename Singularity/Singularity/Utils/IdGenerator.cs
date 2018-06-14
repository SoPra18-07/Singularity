using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Utils
{
    public static class IdGenerator
    {
        private static int sId = 0;

        public static int NextID()
        {
            sId++;
            return sId;
        }

    }
}
