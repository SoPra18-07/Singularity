using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Sound;
using Singularity.Units;

namespace Singularity.Platforms
{
    /// <inheritdoc cref="DefenseBase"/>
    [DataContract]
    public class DefenseLaser : DefenseBase
    {
        [DataMember]
        private const int DrainingEnergy = 40;

        [DataMember]
        private TimeSpan mShootCounter;

        [DataMember]
        private int mDefenseCounter;

        [DataMember]
        private int mShotsDone;

        /// <summary>
        /// Constructs a Laser (i.e. uses energy) defense platform that automatically attacks
        /// enemy units. This platform uses no ammunition but requires energy.
        /// </summary>
        internal DefenseLaser(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true) : base(position,
            platformSpriteSheet,
            baseSprite,
            libSans12,
            ref director,
            EStructureType.Laser,
            friendly: friendly)
        {
            mDrainingEnergy = DrainingEnergy;
            mSpritename = "Cones";
            mCost = GetResourceCosts(EStructureType.Laser);
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            mShootCounter = new TimeSpan(0, 0, 0, 0);
        }


        public override void Shoot(ICollider target)
        {
            if (target != null)
            {
                if (IsActive())
                {
                    mShoot = true;
                    mDirector.GetSoundManager.PlaySound(mSoundId);
                }

                target.MakeDamage(MilitaryUnitStats.mTurretStrength);
            }
        }

        public override bool Die()
        {
            mShootingTarget = null;
            mDefenseAction = null;
            return base.Die();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            //Calculating attackspeed and resetting attacks.
            if (mShootCounter.TotalMilliseconds > 2000)
            {
                mShootCounter = new TimeSpan(0, 0, 0, 0);
                mDefenseCounter = 0;
                mShotsDone = 0;
                foreach (var unitbool in mAssignedUnits[JobType.Defense])
                {
                    if (unitbool.GetSecond())
                    {
                        mDefenseCounter++;
                    }
                }
            }
            mShootCounter = mShootCounter.Add(time.ElapsedGameTime);
            //No defending units = no Shots
            if (mDefenseCounter == 0)
            {
                return;
            }
            //Shooting according to attackspeed
            if (mShootCounter.TotalMilliseconds > 2000/mDefenseCounter * mShotsDone)
            {
                if (mShootingTarget != null && mType == EStructureType.Laser)
                {
                    mDefenseAction.Execute();
                    mShotsDone++;
                }
            }
        }

        public override void ReloadContent(ContentManager content, ref Director dir)
        {
            base.ReloadContent(content, ref dir);
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
        }
    }
}
