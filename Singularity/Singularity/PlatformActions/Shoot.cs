using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    [DataContract]
    internal sealed class Shoot : APlatformAction
    {
        /// <summary>
        /// Stores the amount of ammunition the platform currently has. Max is 50.
        /// </summary>
        public int AmmoCount { get; private set; }
        
        [DataMember]
        private new DefenseBase mPlatform;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")] // we actually want only DefensePlatforms to be able to shoot. Everything else is too generic though.
        public Shoot(DefenseBase platform, ref Director director) : base(platform, ref director)
        {
            AmmoCount = 10;
        }

        public override void Execute()
        {
            if (AmmoCount <= 0) return;
            AmmoCount--;
            ((DefenseBase) mPlatform).Shoot();
        }

        public bool CanShoot()
        {
            AmmoCount--;
            return AmmoCount >= 0;
        }

        public void FillAmmo()
        {
            if (mPlatform.mType == EStructureType.Kinetic)
            {
                var res = mPlatform.GetResource(EResourceType.Metal);
                if (res.IsPresent())
                {
                    AmmoCount += 20;
                }
            }
            if (mPlatform.mType == EStructureType.Laser)
            {
                AmmoCount += 5;
            }
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    State = PlatformActionState.Available;
                    break;
                case PlatformActionState.Deactivated:
                case PlatformActionState.Disabled:
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
        
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> {JobType.Defense};

        public override void Update(GameTime t)
        {
            // do nothing. Shooting is happening in the platform itself, as is Aiming etc.
        }
    }
}
