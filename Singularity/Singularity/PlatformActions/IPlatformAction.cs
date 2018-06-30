using System.Collections.Generic;
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

        PlatformBlank Platform { get; set; }

        bool Die();
        void Kill(GeneralUnit generalUnit);
    }




    public abstract class APlatformAction : IPlatformAction
    {
        protected Dictionary<GeneralUnit, JobType> mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
        protected PlatformBlank mPlatform;
        protected readonly Director mDirector;

        public int Id { get; }

        protected APlatformAction(PlatformBlank platform, ref Director director)
        {
            mPlatform = platform;
            mDirector = director;
            Id = IdGenerator.NextiD();
        }


        public PlatformActionState State { get; protected set; } = PlatformActionState.Active;

        public abstract List<JobType> UnitsRequired { get; }

        PlatformBlank IPlatformAction.Platform
        {
            get { return mPlatform; }
            set { mPlatform = value; }
        }

        Dictionary<GeneralUnit, JobType> IPlatformAction.AssignedUnits => mAssignedUnits;

        /// <summary>
        /// Assigns the unit to this PlatformAction and to this platform.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="job">Job.</param>
        void IPlatformAction.AssignUnit(GeneralUnit unit, JobType job)
        {
            mAssignedUnits.Add(unit, job);
        }

        public abstract void Execute();

        /// <summary>
        /// Gets the required resources of the PlatformAction
        /// (for finishing this action, or for producing the next resource etc)
        /// </summary>
        /// <returns>The required resources.</returns>
        public abstract Dictionary<EResourceType, int> GetRequiredResources();

        public abstract void UiToggleState();
        /* {
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
        */

        public void UnAssignUnits(int amount, JobType job)
        {
            foreach (var unit in mAssignedUnits.Keys)
            {
                if (unit.Job != job || amount <= 0) continue;
                mAssignedUnits.Remove(unit);
                amount -= 1;
            }
        }

        public bool Die()
        {
            mDirector.GetDistributionManager.Kill(this);
            mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
            mPlatform = null;

            return true;
        }

        public void Kill(GeneralUnit unit)
        {
            mAssignedUnits.Remove(unit);
        }
    }
}
