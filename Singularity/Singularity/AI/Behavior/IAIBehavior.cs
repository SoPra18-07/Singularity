﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;
using Microsoft.Xna.Framework;
using Singularity.Manager;

namespace Singularity.AI.Behavior
{
    /// <summary>
    /// An interface for all possible AI behaviors.
    /// </summary>
    interface IAiBehavior
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

        /// <summary>
        /// Needed for reloading the references etc. after deserializing
        /// </summary>
        /// <param name="dir"></param>
        void ReloadContent(ref Director dir);
    }
}