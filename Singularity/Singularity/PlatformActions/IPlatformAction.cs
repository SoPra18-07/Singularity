using System.Collections.Generic;
using System.Diagnostics;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.PlatformActions
{


    /// <summary>
    /// Provides a combined interface for everything required for the implementation of the PlatformActions.
    /// </summary>
    public interface IPlatformAction
    {

        /// <summary>
        /// Gets the state of the PlatformAction
        /// </summary>
        /// <value>The current state of the PlatformAction</value>
        PlatformActionState State { get; }

        /// <summary>
        /// Unique id of this PlatformAction.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the required resources of the PlatformAction
        /// (for finishing this action, or for producing the next resource etc)
        /// </summary>
        /// <returns>The required resources.</returns>
        Dictionary<EResourceType, int> GetRequiredResources();

        /// <summary>
        /// Execute this PlatformAction.
        /// Called from Units through 'Produce' from the Platform.
        /// </summary>
        void Execute();

        /// <summary>
        /// from the interface you toggled the state of the PlatformAction.
        /// responding accordingly.
        /// </summary>
        void UiToggleState();

        /// <summary>
        /// Assigns the unit to this PlatformAction and to this platform.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="job">Job.</param>
        void AssignUnit(GeneralUnit unit, JobType job);

        /// <summary>
        /// Unassigns an amount of assigned units based on their JobType.
        /// </summary>
        /// <param name="amount">The amount of units being UNassigned</param>
        /// <param name="job"></param>
        /// <returns>A list containing the unassigned units.</returns>
        List<GeneralUnit> UnAssignUnits(int amount, JobType job);

        /// <summary>
        /// Gets the JobType required for this PlatformAction. Can be several.
        /// (e.g. Factory needs Carrying units and Producing units,
        /// and every Blueprint needs Building units, ...)
        /// </summary>
        /// <value>The units required.</value>
        List<JobType> UnitsRequired { get; }

        /// <summary>
        /// Gets the assigned units and their respective JobTypes.
        /// </summary>
        /// <value>The assigned units.</value>
        Dictionary<GeneralUnit, JobType> AssignedUnits { get; }

        PlatformBlank Platform { get; set; }

        bool Die();
        void Kill(GeneralUnit generalUnit);
    }
}
