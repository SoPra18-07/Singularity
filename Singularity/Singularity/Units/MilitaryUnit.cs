using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Sound;

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
        protected const float Scale = 0.4f;

        /// <summary>
        /// Indicates the position the closest enemy is at.
        /// </summary>
        private Vector2 mEnemyPosition;

        /// <summary>
        /// Indicates if the unit is currently shooting.
        /// </summary>
        private bool mShoot;

        
        public MilitaryUnit(Vector2 position,
            Camera camera,
            ref Director director,
            ref Map.Map map)
            : base(position, camera, ref director, ref map)
        {
            mSpeed = 4;
            Health = 10;

            AbsoluteSize = new Vector2(DefaultWidth * Scale, DefaultHeight * Scale);

            RevelationRadius = 400;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * Scale / 2, AbsolutePosition.Y + AbsoluteSize.Y * Scale / 2);
        }

        /// <summary>
        /// Static method that can be called to create a new military unit
        /// </summary>
        /// <param name="position"></param>
        /// <param name="director"></param>
        /// <returns></returns>
        public static MilitaryUnit CreateMilitaryUnit(Vector2 position, ref Director director)
        {
            var map = director.GetStoryManager.Level.Map;
            return new MilitaryUnit(position, director.GetStoryManager.Level.Camera, ref director, ref map);
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
                            mSelected ? Color.DarkGray : Color.Gray,
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
                    LayerConstants.MilitaryUnitLayer - 0.1f);
            }

            if (GlobalVariables.DebugState)
            {
                // TODO DEBUG REGION
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
            spriteBatch.DrawLine(Center, MapCoordinates(mEnemyPosition), Color.White, 2);
            spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), MapCoordinates(mEnemyPosition), Color.White * .2f, 6);
            mShoot = false;
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

            else if (mShoot)
            {
                Rotate(mEnemyPosition);
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

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X * Scale / 2, AbsolutePosition.Y + AbsoluteSize.Y * Scale / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int) AbsolutePosition.Y + 11, (int)(AbsoluteSize.X * Scale), (int) (AbsoluteSize.Y * Scale));
            Moved = mIsMoving;

            //TODO this needs to be taken out once the military manager takes control of shooting
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                // shoots at mouse and plays laser sound at full volume
                Shoot(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                mDirector.GetSoundManager.PlaySound("LaserSound", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);

            }
        }

        public void Shoot(Vector2 target)
        {
            mShoot = true;
            mEnemyPosition = target;
            Rotate(target);
        }
        
    }
}
