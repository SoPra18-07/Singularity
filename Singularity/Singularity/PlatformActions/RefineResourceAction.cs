using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.PlatformActions
{
    class ARefineResourceAction : APlatformResourceAction
    {
        private List<Pair<EResourceType, int>> mRequested;

        public ARefineResourceAction(PlatformBlank platform, ResourceMap resourceMap, ref Director director) : base(platform, resourceMap, ref director)
        {
            mDirector.GetDistributionDirector.GetManager(platform.GetGraphIndex()).Register(this);
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Production };

        protected override void CreateResource()
        {
            var res = mPlatform.GetResource(EResourceType.Metal);
            if (res.IsPresent())
            {
                var nRes = new Resource(EResourceType.Steel, mPlatform.Center);
                mPlatform.StoreResource(nRes);
            }

            mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex())
                .RequestResource(mPlatform, EResourceType.Metal, this);

            return;

            if (mRequested.Count == 0)
            {
                for (int a = 3; a > 0; a--)
                {
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex())
                        .RequestResource(mPlatform, EResourceType.Metal, this);
                    mRequested.Add(new Pair<EResourceType, int>(EResourceType.Metal, 5));
                }
            }
            else
            {
                mRequested = mRequested.Select(p => new Pair<EResourceType, int>(p.GetFirst(), p.GetSecond() - 1)).ToList();
                var a = mRequested.RemoveAll(p => p.GetSecond() < 0);
                for (; a > 0; a--)
                {
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex())
                        .RequestResource(mPlatform, EResourceType.Metal, this);
                    mRequested.Add(new Pair<EResourceType, int>(EResourceType.Metal, 5));
                }

            }

        }
    }
}
