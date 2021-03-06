﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.AI.Properties
{
    /// <summary>
    /// An interface for all AI implementations.
    /// </summary>
    public interface IArtificalIntelligence : IUpdate, IDraw
    {
        /// <summary>
        /// Gets all the spawner platforms the AI currently has at its disposal
        /// </summary>
        /// <returns></returns>
        Dictionary<int, List<Spawner>> GetSpawners();

        /// <summary>
        /// Gets the difficulty of the AI
        /// </summary>
        [DataMember]
        EaiDifficulty Difficulty { get; }

        [DataMember]
        bool IsTutorial { get; set; }

        /// <summary>
        /// This is needed to reload the Ai propertys/fields after deserializing
        /// </summary>
        void ReloadContent(ref Director dir);

        /// <summary>
        /// A method to remove the platforms of the ai when they die
        /// </summary>
        /// <param name="platform"></param>
        void Kill(PlatformBlank platform);

        /// <summary>
        /// A method to remove the units of the ai when they die.
        /// </summary>
        /// <param name="unit">The unit to kill</param>
        void Kill(EnemyUnit unit);

        void AddStructureToGame(Triple<CommandCenter, List<PlatformBlank>, List<Road>> structure, Rectangle bounds);

        Rectangle GetBoundsOfStructure(int index);

        int GetStructureCount();

        void Shooting(MilitaryUnit sender, ICollider shootingAt, GameTime gametime);
    }
}
