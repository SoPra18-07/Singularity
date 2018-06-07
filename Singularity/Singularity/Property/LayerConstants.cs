namespace Singularity.Property
{
    
    /// <summary>
    /// Provides layer values for the different game objects. The smaller the value the more in the back the unit
    /// gets drawn. The idea for this class was this we can dynamically add and change values without having to
    /// touch the actual draw code at all. These constants should be used for the layerDepth argument
    /// in the Draw() method for the sprite batch.
    /// </summary>
    internal static class LayerConstants
    {
        /// <summary>
        /// A float depicting the layer for the map
        /// </summary>
        public const float MapLayer = 0.0f;

        /// <summary>
        /// A float depicting the layer for resources
        /// </summary>
        public const float ResourceLayer = 0.1f;

        /// <summary>
        /// A float depicting the layer for military units
        /// </summary>
        public const float MilitaryUnitLayer = 0.2f;

        /// <summary>
        /// A float depicting the layer for roads
        /// </summary>
        public const float RoadLayer = 0.3f;

        /// <summary>
        /// A float depicting the layer for platforms
        /// </summary>
        public const float PlatformLayer = 0.4f;

        /// <summary>
        /// A float depicting the layer for general units
        /// </summary>
        public const float GeneralUnitLayer = 0.5f;

    }
}
