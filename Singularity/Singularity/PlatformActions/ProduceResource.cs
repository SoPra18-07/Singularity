using System;
using System.Collections.Generic;
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

        public ProduceWellResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform: platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Production };

        public override void Execute()
        {
            var res = mResourceMap.GetWellResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }

    public class ProduceQuarryResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceQuarryResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform: platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> { JobType.Production };

        public override void Execute()
        {
            var res = mResourceMap.GetQuarryResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }

    public class ProduceMineResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceMineResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform: platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> { JobType.Production };

        public override void Execute()
        {
            var res = mResourceMap.GetMineResource(location: mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(resource: res.Get());
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }
    }



    public class BuildBluePrint : APlatformAction
    {
        private Dictionary<EResourceType, int> mMRequiredResources;
        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt) : base(platform: platform)
        {
            mMRequiredResources = toBeBuilt.GetResourcesRequired();
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Construction };

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
        public APlatformResourceAction()
    }
}
