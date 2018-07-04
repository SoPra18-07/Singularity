using System;
using System.Collections.Generic;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Screen
{
    public sealed class UserInterfaceController
    {
        private PlatformBlank mActivePlatform;

        public UserInterfaceController()
        {
            // anything useful?
        }

        public void SetDataOfSelectedPlatform(
            EPlatformType type,
            List<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentDict,
            List<IPlatformAction> actionsArray)
        {
            ControlledUserInterface.SetSelectedPlatformValues(type, resourceAmountList, unitAssignmentDict, actionsArray);
        }

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

        public void DeactivateSelection()
        {
            mActivePlatform.IsSelected = false;
        }

        public UserInterfaceScreen ControlledUserInterface { get; set; }

        // TODO : ADD EVENT LOG CONTROLLING
    }
}
