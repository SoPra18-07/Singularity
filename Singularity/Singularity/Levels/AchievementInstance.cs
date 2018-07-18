using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;

namespace Singularity.Levels
{
    [DataContract]
    internal sealed class AchievementInstance
    {
        //The statistics of the Achievements.
        [DataMember]
        internal bool FirstBuilding { get; set; }

        [DataMember]
        internal bool TutorialFinished { get; set; }

        [DataMember]
        internal int PlatformsBuilt { get; set; }

        [DataMember]
        internal int TrashBurned { get; set; }

        [DataMember]
        internal bool CampaignComplete { get; set; }

        [DataMember]
        internal int UnitsBuilt { get; set; }

        [DataMember]
        internal int MilitaryUnitsBuilt { get; set; }

        [DataMember]
        internal bool AllCompleted { get; private set; }

        /// <summary>
        /// Loads all the contents of this class to the static version of this class.
        /// </summary>
        internal void LoadToStatic()
        {
            Achievements.FirstBuilding = FirstBuilding;
            Achievements.TutorialFinished = TutorialFinished;
            Achievements.PlatformsBuilt = PlatformsBuilt;
            Achievements.TrashBurned = TrashBurned;
            Achievements.CampaignComplete = CampaignComplete;
            Achievements.UnitsBuilt = UnitsBuilt;
            Achievements.MilitaryUnitsBuilt = MilitaryUnitsBuilt;
            Achievements.AllCompleted = AllCompleted;
        }

        internal void UpdateFromStatic()
        {
            FirstBuilding = Achievements.FirstBuilding;
            TutorialFinished = Achievements.TutorialFinished;
            PlatformsBuilt = Achievements.PlatformsBuilt;
            TrashBurned = Achievements.TrashBurned;
            CampaignComplete = Achievements.CampaignComplete;
            UnitsBuilt = Achievements.UnitsBuilt;
            MilitaryUnitsBuilt = Achievements.MilitaryUnitsBuilt;
            AllCompleted = Achievements.AllCompleted;
        }
    }
}
