using System.Collections.Generic;
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
                    mDirector.GetSoundManager.PlaySound(mSoundId);
                }

                target.MakeDamage(MilitaryUnitStats.mTurretStrength);

            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            var shotspersecond = 0;
            //Increase attackspeed for every unit present. Only recalculate attackspeed once every second.
            if (mDirector.GetClock.GetShootingLaserTime().TotalMilliseconds > 1000)
            {
                foreach (var unitbool in mAssignedUnits[JobType.Defense])
                {
                    if (unitbool.GetSecond())
                    {
                        shotspersecond++;
                    }
                }
            }

            var shotsdone = 0;
            //Ask for friendly here because the sentinel handles the shooting on its own!
            //To the calculations: 1000 (so one second) is the maximum value of the shootinglaserticker.
            //We now just calculate whether that ticker is in a timespan where we are allowed to shoot again.
            //This formula very likely can be exploited by the player. But we have no time for that now.
            if (shotspersecond != 0 &&
                (1000 / shotspersecond * shotsdone <= mDirector.GetClock.GetShootingLaserTime().TotalMilliseconds
                 && mDirector.GetClock.GetShootingLaserTime().TotalMilliseconds <= 1000 / shotspersecond * shotsdone + 1) 
                && Friendly)
            {
                mDefenseAction.Execute();
                shotsdone++;
            }
        }

        public override void ReloadContent(ContentManager content, ref Director dir)
        {
            base.ReloadContent(content, ref dir);
            mSoundId = mDirector.GetSoundManager.CreateSoundInstance("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
        }
    }
}
