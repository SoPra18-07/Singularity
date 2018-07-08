﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
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
        private bool mRequesting = false;

        /// <summary>
        /// Stores how many times materials have been requested between states
        /// </summary>
        private int mTotalMaterialsRequested;

        /// <summary>
        /// Stores when the last material was requested so it will periodically request to reload to
        /// maximum ammo.
        /// </summary>
        private double mLastRequestTime;

        /// <summary>
        /// Constructs a kinetic (i.e. uses ammunition) defense platform that automatically attacks
        /// enemy units. This platform uses no energy but requires ammunition.
        /// </summary>
        internal DefenseKinetic(Vector2 position,
            Texture2D platformSpriteSheet,
            Texture2D baseSprite,
            ref Director director,
            bool friendly = true)
            : base(position, platformSpriteSheet, baseSprite, ref director, EPlatformType.Kinetic, friendly: friendly)
        {
            //Add possible Actions in this array
            mIPlatformActions.Add(new Shoot(platform: this, director: ref mDirector));
            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
        }

        public override void Shoot(Vector2 target)
        {
            // TODO: See if any unit is here first to be able to shoot
            AmmoCount--;
            mShoot = true;
            EnemyPosition = target;

            mIPlatformActions[0].Execute();
        }
        // TODO: Some way to calculate how many things are sent here

        public override void Update(GameTime t)
        {
            // TODO: Impelement using platform actions (I'm not sure how to do this)
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

            if (mRequesting && mTotalMaterialsRequested + AmmoCount <= 50)
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
