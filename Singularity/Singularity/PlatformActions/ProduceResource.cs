using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    [DataContract]
    public sealed class ProduceWellResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        [DataMember]
        private readonly ResourceMap mResourceMap;

        public ProduceWellResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetWellResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    [DataContract]
    public sealed class ProduceQuarryResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        [DataMember]
        private ResourceMap mResourceMap;

        public ProduceQuarryResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetQuarryResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    [DataContract]
    public sealed class ProduceMineResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        [DataMember]
        private ResourceMap mResourceMap;

        public ProduceMineResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> {JobType.Production};

        public override void Execute()
        {
            var res = mResourceMap.GetMineResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    [DataContract]
    public sealed class BuildBluePrint : AMakeUnit
    {
        [DataMember]
        private Dictionary<EResourceType, int> mMRequiredResources;

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, ref Director director) : base(
            platform: platform,
            director: ref director)
        {
            mMRequiredResources = toBeBuilt.GetResourcesRequired();
        }

        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> {JobType.Construction};

        public override void CreateUnit()
        {
            throw new NotImplementedException();
        }

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

    [DataContract]
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
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).PausePlatformAction(this);
                    State = PlatformActionState.Available;
                    break;
                case PlatformActionState.Available:
                    // TODO: You dont request Units anymore. Are there other things to be changed too then?
                    // mDirector.GetDistributionManager.RequestUnits(mPlatform, JobType.Production, this);
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
