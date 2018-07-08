using System;
using System.Collections.Generic;
using System.Diagnostics;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{

    public sealed class BuildBluePrint : AMakeUnit
    {
        private PlatformBlank mBuilding;

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, ref Director director) : base(
            platform,
            ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int>(toBeBuilt.GetResourcesRequired());
            mBuilding = toBeBuilt;

            Debug.WriteLine(mBuildingCost.Count);
            Debug.WriteLine(toBeBuilt.GetResourcesRequired().Count);
            UiToggleState();
            Debug.WriteLine(mBuildingCost.Count);
            Debug.WriteLine(mMissingResources.Count);
            Debug.WriteLine(mToRequest.Count);
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Construction};

        protected override void CreateUnit()
        {
            Debug.WriteLine("Building Platform!");
            mBuilding.Built();
            Die();
        }

        public override void Execute()
        {

        }
    }
}
