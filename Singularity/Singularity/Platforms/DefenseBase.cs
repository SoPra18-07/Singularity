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

        public int Range { get; } = 400;

        protected ICollider mShootingTarget;

        /// <summary>
        /// Represents an abstract class for all defense platforms. Implements their draw methods.
        /// </summary>
        internal DefenseBase(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            EPlatformType type,
            bool friendly = true)
            : base(position,
                platformSpriteSheet,
                baseSprite,
                libSans12,
                ref director,
                type,
                friendly: friendly)
        {
            mType = type;
            mSpritename = "Cone";
            Property = JobType.Defense;
            SetPlatfromParameters();

            RevelationRadius = 500;
        }

        public abstract void Shoot(ICollider target);

        public override void Draw(SpriteBatch spriteBatch)
        {
            var transparency = mIsBlueprint ? 0.35f : 1f;

            // Cone (Defense Platforms)
            // Draw the basic platform first
            spriteBatch.Draw(mPlatformBaseTexture,
                Vector2.Add(AbsolutePosition, new Vector2(0, 78)),
                null,
                mColorBase * transparency,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                LayerConstants.BasePlatformLayer);
            // then draw what's on top of that
            spriteBatch.Draw(mPlatformSpriteSheet,
                AbsolutePosition,
                new Rectangle(mPlatformWidth * mSheetPosition, 0, 148, 148),
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
            spriteBatch.DrawLine(Center, EnemyPosition, Color.White, 2);
            spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), EnemyPosition, Color.White * .2f, 6);
            mShoot = false;
        }

        public void SetShootingTarget(ICollider target)
        {
            mShootingTarget = target;
            Shoot(target);
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
