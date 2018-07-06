using System.Runtime.Serialization;
using Singularity.Utils;

namespace Singularity.Levels
{
    [DataContract]
    internal sealed class Achievements
    {

        //The statistics of the Achievements.
        [DataMember]
        Pair<int, bool> TrashBurned { get; set; }

        [DataMember]
        bool FirstBuilding { get; set; }

        [DataMember]
        bool TutorialFinished { get; set; }

        [DataMember]
        Pair<int, bool> PlatformsBuilt { get; set; }

        [DataMember]
        bool ReachedLvl5 { get; set; }

        [DataMember]
        Pair<int, bool> UnitsBuilt { get; set; }


        //The methods of the achievements. Call them every time you make progress in it.
        //They return true only in the moment their corresponding requirements are met, because only then an infobox should be triggered
        public bool WallE()
        {
            if (TrashBurned.GetFirst() + 1 == 10000)
            {
                TrashBurned = new Pair<int, bool>(TrashBurned.GetFirst() + 1, true);
            }
            else
            {
                TrashBurned = new Pair<int, bool>(TrashBurned.GetFirst() + 1, TrashBurned.GetSecond());
            }
            return TrashBurned.GetSecond();
        }

        public bool SystemGoesOnline()
        {
            if (FirstBuilding)
            {
                return !FirstBuilding;
            }
            FirstBuilding = true;
            return FirstBuilding;
        }

        public bool TutorialFinish()
        {
            if (TutorialFinished)
            {
                return !TutorialFinished;
            }
            TutorialFinished = true;
            return TutorialFinished;
        }

        public bool Skynet()
        {
            if (PlatformsBuilt.GetFirst() + 1 == 1000)
            {
                PlatformsBuilt = new Pair<int, bool>(PlatformsBuilt.GetFirst() + 1, true);
            }
            else
            {
                PlatformsBuilt = new Pair<int, bool>(PlatformsBuilt.GetFirst() + 1, PlatformsBuilt.GetSecond());
            }
            return PlatformsBuilt.GetSecond();
        }

        public bool Hal9000()
        {
            if (ReachedLvl5)
            {
                return !ReachedLvl5;
            }
            ReachedLvl5 = true;
            return ReachedLvl5;
        }

        public bool Replicant()
        {
            if (UnitsBuilt.GetFirst() + 1 == 1000)
            {
                UnitsBuilt = new Pair<int, bool>(UnitsBuilt.GetFirst() + 1, true);
            }
            else
            {
                UnitsBuilt = new Pair<int, bool>(UnitsBuilt.GetFirst() + 1, UnitsBuilt.GetSecond());
            }
            return UnitsBuilt.GetSecond();
        }
    }
}
