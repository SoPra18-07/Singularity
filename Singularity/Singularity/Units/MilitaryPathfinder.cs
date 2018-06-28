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
            if (mJpParam != null)
            {
                mJpParam.Reset(iStartPos: VectorToGridPos(vector: startPosition), iEndPos: VectorToGridPos(vector: endPosition));
            }
            else
            {
                mJpParam = new JumpPointParam(iGrid: map.GetCollisionMap().GetWalkabilityGrid(),
                    iStartPos: VectorToGridPos(vector: startPosition),
                    iDiagonalMovement: DiagonalMovement.OnlyWhenNoObstacles,
                    iEndPos: VectorToGridPos(vector: endPosition),
                    iAllowEndNodeUnWalkable: EndNodeUnWalkableTreatment.DISALLOW,
                    iMode: HeuristicMode.MANHATTAN);
            }
            var pathGrid = JumpPointFinder.FindPath(iParam: mJpParam);

            var pathVector = new Stack<Vector2>(capacity: pathGrid.Count);
            pathVector.Push(item: endPosition);

            Debug.WriteLine(message: "Path:");
            for (int i = pathGrid.Count - 1; i > 0; i--)
            {
                var gridPos = GridPosToVector2(gridPos: pathGrid[index: i]);
                Debug.WriteLine(message: gridPos.X + ", " + gridPos.Y);
                pathVector.Push(item: gridPos);
            }

            return pathVector;
        }

        /// <summary>
        /// Converts a Vector2 to its corresponding grid position based on map constants
        /// </summary>
        /// <param name="vector">Vector2 to be converted</param>
        /// <returns>Corresponding grid positions</returns>
        private GridPos VectorToGridPos(Vector2 vector)
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

    }
}
