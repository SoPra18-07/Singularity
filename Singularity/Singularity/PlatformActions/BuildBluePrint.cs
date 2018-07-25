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
            mBuildingCost = new Dictionary<EResourceType, int>(PlatformBlank.GetResourceCosts(toBeBuilt.mType));
            mBuilding = toBeBuilt;
            mRBuilding = connectingRoad;
            mRBuilding.Blueprint = true;
            mDirector.GetStoryManager.Level.GameScreen.AddObject(mRBuilding);

            UpdateResources();
            mIsBuilding = true;
            mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
            State = PlatformActionState.Active;
        }

        public BuildBluePrint(PlatformBlank platform, Road road, ref Director director) : base (platform, ref director)
        {
            mBuildingCost = new Dictionary<EResourceType, int> { {EResourceType.Metal, 1}, {EResourceType.Stone, 1} };
            mRBuilding = road;
            mRBuilding.Blueprint = true;
            mBuildRoad = true;
            mIsBuilding = true;
            UpdateResources();
            mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Register(this);
            State = PlatformActionState.Active;
        }

        protected override void CreateUnit()
        {
            if (!mBuildRoad)
            {
                mDirector.GetActionManager.AddObject(mBuilding,
                    delegate(object p)
                    {
                        mDirector.GetStoryManager.Level.GameScreen.RemoveObject(p);
                        mDirector.GetStoryManager.Level.Map.AddPlatform(mBuilding);
                        mBuilding.Built();
                        return true;
                    });
            }
            mDirector.GetActionManager.AddObject(mRBuilding,
                delegate(object r)
                {
                    mDirector.GetStoryManager.Level.GameScreen.RemoveObject(r);
                    mDirector.GetStoryManager.Level.Map.AddRoad(mRBuilding);
                    mRBuilding.Blueprint = false;
                    return Die();
                });
        }

        public override void Execute()
        {
        }
    }
}
