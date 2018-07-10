namespace Singularity.Utils
{
    public interface IId
    {
        int Id { get; }
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
