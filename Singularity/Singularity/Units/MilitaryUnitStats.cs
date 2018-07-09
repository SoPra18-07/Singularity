using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Singularity.Units
{
    internal static class MilitaryUnitStats
    {
        #region Standard Units
        /// <summary>
        /// Standard military unit speed.
        /// </summary>
        internal static int StandardSpeed { get; } = 4;

        /// <summary>
        /// Standard military unit health.
        /// </summary>
        internal static int StandardHealth { get; } = 10;

        /// <summary>
        /// Standard military unit range.
        /// </summary>
        internal static int StandardRange { get; } = 300;

        /// <summary>
        /// Standard military unit scaling.
        /// </summary>
        internal static float StandardScale { get; } = 0.4f;

        #endregion

        #region Fast Units
        /// <summary>
        /// Fast military unit speed.
        /// </summary>
        internal static int FastSpeed { get; } = 8;

        /// <summary>
        /// Fast military unit health.
        /// </summary>
        internal static int FastHealth { get; } = 5;

        /// <summary>
        /// Fast military unit range.
        /// </summary>
        internal static int FastRange { get; } = 300;

        #endregion

        #region Heavy Units
        /// <summary>
        /// Heavy military unit speed.
        /// </summary>
        internal static int HeavySpeed { get; } = 2;

        /// <summary>
        /// Heavy military unit health.
        /// </summary>
        internal static int HeavyHealth { get; } = 20;

        /// <summary>
        /// Heavy military unit range.
        /// </summary>
        internal static int HeavyRange { get; } = 300;

        #endregion

        #region Damage Stats

        /// <summary>
        /// The strength of military units (i.e. how much damage it does to objects).
        /// </summary>
        internal static int UnitStrength = 1;

        /// <summary>
        /// The strength of turrets (i.e. how much damage it does to objects).
        /// </summary>
        internal static int TurretStrength = 2;

        #endregion
    }
}
