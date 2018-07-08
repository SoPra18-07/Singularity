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
        private Dictionary<MilitaryUnit, int[]> FriendlyMilitary;

        /// <summary>
        /// A list of friendly defense platforms.
        /// </summary>
        private Dictionary<DefenseBase, int[]> FriendlyDefensePlatforms;

        /// <summary>
        /// A list of friendly targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private List<IDamageable> FriendlyDamageables;

        #endregion

        #region Hostile unit lists

        /// <summary>
        /// A list of hostile military (i.e. capable of shooting) units.
        /// </summary>
        private List<EnemyUnit> HostileMilitary;

        /// <summary>
        /// A list of hostile defense platforms.
        /// </summary>
        private List<DefenseBase> HostileDefensePlatforms;
        
        /// <summary>
        /// A list of hostile targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private List<IDamageable> HostileDamageables;

        #endregion
        /// <remarks>
        /// The way the MilitaryManager works is that it stores which 2x2 map tile a unit is currently located on. This way,
        /// the MilitaryManager only has to check for all units which are in the same or adjacent 2x2 map tiles for detection.
        /// As such, it won't need to check if 2 units on opposite ends of the map are near enough to each other that it can attack them.
        /// </remarks>

        internal void SetMap(ref Map.Map map)
        {
            mMap = map;
        }

        internal void AddPlatform(PlatformBlank platform)
        {
            var defensePlatform = platform as DefenseBase;

            // Figure out if it is a defense platform. If yes, then figure out if it is friendly
            if (defensePlatform != null)
            {
                if (platform.Friendly)
                {
                    FriendlyDefensePlatforms.Add(defensePlatform);
                }
                else
                {
                    HostileDefensePlatforms.Add(defensePlatform);
                }
            }
            // otherwise, if it is friendly, add to friendly list.
            else if (platform.Friendly)
            {
                FriendlyDamageables.Add(platform);
            }
            // finally, if not, then add to hostile list.
            else
            {
                HostileDamageables.Add(platform);
            }
        }

        internal void AddUnit(FreeMovingUnit unit)
        {
            var friendlyMilitary = unit as MilitaryUnit;
            var hostileMilitary = unit as EnemyUnit;
            var settler = unit as Settler;

            if (settler != null)
            {
                FriendlyDamageables.Add(settler);
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
