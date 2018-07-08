using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Singularity.Map;
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

        private UnitMap mUnitMap;

        #region Friendly unit lists

        /// <summary>
        /// A list of friendly military (i.e. capable of shooting) units.
        /// </summary>
        private Dictionary<MilitaryUnit, Vector2> FriendlyMilitary = new Dictionary<MilitaryUnit, Vector2>();

        /// <summary>
        /// A list of friendly defense platforms.
        /// </summary>
        private Dictionary<DefenseBase, Vector2> FriendlyDefensePlatforms = new Dictionary<DefenseBase, Vector2>();

        /// <summary>
        /// A list of friendly targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private Dictionary<IDamageable, Vector2> FriendlyDamageables;

        #endregion

        #region Hostile unit lists

        /// <summary>
        /// A list of hostile military (i.e. capable of shooting) units.
        /// </summary>
        private Dictionary<EnemyUnit, Vector2> HostileMilitary = new Dictionary<EnemyUnit, Vector2>();

        /// <summary>
        /// A list of hostile defense platforms.
        /// </summary>
        private Dictionary<DefenseBase, Vector2> HostileDefensePlatforms = new Dictionary<DefenseBase, Vector2>();
        
        /// <summary>
        /// A list of hostile targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private Dictionary<IDamageable, Vector2> HostileDamageables;

        #endregion
        /// <remarks>
        /// The way the MilitaryManager works is that it stores which 2x2 map tile a unit is currently located on. This way,
        /// the MilitaryManager only has to check for all units which are in the same or adjacent 2x2 map tiles for detection.
        /// As such, it won't need to check if 2 units on opposite ends of the map are near enough to each other that it can attack them.
        /// </remarks>

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
            mUnitMap = map.GetUnitMap();
        }

        internal void AddPlatform(PlatformBlank platform)
        {
            var defensePlatform = platform as DefenseBase;
            var position = mUnitMap.VectorToTilePos(platform.AbsolutePosition);

            // Figure out if it is a defense platform. If yes, then figure out if it is friendly
            if (defensePlatform != null)
            {
                if (platform.Friendly)
                {
                    FriendlyDefensePlatforms.Add(defensePlatform, position);
                }
                else
                {
                    HostileDefensePlatforms.Add(defensePlatform, position);
                }
            }
            // otherwise, if it is friendly, add to friendly list.
            else if (platform.Friendly)
            {
                FriendlyDamageables.Add(platform, position);
            }
            // finally, if not, then add to hostile list.
            else
            {
                HostileDamageables.Add(platform, position);
            }

            mUnitMap.AddUnit(platform, position);
        }

        internal void AddUnit(FreeMovingUnit unit)
        {
            var friendlyMilitary = unit as MilitaryUnit;
            var hostileMilitary = unit as EnemyUnit;
            var settler = unit as Settler;

            var position = mUnitMap.VectorToTilePos()

            if (settler != null)
            {
                FriendlyDamageables.Add(settler, );
            }

            else if (friendlyMilitary != null)
            {
                FriendlyMilitary.Add(friendlyMilitary);
            }
            else if (hostileMilitary != null)
            {
                HostileMilitary.Add(hostileMilitary);
            }
            
        }
    


        public void Update(GameTime gametime)
        {
            
        }
    }
}
