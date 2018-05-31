using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.serialization
{
    [DataContract()]
    internal class CyclicDummy
    {
        [DataMember()]
        private object mCyclicReference;

        public CyclicDummy(object reference)
        {
            mCyclicReference = reference;
        }

        public object GetCyclicReference()
        {
            return mCyclicReference;
        }
    }
}
