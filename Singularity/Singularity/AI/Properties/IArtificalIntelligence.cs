﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.AI.Properties
{
    /// <summary>
    /// An interface for all AI implementations.
    /// </summary>
    public interface IArtificalIntelligence : IUpdate
    {
        /// <summary>
        /// Gets all the spawner platforms the AI currently has at its disposal
        /// </summary>
        /// <returns></returns>
        IEnumerable<Spawner> GetSpawners();

        /// <summary>
        /// Gets the difficulty of the AI
        /// </summary>
        [DataMember]
        EaiDifficulty Difficulty { get; }

        /// <summary>
        /// This is needed to reload the Ai propertys/fields after deserializing
        /// </summary>
        void ReloadContent(ref Director dir);
    }
}