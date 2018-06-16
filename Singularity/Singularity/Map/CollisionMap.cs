using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Node[,] mCollisionMap;

        /// <summary>
        /// Creates a new Collision map used to store and update all colliding objects.
        /// </summary>
        public CollisionMap()
        {
            mLookUpTable = new Dictionary<int, Rectangle>();


            mCollisionMap = new Node
            [
                (MapConstants.MapWidth / MapConstants.GridWidth), 
                (MapConstants.MapHeight / MapConstants.GridHeight)
            ];
            for (var i = 0; i < mCollisionMap.GetLength(0); i++)
            {
                for (var j = 0; j < mCollisionMap.GetLength(1); j++)
                {
                    mCollisionMap[i, j] = new Node(i, j, Optional<ICollider>.Of(null));

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

                        mCollisionMap[x, y] = new Node(x, y, Optional<ICollider>.Of(null));
                    }
                }
            }

            //add the given collider to the collision map.

            for (var x = collider.AbsBounds.X / MapConstants.GridWidth; x <= (collider.AbsBounds.X + collider.AbsBounds.Width) / MapConstants.GridWidth; x++)
            {
                for (var y = (collider.AbsBounds.Y / MapConstants.GridHeight); y <= ((collider.AbsBounds.Y + collider.AbsBounds.Height) / MapConstants.GridHeight) ; y++)
                {
                    mCollisionMap[x, y] = new Node(x, y, Optional<ICollider>.Of(collider));Optional<ICollider>.Of(collider);
                }
            }

            mLookUpTable[collider.Id] = collider.AbsBounds;
        }

        /// <summary>
        /// Returns the contents of the node x, y. Usage: NodeAt(x, y).Get();
        /// </summary>
        /// <param name="x">x coordinate of the node</param>
        /// <param name="y">y coordinate of the node</param>
        /// <returns>An Optional object containing either the contents or none</returns>
        public Node NodeAt(int x, int y)
        {
            return mCollisionMap[x, y];
        }

        /// <summary>
        /// Returns whether a node is walkable
        /// </summary>
        /// <param name="x">x coordinate of the node</param>
        /// <param name="y">y coordinate of the node</param>
        /// <returns>True if the node is walkable</returns>
        public bool IsWalkableAt(int x, int y)
        {
            return mCollisionMap[x, y].IsWalkable();
        }

        public List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            var x = node.X;
            var y = node.Y;

            /**
             * map of neighbors is like so:
             * +----+----+----+
             * | d0 | s0 | d1 |
             * +----+----+----+
             * | s3 |    | s1 |
             * +----+----+----+
             * | d3 | s2 | d2 |
             * +----+----+----+
             *
             */
            var s0 = false;
            var s1 = false;
            var s2 = false;
            var s3 = false;

            // ↑
            if (IsWalkableAt(x, y - 1))
            {
                neighbors.Add(mCollisionMap[x, y-1]);
                s0 = true;
            }
            // →
            if (IsWalkableAt(x + 1, y))
            {
                neighbors.Add(mCollisionMap[x + 1, y]);
                s1 = true;
            }
            // ↓
            if (IsWalkableAt(x, y + 1))
            {
                neighbors.Add(mCollisionMap[x, y + 1]);
                s2 = true;
            }
            // ←
            if (IsWalkableAt(x-1, y))
            {
                neighbors.Add(mCollisionMap[x-1, y]);
                s3 = true;
            }

            var d0 = s3 || s0;
            var d1 = s0 || s1;
            var d2 = s1 || s2;
            var d3 = s2 || s3;
            
            // ↖
            if (d0 && IsWalkableAt(x - 1, y - 1))
            {
                neighbors.Add(mCollisionMap[x - 1, y - 1]);
            }

            // ↗
            if (d1 && IsWalkableAt(x + 1, y - 1))
            {
                neighbors.Add(mCollisionMap[x + 1, y - 1]);
            }

            // ↘
            if (d2 && IsWalkableAt(x + 1, y + 1))
            {
                neighbors.Add(mCollisionMap[x + 1, y + 1]);
            }

            // ↙
            if (d3 && IsWalkableAt(x - 1, y + 1))
            {
                neighbors.Add(mCollisionMap[x - 1, y + 1]);
            }

            return neighbors;
        }


        //TODO: this method exists solely for debugging purposes, so the map can draw a representation of the current collision map.
        public Node[,] GetCollisionMap()
        {
            return mCollisionMap;
        }
    }
}
