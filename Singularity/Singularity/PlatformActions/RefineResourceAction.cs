﻿using System;
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
    class RefineResourceAction : AMakeUnit
    {
        private EResourceType mRefiningTo;
        private bool mReady;
        private int mCounter;

        public RefineResourceAction(PlatformBlank platform, ref Director director, Dictionary<EResourceType, int> from, EResourceType to) : base(platform, ref director)
        {
            State = PlatformActionState.Available;
            mBuildingCost = from;
            mRefiningTo = to;

            // now, you actually might want to have the mRequestedResoucres higher than the cost of one building ... todo: another time.
        }

        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Production };
        protected override void CreateUnit()
        {
            mReady = true;
            State = PlatformActionState.Active;
            UpdateResources();
        }

        public override void Execute()
        {
            if (State != PlatformActionState.Active || !mReady) return;
            mCounter++;
            if (mCounter % 240 != 0) return;
            mCounter = 0;
            CreateResource();
        }

        private void CreateResource()
        {
            var nRes = new Resource(mRefiningTo, mPlatform.Center);
            mPlatform.StoreResource(nRes);
            mReady = false;
        }


        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(mPlatform);
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

        public void UiToggleAll()
        {
            ((Factory)mPlatform).UiToggleAll();
        }
    }
}