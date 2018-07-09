using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Units;

namespace Singularity.Platforms
{

    internal abstract class DefenseBase : PlatformBlank, IShooting
    {
        /// <summary>
        /// For defense platforms, indicates if they are shooting.
        /// </summary>
        protected bool mShoot;

        /// <summary>
        /// For defense platforms, indicates where there target is
        /// </summary>
        internal Vector2 EnemyPosition { get; set; }

        /// <summary>
        /// Represents an abstract class for all defense platforms. Implements their draw methods.
        /// </summary>
        internal DefenseBase(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            EPlatformType type,
            bool autoRegister = true)
            : base(position, platformSpriteSheet, baseSprite, libSans12, ref director, type)
        {
            if (autoRegister)
            {
                director.GetDistributionManager.Register(this, false);
            }
            mType = type;
            mSpritename = "Cone";
            Property = JobType.Defense;
            SetPlatfromParameters();
        }

        public abstract void Shoot(Vector2 target);

        public override void Draw(SpriteBatch spriteBatch)
        {
            var transparency = mIsBlueprint ? 0.35f : 1f;

            // Cone (Defense Platforms)
            // Draw the basic platform first
            spriteBatch.Draw(mPlatformBaseTexture,
                Vector2.Add(AbsolutePosition, new Vector2(0, 78)),
                null,
                Color.White * transparency,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                LayerConstants.BasePlatformLayer);
            // then draw what's on top of that
            spriteBatch.Draw(mPlatformSpriteSheet,
                AbsolutePosition,
                new Rectangle(PlatformWidth * mSheetPosition, 0, 148, 148),
                mColor * transparency,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                LayerConstants.PlatformLayer);

            if (!mShoot)
            {
                return;
            }

            // draws a laser line a a slight glow around the line, then sets the shoot future off
            spriteBatch.DrawLine(Center, MapCoordinates(EnemyPosition), Color.White, 2);
            spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), MapCoordinates(EnemyPosition), Color.White * .2f, 6);
            mShoot = false;
        }

        /// <summary>
        /// Used to find the coordinates of the given Vector2 based on the overall map
        /// instead of just the camera shot, returns Vector2 of absolute position
        /// </summary>
        /// <returns></returns>
        private Vector2 MapCoordinates(Vector2 v)
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            return new Vector2(Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(camera.GetTransform())).X, Vector2.Transform(new Vector2(v.X, v.Y),
                Matrix.Invert(camera.GetTransform())).Y);
        }
    }
}
