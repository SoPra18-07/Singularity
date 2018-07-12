﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;
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

        protected override void CreateUnit()
        {
            // unsure why this is a static method since it just returns a military unit anyways
            // var unit = MilitaryUnit.CreateMilitaryUnit(mPlatform.Center + mOffset, ref mDirector);

            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryFast(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
        }
    }

    internal sealed class MakeHeavyMilitaryUnit : AMakeUnit
    {
        public MakeHeavyMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 2}};
        }

        protected override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryHeavy(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
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

    internal sealed class MakeStandardMilitaryUnit : AMakeUnit
    {
        public MakeStandardMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Steel, 3 }, { EResourceType.Chip, 2 }, { EResourceType.Fuel, 2 } };
        }

        protected override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var map = mDirector.GetStoryManager.Level.Map;
            var unit = new MilitaryUnit(mPlatform.Center + mOffset, camera, ref mDirector, ref map);
        }
    }

    public abstract class AMakeUnit : APlatformAction
    {

        protected Dictionary<EResourceType, int> mBuildingCost;
        protected Dictionary<EResourceType, int> mMissingResources;
        protected Dictionary<EResourceType, int> mToRequest;
        protected Vector2 mOffset = new Vector2(200f);

        protected AMakeUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            State = PlatformActionState.Available; // now the init is a UIToggle. hacky but works.
            mBuildingCost = new Dictionary<EResourceType, int>();
            mMissingResources = new Dictionary<EResourceType, int>();
            mToRequest = new Dictionary<EResourceType, int>();
        }

        // effectively only this needs to be implemented for all CreateUnit-things. Also the price needs to be set. (mBuildingCost)
        protected abstract void CreateUnit();

        public override void Execute()
        {
            // theres nothing a unit should be able to do, except bringing Resources.
        }

        public override List<JobType> UnitsRequired { get; } = new List<JobType>{ JobType.Logistics };

        public void Update()
        {
            // Debug.WriteLine(mBuildingCost.Values.Sum() + ", " + mMissingResources.Values.Sum() + ", " + mToRequest.Values.Sum());
            if (State != PlatformActionState.Active)
                return;
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
                Debug.WriteLine("Requested " + resource + " just now. Waiting. (" + mPlatform.Id + ")");
            }

            mPlatform.GetPlatformResources().ForEach(r => GetResource(r.Type));

            if (mMissingResources.Count <= 0)
            {
                CreateUnit();
            }
        }

        private void GetResource(EResourceType type)
        {
            if (!mMissingResources.ContainsKey(type)) return;
            var unused = mPlatform.GetResource(type);

            mMissingResources[type] -= 1;
            if (mMissingResources[type] <= 0)
            {
                mMissingResources.Remove(type);
            }
        }

        protected void UpdateResources()
        {
            if (mMissingResources.Count == 0)
            {
                mMissingResources = new Dictionary<EResourceType, int>(mBuildingCost);
            }
            mToRequest = new Dictionary<EResourceType, int>(mMissingResources);
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
                    UpdateResources();
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).PausePlatformAction(this);
                    mToRequest = new Dictionary<EResourceType, int>();
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }
    }
}
