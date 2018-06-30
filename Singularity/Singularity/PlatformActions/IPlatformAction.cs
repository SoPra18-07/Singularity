﻿using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

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
        void UnAssignUnits(int amount, JobType job);

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

        PlatformBlank Platform { get; }
    }




    public abstract class APlatformAction : IPlatformAction
    {
        protected readonly Dictionary<GeneralUnit, JobType> mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
        protected readonly PlatformBlank mPlatform;
        protected readonly Director mDirector;

        protected APlatformAction(PlatformBlank platform, ref Director director)
        {
            mPlatform = platform;
            mDirector = director;
        }


        public PlatformActionState State { get; protected set; } = PlatformActionState.Active;

        public abstract List<JobType> UnitsRequired { get; }

        public PlatformBlank Platform => mPlatform;
        public Dictionary<GeneralUnit, JobType> AssignedUnits => mAssignedUnits;


        public abstract void Execute();

        /// <summary>
        /// Gets the required resources of the PlatformAction
        /// (for finishing this action, or for producing the next resource etc)
        /// </summary>
        /// <returns>The required resources.</returns>
        public abstract Dictionary<EResourceType, int> GetRequiredResources();

        public abstract void UiToggleState();
        /* This is a demonstration of how this might be implemented:
        {
            switch (State)
            {
                case PlatformActionState.Available:
                    mDirector.GetDistributionManager.PausePlatformAction(self);
                    State = PlatformActionState.Deactivated;
                    break;
                case PlatformActionState.Deactivated:
                    State = PlatformActionState.Available;
                    break;
                default:
                    throw new AccessViolationException(message: "Someone/Something acccessed the state!!");
            }
        }
        */

        /// <summary>
        /// Assigns the unit to this PlatformAction and to this platform.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="job">Job.</param>
        void IPlatformAction.AssignUnit(GeneralUnit unit, JobType job)
        {
            mAssignedUnits.Add(key: unit, value: job);
        }

        public void UnAssignUnits(int amount, JobType job)
        {
            foreach (var unit in mAssignedUnits.Keys)
            {
                if (unit.Job != job || amount <= 0) continue;
                mAssignedUnits.Remove(key: unit);
                amount -= 1;
            }
        }
    }
}
