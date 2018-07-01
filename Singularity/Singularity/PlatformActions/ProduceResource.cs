using System;
using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    public class ProduceWellResource : APlatformResourceAction
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
            var res = mResourceMap.GetWellResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    public class ProduceQuarryResource : APlatformResourceAction
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
            var res = mResourceMap.GetQuarryResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    public class ProduceMineResource : APlatformResourceAction
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
            var res = mResourceMap.GetMineResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }


    public class BuildBluePrint : AMakeUnit
    {
        private Dictionary<EResourceType, int> mMRequiredResources;

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, ref Director director) : base(
            platform: platform,
            director: ref director)
        {
            mMRequiredResources = toBeBuilt.GetResourcesRequired();
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> {JobType.Construction};

        public override void Execute()
        {
            throw new NotImplementedException();
            // TODO: Build Blueprints!! (fkarg)
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }


    public abstract class APlatformResourceAction : APlatformAction
    {
        protected APlatformResourceAction(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Active:
                    mDirector.GetDistributionManager.PausePlatformAction(this);
                    State = PlatformActionState.Available;
                    break;
                case PlatformActionState.Available:
                    mDirector.GetDistributionManager.RequestUnits(mPlatform, JobType.Production, this);
                    State = PlatformActionState.Active;
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }
}
