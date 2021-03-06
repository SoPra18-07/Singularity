﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    [DataContract]
    internal sealed class MakeFastMilitaryUnit : AMakeUnit
    {
        public MakeFastMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            // mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Metal, 3 }, { EResourceType.Chip, 2 }, {EResourceType.Fuel, 1} };
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Metal, 2}, {EResourceType.Copper, 2}, {EResourceType.Fuel, 2} };
        }

        protected override void CreateUnit()
        {
            // unsure why this is a static method since it just returns a military unit anyways
            // var unit = MilitaryUnit.CreateMilitaryUnit(mPlatform.Center + mOffset, ref mDirector);

            var camera = mDirector.GetStoryManager.Level.Camera;
            var unit = new MilitaryFast(mPlatform.Center + mOffset, camera, ref mDirector);
            mDirector.GetStoryManager.Level.GameScreen.AddObject(unit);
        }
    }

    [DataContract]
    internal sealed class MakeHeavyMilitaryUnit : AMakeUnit
    {
        public MakeHeavyMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> {{EResourceType.Steel, 3}, {EResourceType.Chip, 2}, {EResourceType.Fuel, 3}};
        }

        protected override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var unit = new MilitaryHeavy(mPlatform.Center + mOffset, camera, ref mDirector);
            mDirector.GetStoryManager.Level.GameScreen.AddObject(unit);
        }
    }

    [DataContract]
    internal sealed class MakeGeneralUnit : AMakeUnit
    {
        public MakeGeneralUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            // Todo: update prices.
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Chip, 3}, {EResourceType.Fuel, 3} };
        }

        protected override void CreateUnit()
        {
            var unit = new GeneralUnit(mPlatform, ref mDirector);
            mDirector.GetStoryManager.Level.GameScreen.AddObject(unit);

            mDirector.GetUserInterfaceController.UpdateSLiderHandler();
        }
    }

    [DataContract]
    internal sealed class MakeSettlerUnit : AMakeUnit
    {
        public MakeSettlerUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            // Todo: update prices
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Metal, 15}, {EResourceType.Stone, 10}, {EResourceType.Fuel, 10}, {EResourceType.Chip, 10}, {EResourceType.Steel, 5} };
        }

        protected override void CreateUnit()
        {
            var settler = Settler.Create(mPlatform.Center + mOffset, ref mDirector);
            mDirector.GetStoryManager.Level.GameScreen.AddObject(settler);
        }
    }


    [DataContract]
    internal sealed class MakeStandardMilitaryUnit : AMakeUnit
    {
        public MakeStandardMilitaryUnit(PlatformBlank platform, ref Director director) : base(platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { { EResourceType.Steel, 3 }, { EResourceType.Chip, 2 }, { EResourceType.Fuel, 2 } };
        }

        protected override void CreateUnit()
        {
            var camera = mDirector.GetStoryManager.Level.Camera;
            var unit = new MilitaryUnit(mPlatform.Center + mOffset, camera, ref mDirector);
            mDirector.GetStoryManager.Level.GameScreen.AddObject(unit);
        }
    }

    [DataContract]
    public abstract class AMakeUnit : APlatformAction
    {
        [DataMember]
        protected Dictionary<EResourceType, int> mBuildingCost;
        [DataMember]
        protected Dictionary<EResourceType, int> mMissingResources;
        [DataMember]
        protected Dictionary<EResourceType, int> mToRequest;
        [DataMember]
        protected Vector2 mOffset = new Vector2(100f);
        [DataMember]
        protected bool mIsBuilding;
        [DataMember]
        private bool mFirstCall = true;
        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Logistics };

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

        public override void Update(GameTime t)
        {
            if (State != PlatformActionState.Active || mFirstCall)
            {
                mFirstCall = false;
                return;
            }

            if (mToRequest.Count > 0)
            {
                var resource = mToRequest.Keys.ElementAt(0);
                if (mToRequest[resource] <= 1)
                {
                    mToRequest.Remove(resource);
                } else {
                    mToRequest[resource] = mToRequest[resource] - 1;
                }
                mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).RequestResource(mPlatform, resource, this, mIsBuilding);
            }

            foreach (var r in mPlatform.GetPlatformResources().ToList())
            {
                GetResource(r.Type);
            }

            if (mMissingResources.Count > 0)
            {
                return;
            }

            State = PlatformActionState.Available;
            CreateUnit();
        }

        #region private function

        private void GetResource(EResourceType type)
        {
            if (!mMissingResources.ContainsKey(type))
            {
                return;
            }

            var unused = mPlatform.GetResource(type);

            mMissingResources[type] -= 1;
            if (mMissingResources[type] <= 0)
            {
                mMissingResources.Remove(type);
            }
        }

        #endregion

        protected void UpdateResources()
        {
            if (mMissingResources.Count == 0)
            {
                mMissingResources = new Dictionary<EResourceType, int>(mBuildingCost);
            }
            foreach (var resources in mMissingResources)
            {
                // ReSharper disable once InlineOutVariableDeclaration
                // ReSharper disable once RedundantAssignment
                var inside = 0;
                if (mToRequest.TryGetValue(resources.Key, out inside))
                {
                    mToRequest[resources.Key] = inside + resources.Value;
                }
                else
                {
                    mToRequest.Add(resources.Key, inside + resources.Value);
                }
            }
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            return mMissingResources;
        }

        public Dictionary<EResourceType, int> GetBuildingCost()
        {
            return mBuildingCost;
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
                    UpdateResources();
                    State = PlatformActionState.Active;
                    break;
                case PlatformActionState.Active:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).PausePlatformAction(this, mDirector);
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
