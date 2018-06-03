using Microsoft.Xna.Framework;

namespace Singularity.Map.Properties
{   
    /// <summary>
    /// Provides constants which are predetermined before the game. May needs tweaks.
    /// </summary>
    internal static class MapConstants
    {   
        /// <summary>
        /// The width of the Grid used for collision detection in pixels.
        /// </summary>
        public const int GridWidth = 40;

        /// <summary>
        /// The height of the Grid used for collision detection in pixels.
        /// </summary>
        public const int GridHeight = 40;

        /// <summary>
        /// The width of the background picture for the map.
        /// </summary>
        public const int MapWidth = 2000;

        /// <summary>
        /// The height of the background picture for the map.
        /// </summary>
        public const int MapHeight = 1000;

        /// <summary>
        /// The most left point of the map.
        /// </summary>
        public static readonly Vector2 sLeft = new Vector2(0, MapConstants.MapHeight / 2f);

        /// <summary>
        /// The most top point of the map.
        /// </summary>
        public static readonly Vector2 sTop = new Vector2(MapConstants.MapWidth / 2f, 0);

        /// <summary>
        /// The most buttom point of the map.
        /// </summary>
        public static readonly Vector2 sBottom = new Vector2(MapConstants.MapWidth / 2f, MapConstants.MapHeight);

        /// <summary>
        /// The most right point of the map.
        /// </summary>
        public static readonly Vector2 sRight = new Vector2(MapConstants.MapWidth, MapConstants.MapHeight / 2f);

    }
}
