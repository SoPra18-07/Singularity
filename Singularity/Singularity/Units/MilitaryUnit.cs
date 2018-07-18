using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Sound;

namespace Singularity.Units
{
    /// <inheritdoc cref="FreeMovingUnit"/>
    [DataContract]
    public class MilitaryUnit : FreeMovingUnit, IShooting
    {
        /// <summary>
        /// Default width of a unit before scaling.
        /// </summary>
        [DataMember]
        protected const int DefaultWidth = 150;

        /// <summary>
        /// Default height of a unit before scaling.
        /// </summary>
        [DataMember]
        protected const int DefaultHeight = 75;

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
        [DataMember]
        protected float mScale = 0.4f;

        /// <summary>
        /// Used to set the enemy target that should be shot at.
        /// </summary>
        [DataMember]
        protected ICollider mShootingTarget;


        /// <summary>
        /// Indicates if the unit is currently shooting.
        /// </summary>
        [DataMember]
        private bool mShoot;
        [DataMember]
        public int Range { get; protected set; }

        /// <summary>
        /// Color of the unit while selected.
        /// </summary>
        [DataMember]
        protected Color mSelectedColor = Color.DarkGray;

        /// <summary>
        /// Color of the unit while not selected
        /// </summary>
        [DataMember]
        protected Color mColor = Color.Gray;

        /// <summary>
        /// Used to make sure that it doesn't shoot too often.
        /// </summary>
        private float mShootingTimer = -1f;


        /// <summary>
        /// ID for the sound effect instance used by this class.
        /// </summary>
        protected int mSoundId;

        [DataMember]
        protected Color mShootColor = Color.White;

        [DataMember]
        protected bool mTargetWasNull;

        private double mCurrentTime;


        public MilitaryUnit(Vector2 position,
            Camera camera,
            ref Director director,
            bool friendly = true)
            : base(position, camera, ref director, friendly)
        {
            Speed = MilitaryUnitStats.StandardSpeed;
            Health = MilitaryUnitStats.StandardHealth;

            AbsoluteSize = new Vector2(DefaultWidth * mScale, DefaultHeight * mScale);

            RevelationRadius = 400;

            Center = new Vector2((AbsolutePosition.X + AbsoluteSize.X) * 0.5f, (AbsolutePosition.Y + AbsoluteSize.Y) * 0.5f );

            Range = MilitaryUnitStats.StandardRange;

            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserSound", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);

            // Track the creation of a military unit in the statistics.
            director.GetStoryManager.UpdateUnits("military created");
        }

        public void ReloadContent(ContentManager content, ref Director director, Camera camera, ref Map.Map map)
        {
            ReloadContent(ref director, camera);
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserSound", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // makes sure that the textures are loaded
            if (mMilSheet == null || mGlowTexture == null)
            {
                throw new Exception("load the MilSheet and GlowTexture first!");
            }

            mHealthBar.Draw(spriteBatch);

            // Draw military unit
            spriteBatch.Draw(
                mMilSheet,
                AbsolutePosition,
                new Rectangle(150 * mColumn, 75 * mRow, (int)(AbsoluteSize.X / mScale), (int)(AbsoluteSize.Y / mScale)),
                Selected ? mSelectedColor : mColor,
                0f,
                Vector2.Zero,
                new Vector2(mScale),
                SpriteEffects.None,
                LayerConstants.MilitaryUnitLayer
            );

            // Draw the glow under it
            if (Selected)
            {
                spriteBatch.Draw(
                    mGlowTexture,
                    Vector2.Add(AbsolutePosition, new Vector2(-4.5f, -4.5f)),
                    new Rectangle(172 * mColumn, 100 * mRow, 172, 100),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(mScale),
                    SpriteEffects.None,
                    LayerConstants.MilitaryUnitLayer - 0.01f);
            }


            if (mShoot)
            {
                if (mCurrentTime <= mShootingTimer + 200)
                {
                    // draws a laser line a a slight glow around the line, then sets the shoot future off
                    spriteBatch.DrawLine(Center, mShootingTarget.Center, mShootColor, 2, .15f);
                    spriteBatch.DrawLine(new Vector2(Center.X - 2, Center.Y), mShootingTarget.Center, mShootColor * .2f, 6, .15f);
                    mShoot = false;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            //make sure to update the relative bounds rectangle enclosing this unit.
            // Bounds = new Rectangle(
                // (int)RelativePosition.X, (int)RelativePosition.Y, (int)RelativeSize.X, (int)RelativeSize.Y);
                // (already happening in FreeMovingUnit )

            mHealthBar.Update(gameTime);


            // this makes the unit rotate according to the mouse position when its selected and not moving.
            // if (mSelected && !Moved && !mShoot)
            // {
                 // Rotate(new Vector2(mMouseX, mMouseY));
            // }
            
            
            // these are values needed to properly get the current sprite out of the spritesheet.
            mRow = mRotation / 18;
            mColumn = (mRotation - mRow * 18) / 3;

            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);
            AbsBounds = new Rectangle((int)AbsolutePosition.X + 16, (int) AbsolutePosition.Y + 11, (int)(AbsoluteSize.X * mScale), (int) (AbsoluteSize.Y * mScale));

            if (Moved || !mShoot) return;
            // Rotate to the center of the shooting target
            Rotate(mShootingTarget.Center);
            

            if (mShootingTimer < 0.5f)
                {
                    mShootingTimer = (float) gameTime.TotalGameTime.TotalMilliseconds;
                    Shoot(mShootingTarget);
                }

                mCurrentTime = gameTime.TotalGameTime.TotalMilliseconds;
                if (mShootingTimer + 750 <= gameTime.TotalGameTime.TotalMilliseconds)
                {
                    mShootingTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    if (mShootingTarget != null)
                    {
                        Shoot(mShootingTarget);
                    }
                }
                mDirector.GetStoryManager.Level.Ai.Shooting(this, mShootingTarget, gameTime);
        }

        private void Shoot(IDamageable target)
        {
            mDirector.GetSoundManager.SetSoundPosition(mSoundId, Center.X, Center.Y);
            mDirector.GetSoundManager.PlaySound(mSoundId);
            target.MakeDamage(MilitaryUnitStats.mUnitStrength);
            
            if (target != null)
            {
                mDirector.GetSoundManager.PlaySound(mSoundId);
                target.MakeDamage(MilitaryUnitStats.mUnitStrength);

                //This should prevent the units to hold the reference to the target platform
                //and further shooting at it despite it already being dead (they shoot in the
                //air then)
                var test = target as PlatformBlank;
                var test2 = target as FreeMovingUnit;
                if (test != null && test.HasDieded)
                {
                    mShootingTarget = null;
                    mShootingTimer = -1;
                    mShoot = false;
                }

                if (test2 != null && test2.HasDieded)
                {
                    mShootingTarget = null;
                    mShootingTimer = -1;
                    mShoot = false;
                }
            }
        }

        public void SetShootingTarget(ICollider target)
        {
            if (target == null)
            {
                mShoot = false;
                mShootingTimer = -1;
                mTargetWasNull = true;
            }
            else
            {
                if (mTargetWasNull)
                {
                    // mTargetPosition = AbsolutePosition;
                    Moved = false;
                    //TODO: THis is a hotfix. Threw an error for the path being null...
                    mShoot = true;
                }

                else
                {
                    mShoot = true;
                }

                mTargetWasNull = false;
            }

            mShootingTarget = target;
        }

    }
}
