using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Sound;

namespace Singularity.Platforms
{
    internal sealed class DefenseKinetic : DefenseBase
    {
        
        /// <summary>
        /// Stores how many times materials have been requested between states
        /// </summary>
        private int mAmmoRequested;

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
            EPlatformType.Kinetic,
            friendly)
        {
            // Todo: Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int> {{EResourceType.Metal, 1}};
        }

        public override void Shoot(ICollider target)
        {
            if (!mDefenseAction.CanShoot()) return;
            mShoot = true;
            mDirector.GetSoundManager.PlaySound("KineticTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
        }

        public override void Update(GameTime t)
        {

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
        }
    }
}
