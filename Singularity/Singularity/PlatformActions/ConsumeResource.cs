using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    public class ConsumeDefenseResource : APlatformResourceAction
    {
        // The ResourceMap is needed for actually 'producing' the resources.
        private readonly ResourceMap mResourceMap;

        public ConsumeDefenseResource(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Defense };

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
        private readonly ResourceMap mResourceMap;

        public ConsumeEnergy(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform: platform, director: ref director)
        {
            mResourceMap = resourceMap;
        }
        // TODO: Implement an energy system
        public override void Execute()
        {
            // currently unimplementable
            throw new NotImplementedException();
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType>(0); // how do you specify a platform resource action that requires 0 units?
    }


}
