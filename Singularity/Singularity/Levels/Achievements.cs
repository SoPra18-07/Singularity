using System.Runtime.Serialization;

namespace Singularity.Levels
{
    /// <summary>
    /// Holds the data related to achievements. To check whether an achievement has been earned, call the method
    /// associated with the achievement.
    /// To add progress towards an achievement, access the property related to the achievement.
    /// </summary>
    [DataContract]
    internal static class Achievements
    {
        //The statistics of the Achievements.
        [DataMember]
        internal static bool FirstBuilding { get; set; }

        [DataMember]
        internal static bool TutorialFinished { get; set; }

        [DataMember]
        internal static int PlatformsBuilt { get; set; }

        [DataMember]
        internal static int TrashBurned { get; set; }

        [DataMember]
        internal static bool CampaignComplete { get; set; }

        [DataMember]
        internal static int UnitsBuilt { get; set; }

        [DataMember]
        internal static int MilitaryUnitsBuilt { get; set; }

        [DataMember]
        internal static bool AllCompleted { get; set; }

        /// <summary>
        /// Get the achievement "The system goes online August 4th 1997"
        /// </summary>
        /// <returns></returns>
        internal static bool SystemOnline()
        {
            return FirstBuilding;
        }

        /// <summary>
        /// Gets the achievement "It becomes self aware at 2:14 AM"
        /// </summary>
        /// <returns></returns>
        internal static bool SelfAware()
        {
            return TutorialFinished;
        }

        /// <summary>
        /// Gets the achievement "Skynet"
        /// </summary>
        /// <returns></returns>
        internal static bool Skynet()
        {
            return PlatformsBuilt >= 30;
        }

        /// <summary>
        /// Gets the achievement "Wall E"
        /// </summary>
        /// <returns></returns>
        internal static bool WallE()
        {
            return TrashBurned >= 10000;
        }

        /// <summary>
        /// Gets the achievement "HAL9000"
        /// </summary>
        /// <returns></returns>
        internal static bool Hal9000()
        {
            return CampaignComplete;
        }

        /// <summary>
        /// Gets the achievement "Replicant"
        /// </summary>
        /// <returns></returns>
        internal static bool Replicant()
        {
            return UnitsBuilt >= 50;
        }

        /// <summary>
        /// Gets the achievement "Please rate our game perfect 5/7"
        /// </summary>
        /// <returns></returns>
        internal static bool RateOurGame()
        {
            return MilitaryUnitsBuilt >= 10000;
        }

        /// <summary>
        /// Gets the achievement "Overachiever"
        /// </summary>
        /// <returns></returns>
        internal static bool Overachiever()
        {
            if (!AllCompleted)
            {
                AllCompleted = SystemOnline() && SelfAware() && Skynet() && WallE() && Hal9000() && Replicant() &&
                               RateOurGame();
            }

            return AllCompleted;
        }
    }
}
