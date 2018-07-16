using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Units;

namespace Singularity.Map
{
    internal sealed class UnitMap : IUpdate
    {
        /// <summary>
        /// Stores which units are in which grid position on the map.
        /// </summary>
        private readonly UnitMapTile[,] mUnitGrid;
        private readonly Dictionary<int, Vector2> mLookupTable;
        private readonly int mMapSizeX;

        /// <summary>
        /// Constructs a unit map which stores a grid with all free moving units on it.
        /// </summary>
        /// <param name="mapSizeX">Width of the map in number of tiles.</param>
        /// <param name="mapSizeY">Height of the map in number of tiles.</param>
        /// <remarks>
        /// The way the unit map works is that it stores which 2x2 map tile a unit is currently located on. This way,
        /// the MilitaryManager only has to check for all units which are in the same or adjacent 2x2 map tiles for detection.
        /// As such, it won't need to check if 2 units on opposite ends of the map are near each other.
        /// </remarks>
        internal UnitMap(int mapSizeX, int mapSizeY)
        {
            mLookupTable = new Dictionary<int, Vector2>();
            // + 1 Because the Unit grid is an array and it can happen that there is an Indexoutofboundsexception
            // when a unit is moving at the corner
            mUnitGrid = new UnitMapTile[mapSizeX / 2 + 1, mapSizeY / 2 + 1];

            for (var i = 0; i < mUnitGrid.GetLength(0); i++)
            {
                for (var j = 0; j < mUnitGrid.GetLength(1); j++)
                {
                    mUnitGrid[i, j] = new UnitMapTile();
                }
            }

            mMapSizeX = mapSizeX;
        }

        /// <summary>
        /// Adds a unit to the UnitMap.
        /// </summary>
        /// <param name="unit"></param>
        internal void AddUnit(ICollider unit)
        {
            // first calculate which tile the unit is on
            var pos = VectorToTilePos(unit.AbsolutePosition);

            AddUnit(unit, pos);
        }

        /// <summary>
        /// Adds a unit to the UnitMap with a precalculated tile position.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="unitPos">Precalculated tile position for optimization.</param>
        internal void AddUnit(ICollider unit, Vector2 unitPos)
        {
            // then put the unit on the grid
            mUnitGrid[(int) unitPos.X, (int) unitPos.Y].UnitList.Add(unit);

            // and put the unit in the lookup table
            mLookupTable.Add(unit.Id, unitPos);
        }

        /// <summary>
        /// Moves a unit that already exists on the UnitMap.
        /// </summary>
        /// <param name="unit"></param>
        internal void MoveUnit(ICollider unit)
        {
            // first check if the unit moved out of its current tile
            var newPos = VectorToTilePos(unit.AbsolutePosition);
            var oldPos = mLookupTable[unit.Id];
            if (newPos.Equals(oldPos))
            {
                return;
            }
            // if yes, then delete the reference to the unit in both the lookup table and the grid
            RemoveUnit(unit, oldPos);

            // then readd the unit
            AddUnit(unit, newPos);
        }

        /// <summary>
        /// Removes a unit from the UnitMap.
        /// </summary>
        /// <param name="unit">The unit to be removed.</param>
        internal void RemoveUnit(ICollider unit)
        {
            var position = VectorToTilePos(unit.AbsolutePosition);

            RemoveUnit(unit, position);
        }

        /// <summary>
        /// Removes a unit from the UnitMap with a precalculated tile position.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="unitPos">Precalculated tile position for optimization.</param>
        internal void RemoveUnit(ICollider unit, Vector2 unitPos)
        {
            for (var x = 0; x < 31; x++)
            {
                for (var y = 0; y < 31; y++)
                {
                    if (mUnitGrid[x, y].UnitList.Contains(unit) && unit is Quarry)
                    {
                        Console.Out.WriteLine(x + " " + y);
                    }
                }
            }
            mUnitGrid[(int) unitPos.X, (int) unitPos.Y].UnitList.Remove(unit);
            mLookupTable.Remove(unit.Id);
        }

        /// <summary>
        /// Returns all units in and in adjacent tile around it.
        /// </summary>
        /// <param name="tilePosition">Tile position whose units and adjacent units is requested.</param>
        /// <returns>List of all free moving units in adjacent tiles.</returns>
        internal List<ICollider> GetAdjacentUnits(Vector2 tilePosition)
        {
            var centerTile = VectorToTilePos(tilePosition);

            // checks to see if adjacent tiles exist
            var n = centerTile.Y >= 1;
            var s = centerTile.Y <= mUnitGrid.GetLength(1) - 2;
            var w = centerTile.X >= 1;
            var e = centerTile.X <= mUnitGrid.GetLength(0) - 2;

            var nw = n && w;
            var ne = n && e;
            var sw = s && w;
            var se = s && e;

            var unitList = new List<ICollider>();

            unitList.AddRange(mUnitGrid[(int)centerTile.X, (int)centerTile.Y].UnitList);

            // ↑
            if (n)
            {
                unitList.AddRange(mUnitGrid[(int) centerTile.X, (int) centerTile.Y - 1].UnitList);
            }
            // ↓
            if (s)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X, (int)centerTile.Y + 1].UnitList);
            }
            // ←
            if (w)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X - 1, (int)centerTile.Y].UnitList);
            }
            // →
            if (e)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X + 1, (int)centerTile.Y].UnitList);
            }
            // ↖
            if (nw)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X - 1, (int)centerTile.Y - 1].UnitList);
            }
            // ↗
            if (ne)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X + 1, (int)centerTile.Y - 1].UnitList);
            }
            // ↙
            if (sw)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X - 1, (int)centerTile.Y + 1].UnitList);
            }
            // ↘
            if (se)
            {
                unitList.AddRange(mUnitGrid[(int)centerTile.X + 1, (int)centerTile.Y + 1].UnitList);
            }

            return unitList;
        }

        /// <summary>
        /// Converts an absolute Vector2 map position to its corresponding UnitMap tile (which is a 2x2 map tile).
        /// </summary>
        /// <param name="vector">Vector2 to be converted</param>
        /// <returns>Corresponding tile position</returns>
        internal Vector2 VectorToTilePos(Vector2 vector)
        {
            int[] squareTilePos = {(int) vector.X / 200, (int) vector.Y / 100};

            var halfWidth = mMapSizeX / 2 - 1;

            var column = (squareTilePos[0] + squareTilePos[1] - halfWidth) / 2;
            var row = Math.Abs(Math.Abs(squareTilePos[0] - halfWidth) - column);

            return new Vector2(column, row);

        }

        public void Update(GameTime gametime)
        {
            var movedUnitList = new List<ICollider>();
            foreach (var unitMapTile in mUnitGrid)
            {
                foreach (var unit in unitMapTile.UnitList)
                {
                    if (unit.Moved)
                    {
                        movedUnitList.Add(unit);
                    }
                }
            }

            foreach (var unit in movedUnitList)
            {
                MoveUnit(unit);
            }
        }
    }
}
