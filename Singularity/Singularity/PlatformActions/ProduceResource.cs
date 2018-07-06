using System;
using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    public sealed class ProduceWellResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private readonly ResourceMap mResourceMap;

        public ProduceWellResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetWellResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }
    }

    public sealed class ProduceQuarryResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceQuarryResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetQuarryResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }
    }

    public sealed class ProduceMineResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceMineResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetMineResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }
    }


    public sealed class BuildBluePrint : AMakeUnit
    {
        private PlatformBlank mBuilding;

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, ref Director director) : base(
            platform: platform,
            director: ref director)
        {
            mBuildingCost = toBeBuilt.GetResourcesRequired();
            mBuilding = toBeBuilt;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Construction};

        protected override void CreateUnit()
        {
            mBuilding.Built();
            Die();
        }

        public override void Execute()
        {

        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }


    public abstract class APlatformResourceAction : APlatformAction
    {
        protected APlatformResourceAction(PlatformBlank platform, ref Director director) : base(platform: platform, director: ref director)
        {
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Active:
                    mDirector.GetDistributionManager.PausePlatformAction(action: this);
                    State = PlatformActionState.Available;
                    break;
                case PlatformActionState.Available:
                    // TODO: You dont request Units anymore. Are there other things to be changed too then?
                    // mDirector.GetDistributionManager.RequestUnits(mPlatform, JobType.Production, this);
                    State = PlatformActionState.Active;
                    break;
                default:
                    throw new AccessViolationException(message: "Someone/Something acccessed the state!!");
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }
}
