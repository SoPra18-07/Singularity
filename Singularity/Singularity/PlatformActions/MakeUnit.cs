using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{

    class MakeFastMilitaryUnit : AMakeUnit
    {
        public MakeFastMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Metal, 3 }, { EResourceType.Chip, 2 }, {EResourceType.Fuel, 1} };
        }

    }

    class MakeStrongMilitrayUnit : AMakeUnit
    {
        public MakeStrongMilitrayUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 2}};
        }

    }



    abstract class AMakeUnit : APlatformAction, IUpdate
    {

        protected Dictionary<EResourceType, int> mBuildingCost;
        protected Dictionary<EResourceType, int> mMissingResources;
        protected Dictionary<EResourceType, int> mToRequest;

        public AMakeUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            State = PlatformActionState.Available;
        }

        public override void Execute()
        {
            // throw new NotImplementedException();
            // theres nothing a unit should be able to do, except bringing Resources.
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Logistics };

        public void Update(GameTime gametime)
        {
            if (mToRequest.Count > 0)
            {
                var resource = mToRequest.Keys.ElementAt(0);
                if (mToRequest[resource] <= 1)
                {
                    mToRequest.Remove(resource);
                } else {
                    mToRequest[resource] = mToRequest[resource] - 1;
                }
                mDirector.GetDistributionManager.RequestResource(mPlatform, resource, this);
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return mMissingResources;
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    mMissingResources = new Dictionary<EResourceType, int>(mBuildingCost);
                    mToRequest = new Dictionary<EResourceType, int>(mMissingResources);
                    State = PlatformActionState.Deactivated;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionManager.PausePlatformAction(this);
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException(message: "Someone/Something acccessed the state!!");
            }
        }
    }
}
