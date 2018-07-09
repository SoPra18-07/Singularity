using System.Collections.Generic;
using System.Runtime.Serialization;
using EpPathFinding.cs;
using Microsoft.Xna.Framework;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Map
{
    /// <summary>
    /// The collision map is used to store all the colliding objects in a grid like fashion.
    /// </summary>
    [DataContract]
    public sealed class CollisionMap
    {
        /// <summary>
        /// The look up table is used to check whether a given collider is already present in the collision map
        /// </summary>
        [DataMember]
        private readonly Dictionary<int, Rectangle> mLookUpTable;

        /// <summary>
        /// The collision map is used to store the position and the id of every object which is able to collide.
        /// </summary>
        [DataMember]
        private CollisionNode[,] mCollisionMap;

        /// <summary>
        /// Stores the walkability information of the map to be used by the pathfinder
        /// </summary>
        private  BaseGrid mWalkableGrid;

        /// <summary>
        /// Creates a new Collision map used to store and update all colliding objects.
        /// </summary>
        public CollisionMap()
        {
            mLookUpTable = new Dictionary<int, Rectangle>();

            var gridXLength = MapConstants.MapWidth / MapConstants.GridWidth;
            var gridYLength = MapConstants.MapHeight / MapConstants.GridHeight;

            mCollisionMap = new CollisionNode
            [
                gridXLength,
                gridYLength
            ];

            // movableMatrix is used to construct a StaticGrid object, which is used by the pathfinder.
            var movableMatrix = new bool[gridXLength][];

            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                movableMatrix[i] = new bool[gridYLength];

                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = new CollisionNode(i, j, Optional<ICollider>.Of(null));
                    movableMatrix[i][j] = Map.IsOnTop(new Vector2(i * MapConstants.GridWidth, j * MapConstants.GridHeight));
                }
            }
            mWalkableGrid = new StaticGrid(gridXLength, gridYLength, movableMatrix);

        }

        public void ReloadContent()
        {
            var gridXLength = MapConstants.MapWidth / MapConstants.GridWidth;
            var gridYLength = MapConstants.MapHeight / MapConstants.GridHeight;

            mCollisionMap = new CollisionNode
            [
                gridXLength,
                gridYLength
            ];

            // movableMatrix is used to construct a StaticGrid object, which is used by the pathfinder.
            var movableMatrix = new bool[gridXLength][];

            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                movableMatrix[i] = new bool[gridYLength];

                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = new CollisionNode(i, j, Optional<ICollider>.Of(null));
                    movableMatrix[i][j] = Map.IsOnTop(new Vector2(i * MapConstants.GridWidth, j * MapConstants.GridHeight));
                }
            }
            mWalkableGrid = new StaticGrid(gridXLength, gridYLength, movableMatrix);

        }

        /// <summary>
        /// Updates the collision map for the given coordinates and id. If the object identified by the id is already present
        /// in the collision map the coordinates get updated, otherwise it gets added.
        /// </summary>
        /// <param name="collider">The collider to be updated.</param>
        internal void UpdateCollider(ICollider collider)
        {
            //Check if the location of an already existing collider needs to be updated.
            if (mLookUpTable.ContainsKey(collider.Id) && collider.Moved)
            {
                var oldBounds = mLookUpTable[collider.Id];

                for (var x = oldBounds.X / MapConstants.GridWidth; x <= (oldBounds.X + oldBounds.Width) / MapConstants.GridWidth; x++)
                {
                    for (var y = oldBounds.Y / MapConstants.GridHeight; y <= (oldBounds.Y + oldBounds.Height) / MapConstants.GridHeight; y++)
                    {
                        mCollisionMap[x, y] = new CollisionNode(x, y, Optional<ICollider>.Of(null));
                        mWalkableGrid.SetWalkableAt(x, y, true);
                    }
                }
            }

            if (collider.ColliderGrid == null)
            {
                return;
            }
            //add the given collider to the collision map.
            for (var i = 0; i < collider.ColliderGrid.GetLength(1); i++)
            {
                for (var j = 0; j < collider.ColliderGrid.GetLength(0); j++)
                {
                    if (!collider.ColliderGrid[j, i])
                    {
                        continue;
                    }

                    var x = (int) (collider.AbsolutePosition.X / MapConstants.GridWidth) + i;
                    var y = (int) (collider.AbsolutePosition.Y / MapConstants.GridHeight) + j;
                    mCollisionMap[x, y] = new CollisionNode(x, y, Optional<ICollider>.Of(collider));
                    Optional<ICollider>.Of(collider);
                    mWalkableGrid.SetWalkableAt(x, y, false);
                }
            }

            mLookUpTable[collider.Id] = collider.AbsBounds;
        }

        //TODO: this method exists solely for debugging purposes, so the map can draw a representation of the current collision map.
        public CollisionNode[,] GetCollisionMap()
        {
            return mCollisionMap;
        }

        public BaseGrid GetWalkabilityGrid()
        {
            return mWalkableGrid;
        }
    }
}
