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

            Debug.WriteLine(mBuildingCost.Values.Sum() + ", " + toBeBuilt.GetResourcesRequired().Values.Sum());
            UpdateResources();
            Debug.WriteLine(mBuildingCost.Values.Sum() + ", " + mMissingResources.Values.Sum() + ", " + mToRequest.Values.Sum());
        }

        protected override void CreateUnit()
        {
            mBuildable = true;
            Debug.WriteLine("Platform buildable!");
        }

        public override void Execute()
        {
            if (!mBuildable) return;
            mBuilding.Built();
            Die();
        }
    }
}
