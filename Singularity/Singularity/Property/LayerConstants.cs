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

        #region ScreenFactors

        //the reason for those factors is, that we also need to establish a sorting on the screens, since
        // the layer depth argument obviously sorts EVERYTHING on the screen according to the layers given,
        // so if our gamescreen already utilizes a depth of 0.99f then the hud almost can't compete, thats
        // why my idea was to establish a sorting from [0, 1] on every screen and then just norm it by the
        // given factor, so everything with a factor 0.1 is smaller than everythign with the factor 0.2.

        private const float GameScreenFactor = 0.1f;

        private const float UIFactor = 0.2f;

        #endregion

        #region GameScreenLayers

        /// <summary>
        /// A float depicting the layer for the map
        /// </summary>
        public const float MapLayer = 0.0f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for resources
        /// </summary>
        public const float ResourceLayer = 0.1f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for military units
        /// </summary>
        public const float MilitaryUnitLayer = 0.2f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for roads
        /// </summary>
        public const float RoadLayer = 0.3f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for platforms
        /// </summary>
        public const float PlatformLayer = 0.4f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for general units
        /// </summary>
        public const float GeneralUnitLayer = 0.5f * GameScreenFactor;

        /// <summary>
        /// A float depciting the layer of the map that is layered above the objects to smooth the fog of war appearance
        /// </summary>
        public const float FogOfWarMapLayer = 0.9f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for the fog of war
        /// </summary>
        public const float FogOfWarLayer = 0.91f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for the hitbox of the collider.
        /// </summary>
        public const float CollisionDebugLayer = 0.95f * GameScreenFactor;

        /// <summary>
        /// A float depicting the layer for the grid debug of the map
        /// </summary>
        public const float GridDebugLayer = 0.99f * GameScreenFactor;

        #endregion

        #region UIScreenLayers

        #endregion

    }
}
