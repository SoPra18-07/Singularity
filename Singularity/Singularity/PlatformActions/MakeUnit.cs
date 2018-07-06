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

    internal sealed class MakeFastMilitaryUnit : AMakeUnit
    {
        public MakeFastMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform: platform, director: ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Metal, 3 }, { EResourceType.Chip, 2 }, {EResourceType.Fuel, 1} };
        }

        protected override void CreateUnit()
        {
            // unsure why this is a static method since it just returns a military unit anyways
            // var unit = MilitaryUnit.CreateMilitaryUnit(mPlatform.Center + mOffset, ref mDirector);
            
            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryUnit(position: mPlatform.Center + mOffset, camera: camera, director: ref mDirector, map: ref map);
        }
    }

    internal sealed class MakeStrongMilitrayUnit : AMakeUnit
    {
        public MakeStrongMilitrayUnit(PlatformBlank platform, ref Director director) : base(platform: platform, director: ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 2}};
        }

        protected override void CreateUnit()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class MakeGeneralUnit : AMakeUnit
    {
        public MakeGeneralUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            // Todo: update prices.
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Steel, 3} };
        }

        protected override void CreateUnit()
        {
            mDirector.GetStoryManager.Level.GameScreen.AddObject(new GeneralUnit(mPlatform, ref mDirector));
        }
    }


    public abstract class AMakeUnit : APlatformAction, IUpdate
    {

        protected Dictionary<EResourceType, int> mBuildingCost;
        protected Dictionary<EResourceType, int> mMissingResources;
        protected Dictionary<EResourceType, int> mToRequest;
        protected Vector2 mOffset = new Vector2(value: 200f);

        protected AMakeUnit(PlatformBlank platform, ref Director director) : base(platform: platform, director: ref director)
        {
            State = PlatformActionState.Available;
        }

        protected abstract void CreateUnit();

        public override void Execute()
        {
            // theres nothing a unit should be able to do, except bringing Resources.
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Logistics };

        public void Update(GameTime gametime)
        {
            if (State != PlatformActionState.Active)
                return;
            if (mToRequest.Count > 0)
            {
                var resource = mToRequest.Keys.ElementAt(index: 0);
                if (mToRequest[key: resource] <= 1)
                {
                    mToRequest.Remove(key: resource);
                } else {
                    mToRequest[key: resource] = mToRequest[key: resource] - 1;
                }
                mDirector.GetDistributionManager.RequestResource(platform: mPlatform, resource: resource, action: this);
            }

            mPlatform.GetPlatformResources().ForEach(action: r => GetResource(type: r.Type));

            if (mMissingResources.Count <= 0)
            {
                CreateUnit();
            }
        }

        protected void GetResource(EResourceType type)
        {
            if (!mMissingResources.ContainsKey(key: type)) return;
            var res = mPlatform.GetResource(resourcetype: type);
                
            mMissingResources[key: type] -= 1;
            if (mMissingResources[key: type] <= 0)
                mMissingResources.Remove(key: type);
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
                    mMissingResources = new Dictionary<EResourceType, int>(dictionary: mBuildingCost);
                    mToRequest = new Dictionary<EResourceType, int>(dictionary: mMissingResources);
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionManager.PausePlatformAction(action: this);
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException(message: "Someone/Something acccessed the state!!");
            }
        }
    }
}
