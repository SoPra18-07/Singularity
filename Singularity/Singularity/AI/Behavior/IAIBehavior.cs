using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;
using Microsoft.Xna.Framework;

namespace Singularity.AI.Behavior
{
    /// <summary>
    /// An interface for all possible AI behaviors.
    /// </summary>
    interface IAIBehavior
    {
        /// <summary>
        /// Moves the units in a specific way specified by the implementation
        /// </summary>
        /// <param name="gametime">A snapshot of timing values</param>
        void Move(GameTime gametime);

        /// <summary>
        /// Spawns units in a specific way specified by the implementation
        /// </summary>
        /// <param name="gametime">A snapshot of timing values</param>
        void Spawn(GameTime gametime);
    }
}
