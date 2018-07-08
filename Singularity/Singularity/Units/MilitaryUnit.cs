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
            : base(position: position, camera: camera, director: ref director, map: ref map)
        {
            mSpeed = 4;
            Health = 10;

            AbsoluteSize = new Vector2(x: DefaultWidth * Scale, y: DefaultHeight * Scale);

            RevelationRadius = 400;

            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X * Scale / 2, y: AbsolutePosition.Y + AbsoluteSize.Y * Scale / 2);
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
            return new MilitaryUnit(position: position, camera: director.GetStoryManager.Level.Camera, director: ref director, map: ref map);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // makes sure that the textures are loaded
            if (mMilSheet == null || mGlowTexture == null)
            {
                throw new Exception(message: "load the MilSheet and GlowTexture first!");
            }

            // Draw military unit
            spriteBatch.Draw(
                            texture: mMilSheet,
                            position: AbsolutePosition,
                            sourceRectangle: new Rectangle(x: 150 * mColumn, y: 75 * mRow, width: (int)(AbsoluteSize.X / Scale), height: (int)(AbsoluteSize.Y / Scale)),
                            color: mSelected ? Color.DarkGray : Color.Gray,
                            rotation: 0f,
                            origin: Vector2.Zero,
                            scale: new Vector2(value: Scale),
                            effects: SpriteEffects.None,
                            layerDepth: LayerConstants.MilitaryUnitLayer
                            );

            // Draw the glow under it
            if (mSelected)
            {
                spriteBatch.Draw(
                    texture: mGlowTexture,
                    position: Vector2.Add(value1: AbsolutePosition, value2: new Vector2(x: -4.5f, y: -4.5f)),
                    sourceRectangle: new Rectangle(x: 172 * mColumn, y: 100 * mRow, width: 172, height: 100),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: new Vector2(value: Scale),
                    effects: SpriteEffects.None,
                    layerDepth: LayerConstants.MilitaryUnitLayer - 0.1f);
            }

            if (GlobalVariables.DebugState)
            {
                // TODO DEBUG REGION
                if (mDebugPath != null)
                {
                    for (var i = 0; i < mDebugPath.Length - 1; i++)
                    {
                        spriteBatch.DrawLine(point1: mDebugPath[i], point2: mDebugPath[i + 1], color: Color.Orange);
                    }
                }
            }

            if (!mShoot)
            {
                return;
            }

            // draws a laser line a a slight glow around the line, then sets the shoot future off
            spriteBatch.DrawLine(point1: Center, point2: MapCoordinates(v: mEnemyPosition), color: Color.White, thickness: 2);
            spriteBatch.DrawLine(point1: new Vector2(x: Center.X - 2, y: Center.Y), point2: MapCoordinates(v: mEnemyPosition), color: Color.White * .2f, thickness: 6);
            mShoot = false;
        }

        public override void Update(GameTime gameTime)
        {

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                x: (int)RelativePosition.X, y: (int)RelativePosition.Y, width: (int)RelativeSize.X, height: (int)RelativeSize.Y);

            // this makes the unit rotate according to the mouse position when its selected and not moving.
            if (mSelected && !mIsMoving && !mShoot)
            {
                Rotate(target: new Vector2(x: mMouseX, y: mMouseY));
            }

            else if (mShoot)
            {
                Rotate(target: mEnemyPosition);
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
                    MoveToTarget(target: mPath.Peek(), speed: mSpeed);
                }
                else
                {
                    mPath.Pop();
                    MoveToTarget(target: mPath.Peek(), speed: mSpeed);
                }
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X * Scale / 2, y: AbsolutePosition.Y + AbsoluteSize.Y * Scale / 2);
            AbsBounds = new Rectangle(x: (int)AbsolutePosition.X + 16, y: (int) AbsolutePosition.Y + 11, width: (int)(AbsoluteSize.X * Scale), height: (int) (AbsoluteSize.Y * Scale));
            Moved = mIsMoving;

            //TODO this needs to be taken out once the military manager takes control of shooting
            if (Keyboard.GetState().IsKeyDown(key: Keys.Space))
            {
                // shoots at mouse and plays laser sound at full volume
                Shoot(target: new Vector2(x: Mouse.GetState().X, y: Mouse.GetState().Y));
                mDirector.GetSoundManager.PlaySound(name: "LaserSound", x: Center.X, y: Center.Y, volume: 1f, pitch: 1f, isGlobal: true, loop: false, soundClass: SoundClass.Effect);

            }
        }

        public void Shoot(Vector2 target)
        {
            mShoot = true;
            mEnemyPosition = target;
            Rotate(target: target);
        }

    }
}
