using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Manager
{

    /// <remarks>
    /// The way the MilitaryManager works is that it stores which 2x2 map tile a unit is currently located on. This way,
    /// the MilitaryManager only has to check for all units which are in the same or adjacent 2x2 map tiles for detection.
    /// As such, it won't need to check if 2 units on opposite ends of the map are near enough to each other that it can attack them.
    /// </remarks>
    [DataContract]
    public sealed class MilitaryManager : IUpdate
    {
        /// <summary>
        /// The game unitMap that is used to keep track of all damageable objects.
        /// </summary>
        private UnitMap mUnitMap;

        /// <summary>
        /// Required because it holds a reference to the unit.
        /// </summary>
        private Map.Map mMap;

        #region Friendly unit lists

        /// <summary>
        /// A list of friendly military (i.e. capable of shooting) units.
        /// </summary>
        [DataMember]
        private readonly List<MilitaryUnit> mFriendlyMilitary = new List<MilitaryUnit>();

        /// <summary>
        /// A list of friendly defense platforms.
        /// </summary>
        [DataMember]
        private readonly List<DefenseBase> mFriendlyDefensePlatforms = new List<DefenseBase>();

        #endregion

        #region Hostile unit lists

        /// <summary>
        /// A list of hostile military (i.e. capable of shooting) units.
        /// </summary>
        [DataMember]
        private readonly List<EnemyUnit> mHostileMilitary = new List<EnemyUnit>();

        /// <summary>
        /// A list of hsotile defense platforms.
        /// </summary>
        [DataMember]
        private readonly List<DefenseBase> mHostileDefensePlatforms = new List<DefenseBase>();

        #endregion

        private Director mDirector;

        /// <summary>
        /// The total number of military units on the map.
        /// </summary>
        [DataMember]
        internal int TotalUnitCount { get; private set; }



        internal MilitaryManager(Director director)
        {
            mDirector = director;
        }

        /// <summary>
        /// Sets the unit map for referencing later on. This is required because the map is created
        /// after the director so it cannot be included in the constructor.
        /// </summary>
        internal void SetMap(ref Map.Map map)
        {
            mUnitMap = new UnitMap((int)map.GetMeasurements().X, (int)map.GetMeasurements().Y);
            mMap = map;
        }

        public void ReloadContent(Vector2 mapmeasurements, Director director)
        {
            mDirector = director;
        }

        public void ReloadSetMap(ref Map.Map map)
        {
            mMap = map;
            mUnitMap = new UnitMap((int) map.GetMeasurements().X, (int) map.GetMeasurements().Y);
            foreach (var funit in mFriendlyMilitary)
            {
                mUnitMap.AddUnit(funit);
            }

            foreach (var fplatform in mFriendlyDefensePlatforms)
            {

                mUnitMap.AddUnit(fplatform);
            }

            foreach (var hunit in mHostileMilitary)
            {
                mUnitMap.AddUnit(hunit);
            }

            foreach (var hplatform in mHostileDefensePlatforms)
            {
                mUnitMap.AddUnit(hplatform);
            }
        }

        #region Methods to add objects to the manager

        /// <summary>
        /// Adds a new platform to the military manager. This also adds it to the UnitMap.
        /// </summary>
        /// <param name="platform">Any type of platform which can be damaged (i.e. all platforms)</param>
        internal void AddPlatform(PlatformBlank platform)
        {
            var defensePlatform = platform as DefenseBase;
            var position = mUnitMap.VectorToTilePos(platform.AbsolutePosition);

            // Figure out if it is a defense platform. If yes, then figure out its allegiance then
            // add it to the appropriate list.
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

            // Then add it to the unitMap.
            mUnitMap.AddUnit(platform, position);
        }

        /// <summary>
        /// Adds a FreeMovingUnit object to the military manager. This also adds it to the UnitMap.
        /// </summary>
        /// <param name="unit">The unit to be added to the manager.</param>
        internal void AddUnit(FreeMovingUnit unit)
        {
            //I dont know who made the inheritance but it fucked everything up. The Enemy unit is now a Military unit too
            //And that means the enemy unit is also added to the friendlymilitarylist lol.
            //Hotfix: I just ask for both casts to be not null.
            var friendlyMilitary = unit as MilitaryUnit;
            var friendlySettler = unit as Settler;
            var hostileMilitary = unit as EnemyUnit;

            if (friendlyMilitary != null && hostileMilitary == null)
            {
                mFriendlyMilitary.Add(friendlyMilitary);
            }
            else if (hostileMilitary != null)
            {
                mHostileMilitary.Add(hostileMilitary);
            }

            mUnitMap.AddUnit(unit);
            if (friendlySettler == null)
            {
                TotalUnitCount++;
            }
        }

        #endregion

        #region Methods to remove from the manager

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

            mUnitMap.RemoveUnit(platform);
        }

        /// <summary>
        /// Removes a unit from the Military Manager (i.e. it died).
        /// </summary>
        /// <param name="unit">The unit to be removed.</param>
        internal void RemoveUnit(ICollider unit)
        {
            var friendlyMilitary = unit as MilitaryUnit;
            var hostileMilitary = unit as EnemyUnit;

            if (friendlyMilitary != null)
            {
                mFriendlyMilitary.Remove(friendlyMilitary);
            }
            else if (hostileMilitary != null)
            {
                mHostileMilitary.Remove(hostileMilitary);
            }

            mUnitMap.RemoveUnit(unit);
            TotalUnitCount--;
        }

        #endregion


        public void Update(GameTime gametime)
        {
            var unitsToKill = new List<FreeMovingUnit>();
            var platformsToKill = new List<PlatformBlank>();

            #region Check targets for friendly units

            foreach (var unit in mFriendlyMilitary)
            {
                // iterate through each friendly unit, if there's a target nearby, shoot the closest one.
                if (unit.Moved)
                {
                    mUnitMap.MoveUnit(unit);
                }

                // get all the adjacent units
                var adjacentUnits = mUnitMap.GetAdjacentUnits(unit.AbsolutePosition);

                // remember the closest adjacent unit.
                ICollider closestAdjacent = null;
                var closestAdjacentDistance =
                    1500f; // a separate variable is used so that it can be initalized with a very big value.

                // iterate through all adjacent units to find the closest adjacent unit.
                foreach (var adjacentUnit in adjacentUnits)
                {
                    // only calculate the distance to the adjacent unit if the unit is not friendly.
                    if (!adjacentUnit.Friendly)
                    {
                        // calculate the distance
                        var dist = Geometry.GetQuickDistance(unit.AbsolutePosition, adjacentUnit.AbsolutePosition);
                        // check if it's within range
                        if (dist < unit.Range)
                        {
                            // if yes, check if it's closer than the previous closest
                            if (dist < closestAdjacentDistance)
                            {
                                closestAdjacentDistance = dist;
                                closestAdjacent = adjacentUnit;
                            }
                        }
                    }

                }

                // if there is something close enough, shoot it. Else, set the target to null.
                if (closestAdjacent != null)
                {
                    unit.SetShootingTarget(closestAdjacent);
                    if (closestAdjacent.Health <= 0)
                    {
                        // try to turn the closest into a freemoving unit
                        var killUnit = closestAdjacent as FreeMovingUnit;
                        if (killUnit != null)
                        {
                            // if it works, add it to the unit kill list.
                            unitsToKill.Add(killUnit);
                        }
                        else
                        {
                            // otherwise it's a platform and add it to the platform kill list.
                            platformsToKill.Add(closestAdjacent as PlatformBlank);
                        }
                    }
                }
                
                else
                {
                    unit.SetShootingTarget(null);
                }
                //Debug.WriteLineIf(closestAdjacent != null, closestAdjacent);
            }

            #endregion

            #region Check targets for friendly turrets

            foreach (var turret in mFriendlyDefensePlatforms)
            {
                // iterate through each friendly turret, if there's a target nearby, shoot it.
                // get all the adjacent units
                var adjacentUnits = mUnitMap.GetAdjacentUnits(turret.AbsolutePosition);

                // remember the closest adjacent unit.
                ICollider closestAdjacent = null;
                var closestAdjacentDistance =
                    1500f; // a separate variable is used so that it can be initalized with a very big value.

                // iterate through all adjacent units to find the closest adjacent unit.
                foreach (var adjacentUnit in adjacentUnits)
                {
                    // only calculate the distance to the adjacent unit if the unit is not friendly.
                    if (!adjacentUnit.Friendly)
                    {
                        // calculate the distance
                        var dist = Geometry.GetQuickDistance(turret.AbsolutePosition, adjacentUnit.AbsolutePosition);
                        // check if it's within range
                        if (dist < turret.Range)
                        {
                            // if yes, check if it's closer than the previous closest
                            if (dist < closestAdjacentDistance)
                            {
                                closestAdjacentDistance = dist;
                                closestAdjacent = adjacentUnit;
                            }
                        }
                    }

                }

                // if there is something close enough, shoot it. Else, set the target to null.
                if (closestAdjacent != null)
                {
                    turret.SetShootingTarget(closestAdjacent);
                    if (closestAdjacent.Health <= 0)
                    {
                        // try to turn the closest into a freemoving unit
                        var killUnit = closestAdjacent as FreeMovingUnit;
                        if (killUnit != null)
                        {
                            // if it works, add it to the unit kill list.
                            unitsToKill.Add(killUnit);
                        }
                        else
                        {
                            // otherwise it's a platform and add it to the platform kill list.
                            platformsToKill.Add(closestAdjacent as PlatformBlank);
                        }
                    }
                }
                else
                {
                    turret.SetShootingTarget(null);
                }

            }

            #endregion

            #region Check targets for hostile units

            foreach (var unit in mHostileMilitary)
            {
                // iterate through each hostile unit, if there's a target nearby, shoot it.
                // iterate through each friendly unit, if there's a target nearby, shoot the closest one.
                if (unit.Moved)
                {
                    mUnitMap.MoveUnit(unit);
                }

                // get all the adjacent units
                var adjacentUnits = mUnitMap.GetAdjacentUnits(unit.AbsolutePosition);

                // remember the closest adjacent unit.
                ICollider closestAdjacent = null;
                var closestAdjacentDistance =
                    1500f; // a separate variable is used so that it can be initalized with a very big value.

                // iterate through all adjacent units to find the closest adjacent unit.
                foreach (var adjacentUnit in adjacentUnits)
                {
                    // only calculate the distance to the adjacent unit if the unit is friendly.
                    if (adjacentUnit.Friendly)
                    {
                        // calculate the distance
                        var dist = Geometry.GetQuickDistance(unit.AbsolutePosition, adjacentUnit.AbsolutePosition);
                        // check if it's within range
                        if (dist < unit.Range)
                        {
                            // if yes, check if it's closer than the previous closest
                            if (dist < closestAdjacentDistance)
                            {
                                closestAdjacentDistance = dist;
                                closestAdjacent = adjacentUnit;
                            }
                        }
                    }
                }

                // if there is something close enough, shoot it. Else, set the target to null.
                if (closestAdjacent != null)
                {
                    unit.SetShootingTarget(closestAdjacent);
                    if (closestAdjacent.Health <= 0)
                    {
                        // try to turn the closest into a freemoving unit
                        var killUnit = closestAdjacent as FreeMovingUnit;
                        if (killUnit != null)
                        {
                            // if it works, add it to the unit kill list.
                            unitsToKill.Add(killUnit);
                        }
                        else
                        {
                            // otherwise it's a platform and add it to the platform kill list.
                            platformsToKill.Add(closestAdjacent as PlatformBlank);
                        }
                    }
                }
                else
                {
                    unit.SetShootingTarget(null);
                }
            }

            #endregion

            #region Check targets for hostile turrets

            foreach (var turret in mHostileDefensePlatforms)
            {
                // iterate through each friendly turret, if there's a target nearby, shoot it.
                // get all the adjacent units
                var adjacentUnits = mUnitMap.GetAdjacentUnits(turret.AbsolutePosition);

                // remember the closest adjacent unit.
                ICollider closestAdjacent = null;
                var closestAdjacentDistance =
                    1500f; // a separate variable is used so that it can be initalized with a very big value.

                // iterate through all adjacent units to find the closest adjacent unit.
                foreach (var adjacentUnit in adjacentUnits)
                {
                    // only calculate the distance to the adjacent unit if the unit is friendly.
                    if (adjacentUnit.Friendly)
                    {
                        // calculate the distance
                        var dist = Geometry.GetQuickDistance(turret.AbsolutePosition, adjacentUnit.AbsolutePosition);
                        // check if it's within range
                        if (dist < turret.Range)
                        {
                            // if yes, check if it's closer than the previous closest
                            if (dist < closestAdjacentDistance)
                            {
                                closestAdjacentDistance = dist;
                                closestAdjacent = adjacentUnit;
                            }
                        }
                    }

                }

                // if there is something close enough, shoot it. Else, set the target to null.
                if (closestAdjacent != null)
                {
                    turret.SetShootingTarget(closestAdjacent);
                    if (closestAdjacent.Health <= 0)
                    {
                        // try to turn the closest into a freemoving unit
                        var killUnit = closestAdjacent as FreeMovingUnit;
                        if (killUnit != null)
                        {
                            // if it works, add it to the unit kill list.
                            unitsToKill.Add(killUnit);
                        }
                        else
                        {
                            // otherwise it's a platform and add it to the platform kill list.
                            platformsToKill.Add(closestAdjacent as PlatformBlank);
                        }
                    }
                }
                else
                {
                    turret.SetShootingTarget(null);
                }
            }

            #endregion

            #region Kill them

            var newUnitKillList = new List<FreeMovingUnit>();
            foreach (var unit in unitsToKill)
            {
                // tell the unit to die.
                unit.Die();
                mDirector.GetStoryManager.Level.GameScreen.RemoveObject(unit);
                mUnitMap.RemoveUnit(unit);
                RemoveUnit(unit);
                mMap.GetCollisionMap().RemoveCollider(unit);
            }

            foreach (var platform in platformsToKill)
            {
                RemovePlatform(platform);

                platform.Die();
            }
            #endregion
        }
    }
}
