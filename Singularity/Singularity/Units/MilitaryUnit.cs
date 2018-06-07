using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;
using Singularity.Utils;

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

        private Vector2 mMovementVector;

        private int mMovementApplyTimes;

        private Vector2 mTargetPosition;
        private int mRotation;
        private readonly Texture2D mMilSheet;
        private bool mSelected;
         
        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            
            AbsolutePosition = position;
            AbsoluteSize = new Vector2(DefaultWidth, DefaultHeight);
            mIsMoving = false;
            mMovementApplyTimes = 0;

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
            // calculate correct sprite on spritesheet to use based on angle
            // direction that unit is meant to be faceing
            //var rowNumber = (mRotation / 18);
           // var columnNumber = ((mRotation - (rowNumber * 18)) / 3);

            // darken color if unit is selected
           // Color color;
            //if (!mSelected) { color = Color.White; }
            //else { color = Color.Gainsboro; }
            
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
            mBounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            mSelected = IsSelected();

            if (mSelected && !mIsMoving)
            {
                Rotate(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            }

            if (mSelected && !mIsMoving && Mouse.GetState().LeftButton == ButtonState.Pressed && 
                ((Math.Abs(mBounds.Center.X - Mouse.GetState().X) > 60) ||
                 (Math.Abs(mBounds.Center.Y - Mouse.GetState().Y) > 45)))
            {
                mIsMoving = true;
                mTargetPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            } else if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            if (mIsMoving && !HasReachedTarget())
            {
                MoveToTarget(mTargetPosition);
            }

            mRow = (mRotation / 18);
            mColumn = ((mRotation - (mRow * 18)) / 3);

            mColor = mSelected ? sSelectedColor : sNotSelectedColor;
        }


        /// <summary>
        /// determines if the the military unit is currently selected by user
        /// left click within area is select
        /// right click anywhere else is deselect
        /// </summary>
        private bool IsSelected()
        {

            // if left click within unit area : selected
            if ((Math.Abs(mBounds.Center.X - Mouse.GetState().X) < 60) &&
                (Math.Abs(mBounds.Center.Y - Mouse.GetState().Y) < 45) &&
                Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            // right click deselects unit
            return Mouse.GetState().RightButton == ButtonState.Pressed ? false : mSelected;

        }

        /// <summary>
        /// determine x and y movement of unit in order to
        /// reach target destination. This will then be used
        /// by the Move method
        /// </summary>
        /// <param name="target"></param>
        private void MoveToTarget(Vector2 target)
        {

            mMovementVector = Geometry.NormalizeVector(new Vector2(target.X - mBounds.Center.X, target.Y - mBounds.Center.Y));
            AbsolutePosition = new Vector2((int) (AbsolutePosition.X + mMovementVector.X * Speed), (int) (AbsolutePosition.Y + mMovementVector.Y * Speed));
        }

        /// <summary>
        /// Updates position of unit in order to reach
        /// target position
        /// </summary>
        private bool HasReachedTarget()
        {
            return Math.Abs(RelativePosition.X - (RelativeSize.X / 2) - mTargetPosition.X) < 8 &&
                   Math.Abs(RelativePosition.Y - (RelativeSize.Y / 2) - mTargetPosition.Y) < 8;

        }
    }
}
