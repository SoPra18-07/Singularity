using System;
using Singularity.Property;

namespace Singularity.Units
{
    interface IUnit
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
