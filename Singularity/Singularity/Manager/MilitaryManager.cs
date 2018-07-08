using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Manager
{
    public sealed class MilitaryManager : IUpdate
    {
        /// <summary>
        /// The game map.
        /// </summary>
        private Map.Map mMap;

        #region Friendly unit lists

        /// <summary>
        /// A list of friendly military (i.e. capable of shooting) units.
        /// </summary>
        private List<IShooting> FriendlyShooters;

        /// <summary>
        /// A list of friendly targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private List<IDamageable> FriendlyDamageables;

        #endregion

        #region Hostile unit lists

        /// <summary>
        /// A list of hostile military (i.e. capable of shooting) units.
        /// </summary>
        private List<IShooting> HostileShooters;
        
        /// <summary>
        /// A list of hostile targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private List<IDamageable> HostileDamageables;

        #endregion

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
        }

        internal void AddDefensePlatform(DefenseBase platform, bool friendly = true)
        {
            if (friendly)
            {

            }
        }

        internal void AddPlatform(PlatformBlank platform)
        {
            if (platform.Friendly)
            {
                FriendlyDamageables.Add(platform);
            }
        }

        internal void AddUnit(FreeMovingUnit unit, EUnitType type)
        {
            switch (type)
            {
                case EUnitType.Settler:
                    
                    break;
                case EUnitType.StandardFriendly:
                    break;
                case EUnitType.FastFriendly:
                    break;
                case EUnitType.HeavyFriendly:
                    break;
                case EUnitType.StandardEnemy:
                    break;
                case EUnitType.FastEnemy:
                    break;
                case EUnitType.HeavyEnemy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "That's not a possible unit type.");
            }
        }
    


        public void Update(GameTime gametime)
        {
            
        }
    }
}
