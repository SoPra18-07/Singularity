namespace Singularity.Platforms
{
    public enum EPlacementType
    {

        /// <summary>
        /// Means the platform will follow the mouse and will need a road connected to be placed.
        /// </summary>
        PlatformMouseFollowAndRoad,

        /// <summary>
        /// Means the road will follow the mouse and will need to be connected to another platform to be placed.
        /// </summary>
        RoadMouseFollowAndRoad
    }
}
