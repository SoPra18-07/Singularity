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
        private Map.Map mMap;

        private List<FreeMovingUnit> Units;
        private List<IDamageable> Damageables;
        private List<EnemyUnit> EnemyUnits;
        private List<DefenseBase> DefensePlatforms;

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
        }

        internal void AddDefensePlatform(DefenseBase platform)
        {
            DefensePlatforms.Add(platform);
        }

        internal void AddPlatform(PlatformBlank platform)
        {
            Damageables.Add(platform);
        }

        internal void AddUnit(Vector2 position, EUnitType type, bool friendly = true)
        {
            FreeMovingUnit unit;
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
