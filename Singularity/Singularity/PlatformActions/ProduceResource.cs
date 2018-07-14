using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

        public ProduceWellResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform, ref director)
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

        public ProduceQuarryResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform, ref director)
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

        public ProduceMineResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform, ref director)
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
                    // does this work, then?
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(mPlatform, false);
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Deactivated:
                case PlatformActionState.Disabled:
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }

        public override void Update(GameTime t)
        {
            // do nothing
        }
    }
}
