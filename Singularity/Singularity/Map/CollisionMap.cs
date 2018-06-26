﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    internal sealed class CollisionMap
    {
        // Number of cells in the grid
        private readonly int mGridXLength;
        private readonly int mGridYLength;
        /// <summary>
        /// The look up table is used to check whether a given collider is already present in the collision map
        /// </summary>
        private readonly Dictionary<int, Rectangle> mLookUpTable;

        /// <summary>
        /// The collision map is used to store the position and the id of every object which is able to collide.
        /// </summary>
        private readonly CollisionNode[,] mCollisionMap;

        /// <summary>
        /// Stores the walkability information of the map to be used by the pathfinder
        /// </summary>
        private readonly BaseGrid mWalkableGrid;

        /// <summary>
        /// Creates a new Collision map used to store and update all colliding objects.
        /// </summary>
        public CollisionMap()
        {
            mLookUpTable = new Dictionary<int, Rectangle>();

            mGridXLength = MapConstants.MapWidth / MapConstants.GridWidth;
            mGridYLength = MapConstants.MapHeight / MapConstants.GridHeight;

            mCollisionMap = new CollisionNode
            [
                mGridXLength,
                mGridYLength
            ];
            bool[][] movableMatrix = new bool[mGridXLength][];




            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                movableMatrix[i] = new bool[mGridYLength];

                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = new CollisionNode(i, j, Optional<ICollider>.Of(null));
                    movableMatrix[i][j] = Map.IsOnTop(new Vector2(i * MapConstants.GridWidth, j * MapConstants.GridHeight));
                }
            }
            mWalkableGrid = new StaticGrid(mGridXLength, mGridYLength, movableMatrix);

        }

        /// <summary>
        /// Updates the collision map for the given coordinates and id. If the object identified by the id is already present
        /// in the collision map the coordinates get updated, otherwise it gets added.
        /// </summary>
        /// <param name="collider">The collider to be updated updated</param>
        public void UpdateCollider(ICollider collider)
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

            //add the given collider to the collision map.

            for (var x = collider.AbsBounds.X / MapConstants.GridWidth; x <= (collider.AbsBounds.X + collider.AbsBounds.Width) / MapConstants.GridWidth; x++)
            {
                for (var y = (collider.AbsBounds.Y / MapConstants.GridHeight); y <= ((collider.AbsBounds.Y + collider.AbsBounds.Height) / MapConstants.GridHeight) ; y++)
                {
                    mCollisionMap[x, y] = new CollisionNode(x, y, Optional<ICollider>.Of(collider));Optional<ICollider>.Of(collider);
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
