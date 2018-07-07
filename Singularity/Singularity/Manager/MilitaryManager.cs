using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Property;
using Singularity.Units;

namespace Singularity.Manager
{
    public sealed class MilitaryManager : IUpdate
    {
        private Map.Map mMap;

        private List<MilitaryUnit> Units;
        private List<IDamageable> Damageables;
        private List<EnemyUnit> EnemyUnits;

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
            Damageables.AddRange(map.GetStructureMap().GetPlatformList());
        }

        internal void AddUnit(Vector2 position)
        {
            
        }


        public void Update(GameTime gametime)
        {
            
        }
    }
}
