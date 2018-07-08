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
        private readonly List<MilitaryUnit> mFriendlyMilitary = new List<MilitaryUnit>();

        /// <summary>
        /// A list of friendly defense platforms.
        /// </summary>
        private readonly List<DefenseBase> mFriendlyDefensePlatforms = new List<DefenseBase>();

        /// <summary>
        /// A list of friendly targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private readonly List<PlatformBlank> mFriendlyPlatforms = new List<PlatformBlank>();

        /// <summary>
        /// A list of friendly settlers
        /// </summary>
        private List<Settler> mFriendlySettler = new List<Settler>();

        #endregion

        #region Hostile unit lists

        /// <summary>
        /// A list of hostile military (i.e. capable of shooting) units.
        /// </summary>
        private readonly List<EnemyUnit> mHostileMilitary = new List<EnemyUnit>();

        /// <summary>
        /// A list of hsotile defense platforms.
        /// </summary>
        private readonly List<DefenseBase> mHostileDefensePlatforms = new List<DefenseBase>();

        /// <summary>
        /// A list of hostile targets (i.e. platforms and settlers that can be damaged).
        /// </summary>
        private readonly List<PlatformBlank> mHostilePlatforms = new List<PlatformBlank>();

        /// <summary>
        /// A list of hostile settlers
        /// </summary>
        private List<Settler> mHostileSettler = new List<Settler>();

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

        /// <summary>
        /// Adds a new platform to the military manager. This also adds it to the UnitMap.
        /// </summary>
        /// <param name="platform">Any type of platform which can be damaged (i.e. all platforms)</param>
        internal void AddPlatform(PlatformBlank platform)
        {
            var defensePlatform = platform as DefenseBase;
            var position = mUnitMap.VectorToTilePos(platform.AbsolutePosition);

            // Figure out if it is a defense platform. If yes, then figure out if it is friendly
            if (defensePlatform != null)
            {
                if (platform.Friendly)
                {
                    mFriendlyDefensePlatforms.Add(defensePlatform);
                }
                else
                {
                    mHostileDefensePlatforms.Add(defensePlatform);
                }
            }
            // otherwise, if it is friendly, add to friendly list.
            else if (platform.Friendly)
            {
                mFriendlyPlatforms.Add(platform);
            }
            // finally, if not, then add to hostile list.
            else
            {
                mHostilePlatforms.Add(platform);
            }

            mUnitMap.AddUnit(platform, position);
        }

        /// <summary>
        /// Adds a FreeMovingUnit object to the military manager. This also adds it to the UnitMap.
        /// </summary>
        /// <param name="unit">The unit to be added to the manager.</param>
        internal void AddUnit(FreeMovingUnit unit)
        {
            var friendlyMilitary = unit as MilitaryUnit;
            var hostileMilitary = unit as EnemyUnit;
            var settler = unit as Settler;

            if (settler != null)
            {
                if (settler.Friendly)
                {
                    mFriendlySettler.Add(settler);
                }
                else
                {
                    mHostileSettler.Add(settler);
                }
            }

            else if (friendlyMilitary != null)
            {
                mFriendlyMilitary.Add(friendlyMilitary);
            }
            else if (hostileMilitary != null)
            {
                mHostileMilitary.Add(hostileMilitary);
            }

            mUnitMap.RemoveUnit(unit);
        }

        /// <summary>
        /// Removes a platform from the Military Manager (i.e. it died).
        /// </summary>
        /// <param name="platform">The platform to be removed.</param>
        internal void RemovePlatform(PlatformBlank platform)
        {
            var defensePlatform = platform as DefenseBase;

            // Figure out if it is a defense platform. If yes, then figure out if it is friendly
            if (defensePlatform != null)
            {
                if (platform.Friendly)
                {
                    mFriendlyDefensePlatforms.Remove(defensePlatform);
                }
                else
                {
                    mHostileDefensePlatforms.Remove(defensePlatform);
                }
            }
            // otherwise, if it is friendly, add to friendly list.
            else if (platform.Friendly)
            {
                mFriendlyPlatforms.Remove(platform);
            }
            // finally, if not, then add to hostile list.
            else
            {
                mHostilePlatforms.Remove(platform);
            }

            mUnitMap.RemoveUnit(platform);
        }

        /// <summary>
        /// Removes a unit from the Military Manager (i.e. it died).
        /// </summary>
        /// <param name="unit">The unit to be removed.</param>
        internal void RemoveUnit(FreeMovingUnit unit)
        {
            var friendlyMilitary = unit as MilitaryUnit;
            var hostileMilitary = unit as EnemyUnit;
            var settler = unit as Settler;

            if (settler != null)
            {
                if (settler.Friendly)
                {
                    mFriendlySettler.Remove(settler);
                }
                else
                {
                    mHostileSettler.Remove(settler);
                }
            }

            else if (friendlyMilitary != null)
            {
                mFriendlyMilitary.Remove(friendlyMilitary);
            }
            else if (hostileMilitary != null)
            {
                mHostileMilitary.Remove(hostileMilitary);
            }

            mUnitMap.RemoveUnit(unit);
        }
    


        public void Update(GameTime gametime)
        {
            foreach (var unit in mFriendlyMilitary)
            {
                // iterate through each friendly unit, if there's a target nearby, shoot it.
                if (unit.Moved)
                {
                    mUnitMap.MoveUnit(unit);
                }

                var adjacentUnits = mUnitMap.GetAdjacentUnits(unit.AbsolutePosition);
                foreach (var adjacentUnit in adjacentUnits)
                {
                    if (adjacentUnit.Friendly != true)
                    {
                        if (M)
                    }
                }
            }

            foreach (var turret in mFriendlyDefensePlatforms)
            {
                // iterate through each friendly turret, if there's a target nearby, shoot it.
            }

            foreach (var unit in HostileMilitary)
            {
                // iterate through each hostile unit, if there's a target nearby, shoot it.
            }

            foreach (var turret in HostileDefensePlatforms)
            {
                // iterate through each hostile turret, if there's a target nearby, shoot it.
            }
        }
    }
}
