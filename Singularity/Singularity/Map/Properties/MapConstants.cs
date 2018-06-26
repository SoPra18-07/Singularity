﻿using Microsoft.Xna.Framework;

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
        public const int FogOfWarGridWidth = 10;

        /// <summary>
        /// The height of the Grid used for collision detection in pixels.
        /// </summary>
        public const int FogOfWarGridHeight = 10;

        /// <summary>
        /// The width of the Grid used for collision detection in pixels.
        /// </summary>
        public const int GridWidth = 20;

        /// <summary>
        /// The height of the Grid used for collision detection in pixels.
        /// </summary>
        public const int GridHeight = 20;

        /// <summary>
        /// The width of the background picture for the map.
        /// </summary>
        public const int MapWidth = 4000;

        /// <summary>
        /// The height of the background picture for the map.
        /// </summary>
        public const int MapHeight = 2000;

        /// <summary>
        /// The width of each map tile.
        /// </summary>
        public const int TileWidth = 200;

        /// <summary>
        /// The height of each map tile.
        /// </summary>
        public const int TileHeight = 100;

        /// <summary>
        /// The most left point of the map.
        /// </summary>
        public static readonly Vector2 sLeft = new Vector2(0, MapHeight / 2f);

        /// <summary>
        /// The most top point of the map.
        /// </summary>
        public static readonly Vector2 sTop = new Vector2(MapWidth / 2f, 0);

        /// <summary>
        /// The most buttom point of the map.
        /// </summary>
        public static readonly Vector2 sBottom = new Vector2(MapWidth / 2f, MapHeight);

        /// <summary>
        /// The most right point of the map.
        /// </summary>
        public static readonly Vector2 sRight = new Vector2(MapWidth, MapHeight / 2f);

    }
}
