using System.Collections.Generic;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Screen
{
    /// <summary>
    /// The UserInterfaceController manages the exchange between classes and the UI
    /// </summary>
    public sealed class UserInterfaceController
    {
        private readonly Director mDirector;

        // the currently selected platform
        private PlatformBlank mActivePlatform;

        // the UI that is controlled by this UIController
        public UserInterfaceScreen ControlledUserInterface { private get; set; }

        /// <summary>
        /// Creates an UserInterfaceController which manages the exchange between classes and the UI
        /// </summary>
        /// <param name="director">the director</param>
        public UserInterfaceController(Director director)
        {
            mDirector = director;
        }

        /// <summary>
        /// Set the selectedPlatform's values in the UI when called
        /// </summary>
        /// <param name="id">the platform's id</param>
        /// <param name="type">the platform's type</param>
        /// <param name="resourceAmountList">resources on platform</param>
        /// <param name="unitAssignmentDict">units assigned to platform</param>
        /// <param name="actionsArray">possible actions of platform</param>
        public void SetDataOfSelectedPlatform(
            int id,
            bool isActive,
            bool isManuallyDeactivated,
            EPlatformType type,
            List<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentDict,
            List<IPlatformAction> actionsArray)
        {
            // set/update data in UI
            ControlledUserInterface.SetSelectedPlatformValues(id, isActive, isManuallyDeactivated, type, resourceAmountList, unitAssignmentDict, actionsArray);
        }

        /// <summary>
        /// Returns the idle-units amount
        /// </summary>
        /// <returns>amount of idle units</returns>
        public int GetIdleUnits(int graphid)
        {
            return mDirector.GetDistributionDirector.GetManager(graphid).GetJobCount(JobType.Idle);
        }

        /// <summary>
        /// Handles selection of platforms for the UI.
        /// Gets called by platforms if they notice that they're being clicked on.
        /// </summary>
        /// <param name="platform">platform that was selected</param>
        public void ActivateMe(PlatformBlank platform)
        {
            // deactivate previously selected platform
            if (mActivePlatform != null)
            {
                mActivePlatform.IsSelected = false;
            }

            platform.IsSelected = true;
            mActivePlatform = platform;
        }

        /// <summary>
        /// Handles deselection of platforms for the UI.
        /// Will be called if a platform is actively deselected (not automatically by selecting another platform)
        /// </summary>
        public void DeactivateSelection()
        {
            mActivePlatform.IsSelected = false;
        }

        public void ActivateSelectedPlatform()
        {
            mActivePlatform.Activate(true);
        }

        public void DeactivateSelectedPlatform()
        {
            mActivePlatform.Deactivate(true);
        }

        // TODO : ADD EVENT LOG CONTROLLING
    }
}
