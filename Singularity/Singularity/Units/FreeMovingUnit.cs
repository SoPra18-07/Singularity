using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="ICollider"/>
    /// <inheritdoc cref="IRevealing"/>
    internal abstract class FreeMovingUnit : ICollider, IRevealing
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
        protected Vector2 mToAdd;

        /// <summary>
        /// Target position that the unit wants to reach.
        /// </summary>
        protected Vector2 mTargetPosition;

        /// <summary>
        /// Indicates if the unit is currently moving towards a target.
        /// </summary>
        protected bool mIsMoving;

        /// <summary>
        /// Path the unit must take to get to the target position without colliding with obstacles.
        /// </summary>
        protected Stack<Vector2> mPath;

        /// <summary>
        /// Stores the path the unit is taking so that it can be drawn for debugging.
        /// </summary>
        protected Vector2[] mDebugPath;

        /// <summary>
        /// Provides a snapshot of the current bounds of the unit at every update call.
        /// </summary>
        protected Rectangle mBoundsSnapshot;

        /// <summary>
        /// Normalized vector to indicate direction of movement.
        /// </summary>
        protected Vector2 mMovementVector;

        protected float mSpeed;

        #endregion

        #region Director/map/camera/library/fow Variables

        /// <summary>
        /// Stores a reference to the game director.
        /// </summary>
        protected Director mDirector;

        /// <summary>
        /// MilitaryPathfinders enables pathfinding using jump point search or line of sight.
        /// </summary>
        protected readonly MilitaryPathfinder mPathfinder;

        /// <summary>
        /// Stores a reference to the game map.
        /// </summary>
        protected Map.Map mMap;

        /// <summary>
        /// Stores the game camera.
        /// </summary>
        protected readonly Camera mCamera;

        /// <summary>
        /// Provides IMouseClickListener its bounds to know when it is clicked
        /// </summary>
        public Rectangle Bounds { get; protected set; }

        /// <summary>
        /// Used by the camera to figure out where it is.
        /// </summary>
        protected double mZoomSnapshot;

        public Rectangle AbsBounds { get; protected set; }

        public bool[,] ColliderGrid { get; protected set; }

        public int RevelationRadius { get; protected set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public EScreen Screen { get; } = EScreen.GameScreen;

        #endregion

        #region Positioning Variables

        /// <summary>
        /// Indicates if a unit has moved between updates.
        /// </summary>
        public bool Moved { get; protected set; }

        /// <summary>
        /// Stores the center of a unit's position.
        /// </summary>
        public Vector2 Center { get; protected set; }

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        /// <summary>
        /// Value of the unit's rotation.
        /// </summary>
        protected int mRotation;

        /// <summary>
        /// Column of the spritesheet to be used in case the unit is a military unit.
        /// </summary>
        protected int mColumn;

        /// <summary>
        /// Row of the spritesheet to be used in case the unit is a military unit.
        /// </summary>
        protected int mRow;


        #endregion

        /// <summary>
        /// Base abstract class for units that are not restricted to the graph.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        /// FreeMovingUnit is an abstract class that can be implemented to allow free movement outside
        /// of the graphs that represent bases in the game. It provides implementations to allow for
        /// the camera to understand movement and zoom on the object, holds references of the director and
        /// map, and implements pathfinding for objects on the map. It also allows subclasses to have
        /// health and to be damaged.
        protected FreeMovingUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.

            AbsolutePosition = position;
            mMap = map;

            Moved = false;
            mIsMoving = false;

            mDirector = director;
            mCamera = camera;
            mPathfinder = new MilitaryPathfinder();

            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X / 2, y: AbsolutePosition.Y + AbsoluteSize.Y / 2);
        }

        #region Pathfinding Methods

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move.</param>
        /// <param name="speed">Speed to go towards the target at.</param>
        protected void MoveToTarget(Vector2 target, float speed)
        {

            var movementVector = new Vector2(x: target.X - Center.X, y: target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float)(mZoomSnapshot * speed);

            AbsolutePosition = new Vector2(x: AbsolutePosition.X + movementVector.X * speed, y: AbsolutePosition.Y + movementVector.Y * speed);
        }

        protected void FindPath(Vector2 currentPosition, Vector2 targetPosition)
        {

            mIsMoving = true;
            Debug.WriteLine(message: "Starting path finding at: " + currentPosition.X + ", " + currentPosition.Y);
            Debug.WriteLine(message: "Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);

            mPath = new Stack<Vector2>();
            mPath = mPathfinder.FindPath(startPosition: currentPosition,
                endPosition: mTargetPosition,
                map: ref mMap);

            if (GlobalVariables.DebugState)
            {
                // TODO: DEBUG REGION
                mDebugPath = mPath.ToArray();
                // TODO: END DEBUG REGION
            }

            mBoundsSnapshot = Bounds;
            mZoomSnapshot = mCamera.GetZoom();
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        protected bool HasReachedTarget()
        {

            if (!(Math.Abs(value: Center.X + mToAdd.X -
                           mTargetPosition.X) < 8 &&
                  Math.Abs(value: Center.Y + mToAdd.Y -
                           mTargetPosition.Y) < 8))
            {
                return false;
            }
            mToAdd = Vector2.Zero;
            return true;
        }

        /// <summary>
        /// Checks whether the next waypoint has been reached.
        /// </summary>
        /// <returns></returns>
        protected bool HasReachedWaypoint()
        {
            if (Math.Abs(value: Center.X + mToAdd.X - mPath.Peek().X) < 8
                && Math.Abs(value: Center.Y + mToAdd.Y - mPath.Peek().Y) < 8)
            {
                // If the position is within 8 pixels of the waypoint, (i.e. it will overshoot the waypoint if it moves
                // for one more update, do the following

                Debug.WriteLine(message: "Waypoint reached.");
                Debug.WriteLine(message: "Next waypoint: " + mPath.Peek());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target"></param>
        protected void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite
            var x = target.X - (RelativePosition.X + RelativeSize.X / 2);
            var y = target.Y - (RelativePosition.Y + RelativeSize.Y / 2);
            var hypot = Math.Sqrt(d: Math.Pow(x: x, y: 2) + Math.Pow(x: y, y: 2));

            // calculate degree between formed triangle
            double degree;
            if (Math.Abs(value: hypot) < 0.01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(d: y / hypot) * (180.0 / Math.PI);
            }

            // calculate rotation with increased degrees going counterclockwise
            if (x >= 0)
            {
                mRotation = (int)Math.Round(value: 270 - degree, mode: MidpointRounding.AwayFromZero);
            }
            else
            {
                mRotation = (int)Math.Round(value: 90 + degree, mode: MidpointRounding.AwayFromZero);
            }

            // add 42 degrees since sprite sheet starts at sprite -42deg not 0
            mRotation = (mRotation + 42) % 360;
        }

        #endregion

        #region Abstract Methods

        public abstract void Update(GameTime gametime);

        public abstract void Draw(SpriteBatch spriteBatch);

        #endregion

        #region Health, damage methods

        /// <summary>
        /// Defines the health of the unit, defaults to 10.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool Die()
        {
            // mDirector.GetMilitaryManager.Kill(this);
            // todo: MilitaryManager implement

            return true;
        }

        #endregion

        /// <summary>
        /// Used to find the coordinates of the given Vector2 based on the overall map
        /// instead of just the camera shot, returns Vector2 of absolute position
        /// </summary>
        /// <returns></returns>
        protected Vector2 MapCoordinates(Vector2 v)
        {
            return new Vector2(x: Vector2.Transform(position: new Vector2(x: v.X, y: v.Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).X, y: Vector2.Transform(position: new Vector2(x: v.X, y: v.Y),
                matrix: Matrix.Invert(matrix: mCamera.GetTransform())).Y);
        }
    }
}
