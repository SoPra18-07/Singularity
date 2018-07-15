using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface IArtificalIntelligence : IUpdate
    {
        IEnumerable<Spawner> GetSpawners();

        EAIDifficulty Difficulty { get; }
    }
}
