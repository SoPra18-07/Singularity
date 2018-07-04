using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="ICollider"/>
    /// <inheritdoc cref="IRevealing"/>
    class FreeMovingUnit : ICollider, IRevealing
    {
        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        public int Id { get; }

        #region Movement Variables

        /// <summary>
        /// Indicates the vector that needs to be added to the current vector to indicate the movement
        /// direction.
        /// </summary>
        internal Vector2 mToAdd;

        /// <summary>
        /// Target position that the unit wants to reach.
        /// </summary>
        internal Vector2 mTargetPosition;

        /// <summary>
        /// Indicates if the unit is currently moving towards a target.
        /// </summary>
        internal bool mIsMoving;

        /// <summary>
        /// Path the unit must take to get to the target position without colliding with obstacles.
        /// </summary>
        internal Stack<Vector2> mPath;

        /// <summary>
        /// Stores the path the unit is taking so that it can be drawn for debugging.
        /// </summary>
        internal Vector2[] mDebugPath;


        #endregion




        #region Director and map/camera/library variables

        /// <summary>
        /// Stores a reference to the game director.
        /// </summary>
        internal Director mDirector;

        /// <summary>
        /// MilitaryPathfinders enables pathfinding using jump point search or line of sight.
        /// </summary>
        internal readonly MilitaryPathfinder mPathfinder;

        /// <summary>
        /// Stores a reference to the game map.
        /// </summary>
        internal Map.Map mMap;

        /// <summary>
        /// Stores the game camera.
        /// </summary>
        private readonly Camera mCamera;

        #endregion


        /// <summary>
        /// Provides a snapshot of the current bounds of the unit at every update call.
        /// </summary>
        internal Rectangle mBoundsSnapshot;

        /// <summary>
        /// Provides IMouseClickListener its bounds to know when it is clicked
        /// </summary>
        public Rectangle Bounds { get; internal set; }

        /// <summary>
        /// Used by the camera to figure out where it is.
        /// </summary>
        internal double mZoomSnapshot;

        

        /// <summary>
        /// Indicates if a unit has moved between updates.
        /// </summary>
        public bool Moved { get; internal set; }
        
        /// <summary>
        /// Stores the center of a unit's position.
        /// </summary>
        public Vector2 Center { get; internal set; }

        /// <summary>
        /// Stores the unit's absolute position on the map, not relative to the camera.
        /// </summary>
        public Vector2 AbsolutePosition { get; set; }

        /// <summary>
        /// Stores the unit's absolute size on the map, not relative to the camera.
        /// </summary>
        public Vector2 AbsoluteSize { get; set; }

        public int RevelationRadius { get; internal set; }

        public bool[,] ColliderGrid { get; }

        public Rectangle AbsBounds { get; internal set; }



        /// <summary>
        /// Base class for units that are not stuck to the graph.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="director"></param>
        /// <param name="map"></param>
        public FreeMovingUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.

            AbsolutePosition = position;
            mMap = map;

            mCamera = camera;

            mPathfinder = new MilitaryPathfinder();
        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move</param>
        internal void MoveToTarget(Vector2 target, float speed)
        {

            var movementVector = new Vector2(target.X - Center.X, target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float)(mZoomSnapshot * speed);

            AbsolutePosition = new Vector2((float)(AbsolutePosition.X + movementVector.X * Speed), (float)(AbsolutePosition.Y + movementVector.Y * Speed));
        }

        internal void FindPath(Vector2 currentPosition, Vector2 targetPosition)
        {

            mIsMoving = true;
            Debug.WriteLine("Starting path finding at: " + currentPosition.X + ", " + currentPosition.Y);
            Debug.WriteLine("Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);

            mPath = new Stack<Vector2>();
            mPath = mPathfinder.FindPath(currentPosition,
                mTargetPosition,
                ref mMap);

            if (GlobalVariables.DebugState)
            {
                // TODO: DEBUG REGION
                mDebugPath = mPath.ToArray();
                // TODO: END DEBUG REGION
            }

            mBoundsSnapshot = Bounds;
            mZoomSnapshot = mCamera.GetZoom();

        }
    }
}
