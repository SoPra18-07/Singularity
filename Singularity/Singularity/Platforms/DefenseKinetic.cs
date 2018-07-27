using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Sound;
using Singularity.Units;

namespace Singularity.Platforms
{
    [DataContract]
    internal sealed class DefenseKinetic : DefenseBase
    {

        /// <summary>
        /// Stores how many times materials have been requested between states
        /// </summary>
        [DataMember]
        private int mAmmoRequested;

        [DataMember]
        private TimeSpan mShootCounter;

        [DataMember]
        private int mDefenseCounter;

        [DataMember]
        private int mShotsDone;

        /// <summary>
        /// Constructs a kinetic (i.e. uses ammunition) defense platform that automatically attacks
        /// enemy units. This platform uses no energy but requires ammunition.
        /// </summary>
        internal DefenseKinetic(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            SpriteFont libSans12,
            ref Director director,
            bool friendly = true) : base(position,
            platformSpriteSheet,
            baseSprite,
            libSans12,
            ref director,
            EStructureType.Kinetic,
            friendly: friendly)
        {
            mSpritename = "Cones";
            mCost = GetResourceCosts(EStructureType.Kinetic);
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("KineticTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            mShootCounter = new TimeSpan(0, 0, 0, 0);
        }

        public override void Shoot(ICollider target)
        {
            if (!mDefenseAction.CanShoot())
            {
                return;
            }

            mShoot = true;
            mDirector.GetSoundManager.PlaySound(mSoundId);
        }

        public override void Update(GameTime t)
        {
            base.Update(t);

            //Requesting Metal to shoot
            if (mAmmoRequested > 0 && mResources.Count > 0 && mResources.Any(r => r.Type == EResourceType.Metal))
            {
                var res = GetResource(EResourceType.Metal);
                if (res.IsPresent())
                {
                    mDefenseAction.FillAmmo();
                }
            }

            if (mDefenseAction.AmmoCount <= 30 && mAmmoRequested < 2)
            {
                mDirector.GetDistributionDirector.GetManager(GetGraphIndex())
                    .RequestResource(this, EResourceType.Metal, null);
                mAmmoRequested++;
            }

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
            mShootCounter = mShootCounter.Add(t.ElapsedGameTime);
            //No defending units = no Shots
            if (mDefenseCounter == 0)
            {
                return;
            }
            //Shooting according to attackspeed
            if (mShootCounter.TotalMilliseconds > 2000 / mDefenseCounter * mShotsDone)
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
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("KineticTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            base.ReloadContent(content, ref dir);
        }
    }
}
