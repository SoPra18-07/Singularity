using System;
using System.Collections.Generic;
using System.Diagnostics;
using EpPathFinding.cs;
using Microsoft.Xna.Framework;
using Singularity.Map.Properties;

namespace Singularity.Units
{
    /// <summary>
    /// A Jump Point Search pathfinder implementation for military units
    /// </summary>
    public sealed class FreeMovingPathfinder
    {
        private JumpPointParam mJpParam;

        /// <summary>
        /// Finds a path between 2 position on a map
        /// </summary>
        /// <param name="startPosition">Starting position</param>
        /// <param name="endPosition">Destination</param>
        /// <param name="map">Game map currently being used</param>
        /// <returns>A list of Vector2 waypoints that the object must traverse to get to its destination</returns>
        internal Stack<Vector2> FindPath(Vector2 startPosition, Vector2 endPosition, ref Map.Map map)
        {
            Debug.WriteLine("Searching for path");
            var startGridPos = VectorToGridPos(startPosition);
            var endGridPos = VectorToGridPos(endPosition);

            if (ClearDirectPath(startGridPos.x, startGridPos.y, endGridPos.x, endGridPos.y, ref map))
            {
                var pathVector = new Stack<Vector2>(1);
                pathVector.Push(endPosition);
                Debug.WriteLine("returning direct Path!");
                return pathVector;
            }
            else
            {
                if (mJpParam != null)
                {
                    mJpParam.Reset(VectorToGridPos(startPosition), VectorToGridPos(endPosition));
                }
                else
                {
                    mJpParam = new JumpPointParam(map.GetCollisionMap().GetWalkabilityGrid(),
                        startGridPos,
                        iDiagonalMovement: DiagonalMovement.OnlyWhenNoObstacles,
                        iEndPos: endGridPos,
                        iAllowEndNodeUnWalkable: EndNodeUnWalkableTreatment.DISALLOW,
                        iMode: HeuristicMode.MANHATTAN);
                }

                var pathGrid = JumpPointFinder.FindPath(mJpParam);

                var pathVector = new Stack<Vector2>(pathGrid.Count);

                pathVector.Push(endPosition);


                Debug.WriteLine("Path:");
                for (var i = pathGrid.Count - 1; i > 0; i--)
                {
                    var gridPos = GridPosToVector2(pathGrid[i]);
                    Debug.WriteLine(gridPos.X + ", " + gridPos.Y);
                    pathVector.Push(gridPos);
                }
                Debug.WriteLine("Path is this long: " + pathVector.Count);

                return pathVector;
            }
        }

        /// <summary>
        /// Converts a Vector2 to its corresponding grid position based on map constants
        /// </summary>
        /// <param name="vector">Vector2 to be converted</param>
        /// <returns>Corresponding grid positions</returns>
        private static GridPos VectorToGridPos(Vector2 vector)
        {
            return new GridPos((int) Math.Floor(vector.X / MapConstants.GridWidth),
                               (int) Math.Floor(vector.Y / MapConstants.GridHeight));
        }

        /// <summary>
        /// Converts a GridPos to a Vector2 based on map constants
        /// </summary>
        /// <param name="gridPos">GridPos to be converted</param>
        /// <returns>Corresponding Vector2 position</returns>
        private static Vector2 GridPosToVector2(GridPos gridPos)
        {
            return new Vector2(gridPos.x * MapConstants.GridWidth,
                               gridPos.y * MapConstants.GridHeight);
        }

        /// <summary>
        /// Checks if a direct plath is clear of obstacles. Based on Bresenham's
        /// line algorithm.
        /// </summary>
        /// <see cref="http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm"/>
        /// <param name="x">Starting x position.</param>
        /// <param name="y">Starting y position.</param>
        /// <param name="x2">Target x position.</param>
        /// <param name="y2">Target y position.</param>
        /// <param name="map">Reference to the game map.</param>
        /// <returns>True if there is a clear direct path to the target.</returns>
        public static bool ClearDirectPath(int x, int y, int x2, int y2, ref Map.Map map)
        {
            var w = x2 - x;
            var h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0)
            {
                dx1 = -1;
            }
            else if (w > 0)
            {
                dx1 = 1;
            }

            if (h < 0)
            {
                dy1 = -1;
            }
            else if (h > 0)
            {
                dy1 = 1;
            }

            if (w < 0)
            {
                dx2 = -1;
            }
            else if (w > 0)
            {
                dx2 = 1;
            }

            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0)
                {
                    dy2 = -1;
                }
                else if (h > 0)
                {
                    dy2 = 1;
                }

                dx2 = 0;
            }
            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++)
            {
                if (!map.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(x, y))
                {
                    return false;
                }

                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }

            return true;
        }
    }

    // some further ideas:
    // - Flocking ( https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444 )
    // - Steering ( http://www.simoncoenen.com/downloads/ai_paper.pdf )
    // - Nav Mesh ( http://theory.stanford.edu/~amitp/GameProgramming/MapRepresentations.html )
}
