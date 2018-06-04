using System;
using System.Collections.Generic;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Platform
{

    public class ProduceWellResource : APlatformAction
    {
        public ProduceWellResource(PlatformBlank platform) : base(platform)
        {
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Production };

        public override void Execute()
        {
            throw new NotImplementedException();
            // if PlatformBlank.isNotFull() ... // (of resources obviously)
            // Optional<Resources> r = ResourceMap.get<well/quarry/mine>ResourceFrom(Platform.coordinates.get()); // or similiar
            // if (r.IsPresent())
            // {
            //     PlatformBlank.addResource(r);
            // }
        }
    }

    public class ProduceQuarryResource : APlatformAction
    {
        public ProduceQuarryResource(PlatformBlank platform) : base(platform)
        {
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
        public ProduceMineResource(PlatformBlank platform) : base(platform)
        {
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
        internal Dictionary<EResourceType, int> RequiredResources;
        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt) : base(platform)
        {
            RequiredResources = toBeBuilt.GetResourcesRequired();
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Construction };

        public override void Execute()
        {
            throw new NotImplementedException();
            // TODO: Build Blueprints!!
        }
    }
}
