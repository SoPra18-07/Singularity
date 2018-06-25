using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    internal sealed class EnemyUnit : ICollider, IRevealing
    {
        private const int DefaultWidth = 150;
        private const int DefaultHeight = 75;

        private static readonly Color sSelectedColor = Color.DarkRed;
        private static readonly Color sNotSelectedColor = Color.Red;

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

        private bool mSelected = true;

        private float mMouseX;

        private float mMouseY;

        private float mElapsedTime;
        private Random mRand = new Random();

        private readonly Director mDirector;

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        public Vector2 Center { get; private set; }

        public bool Moved { get; private set; }

        public int RevelationRadius { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Rectangle AbsBounds { get; private set; }

        public EnemyUnit(Vector2 position, Texture2D spriteSheet, Camera camera, ref Director director)
        {
            Id = IdGenerator.NextiD(); // TODO this will later use a random number generator to create a unique
                                       // id for the specific unit.
            Health = 10; //TODO

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(DefaultWidth, DefaultHeight);

            RevelationRadius = (int)AbsoluteSize.X;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            Moved = false;
            mIsMoving = false;
            mCamera = camera;

            mDirector = director;

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
                mRotation = (int)(Math.Round(270 - degree, MidpointRounding.AwayFromZero));
            }
            else
            {
                mRotation = (int)(Math.Round(90 + degree, MidpointRounding.AwayFromZero));
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
                new Rectangle((150 * mColumn), (75 * mRow), (int)AbsoluteSize.X, (int)AbsoluteSize.Y),
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
            // Generate random positions for the enemy unit to move to.
            mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mElapsedTime > 4)
            { //Get a new random direction every 4 seconds
                mElapsedTime -= 4; //Subtract the 4 seconds we've already checked
                mMouseX = (float)mRand.NextDouble() * 2000; //Set the position to a random value within the screen
                mMouseY = (float)mRand.NextDouble() * 1000;
            }

            // Check if the target position is on the map.
            if (mSelected && !mIsMoving && Map.Map.IsOnTop(new Rectangle((int)(mMouseX - RelativeSize.X / 2f), (int)(mMouseY - RelativeSize.Y / 2f), (int)RelativeSize.X, (int)RelativeSize.Y), mCamera))
            {
                Rotate(new Vector2(mMouseX, mMouseY));
                mIsMoving = true;
                mTargetPosition = new Vector2(mMouseX, mMouseY);
                mBoundsSnapshot = Bounds;
                mZoomSnapshot = mCamera.GetZoom();
            }

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            if (HasReachedTarget())
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

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Moved = mIsMoving;

        }

        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction. 
        /// </summary>
        /// <param name="target">The target to which to move</param>
        private void MoveToTarget(Vector2 target)
        {

            mMovementVector = new Vector2(target.X - mBoundsSnapshot.Center.X, target.Y - mBoundsSnapshot.Center.Y);
            mMovementVector.Normalize();
            mToAdd += mMovementVector * (float)(mZoomSnapshot * Speed);

            AbsolutePosition = new Vector2((float)(AbsolutePosition.X + mMovementVector.X * Speed), (float)(AbsolutePosition.Y + mMovementVector.Y * Speed));
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
