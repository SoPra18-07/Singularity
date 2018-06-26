using System.Runtime.Serialization;

namespace Singularity.serialization
{
    /// <summary>
    /// A dummy class to create a cyclic reference in SerializationDummy.cs.
    /// </summary>
    [DataContract]
    internal class CyclicDummy
    {
        [DataMember]
        private object mMCyclicReference;

        public CyclicDummy(object reference)
        {
            mMCyclicReference = reference;
        }

        public object GetCyclicReference()
        {
            return mMCyclicReference;
        }
    }
}
