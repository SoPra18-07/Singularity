using System.Collections.Generic;
using System.Diagnostics;
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
        private CollisionNode[,] mCollisionMap;

        /// <summary>
        /// Stores the walkability information of the map to be used by the pathfinder
        /// </summary>
        private  BaseGrid mWalkableGrid;

        private int mCounter;

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

            if (mLookUpTable.ContainsKey(collider.Id) && collider.Moved)
            {
                RemoveCollider(collider);
            }

            mLookUpTable[collider.Id] = collider.AbsBounds;

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

                    var x = collider.AbsBounds.X / MapConstants.GridWidth + i;
                    var y = collider.AbsBounds.Y / MapConstants.GridHeight + j;
                    mCollisionMap[x, y] = new CollisionNode(x, y, Optional<ICollider>.Of(collider));
                    mWalkableGrid.SetWalkableAt(x, y, false);
                }
            }
        }

        public CollisionNode[,] GetCollisionMap()
        {
            return mCollisionMap;
        }

        public BaseGrid GetWalkabilityGrid()
        {
            return mWalkableGrid;
        }

        public void RemoveCollider(ICollider toRemove)
        {
            if (toRemove.ColliderGrid == null)
            {
                return;
            }

            mCounter++;

            var oldBounds = mLookUpTable[toRemove.Id];

            for (var i = 0; i < toRemove.ColliderGrid.GetLength(1); i++)
            {
                for (var j = 0; j < toRemove.ColliderGrid.GetLength(0); j++)
                {
                    if (!toRemove.ColliderGrid[j, i])
                    {
                        continue;
                    }

                    var x = oldBounds.X / MapConstants.GridWidth + i;
                    var y = oldBounds.Y / MapConstants.GridHeight + j;
                    mCollisionMap[x, y] = new CollisionNode(x, y, Optional<ICollider>.Of(null));
                    mWalkableGrid.SetWalkableAt(x, y, true);
                }
            }

            // seems like a reasonable number. Grid Cleaning works.
            if (mCounter > 100 * mLookUpTable.Count)
            {
                CleanGrid();
                mCounter = 0;
            }
        }


        private void CleanGrid()
        {

            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    if (mCollisionMap[i, j].Collider.IsPresent())
                    {
                        if ((mCollisionMap[i, j].Collider.Get().Center / new Vector2(MapConstants.GridWidth, MapConstants.GridHeight)).Length() > 2)
                        {
                            mCollisionMap[i, j] = new CollisionNode(i, j, Optional<ICollider>.Of(null));
                            mWalkableGrid.SetWalkableAt(i, j, true);
                        }
                    }
                }
            }

        }


        public bool CanPlaceCollider(ICollider tester)
        {

            var xConst = tester.AbsBounds.X / MapConstants.GridWidth;
            var yConst = tester.AbsBounds.Y / MapConstants.GridHeight;

            //add the given collider to the collision map.
            for (var i = 0; i < tester.ColliderGrid.GetLength(1); i++)
            {
                for (var j = 0; j < tester.ColliderGrid.GetLength(0); j++)
                {
                    if (!tester.ColliderGrid[j, i])
                    {
                        continue;
                    }

                    var x = xConst + i;
                    var y = yConst + j;
                    if (!mWalkableGrid.IsWalkableAt(x, y) && mCollisionMap[x, y].Collider.IsPresent() && !Equals(mCollisionMap[x, y].Collider.Get(), tester))
                    {
                        return false;
                    }
                }
            }
            return true;

        }
    }
}
