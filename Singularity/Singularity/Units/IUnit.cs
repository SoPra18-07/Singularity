using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;

namespace Singularity.Units
{
    interface IUnit : ISpatial
    {

        /// <summary>
        /// get method for unit ID
        /// </summary>
        int Id{ get; }

        /// <summary>
        /// Type will eventually be Assignment type
        /// </summary>
        String Assignment { get; set; }
    }
}
