using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Resources;

namespace Singularity.Platforms
{
    internal sealed class DefenseKinetic : DefenseBase
    {

        /// <summary>
        /// Stores the amount of ammunition the platform currently has. Max is 50.
        /// </summary>
        internal int AmmoCount { get; private set; }

        /// <summary>
        /// State of the platform.
        /// </summary>
        /// 0 means not requesting
        /// 1 means requesting
        /// 2 means has ammo and not requesting
        /// 3 means full ammo and currently requesting
        private bool mRequesting = false;

        private int mTotalMaterialsRequested;

        private double mLastRequestTime;

        /// <summary>
        /// Constructs a kinetic (i.e. uses ammunition) defense platform that automatically attacks
        /// enemy units. This platform uses no energy but requires ammunition
        /// </summary>
        internal DefenseKinetic(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            ref Director director)
            : base(position, platformSpriteSheet, baseSprite, ref director, EPlatformType.Kinetic)
        {
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
        }

        public override void Shoot(Vector2 target)
        {
            // TODO: See if any unit is here first to be able to shoot
            AmmoCount--;
            mShoot = true;
            mEnemyPosition = target;
        }
        // TODO: Some way to calculate how many things are sent here

        public override void Update(GameTime t)
        {
            if (!mRequesting)
            {
                if (AmmoCount == 0
                    || (t.TotalGameTime.TotalMilliseconds - mLastRequestTime > 30000
                        && AmmoCount < 50))
                {
                    // TODO: Change metal to ammo once armory is implemented
                    mDirector.GetDistributionManager.RequestResource(this, EResourceType.Metal, null);
                    mTotalMaterialsRequested++;
                    mRequesting = true;
                }
            }

            if (mRequesting && mTotalMaterialsRequested <= 50)
            {
                mDirector.GetDistributionManager.RequestResource(this, EResourceType.Metal, null);
            }

            if (mTotalMaterialsRequested == 50)
            {
                mRequesting = false;
                mLastRequestTime = t.TotalGameTime.TotalMilliseconds;
            }
        }
    }
}
