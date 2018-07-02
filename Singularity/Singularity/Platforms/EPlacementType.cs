namespace Singularity.Platforms
{
    public enum EPlacementType
    {
        /// <summary>
        /// Means the platform will get placed instantly at the given position.
        /// </summary>
        Instant,

        /// <summary>
        /// Means the platform will follow the mouse and be placed without a road needed to connect to it.
        /// </summary>
        OnlyMouseFollow,

        /// <summary>
        /// Means the platform will follow the mouse and will need a road connected to be placed.
        /// </summary>
        MouseFollowAndRoad,

        /// <summary>
        /// Means the platform will be placed at the given position, but a road needs to be connected to it.
        /// </summary>
        OnlyRoad,
    }
}
