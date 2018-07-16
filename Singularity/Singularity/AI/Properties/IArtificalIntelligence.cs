using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Screen.ScreenClasses;
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

        /// <summary>
        /// This is needed to reload the Ai propertys/fields after deserializing
        /// </summary>
        void ReloadContent(ref Director dir);

        /// <summary>
        /// A method to remove the platforms of the ai when they die
        /// </summary>
        /// <param name="platform"></param>
        void Kill(PlatformBlank platform);

        void AddStructureToGame(Triple<CommandCenter, List<PlatformBlank>, List<Road>> structure, Rectangle bounds);
    }
}
