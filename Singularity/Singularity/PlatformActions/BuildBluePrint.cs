using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{

    [DataContract]
    public sealed class BuildBluePrint : AMakeUnit
    {
        [DataMember]
        public override List<JobType> UnitsRequired { get; set; } = new List<JobType> { JobType.Construction };

        [DataMember]
        private readonly PlatformBlank mBuilding;

        [DataMember]
        private bool mBuildRoad;

        [DataMember]
        private readonly Road mRBuilding;

        [DataMember]
        private bool mBuildable; // defaults to false

        public BuildBluePrint(PlatformBlank platform, PlatformBlank toBeBuilt, Road connectingRoad, ref Director director) : base(
            platform,
            ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int>(toBeBuilt.GetResourcesRequired());
            mBuilding = toBeBuilt;
            mRBuilding = connectingRoad;
            
            UpdateResources();
            mIsBuilding = true;
            mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
            State = PlatformActionState.Active;
        }

        public BuildBluePrint(PlatformBlank platform, Road road, ref Director director) : base (platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Metal, 1}, {EResourceType.Stone, 1} };
            mRBuilding = road;
            mBuildRoad = true;
            mIsBuilding = true;
            UpdateResources();
            mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
            State = PlatformActionState.Active;
            mRBuilding.Blueprint = true;
        }

        protected override void CreateUnit()
        {
            mRBuilding.Blueprint = false;
            if (!mBuildRoad)
            {
                mBuilding.Built();
            }
            Die();
        }

        public override void Execute()
        {
        }
    }
}
