using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{

    public sealed class BuildBluePrint : AMakeUnit
    {
        public override List<JobType> UnitsRequired { get; } = new List<JobType> { JobType.Construction };

        private readonly PlatformBlank mBuilding;

        private bool mBuildable; // defaults to false

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, ref Director director) : base(
            platform,
            ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int>(toBeBuilt.GetResourcesRequired());
            mBuilding = toBeBuilt;
            
            UpdateResources();
            mIsBuilding = true;
        }

        protected override void CreateUnit()
        {
            mBuilding.Built();
            Debug.WriteLine("Platform built!");
        }

        public override void Execute()
        {
            if (!mBuildable) return;
            Die();
        }
    }
}
