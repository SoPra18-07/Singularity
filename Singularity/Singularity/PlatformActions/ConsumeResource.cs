using System;
using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    public class ConsumeDefenseResource : APlatformResourceAction
    {
        public ConsumeDefenseResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, resourceMap: resourceMap, director: ref director)
        {
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Defense };

        protected override void CreateResource()
        {
            // since you overwrote Execute, this has supposedly no way to get called.
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            var res = mResourceMap.GetAmmoResource(mPlatform.AbsolutePosition);
            if (res.IsPresent())
            {
                mPlatform.StoreResource(res.Get());
            }
        }
    }

    public class ConsumeEnergy : APlatformResourceAction
    {
        public ConsumeEnergy(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, resourceMap: resourceMap, director: ref director)
        {
        }

        // TODO: Implement an energy system
        public override void Execute()
        {
            // currently unimplementable .... well shit.
            throw new NotImplementedException();
        }
        protected override void CreateResource()
        {
            // since you overwrote Execute, this has supposedly no way to get called.
            throw new NotImplementedException();
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType>(0); // how do you specify a platform resource action that requires 0 units?
    }


}
