using System.Runtime.Serialization;

namespace Singularity.serialization
{
    /// <summary>
    /// A dummy class to create a cyclic reference in SerializationDummy.cs.
    /// </summary>
    [DataContract()]
    internal class CyclicDummy
    {
        [DataMember()]
        private object _mCyclicReference;

        public CyclicDummy(object reference)
        {
            _mCyclicReference = reference;
        }

        public object GetCyclicReference()
        {
            return _mCyclicReference;
        }
    }
}
