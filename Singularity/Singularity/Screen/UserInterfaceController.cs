using System.Collections.Generic;
using System.Runtime.Serialization;
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
    /// The GetUserInterfaceController manages the exchange between anything and the UI
    /// </summary>
    [DataContract]
    public sealed class UserInterfaceController
    {
        // the basic director
        private Director mDirector;

        // the currently selected platform
        [DataMember]
        private PlatformBlank mActivePlatform;

        // the UI that is controlled by this UIController
        internal UserInterfaceScreen ControlledUserInterface { get; set; }

        /// <summary>
        /// Creates an GetUserInterfaceController which manages the exchange between classes and the UI
        /// </summary>
        /// <param name="director">the director</param>
        internal UserInterfaceController(Director director)
        {
            mDirector = director;
        }

        /// <summary>
        /// Called during load-game
        /// </summary>
        /// <param name="dir"></param>
        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
        }

        /// <summary>
        /// Set the selectedPlatform's values in the UI when called
        /// </summary>
        /// <param name="id">the platform's id</param>
        /// <param name="isManuallyDeactivated">true, if the platform was manually deactivated</param>
        /// <param name="type">the platform's type</param>
        /// <param name="resourceAmountList">resources on platform</param>
        /// <param name="unitAssignmentDict">units assigned to platform</param>
        /// <param name="actionsList">possible actions of platform</param>
        /// <param name="isActive">true, if the platform is active</param>
        internal void SetDataOfSelectedPlatform(
            int id,
            bool isActive,
            bool isManuallyDeactivated,
            EStructureType type,
            List<Resource> resourceAmountList,
            Dictionary<JobType, List<Pair<GeneralUnit, bool>>> unitAssignmentDict,
            List<IPlatformAction> actionsList)
        {
            // set/update data in UI
            ControlledUserInterface.SetSelectedPlatformValues(id, isActive, isManuallyDeactivated, type, resourceAmountList, unitAssignmentDict, actionsList);
        }

        /// <summary>
        /// Handles selection of platforms for the UI.
        /// Gets called by platforms if they notice that they're being clicked on.
        /// </summary>
        /// <param name="platform">platform that was selected</param>
        internal void ActivateMe(PlatformBlank platform)
        {
            // deactivate previously selected platform
            if (mActivePlatform != null)
            {
                mActivePlatform.IsSelected = false;
            }

            platform.IsSelected = true;
            mActivePlatform = platform;
        }

        /*
        /// <summary>
        /// Handles deselection of platforms for the UI.
        /// Will be called if a platform is actively deselected (not automatically by selecting another platform)
        /// </summary>
        public void DeactivateSelection()
        {
            mActivePlatform.IsSelected = false;
        } */

        /// <summary>
        /// Activates a platform manually. Is called from the UI.
        /// </summary>
        public void ActivateSelectedPlatform()
        {
            mActivePlatform.Activate(true);
        }

        /// <summary>
        /// Deactivates a platform manually. Is called from the UI.
        /// </summary>
        public void DeactivateSelectedPlatform()
        {
            mActivePlatform.Deactivate(true);
        }

        /// <summary>
        /// Updates the eventLog by passing the newest event and the oldest event from the GetEventLog
        /// </summary>
        /// <param name="newEvent">event to add to eventLog</param>
        /// <param name="oldEvent">oldest event to eventually delete</param>
        internal void UpdateEventLog(EventLogIWindowItem newEvent, EventLogIWindowItem oldEvent)
        {
            // if the eventLog exists -> update with new event + possible deletion of oldest event
            ControlledUserInterface?.UpdateEventLog(newEvent, oldEvent);
        }

        /// <summary>
        /// Merge two graphs of the graphswitcher in the UI
        /// </summary>
        /// <param name="newGraphId">the new, merged graph id</param>
        /// <param name="oldGraphId1">first merged graph</param>
        /// <param name="oldGraphId2">second merged graph</param>
        internal void MergeGraph(int newGraphId, int oldGraphId1, int oldGraphId2)
        {
            ControlledUserInterface?.MergeGraph(newGraphId, oldGraphId1, oldGraphId2);
        }

        /// <summary>
        /// A Graph is split -> just forward the information to the UI
        /// /// </summary>
        /// <param name="splittedGraphId"></param>
        internal void SplitGraph(int splittedGraphId)
        {
            ControlledUserInterface?.SplitGraph(splittedGraphId);
        }

        /// <summary>
        /// A SliderUpdate was requested -> just forward the information to the UI
        /// /// </summary>
        public void UpdateSLiderHandler()
        {
            ControlledUserInterface.UpdateSLiderHandler();
        }

        /// <summary>
        /// Automatically update the civilUnitsWindow with the selected Platform's graphId
        /// </summary>
        /// <param name="graphId">selectedPlatform graphID</param>
        public void SelectedPlatformSetsGraphId(int graphId)
        {
            ControlledUserInterface?.SelectedPlatformSetsGraphId(graphId);
        }

        public void BuildingProcessStarted(EStructureType structureType)
        {
            ControlledUserInterface.BuildingProcessStarted(structureType);
        }

        public void BuildingProcessFinished(EStructureType structureType)
        {
            ControlledUserInterface.BuildingProcessFinished(structureType);
        }
    }
}
