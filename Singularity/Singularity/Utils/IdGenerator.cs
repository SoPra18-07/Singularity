namespace Singularity.Utils
{
    public interface IId
    {
        /// <summary>
        /// The unique ID of an object.
        /// </summary>
        int Id { get; }
    }

    /// <summary>
    /// This static class provides unique IDs to anything that requests it.
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Holds the last ID that was generated to make sure the next ID is not the same.
        /// </summary>
        private static int sId; // (defaults to 0)

        /// <summary>
        /// Get the next unique ID from the ID generator.
        /// </summary>
        /// <returns>A unique ID for the object.</returns>
        public static int NextiD()
        {
            sId++;
            return sId;
        }

    }
}
