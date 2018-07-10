using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="ControllableUnit"/>
    internal class MilitaryUnit : ControllableUnit, IShooting
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
        /// Sprite sheet for military units.
        /// </summary>
        internal static Texture2D mMilSheet;

        /// <summary>
        /// Sprite sheet for the glow effect when a unit is selected.
        /// </summary>
        internal static Texture2D mGlowTexture;

        /// <summary>
        /// Scalar for the unit size.
        /// </summary>
        private const float Scale = 0.4f;

        /// <summary>
        /// Indicates the position the closest enemy is at.
        /// </summary>
        private ICollider mShootingTarget;

        /// <summary>
        /// Indicates if the unit is currently shooting.
        /// </summary>
        private bool mShoot;

        public int Range { get; protected set; }

        /// <summary>
        /// Color of the unit while selected.
        /// </summary>
        protected Color mSelectedColor = Color.DarkGray;

        /// <summary>
        /// Color of the unit while not selected
        /// </summary>
        protected Color mColor = Color.Gray;

        private float mShootingTimer = -1f;
        private double mCurrentTime;
        
        public MilitaryUnit(Vector2 position,
            Camera camera,
            ref Director director,
            ref Map.Map map)
            : base(position, camera, ref director, ref map)
        {
            mSpeed = MilitaryUnitStats.StandardSpeed;
            Health = MilitaryUnitStats.StandardHealth;

            AbsoluteSize = new Vector2(DefaultWidth * Scale, DefaultHeight * Scale);

            RevelationRadius = 400;

            Center = new Vector2((AbsolutePosition.X + AbsoluteSize.X) * 0.5f, (AbsolutePosition.Y + AbsoluteSize.Y) * 0.5f );

            Range = MilitaryUnitStats.StandardRange;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // makes sure that the textures are loaded
            if (mMilSheet == null || mGlowTexture == null)
            {
                throw new Exception("load the MilSheet and GlowTexture first!");
            }

            // Draw military unit
            spriteBatch.Draw(
                            mMilSheet,
                            AbsolutePosition,
                            new Rectangle(150 * mColumn, 75 * mRow, (int)(AbsoluteSize.X / Scale), (int)(AbsoluteSize.Y / Scale)),
                            mSelected ? mSelectedColor : mColor,
                            0f,
                            Vector2.Zero,
                            new Vector2(Scale),
                            SpriteEffects.None,
                            LayerConstants.MilitaryUnitLayer
                            );

            // Draw the glow under it
            if (mSelected)
            {
                spriteBatch.Draw(
                    mGlowTexture,
                    Vector2.Add(AbsolutePosition, new Vector2(-4.5f, -4.5f)),
                    new Rectangle(172 * mColumn, 100 * mRow, 172, 100),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(Scale),
                    SpriteEffects.None,
                    LayerConstants.MilitaryUnitLayer - 0.01f);
            }

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

            if (mCurrentTime <= mShootingTimer + 200)
            {
                // draws a laser line a a slight glow around the line, then sets the shoot future off
                spriteBatch.DrawLine(Center, mShootingTarget.Center, Color.White, 2, .15f);
                spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), mShootingTarget.Center, Color.White * .2f, 6, .15f);
                mShoot = false;
            }
        }

        public override void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);

            
            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving && !mShoot)
            {
                 Rotate(new Vector2(mMouseX, mMouseY));
            }
            

            else if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            else if (mIsMoving)
            {
                Rotate(mPath.Peek());
                if (!HasReachedWaypoint())
                {
                    MoveToTarget(mPath.Peek(), mSpeed);
                }
                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek(), mSpeed);
                }
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int) AbsolutePosition.Y + 11, (int)(AbsoluteSize.X * Scale), (int) (AbsoluteSize.Y * Scale));
            Moved = mIsMoving;

            //TODO this needs to be taken out once the military manager takes control of shooting
            if (!mIsMoving && mShoot)
            {
                if (mShootingTimer < 0.5f)
                {
                    mShootingTimer = (float) gameTime.TotalGameTime.TotalMilliseconds;
                    Shoot(mShootingTarget);
                }

                mCurrentTime = gameTime.TotalGameTime.TotalMilliseconds;
                if (mShootingTimer + 750 <= gameTime.TotalGameTime.TotalMilliseconds)
                {
                    mShootingTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    Shoot(mShootingTarget);
                }
            }
        }

        public void Shoot(ICollider target)
        {
            mDirector.GetSoundManager.PlaySound("LaserSound", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            target.MakeDamage(MilitaryUnitStats.UnitStrength);
        }

        public void SetShootingTarget(ICollider target)
        {
            if (target == null)
            {
                mShoot = false;
                mShootingTimer = -1;
            }
            else
            {
                mShoot = true;
            }

            mShootingTarget = target;
        }
        
    }
}
