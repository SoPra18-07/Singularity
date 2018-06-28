using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                mJpParam.Reset(VectorToGridPos(startPosition), VectorToGridPos(endPosition));
            }
            else
            {
                mJpParam = new JumpPointParam(iGrid: map.GetCollisionMap().GetWalkabilityGrid(),
                    iStartPos: VectorToGridPos(startPosition),
                    iDiagonalMovement: DiagonalMovement.OnlyWhenNoObstacles,
                    iEndPos: VectorToGridPos(endPosition),
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

        /// <summary>
        /// Converts a Vector2 to its corresponding grid position based on map constants
        /// </summary>
        /// <param name="vector">Vector2 to be converted</param>
        /// <returns>Corresponding grid positions</returns>
        private GridPos VectorToGridPos(Vector2 vector)
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

    }
}
