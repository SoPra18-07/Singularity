using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="ICollider"/>
    /// <inheritdoc cref="IRevealing"/>
    /// <inheritdoc cref="AFlocking"/>
    [DataContract]
    public abstract class FreeMovingUnit : AFlocking, IRevealing, IMouseClickListener, IMousePositionListener
    {

        /// <summary>
        /// The state of the unit in terms of living or dead. False when alive.
        /// </summary>
        [DataMember]
        public bool HasDieded { get; private set; }

        /// <summary>
        /// The time of death of the unit, used to calculate when to fade away.
        /// </summary>
        [DataMember]
        protected double mTimeOfDeath;

        [DataMember]
        protected readonly HealthBar mHealthBar;

        [DataMember]
        public bool KillMe { get; protected set; }
        #region Movement Variables

        /// <summary>
        /// Target position that the unit wants to reach.
        /// </summary>
        // [DataMember]
        // protected Vector2 mTargetPosition;

        /// <summary>
        /// Indicates if the unit is currently moving towards a target.
        /// </summary>
        // [DataMember]
        // protected bool mIsMoving;



        /// <summary>
        /// Normalized vector to indicate direction of movement.
        /// </summary>
        // [DataMember]
        // protected Vector2 mMovementVector;

        #endregion

        #region Director/map/camera/library/fow Variables


        /// <summary>
        /// Stores a reference to the game map.
        /// </summary>
        // protected Map.Map mMap;

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
        // [DataMember]
        // protected double mZoomSnapshot;

            //   the following are already in AFlocking.
        // [DataMember]
        // public Rectangle AbsBounds { get; protected set; }
        // TODO: Make clear whether we need to reload that
        // [DataMember]
        // public bool[,] ColliderGrid { get; protected set; }
        [DataMember]
        public int RevelationRadius { get; protected set; }
        // [DataMember]
        // public Vector2 RelativePosition { get; set; }
        // [DataMember]
        // public Vector2 RelativeSize { get; set; }
        [DataMember]
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        #endregion

        #region Positioning Variables

        /// <summary>
        /// Stores the center of a unit's position.
        /// </summary>
        /// already
        // [DataMember]
        // public override Vector2 Center { get; protected set; }





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
        // [DataMember]
        // internal bool mSelected;

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
        /// <param name="friendly">The allegiance of the unit. True if the unit is player controlled.</param>
        /// <remarks>
        /// FreeMovingUnit is an abstract class that can be implemented to allow free movement outside
        /// of the graphs that represent bases in the game. It provides implementations to allow for
        /// the camera to understand movement and zoom on the object, holds references of the director and
        /// map, and implements pathfinding for objects on the map. It also allows subclasses to have
        /// health and to be damaged.
        /// </remarks>
        protected FreeMovingUnit(Vector2 position, Camera camera, ref Director director, bool friendly = true) : base(ref director, Optional<FlockingGroup>.Of(null))
        {

            AbsolutePosition = position;

            Moved = false;

            mDirector = director;
            mCamera = camera;

            Friendly = friendly;

            mHealthBar = new HealthBar(this);

            if (friendly)
            {
                mDirector.GetInputManager.FlagForAddition(this, EClickType.Both, EClickType.Both);
                mDirector.GetInputManager.AddMousePositionListener(this);
            }

            mGroup = Optional<FlockingGroup>.Of(null);

            /* too inefficient
            ColliderGrid = new[,]
                {
                    { true, true, true },
                    { true, true, true }
                };
             */
        }

        protected void ReloadContent(ref Director director, Camera camera)
        {
            base.ReloadContent(ref director);
            mCamera = camera;
            mGroup = Optional<FlockingGroup>.Of(null);
            if (Friendly)
            {
                mDirector.GetInputManager.FlagForAddition(this, EClickType.Both, EClickType.Both);
                mDirector.GetInputManager.AddMousePositionListener(this);
            }
            mGroup = Optional<FlockingGroup>.Of(null);
        }

        public override void Move()
        {
            Rotate(Vector2.Normalize(Velocity) * 180 + AbsolutePosition, true);
            base.Move();
        }

        /// <summary>
        /// Rotates unit when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target">Target rotation position</param>
        /// <param name="absolute">Whether the target position is absolute or relative. True for absolute.</param>
        protected void Rotate(Vector2 target, bool absolute = false)
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

        #region Overridden Methods

        public override void Update(GameTime gametime)
        {

            // The unit might have gotten moved. base.Update() would only call Move(), but it's the job of the FlockingGroup to do that.
            // base.Update(gametime);


            // ============ now update all the values, since the position changed ===========

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle((int)RelativePosition.X,
                (int)RelativePosition.Y,
                (int)RelativeSize.X,
                (int)RelativeSize.Y);

            SetAbsBounds();
        }

        public override void SetAbsBounds()
        {
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16,
                (int)AbsolutePosition.Y + 11,
                (int)AbsoluteSize.X,
                (int)AbsoluteSize.Y);
        }

        #endregion

        #region Health, damage methods

        /// <summary>
        /// Defines the health of the unit, defaults to 10.
        /// </summary>
        // [DataMember]
        // public int Health { get; set; }

        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public override void MakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0 && !HasDieded)
            {
                FlagForDeath();
            }
        }

        public override bool Die()
        {
            HasDieded = true;
            // stats tracking for the death of any free moving unit
            mDirector.GetStoryManager.UpdateUnits(Friendly ? "lost" : "killed");

            mDirector.GetInputManager.FlagForRemoval(this);
            mDirector.GetInputManager.RemoveMousePositionListener(this);
            mDirector.GetStoryManager.Level.GameScreen.RemoveObject(this);
            mDirector.GetMilitaryManager.RemoveUnit(this);
            Moved = false;
            if (!Friendly)
            {
                // note that this has to be an enemy unit, otherwise it wouldn't be friendly.
                mDirector.GetStoryManager.Level.Ai.Kill((EnemyUnit)this);
            }
            mDirector.GetEventLog.AddEvent(ELogEventType.UnitAttacked, (Friendly ? "A friendly" : "An enemy") + " unit was killed!", this);

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
                    if (Selected
                        // && !Moved // now this should do pathfinding even while moving
                        && !withinBounds
                        && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f),
                                (int)(mMouseY - RelativeSize.Y / 2f),
                                (int)RelativeSize.X,
                                (int)RelativeSize.Y),
                            mCamera))
                    {

                        if (!mGroup.IsPresent())
                        {
                            var group = Optional<FlockingGroup>.Of(mDirector.GetMilitaryManager.GetNewFlock());
                            group.Get().AssignUnit(this);
                            mGroup = group;
                            // do the fuck not change these lines here. Costs you at least 3h for debugging.
                        }

                        var target = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                            Matrix.Invert(mCamera.GetTransform()));

                        if (mGroup.Get().Map.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(
                            (int)target.X / MapConstants.GridWidth,
                            (int)target.Y / MapConstants.GridHeight))
                        {
                            mGroup.Get().FindPath(target);
                        }
                    }


                    if (withinBounds)
                    {
                        Selected = true;
                        giveThrough = false;

                        if (!mGroup.IsPresent())
                        {
                            var group = Optional<FlockingGroup>.Of(mDirector.GetMilitaryManager.GetNewFlock());
                            group.Get().AssignUnit(this);
                            mGroup = group;
                            // do the fuck not change these lines here. Costs you at least 3h for debugging.
                        }
                        else if (!mGroup.Get().AllSelected())
                        {
                            var group = Optional<FlockingGroup>.Of(mDirector.GetMilitaryManager.GetNewFlock());
                            group.Get().AssignUnit(this);
                            mGroup = group;
                        }
                    }

                    break;

                case EMouseAction.RightClick:
                    Selected = false;
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
            if (!Friendly)
            {
                return;
            }

            // create a rectangle from given parameters
            Rectangle selBox = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            // check if selection box intersects with MUnit bounds
            if (selBox.Intersects(AbsBounds))
            {
                Selected = true;
                mDirector.GetMilitaryManager.AddSelected(this); // send to FlockingManager
            }
        }
        #endregion
    }
}
