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
        
        internal int AmmoCount { get; private set; }

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
            if (AmmoCount < 1)
            {
                mDirector.GetDistributionManager.RequestResource(this, EResourceType.Metal, null);
            }
            else
            {
                AmmoCount--;
                mShoot = true;
                mEnemyPosition = target;
            }
        }
    }
}
