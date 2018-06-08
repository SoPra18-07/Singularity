using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Map;
using Singularity.Property;

namespace Singularity.Units
{
    internal sealed class MilitaryUnit : IUnit, IDraw, IUpdate
    {
        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private static readonly Color sSelectedColor = Color.Gainsboro;
        private static readonly Color sNotSelectedColor = Color.White;

        private const double Speed = 4;

        private Color mColor;

        private int mColumn;
        private int mRow;

        private bool mIsMoving;
        private Rectangle mBounds;
        private Rectangle mBoundsSnapshot;
        private Vector2 mToAdd;
        private double mZoomSnapshot;

        private Vector2 mMovementVector;

        private readonly Camera mCamera;

        private Vector2 mTargetPosition;
        private int mRotation;
        private readonly Texture2D mMilSheet;
<<<<<<< HEAD
        private double mXstep;
        private double mYstep;
=======
>>>>>>> master
        private bool mSelected;
         
        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet, Camera camera)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(DefaultWidth, DefaultHeight);
            mIsMoving = false;
            mCamera = camera;

            mMilSheet = spriteSheet;
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination
        /// </summary>
        /// <param name="target"></param>
        private void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite 150x75
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
        /// The property which defines the health of the unit
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
                new Rectangle((150 * mColumn), (75 * mRow), (int) AbsoluteSize.X, (int) AbsoluteSize.Y), 
                mColor, 
                0f, 
                Vector2.Zero, 
                Vector2.One, 
                SpriteEffects.None, 
                LayerConstants.MilitaryUnitLayer
                );

        }


        public void Update(GameTime gameTime)
        {
            //make sure to update the relative bounds rectangle enclosing this unit.
            mBounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            UpdateSelected();

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving)
            {
                Rotate(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            }

            // this is the point when the unit starts moving. We need to make snapshots of the source point, destination point and the 
            // current zoom, this is important, since we want our unit to move to the point we click and not move to the point we click
            // that gets affected by zooming and stuff.
            if (mSelected && !mIsMoving && Mouse.GetState().LeftButton == ButtonState.Pressed && 
                ((Math.Abs(mBounds.Center.X - Mouse.GetState().X) > mBounds.Width - 15) ||
                 (Math.Abs(mBounds.Center.Y - Mouse.GetState().Y) > mBounds.Height - 30)))
            {
                mIsMoving = true;
                mTargetPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                mBoundsSnapshot = mBounds;
                mZoomSnapshot = mCamera.GetZoom();

            } else if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            if (mIsMoving && !HasReachedTarget())
            {
                MoveToTarget(mTargetPosition);
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = (mRotation / 18);
            mColumn = ((mRotation - (mRow * 18)) / 3);

            //finally select the appropriate color for selected/deselected units.
            mColor = mSelected ? sSelectedColor : sNotSelectedColor;
        }


        /// <summary>
        /// Sets the selected status of this unit to true or false, according to what mouse action was executed.
        /// </summary>
        private void UpdateSelected()
        {

            // if left click within unit area : selected
            if ((Math.Abs(mBounds.Center.X - Mouse.GetState().X) < mBounds.Width - 15) &&
                (Math.Abs(mBounds.Center.Y - Mouse.GetState().Y) < mBounds.Height - 30) &&
                Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mSelected = true;
            }

            // right click deselects unit
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                mSelected = false;
            }

        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction. 
        /// </summary>
        /// <param name="target">The target to which to move</param>
        private void MoveToTarget(Vector2 target)
        {

            mMovementVector = new Vector2(target.X - mBoundsSnapshot.Center.X, target.Y - mBoundsSnapshot.Center.Y);
            mMovementVector.Normalize();
            mToAdd += mMovementVector * (float) (mZoomSnapshot *  Speed);

            AbsolutePosition = new Vector2((float) (AbsolutePosition.X + mMovementVector.X * Speed), (float) (AbsolutePosition.Y + mMovementVector.Y * Speed));
        }

        /// <summary>
        /// Checks whether the target position is reached or not.
        /// </summary>
        private bool HasReachedTarget()
        {

            if (!(Math.Abs(mBoundsSnapshot.Center.X + mToAdd.X -
                           mTargetPosition.X) < 8 &&
                  Math.Abs(mBoundsSnapshot.Center.Y + mToAdd.Y -
                           mTargetPosition.Y) < 8))
            {
                return false;
            }
            mToAdd = Vector2.Zero;
            return true;

        }
    }
}
