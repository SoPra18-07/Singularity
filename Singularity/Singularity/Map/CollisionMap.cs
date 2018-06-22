using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Map.Properties;
using Singularity.Property;
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
        private readonly Dictionary<int, Rectangle> mLookUpTable;

        /// <summary>
        /// The collision map is used to store the position and the id of every object which is able to collide.
        /// </summary>
        private readonly Optional<ICollider>[,] mCollisionMap;

        /// <summary>
        /// Creates a new Collision map used to store and update all colliding objects.
        /// </summary>
        public CollisionMap()
        {
            mLookUpTable = new Dictionary<int, Rectangle>();

            mCollisionMap = new Optional<ICollider>
            [
                (MapConstants.MapWidth / MapConstants.GridWidth),
                (MapConstants.MapHeight / MapConstants.GridHeight)
            ];
            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = Optional<ICollider>.Of(null);

                }
            }

        }

        /// <summary>
        /// Updates the collision map for the given coordinates and id. If the object identified by the id is already present
        /// in the collision map the coordinates get updated, otherwise it gets added.
        /// </summary>
        /// <param name="collider">The collider to be updated updated</param>
        /// <param name="id">The id of the collider to be updated</param>
        public void UpdateCollider(ICollider collider)
        {

            //Check if the location of an already existing collider needs to be updated.
            if (mLookUpTable.ContainsKey(collider.Id) && collider.Moved)
            {
                var oldBounds = mLookUpTable[collider.Id];

                for (var x = oldBounds.X / MapConstants.GridWidth; x <= (oldBounds.X + oldBounds.Width) / MapConstants.GridWidth; x++)
                {
                    for (var y = (oldBounds.Y / MapConstants.GridHeight); y <= ((oldBounds.Y + oldBounds.Height) / MapConstants.GridHeight); y++)
                    {

                        mCollisionMap[x, y] = Optional<ICollider>.Of(null);
                    }
                }
            }

            //add the given collider to the collision map.

            for (var x = collider.AbsBounds.X / MapConstants.GridWidth; x <= (collider.AbsBounds.X + collider.AbsBounds.Width) / MapConstants.GridWidth; x++)
            {
                for (var y = (collider.AbsBounds.Y / MapConstants.GridHeight); y <= ((collider.AbsBounds.Y + collider.AbsBounds.Height) / MapConstants.GridHeight) ; y++)
                {
                    mCollisionMap[x, y] = Optional<ICollider>.Of(collider);
                }
            }

            mLookUpTable[collider.Id] = collider.AbsBounds;
        }

        //TODO: this method exists solely for debugging purposes, so the map can draw a representation of the current collision map.
        public Optional<ICollider>[,] GetCollisionMap()
        {
            return mCollisionMap;
        }
    }
}
