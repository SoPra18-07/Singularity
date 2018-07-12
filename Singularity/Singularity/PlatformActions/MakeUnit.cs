﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public MakeFastMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Metal, 3 }, { EResourceType.Chip, 2 }, {EResourceType.Fuel, 1} };
        }

        public override void CreateUnit()
        {
            // unsure why this is a static method since it just returns a military unit anyways
            // var unit = MilitaryUnit.CreateMilitaryUnit(mPlatform.Center + mOffset, ref mDirector);

            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryFast(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
            mDirector.GetMilitaryManager.AddUnit(unit);
        }
    }

    internal sealed class MakeHeavyMilitaryUnit : AMakeUnit
    {
        public MakeHeavyMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 2}};
        }

        public override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryHeavy(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
            mDirector.GetMilitaryManager.AddUnit(unit);
        }
    }

    internal sealed class MakeStandardMilitaryUnit : AMakeUnit
    {
        public MakeStandardMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Steel, 3 }, { EResourceType.Chip, 2 }, { EResourceType.Fuel, 2 } };
        }

        public override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryUnit(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
            mDirector.GetMilitaryManager.AddUnit(unit);
        }
    }

    public abstract class AMakeUnit : APlatformAction, IUpdate
    {

        protected Dictionary<EResourceType, int> mBuildingCost;
        protected Dictionary<EResourceType, int> mMissingResources = new Dictionary<EResourceType, int>();
        protected Dictionary<EResourceType, int> mToRequest = new Dictionary<EResourceType, int>();
        protected Vector2 mOffset = new Vector2(200f);

        protected AMakeUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
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
                mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).RequestResource(mPlatform, resource, this);
                Debug.WriteLine("just requested " + resource + " from the DistrMgr. Let's wait and see.");
            }

            mPlatform.GetPlatformResources().ForEach(action: r => GetResource(r.Type));

            if (mMissingResources.Count <= 0 && State == PlatformActionState.Active)
            {
                CreateUnit();
                State = PlatformActionState.Available;
            }
        }

        protected void GetResource(EResourceType type)
        {
            if (!mMissingResources.ContainsKey(type))
            {
                return;
            }

            var res = mPlatform.GetResource(type);

            mMissingResources[type] -= 1;
            if (mMissingResources[type] <= 0)
            {
                mMissingResources.Remove(type);
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return mMissingResources;
        }

        private void UpdateResources()
        {
            if (mMissingResources.Values.Sum() > 0)
            {
                mToRequest = new Dictionary<EResourceType, int>(mMissingResources);
            }
            else
            {
                mMissingResources = new Dictionary<EResourceType, int>(mBuildingCost);
                mToRequest = new Dictionary<EResourceType, int>(mMissingResources);
            }
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    UpdateResources();
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).PausePlatformAction(this);
                    State = PlatformActionState.Available;
                    break;
                case PlatformActionState.Deactivated:
                case PlatformActionState.Disabled:
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }
    }
}
