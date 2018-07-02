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

        public override void CreateUnit()
        {
            var unit = new MilitaryUnit(mPlatform.Center + mOffset, mDirector.GetMilitaryManager.GetMilitarySpriteSheet(), mDirector.GetStoryManager.Level.Camera, ref mDirector, mDirector.GetStoryManager.StructureMap.);
            mDirector.
        }
    }

    class MakeStrongMilitrayUnit : AMakeUnit
    {
        public MakeStrongMilitrayUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 2}};
        }

        public override void CreateUnit()
        {
            throw new NotImplementedException();
        }
    }


    public abstract class AMakeUnit : APlatformAction, IUpdate
    {

        protected Dictionary<EResourceType, int> mBuildingCost;
        protected Dictionary<EResourceType, int> mMissingResources;
        protected Dictionary<EResourceType, int> mToRequest;
        protected Vector2 mOffset = new Vector2(200f);

        public AMakeUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            State = PlatformActionState.Available;
        }

        public abstract void CreateUnit();

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

            mPlatform.GetPlatformResources().ForEach(action: r => GetResource(r.Type));

            if (mMissingResources.Count <= 0)
            {
                CreateUnit();
            }
        }

        protected void GetResource(EResourceType type)
        {
            if (!mMissingResources.ContainsKey(type)) return;
            var res = mPlatform.GetResource(type);
                
            mMissingResources[type] -= 1;
            if (mMissingResources[type] <= 0)
                mMissingResources.Remove(type);
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
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionManager.PausePlatformAction(this);
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }
    }
}
