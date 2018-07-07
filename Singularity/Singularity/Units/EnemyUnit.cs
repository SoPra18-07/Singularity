using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Map.Properties;
using Singularity.Property;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    internal sealed class EnemyUnit : MilitaryUnit
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
        private readonly Color mColor = Color.Red;

        /// <summary>
        /// Stores the time since the last random movement.
        /// </summary>
        private float mElapsedTime;

        /// <summary>
        /// Random seed to calculate paths.
        /// </summary>
        private readonly Random mRand = new Random();

        /// <summary>
        /// Enemy units controlled by AI and opposed to the player.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="spriteSheet">Military unit sprite sheet.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        /// <param name="map">Reference to the game map.</param>
        public EnemyUnit(Vector2 position, Texture2D spriteSheet, Camera camera, ref Director director, ref Map.Map map)
            : base(position: position, camera: camera, director: ref director, map: ref map)
        {
            Health = 10; //TODO

            AbsoluteSize = new Vector2(x: DefaultWidth * Scale, y: DefaultHeight * Scale);

            RevelationRadius = 0;

            mSpeed = 4;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: mMilSheet,
                position: AbsolutePosition,
                sourceRectangle: new Rectangle(x: 150 * mColumn, y: 75 * mRow, width: (int)AbsoluteSize.X, height: (int)AbsoluteSize.Y),
                color: mColor,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: LayerConstants.MilitaryUnitLayer
                );
        }


        public override void Update(GameTime gameTime)
        {
            // Generate random positions for the enemy unit to move to.
            mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mElapsedTime > 4)
            { //Get a new random direction every 4 seconds
                mElapsedTime -= 4; //Subtract the 4 seconds we've already checked
                var dirX = (float)mRand.NextDouble() * MapConstants.MapWidth; //Set the position to a random value within the screen
                var dirY = (float)mRand.NextDouble() * MapConstants.MapHeight;

                // Check if the target position is on the map.
                if (!mIsMoving && Map.Map.IsOnTop(rect: new Rectangle(x: (int)(dirX - RelativeSize.X / 2f), y: (int)(dirY - RelativeSize.Y / 2f), width: (int)RelativeSize.X, height: (int)RelativeSize.Y), camera: mCamera))
                {
                    Rotate(target: new Vector2(x: dirX, y: dirY));
                    mIsMoving = true;
                    mTargetPosition = new Vector2(x: dirX, y: dirY);
                    mBoundsSnapshot = Bounds;
                    mZoomSnapshot = mCamera.GetZoom();
                }
            }

            //make sure to update the relative bounds rectangle enclosing this unit.
            Bounds = new Rectangle(
                x: (int)RelativePosition.X, y: (int)RelativePosition.Y, width: (int)RelativeSize.X, height: (int)RelativeSize.Y);

            if (HasReachedTarget())
            {
                mIsMoving = false;
            }

            // calculate path to target position
            if (mIsMoving && !HasReachedTarget())
            {
                MoveToTarget(target: mTargetPosition, speed: mSpeed);
            }

            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            Center = new Vector2(x: AbsolutePosition.X + AbsoluteSize.X / 2, y: AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle(x: (int)AbsolutePosition.X, y: (int)AbsolutePosition.Y, width: (int)AbsoluteSize.X, height: (int)AbsoluteSize.Y);
            Moved = mIsMoving;

        }
    }
}
