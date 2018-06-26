using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Units
{
    internal sealed class MilitaryUnit : ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        public EScreen Screen { get; private set; } = EScreen.GameScreen;

        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private static readonly Color sSelectedColor = Color.Gray;
        private static readonly Color sNotSelectedColor = Color.DarkGray;

        private const double Speed = 4;

        private Color mColor;

        private int mColumn;
        private int mRow;

        private bool mIsMoving;
        private Rectangle mBoundsSnapshot;
        private Vector2 mToAdd;
        private double mZoomSnapshot;

        private Vector2 mMovementVector;

        private readonly Camera mCamera;

        private Vector2 mTargetPosition;
        private int mRotation;
        private readonly Texture2D mMilSheet;

        private bool mSelected;

        private float mMouseX;

        private float mMouseY;

        private readonly float mScale;

        private readonly Map.Map mMap;

        private Stack<Vector2> mPath;

        private Director mDirector;

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 Center { get; private set; }

        public bool Moved { get; private set; }

        public int RevelationRadius { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Rectangle AbsBounds { get; private set; }

        
        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10;

            mScale = 0.4f;

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(DefaultWidth * mScale, DefaultHeight * mScale);

            RevelationRadius = (int) AbsoluteSize.X * 3;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * mScale / 2, AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);

            Moved = false;
            mIsMoving = false;
            mCamera = camera;

            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);

            mMilSheet = spriteSheet;

            mMap = map;
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination.
        /// </summary>
        /// <param name="target"></param>
        private void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite
            var x = (target.X - (RelativePosition.X + RelativeSize.X / 2));
            var y = (target.Y - (RelativePosition.Y + RelativeSize.Y / 2));
            var hypot = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

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
                mRotation = (int) (Math.Round(270 - degree, MidpointRounding.AwayFromZero));
            }
            else
            {
                mRotation = (int) (Math.Round(90 + degree, MidpointRounding.AwayFromZero));
            }

            // add 42 degrees since sprite sheet starts at sprite -42d not 0
            mRotation = (mRotation + 42) % 360;

        }

        /// <summary>
        /// Defines the health of the unit, defaults to 10.
        /// </summary>
        private int Health { get; set; }

        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Used to set and get assignment. Assignment will potentially be implemented as a new type which
        /// can be used to determine what assignment a unit has.
        /// </summary>
        public string Assignment
        {
            get; set; //TODO
        }


        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
             Health -= damage; // TODO
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(
                mMilSheet,
                AbsolutePosition,
                new Rectangle((150 * mColumn), (75 * mRow), (int) (AbsoluteSize.X / mScale), (int) (AbsoluteSize.Y / mScale)),
                mColor,
                0f,
                Vector2.Zero,
                new Vector2(mScale),
                SpriteEffects.None,
                LayerConstants.MilitaryUnitLayer
                );

        }


        public void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving)
            {
                Rotate(new Vector2(mMouseX, mMouseY));
            }

            if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            else if (mIsMoving)
            {
                if (!HasReachedWaypoint())
                {
                    MoveToTarget(mPath.Peek());
                }
                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek());
                }
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = (mRotation / 18);
            mColumn = ((mRotation - (mRow * 18)) / 3);

            //finally select the appropriate color for selected/deselected units.
            mColor = mSelected ? sSelectedColor : sNotSelectedColor;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * mScale / 2, AbsolutePosition.Y + AbsoluteSize.Y * mScale / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int) AbsolutePosition.Y + 11, (int)(AbsoluteSize.X * mScale), (int) (AbsoluteSize.Y * mScale));
            Moved = mIsMoving;
        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move</param>
        private void MoveToTarget(Vector2 target)
        {

            var movementVector = new Vector2(target.X - Center.X, target.Y - Center.Y);
            movementVector.Normalize();
            mToAdd += mMovementVector * (float) (mZoomSnapshot *  Speed);

            AbsolutePosition = new Vector2((float) (AbsolutePosition.X + movementVector.X * Speed), (float) (AbsolutePosition.Y + movementVector.Y * Speed));
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        private bool HasReachedTarget()
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

        private bool HasReachedWaypoint()
        {
            if (Math.Abs(Center.X + mToAdd.X - mPath.Peek().X) < 8
                && Math.Abs(Center.Y + mToAdd.Y - mPath.Peek().Y) < 8)
            {
                Debug.WriteLine("Waypoint reached.");
                Debug.WriteLine("Next waypoint: " +  mPath.Peek());
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            var giveThrough = true;

            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    if (mSelected && !mIsMoving && !withinBounds && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f), (int)(mMouseY - RelativeSize.Y / 2f), (int)RelativeSize.X, (int)RelativeSize.Y), mCamera))
                    {
                        mIsMoving = true;
                        mTargetPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                            Matrix.Invert(mCamera.GetTransform()));
                        var currentPosition = Center;
                        Debug.WriteLine("Starting path finding at: " + currentPosition.X +", " + currentPosition.Y);
                        Debug.WriteLine("Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);
                        mPath?.Clear();

                        mPath = MilitaryPathfinder.FindPath(currentPosition,
                            mTargetPosition,
                            mMap);

                        mBoundsSnapshot = Bounds;
                        mZoomSnapshot = mCamera.GetZoom();
                        giveThrough = true;
                    }

                    if (withinBounds) {
                        mSelected = true;
                        giveThrough = false;
                    }

                    break;

                case EMouseAction.RightClick:
                    mSelected = false;
                    giveThrough = true;
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

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }
    }
}
