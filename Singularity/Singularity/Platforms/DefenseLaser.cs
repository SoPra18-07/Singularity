using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Property;
using Singularity.Resources;
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

            //Add Costs of the platform here if you got them.
            mCost = new Dictionary<EResourceType, int>();
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
        }


        public override void Shoot(ICollider target)
        {
            if (target != null)
            {
                if (IsActive())
                {
                    mShoot = true;
                    mDirector.GetSoundManager.PlaySound("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
                }

                target.MakeDamage(MilitaryUnitStats.mTurretStrength);

            }
        }

        public override void Update(GameTime time)
        {
            //IF YOU CHANGE THIS THRESHOLD CHANGE IT IN THE CLOCK, TOO. THIS DETERMINES THE ATTACKSPEED.
            //Ask for friendly here because the sentinel handles the shooting on its own!
            if (mDirector.GetClock.GetShootingLaserTime().TotalMilliseconds > 1000)
            {
                //Shoot for every unit thats present
                foreach (var unitbool in mAssignedUnits[JobType.Defense])
                {
                    if (unitbool.GetSecond())
                    {
                        mDefenseAction.Execute();
                    }
                }
            }
        }

        public override void ReloadContent(ContentManager content, ref Director dir)
        {
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            base.ReloadContent(content, ref dir);
        }
    }
}
