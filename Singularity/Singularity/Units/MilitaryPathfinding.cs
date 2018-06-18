using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Map;

namespace Singularity.Units
{
    /// <summary>
    /// Static class enabling military units to find a path to a destination. Implements Jump Point Search algorithm
    /// </summary>
    internal sealed class MilitaryPathfinding
    {
        private CollisionMap mGrid;

        /// <summary>
        /// Creates a JumpPointSearch object which can be used to find a path to a
        /// target position using JumpPointSearch
        /// </summary>
        /// <param name="map">Map used by the game</param>
        internal void JumpPointSearch(Map.Map map)
        {
            mGrid = map.GetCollisionMap();
        }

        internal List<Node> FindPath(Vector2 startPosition, Vector2 destinationPosition)
        {
            throw new NotImplementedException();
        }

        private Vector2 Jump()
        {
            throw new NotImplementedException();
        }

        private int[,] FindNeighbors(Node node)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates the manhatten distance for heuristics.
        /// </summary>
        /// <param name="distX">Distance to target in X</param>
        /// <param name="distY">Distance to target in Y</param>
        /// <returns></returns>
        private int Manhatten(int distX, int distY)
        {
            return distX + distY;
        }
    }
}
