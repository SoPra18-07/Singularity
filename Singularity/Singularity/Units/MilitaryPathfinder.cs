﻿using System;
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
    internal sealed class MilitaryPathfinder
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
            var startGridPos = VectorToGridPos(startPosition);
            var endGridPos = VectorToGridPos(endPosition);

            if (ClearDirectPath(startGridPos.x, startGridPos.y, endGridPos.x, endGridPos.y, ref map))
            {
                var pathVector = new Stack<Vector2>(1);
                pathVector.Push(endPosition);
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
                    mJpParam = new JumpPointParam(iGrid: map.GetCollisionMap().GetWalkabilityGrid(),
                        iStartPos: startGridPos,
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
            return new GridPos(iX: (int) Math.Floor(d: vector.X / MapConstants.GridWidth),
                               iY: (int) Math.Floor(d: vector.Y / MapConstants.GridHeight));
        }

        /// <summary>
        /// Converts a GridPos to a Vector2 based on map constants
        /// </summary>
        /// <param name="gridPos">GridPos to be converted</param>
        /// <returns>Corresponding Vector2 position</returns>
        private static Vector2 GridPosToVector2(GridPos gridPos)
        {
            return new Vector2(x: gridPos.x * MapConstants.GridWidth,
                               y: gridPos.y * MapConstants.GridHeight);
        }

        public bool ClearDirectPath(int x, int y, int x2, int y2, ref Map.Map map)
        {
            int w = x2 - x;
            int h = y2 - y;
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
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
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
}
