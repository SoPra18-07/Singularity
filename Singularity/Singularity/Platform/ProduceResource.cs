using System;
using System.Collections.Generic;
using Singularity.Map;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.Platform
{

    public class ProduceWellResource : APlatformAction
    {

        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceWellResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Production };

        public override void Execute()
        {
            var resource = mResourceMap.GetResources(mPlatform.GetLocation());
            if (!resource.IsPresent() || !mPlatform.PlatformHasSpace()) return;
            var res = new Resource(EResourceType.Oil, mPlatform.AbsolutePosition, 0);
            mPlatform.StoreResource(res);
        }
    }

    public class ProduceQuarryResource : APlatformAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceQuarryResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> { JobType.Production };

        public override void Execute()
        {
            throw new NotImplementedException();
            // getting a Quarry-Resource from the ResourceMap
        }
    }

    public class ProduceMineResource : APlatformAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private ResourceMap mResourceMap;

        public ProduceMineResource(PlatformBlank platform, ResourceMap resourceMap) : base(platform)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType> { JobType.Production };

        public override void Execute()
        {
            throw new NotImplementedException();
            // getting a Mine-Resource from the ResourceMap
        }
    }

    public class BuildBluePrint : APlatformAction
    {
        private Dictionary<EResourceType, int> mRequiredResources;
        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt) : base(platform)
        {
            mRequiredResources = toBeBuilt.GetResourcesRequired();
        }   

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Construction };

        public override void Execute()
        {
            throw new NotImplementedException();
            // TODO: Build Blueprints!!
        }
    }
}
