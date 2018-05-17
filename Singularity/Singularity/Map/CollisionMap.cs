using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.Xna.Framework;
using Singularity.Map.Properties;
using Singularity.Utils;

namespace Singularity.Map
{   
    /// <summary>
    /// The collision map is used to store all the colliding objects in a grid like fashion.
    /// </summary>
    internal sealed class CollisionMap
    {   
        /// <summary>
        /// The look up table is used to check whether a given collider is already present in the collision map
        /// </summary>
        private readonly Dictionary<int, Vector2> mLookUpTable;

        /// <summary>
        /// The collision map is used to store the position and the id of every object which is able to collide.
        /// </summary>
        private readonly Optional<Pair<Vector2, int>>[,] mCollisionMap;

        /// <summary>
        /// Creates a new Collision map used to store and update all colliding objects.
        /// </summary>
        public CollisionMap()
        {
            mLookUpTable = new Dictionary<int, Vector2>();

            mCollisionMap = new Optional<Pair<Vector2, int>>
            [
                (MapConstants.MapWidth / MapConstants.GridWidth), 
                (MapConstants.MapHeight / MapConstants.GridHeight)
            ];
            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = Optional<Pair<Vector2, int>>.Of(null);

                }
            }

        }

        /// <summary>
        /// Updates the collision map for the given coordinates and id. If the object identified by the id is already present
        /// in the collision map the coordinates get updates, otherwise it gets added.
        /// </summary>
        /// <param name="coordinates">The coordinates of the collider to be updated</param>
        /// <param name="id">The id of the collider to be updated</param>
        public void UpdateCollider(Vector2 coordinates, int id)
        {
            //Check if the location of an already existing collider needs to be updated.
            if (mLookUpTable.ContainsKey(id))
            {
                var oldGridCoords = mLookUpTable[id];
                mCollisionMap[(int) oldGridCoords.X / MapConstants.GridWidth,
                    (int) oldGridCoords.Y / MapConstants.GridHeight] = Optional<Pair<Vector2, int>>.Of(null);
            }

            mCollisionMap[(int) coordinates.X / MapConstants.GridWidth,
                    (int) coordinates.Y / MapConstants.GridHeight] =
                Optional<Pair<Vector2, int>>.Of(new Pair<Vector2, int>(coordinates, id));

            mLookUpTable[id] = coordinates;
        }

        //TODO: this method exists solely for debugging purposes, so the map can draw a representation of the current collision map.
        public Optional<Pair<Vector2, int>>[,] GetCollisionMap()
        {
            return mCollisionMap;
        }
    }
}
