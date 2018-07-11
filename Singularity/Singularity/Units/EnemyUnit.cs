using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    internal class EnemyUnit : FreeMovingUnit, IShooting
    {
        /// <summary>
        /// Default width of a unit before scaling.
        /// </summary>
        private const int DefaultWidth = 150;

        /// <summary>
        /// Default height of a unit before scaling.
        /// </summary>
        private const int DefaultHeight = 75;

        /// <summary>
        /// Color overlay used on the unit to show it is an enemy unit not a friendly unit.
        /// </summary>
        protected readonly Color mColor = Color.Red;
        
        /// <summary>
        /// Stores the time since the last random movement.
        /// </summary>
        private float mElapsedTime;

        /// <summary>
        /// Scalar for the unit size.
        /// </summary>
        protected const float Scale = 0.4f;

        /// <summary>
        /// Random seed to calculate paths.
        /// </summary>
        private readonly Random mRand = new Random();

        /// <summary>
        /// The spriteSheet used to draw enemy units.
        /// </summary>
        protected readonly Texture2D mMilSheet = MilitaryUnit.mMilSheet;

        public new bool Friendly { get; } = false;

        protected bool mShoot;

        protected ICollider mShootingTarget;

        public int Range { get; protected set; }

        public void Shoot(ICollider target)
        {
            // Todo
        }

        public void SetShootingTarget(ICollider target)
        {
            if (target == null)
            {
                mShoot = false;
            }
            else
            {
                mShoot = true;
                mShootingTarget = target;
            }
        }

        /// <summary>
        /// Enemy units controlled by AI and opposed to the player.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        public EnemyUnit(Vector2 position, Camera camera, ref Director director, ref Map.Map map) 
            : base(position, camera, ref director, ref map, false)
        {
            AbsoluteSize = new Vector2(DefaultWidth * Scale, DefaultHeight * Scale);

            RevelationRadius = 400;

            mSpeed = MilitaryUnitStats.StandardSpeed;
            Health = MilitaryUnitStats.StandardHealth;
            Range = MilitaryUnitStats.StandardRange;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // makes sure that the textures are loaded
            if (mMilSheet == null)
            {
                throw new Exception("load the MilSheet and GlowTexture first!");
            }

            // Draw military unit
            spriteBatch.Draw(
                            mMilSheet,
                            AbsolutePosition,
                            new Rectangle(150 * mColumn, 75 * mRow, (int)(AbsoluteSize.X / Scale), (int)(AbsoluteSize.Y / Scale)),
                            mColor,
                            0f,
                            Vector2.Zero,
                            new Vector2(Scale),
                            SpriteEffects.None,
                            LayerConstants.MilitaryUnitLayer
                            );

            

            if (GlobalVariables.DebugState)
            {
                if (mDebugPath != null)
                {
                    for (var i = 0; i < mDebugPath.Length - 1; i++)
                    {
                        spriteBatch.DrawLine(mDebugPath[i], mDebugPath[i + 1], Color.Orange);
                    }
                }
            }

            if (!mShoot)
            {
                return;
            }

            // draws a laser line a a slight glow around the line, then sets the shoot future off
            spriteBatch.DrawLine(Center, mShootingTarget.Center, Color.White, 2, .15f);
            spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), mShootingTarget.Center, Color.White * .2f, 6, .15f);
            mShoot = false;
        }


        public override void Update(GameTime gameTime)
        {
            /*
            // Generate random positions for the enemy unit to move to.
            mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mElapsedTime > 4)
            { //Get a new random direction every 4 seconds
                mElapsedTime -= 4; //Subtract the 4 seconds we've already checked
                var dirX = (float)mRand.NextDouble() * MapConstants.MapWidth; //Set the position to a random value within the screen
                var dirY = (float)mRand.NextDouble() * MapConstants.MapHeight;

                // Check if the target position is on the map.
                if (!mIsMoving && Map.Map.IsOnTop(new Rectangle((int)(dirX - RelativeSize.X / 2f), (int)(dirY - RelativeSize.Y / 2f), (int)RelativeSize.X, (int)RelativeSize.Y), mCamera))
                {
                    Rotate(new Vector2(dirX, dirY));
                    mIsMoving = true;
                    mTargetPosition = new Vector2(dirX, dirY);
                    mBoundsSnapshot = Bounds;
                    mZoomSnapshot = mCamera.GetZoom();
                }
            }
            */

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
                MoveToTarget(mTargetPosition, mSpeed);
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;
            
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Moved = mIsMoving;
            
        }
    }
}
