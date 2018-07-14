using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
    internal class DefenseLaser : DefenseBase
    {
        [DataMember]
        private const int DrainingEnergy = 40;

        [DataMember]
        ICollider mEnemyPosition;

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
        }

        public override void Shoot(ICollider target)
        {
            /*
            if (IsActive()) {
                mShoot = true;
                mEnemyPosition = target;
                mDirector.GetSoundManager.PlaySound("LaserTowerShot", Center.X, Center.Y, 1f, 1f, true, false, SoundClass.Effect);
            }*/
        }

        public override void Update(GameTime time)
        {
            //Shoot for every unit thats present
            foreach (var unitbool in mAssignedUnits[JobType.Defense])
            {
                if (unitbool.GetSecond())
                {
                    Shoot(mEnemyPosition);
                }
            }
        }
    }
}
