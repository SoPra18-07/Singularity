using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Screen;
using EventLog = Singularity.Screen.EventLog;

namespace Singularity.Units
{
    /// <inheritdoc cref="ICollider"/>
    /// <inheritdoc cref="IRevealing"/>
    [DataContract]
    internal abstract class FreeMovingUnit : ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public bool Friendly { get; protected set; }

        /// <summary>
        /// The state of the unit in terms of living or dead. False when alive.
        /// </summary>
        [DataMember]
        protected bool mDead;

        /// <summary>
        /// The time of death of the unit, used to calculate when to fade away.
        /// </summary>
        [DataMember]
        protected double mTimeOfDeath;

        /// <summary>
        /// The current time used for moving time information between methods not called by update.
        /// </summary>
        [DataMember]
        protected double mCurrentTime;

        [DataMember]
        public bool KillMe { get; protected set; }
        #region Movement Variables

        /// <summary>
        /// Indicates the vector that needs to be added to the current vector to indicate the movement
        /// direction.
        /// </summary>
        [DataMember]
        protected Vector2 mToAdd;

        /// <summary>
        /// Target position that the unit wants to reach.
        /// </summary>
        [DataMember]
        protected Vector2 mTargetPosition;

        /// <summary>
        /// Indicates if the unit is currently moving towards a target.
        /// </summary>
        [DataMember]
        protected bool mIsMoving;

        /// <summary>
        /// Path the unit must take to get to the target position without colliding with obstacles.
        /// </summary>
        [DataMember]
        protected Stack<Vector2> mPath;

        /// <summary>
        /// Stores the path the unit is taking so that it can be drawn for debugging.
        /// </summary>
        [DataMember]
        protected Vector2[] mDebugPath;

        /// <summary>
        /// Provides a snapshot of the current bounds of the unit at every update call.
        /// </summary>
        [DataMember]
        protected Rectangle mBoundsSnapshot;

        /// <summary>
        /// Normalized vector to indicate direction of movement.
        /// </summary>
        [DataMember]
        protected Vector2 mMovementVector;
        [DataMember]
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
        protected FreeMovingPathfinder mPathfinder;

        /// <summary>
        /// Stores a reference to the game map.
        /// </summary>
        protected Map.Map mMap;

        /// <summary>
        /// Stores the game camera.
        /// </summary>
        protected Camera mCamera;

        /// <summary>
        /// Provides IMouseClickListener its bounds to know when it is clicked
        /// </summary>
        [DataMember]
        public Rectangle Bounds { get; protected set; }

        /// <summary>
        /// Used by the camera to figure out where it is.
        /// </summary>
        [DataMember]
        protected double mZoomSnapshot;
        [DataMember]
        public Rectangle AbsBounds { get; protected set; }
        //TODO: Make clear whether we need to reload that
        public bool[,] ColliderGrid { get; protected set; }
        [DataMember]
        public int RevelationRadius { get; protected set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        #endregion

        #region Positioning Variables

        /// <summary>
        /// Indicates if a unit has moved between updates.
        /// </summary>
        [DataMember]
        public bool Moved { get; protected set; }

        /// <summary>
        /// Stores the center of a unit's position.
        /// </summary>
        [DataMember]
        public Vector2 Center { get; protected set; }
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }

        /// <summary>
        /// Value of the unit's rotation.
        /// </summary>
        [DataMember]
        protected int mRotation;

        /// <summary>
        /// Column of the spritesheet to be used in case the unit is a military unit.
        /// </summary>
        [DataMember]
        protected int mColumn;

        /// <summary>
        /// Row of the spritesheet to be used in case the unit is a military unit.
        /// </summary>
        [DataMember]
        protected int mRow;


        #endregion

        #region Mouse Listener Fields

        /// <summary>
        /// Indicates if the unit is currently selected.
        /// </summary>
        [DataMember]
        internal bool mSelected;

        /// <summary>
        /// Stores the current x position of the mouse
        /// </summary>
        [DataMember]
        internal float mMouseX;

        /// <summary>
        /// Stores the current y position of the mouse
        /// </summary>
        [DataMember]
        internal float mMouseY;

        #endregion

        /// <summary>
        /// Base abstract class for units that are not restricted to the graph.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        /// <param name="friendly">The allegiance of the unit. True if the unit is player controlled.</param>
        /// <remarks>
        /// FreeMovingUnit is an abstract class that can be implemented to allow free movement outside
        /// of the graphs that represent bases in the game. It provides implementations to allow for
        /// the camera to understand movement and zoom on the object, holds references of the director and
        /// map, and implements pathfinding for objects on the map. It also allows subclasses to have
        /// health and to be damaged.
        /// </remarks>
        protected FreeMovingUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map, bool friendly = true)
        {
            Id = director.GetIdGenerator.NextiD(); // id for the specific unit.

            AbsolutePosition = position;
            mMap = map;

            Moved = false;
            mIsMoving = false;

            mDirector = director;
            mCamera = camera;
            mPathfinder = new FreeMovingPathfinder();

            Friendly = friendly;

            if (friendly)
            {
                mDirector.GetInputManager.FlagForAddition(this, EClickType.Both, EClickType.Both);
                mDirector.GetInputManager.AddMousePositionListener(this);
            }
        }

        protected void ReloadContent(ref Director director, Camera camera, ref Map.Map map)
        {
            mPathfinder = new FreeMovingPathfinder();
            mDirector = director;
            mCamera = camera;
            mMap = map;
        }

        #region Pathfinding Methods

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move.</param>
        /// <param name="speed">Speed to go towards the target at.</param>
        protected void MoveToTarget(Vector2 target, float speed)
        {
            var movementVector = new Vector2(target.X - Center.X, target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float)(mZoomSnapshot * speed);

            Rotate(target, true);

            AbsolutePosition = new Vector2(AbsolutePosition.X + movementVector.X * speed, AbsolutePosition.Y + movementVector.Y * speed);
        }

        protected void FindPath(Vector2 currentPosition, Vector2 targetPosition)
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
                mDebugPath = mPath.ToArray();
            }

            mBoundsSnapshot = Bounds;
            mZoomSnapshot = mCamera.GetZoom();
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        protected bool HasReachedTarget()
        {

            if (!(Math.Abs(Center.X + mToAdd.X -
                           mTargetPosition.X) < 8 &&
                  Math.Abs(Center.Y + mToAdd.Y -
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
            //TODO: This is a hotfix for mPath being empty but peek being called. I dont know if this could cause additional errors.
            if (mPath.Count == 0)
            {
                return true;
            }
            if (Math.Abs(Center.X + mToAdd.X - mPath.Peek().X) < 8
                && Math.Abs(Center.Y + mToAdd.Y - mPath.Peek().Y) < 8)
            {
                // If the position is within 8 pixels of the waypoint, (i.e. it will overshoot the waypoint if it moves
                // for one more update, do the following

                Debug.WriteLine("Waypoint reached.");
                Debug.WriteLine("Next waypoint: " + mPath.Peek());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates unit when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target">Target rotation position</param>
        /// <param name="absolute">Whether the target position is absolute or relative. True for absolute.</param>
        protected void Rotate(Vector2 target, bool absolute=false)
        {
            float x;
            float y;
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite
            if (absolute)
            {
                x = target.X - (AbsolutePosition.X + AbsoluteSize.X / 2);
                y = target.Y - (AbsolutePosition.Y + AbsoluteSize.Y / 2);
            }
            else
            {
                x = target.X - (RelativePosition.X + RelativeSize.X / 2);
                y = target.Y - (RelativePosition.Y + RelativeSize.Y / 2);
            }
            
            
            var hypot = Math.Sqrt(x * x + y * y);

            // calculate degree between formed triangle
            double degree;
            if (Math.Abs(hypot) < 0.01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(y / hypot) * (180.0 / Math.PI);
            }

            // calculate rotation with increased degrees going counterclockwise
            if (x >= 0)
            {
                mRotation = (int)Math.Round(270 - degree, MidpointRounding.AwayFromZero);
            }
            else
            {
                mRotation = (int)Math.Round(90 + degree, MidpointRounding.AwayFromZero);
            }

            // add 42 degrees since sprite sheet starts at sprite -42deg not 0
            mRotation = (mRotation + 42) % 360;
        }

        internal void SetMovementTarget(Vector2 target)
        {
            mTargetPosition = target;

            if (mMap.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                (int)mTargetPosition.X / MapConstants.GridWidth,
                (int)mTargetPosition.Y / MapConstants.GridWidth))
            {
                FindPath(Center, mTargetPosition);
            }
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
        [DataMember]
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
            Console.Out.Write("I died and my friendly value is: " + Friendly);
            mDead = true;
            mDirector.GetEventLog.AddEvent(ELogEventType.UnitAttacked, Friendly ? "A Friendly" : "An enemy" + " unit was killed!", this);
            return true;
        }

        #endregion
        
        #region Mouse Handlers
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    // check for if the unit is selected, not moving, the click is not within the bounds of the unit, and the click was on the map.
                    if (mSelected
                        && !mIsMoving
                        && !withinBounds
                        && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f),
                                (int)(mMouseY - RelativeSize.Y / 2f),
                                (int)RelativeSize.X,
                                (int)RelativeSize.Y),
                            mCamera))
                    {
                        SetMovementTarget(Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                            Matrix.Invert(mCamera.GetTransform())));
                    }


                    if (withinBounds)
                    {
                        mSelected = true;
                        giveThrough = false;
                    }

                    break;

                case EMouseAction.RightClick:
                    mSelected = false;
                    break;
            }

            return giveThrough;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            mMouseX = screenX;
            mMouseY = screenY;
        }

        /// <summary>
        /// This is called up every time a selection box is created
        /// if MUnit bounds intersects with the selection box then it become selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="position"> top left corner of the selection box</param>
        /// <param name="size"> size of selection box</param>
        public void BoxSelected(object sender, EventArgs e, Vector2 position, Vector2 size)
        {
            // create a rectangle from given parameters
            Rectangle selBox = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            // check if selection box intersects with MUnit bounds
            if (selBox.Intersects(AbsBounds))
            {
                mSelected = true;
            }
        }

        #endregion
    }
}
