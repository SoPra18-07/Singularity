using System;
using Microsoft.Xna.Framework;

namespace Singularity.Units
{
    interface IUnit
    {

        /// <summary>
        /// get method for unit ID
        /// </summary>
        int Id{ get; }

        /// <summary>
        /// get method for unit Position
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Type will eventually be Assignment type
        /// </summary>
        String Assignment { get; set; }
    }
}
