using System.ComponentModel.Design.Serialization;

namespace Singularity.Utils
{
	public interface IId
	{
		int MId { get; }
	}
	
    public static class IdGenerator
    {
        private static int _sId; // (defaults to 0)

        public static int NextiD()
        {
            _sId++;
            return _sId;
        }

    }
}
