﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
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
    /// <inheritdoc cref="AFlocking"/>
    [DataContract]
    internal abstract class FreeMovingUnit : AFlocking, ICollider, IRevealing
    {
        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public bool Friendly { get; protected set; }

        #region Movement Variables

        /// <summary>
        /// Target position that the unit wants to reach.
        /// </summary>
        [DataMember]
        protected Vector2 mTargetPosition;

        /// <summary>
        /// Indicates if the unit is currently moving towards a target.
        /// </summary>
        // [DataMember]
        // protected bool mIsMoving;

        
        
        /// <summary>
        /// Normalized vector to indicate direction of movement.
        /// </summary>
        [DataMember]
        protected Vector2 mMovementVector;
        
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
        /// Stores the center of a unit's position.
        /// </summary>
        [DataMember]
        public Vector2 Center { get; protected set; }

        public bool Moved { get; set; }
        

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

        /// <summary>
        /// Base abstract class for units that are not restricted to the graph.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <remarks>
        /// FreeMovingUnit is an abstract class that can be implemented to allow free movement outside
        /// of the graphs that represent bases in the game. It provides implementations to allow for
        /// the camera to understand movement and zoom on the object, holds references of the director and
        /// map, and implements pathfinding for objects on the map. It also allows subclasses to have
        /// health and to be damaged.
        /// </remarks>
        protected FreeMovingUnit(Vector2 position, Camera camera, ref Director director, bool friendly = true) : base(ref director, null)
        {
            Id = director.GetIdGenerator.NextId(); // id for the specific unit.

            AbsolutePosition = position;

            Moved = false;

            mDirector = director;
            mCamera = camera;

            Friendly = friendly;
        }

        protected void ReloadContent(ref Director director, Camera camera)
        {
            base.ReloadContent(ref director);
            mCamera = camera;
        }

        #region Pathfinding Methods



        /// <summary>
        /// Rotates unit when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target"></param>
        protected void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite
            var x = target.X - (RelativePosition.X + RelativeSize.X / 2);
            var y = target.Y - (RelativePosition.Y + RelativeSize.Y / 2);
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

        #endregion

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
            // TODO: Allow implementation

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
            return new Vector2(Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(mCamera.GetTransform())).X, Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(mCamera.GetTransform())).Y);
        }
    }
}
