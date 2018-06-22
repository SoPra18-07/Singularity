namespace Singularity.Utils
{
	public interface IId
	{
		int MId { get; }
	}
	
    public static class IdGenerator
    {
        private static int sId; // (defaults to 0)

        public static int NextiD()
        {
            sId++;
            return sId;
        }

    }
}
