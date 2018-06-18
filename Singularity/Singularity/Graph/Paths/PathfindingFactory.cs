namespace Singularity.Graph.Paths
{

    /// <summary>
    /// Provides a singleton for the current pathfinding instance. This is useful since we
    /// can access the pathfinding from everywhere now and can change the implementation
    /// of the pathfining object here.
    /// </summary>
    public static class PathfindingFactory
    {
        private static IPathfinding _sPathfinding;

        public static IPathfinding GetPathfinding()
        {
            return _sPathfinding ?? (_sPathfinding = new DefaultPathfinding());
        }

    }
}
