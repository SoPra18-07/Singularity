using System;
using System.Collections.Generic;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.platform
{


    /// <summary>
    /// Provides a combined interface for everything required for the implementation of the PlatformActions.
    /// </summary>
    interface IPlatformAction
    {

        /// <summary>
        /// Gets the state of the PlatformAction
        /// </summary>
        /// <value>The current state of the PlatformAction</value>
        PlatformActionState State { get; }

        /// <summary>
        /// Gets the required resources of the PlatformAction
        /// (for finishing this action, or for producing the next resource etc)
        /// </summary>
        /// <returns>The required resources.</returns>
        Dictionary<EResourceType, int> getRequiredResources();

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
        /// Unassigns the assigned unit with the given unitid.
        /// </summary>
        /// <param name="unitid">UnitId of the unit to be unassigned</param>
        void UnAssignUnit(int unitid);

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
        Dictionary<GeneralUnit, Singularity.Units.JobType> AssignedUnits { get; }

        PlatformBlank Platform { get; }
    }




    public abstract class APlatformAction : IPlatformAction
    {
        private Dictionary<GeneralUnit, JobType> _assignedUnits = new Dictionary<GeneralUnit, JobType>();
        private readonly PlatformBlank _platform;

        public APlatformAction(PlatformBlank platform)
        {
             _platform = platform;
        }


        public PlatformActionState State { get; set; } = PlatformActionState.Active;

        public abstract List<JobType> UnitsRequired { get; }

        PlatformBlank IPlatformAction.Platform => _platform;
        Dictionary<GeneralUnit, JobType> IPlatformAction.AssignedUnits => _assignedUnits;

        /// <summary>
        /// Assigns the unit to this PlatformAction and to this platform.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="job">Job.</param>
        void IPlatformAction.AssignUnit(GeneralUnit unit, JobType job)
        {
            _assignedUnits.Add(unit, job);
        }

        public abstract void Execute();

        /// <summary>
        /// Gets the required resources of the PlatformAction
        /// (for finishing this action, or for producing the next resource etc)
        /// </summary>
        /// <returns>The required resources.</returns>
        Dictionary<EResourceType, int> IPlatformAction.getRequiredResources()
        {
            return new Dictionary<EResourceType, int>();
        }

        void IPlatformAction.UiToggleState()
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    State = PlatformActionState.Deactivated;
                    break;
                case PlatformActionState.Deactivated:
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException("Someone/Something acccessed the state!!");
            }
        }

        public void UnAssignUnit(int unitid)
        {
        // TODO: depending on the UI this requires to be written differently.

            foreach (var unit in _assignedUnits.Keys)
            {
                if (unit.Id == unitid)
                {
                    _assignedUnits.Remove(unit);
                }
                break;
            }
        }
    }
}
