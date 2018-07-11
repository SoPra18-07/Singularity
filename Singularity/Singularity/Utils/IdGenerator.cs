using System.Runtime.Serialization;

namespace Singularity.Utils
{
    public interface IId
    {
        int MId { get; }
    }

    [DataContract]
    public class IdGenerator
    {
        [DataMember]
        private int mId; // (defaults to 0)

        public int NextiD()
        {
            mId++;
            return mId;
        }

    }
}
