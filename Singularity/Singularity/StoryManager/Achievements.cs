using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.StoryManager
{
    [DataContract()]
    class Achievements
    {
        [DataMember()]
        private int mTrashBurned;

        [DataMember()]
        private bool mFirstBuilding;

        [DataMember()]
        private bool mTutorialFinished;

        [DataMember()]
        private int mPlatformsBuilt;

        [DataMember()]
        private bool mReachedLvl5;

        [DataMember()]
        private int mUnitsBuilt;

        public bool WallE()
        {
            mTrashBurned++;
            return mTrashBurned == 10000;
        }

        public bool SystemGoesOnline()
        {
            return !mFirstBuilding;
        }

        public bool TutorialFinished()
        {
            return !mTutorialFinished;
        }

        public bool Skynet()
        {
            mPlatformsBuilt++;
            return mPlatformsBuilt == 1000;
        }

        public bool Hal9000()
        {
            return !mReachedLvl5;
        }

        public bool Replicant()
        {
            mUnitsBuilt++;
            return mUnitsBuilt == 1000;
        }
    }
}
