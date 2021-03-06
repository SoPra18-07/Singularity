﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    [DataContract]
    sealed class RefineResourceAction : AMakeUnit
    {
        [DataMember]
        private EResourceType mRefiningTo;
        [DataMember]
        private bool mReady;
        [DataMember]
        private bool mSecondReady;
        [DataMember]
        private int mCounter;
        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Production };

        public RefineResourceAction(PlatformBlank platform,
            ref Director director,
            Dictionary<EResourceType, int> from,
            EResourceType to) : base(platform, ref director)
        {
            State = PlatformActionState.Available;
            mBuildingCost = from;
            mRefiningTo = to;

            // now, you actually might want to have the mRequestedResoucres higher than the cost of one building ... todo: another time.
        }
        protected override void CreateUnit()
        {
            State = PlatformActionState.Active;
            if (mReady && !mSecondReady)
            {
                mSecondReady = true;
                UpdateResources();
            }
            mReady = true;
        }

        public override void Execute()
        {
            if (State != PlatformActionState.Active || !mReady)
            {
                return;
            }

            mCounter++;
            if (mCounter % 240 != 0)
            {
                return;
            }

            mCounter = 0;
            CreateResource();
            UpdateResources();
        }

        private void CreateResource()
        {
            var nRes = new Resource(mRefiningTo, mPlatform.Center, mDirector);
            mPlatform.StoreResource(nRes);

            // Track the creation of a resource in the statistics.
            mDirector.GetStoryManager.UpdateResources(mRefiningTo);

            if (mSecondReady)
            {
                mSecondReady = false;
            }
            else
            {
                mReady = false;
            }
        }

        public EResourceType GetRefiningTo()
        {
            return mRefiningTo;
        }

        public override void UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
                    // mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(mPlatform);
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
