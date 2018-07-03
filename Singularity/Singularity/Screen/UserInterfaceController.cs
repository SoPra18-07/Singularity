using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
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
        public UserInterfaceController()
        {
            
        }

        public void SetDataOfSelectedPlatform(EPlatformType type,
            List<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentDict,
            IPlatformAction[] actionsArray)
        {
            ControlledUserInterface.SetSelectedPlatformValues(type, resourceAmountList, unitAssignmentDict, actionsArray);
        }

        public UserInterfaceScreen ControlledUserInterface { get; set; }

        // TODO : ADD EVENT LOG CONTROLLING

        // TODO : ADD UNIT DISTRIBUTION CONTROLLING
    }
}
