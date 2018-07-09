using System.Collections.Generic;
using System.Diagnostics;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.PlatformActions
{
    public abstract class APlatformAction : IPlatformAction
    {
        protected Dictionary<GeneralUnit, JobType> mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
        protected PlatformBlank mPlatform;
        protected Director mDirector;

        public int Id { get; }

        protected APlatformAction(PlatformBlank platform, ref Director director)
        {
            if (platform == null)
                Debug.WriteLine("Platform null. Panic.");

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

        // The button in the UI got clicked. Now change the state accordingly.
        public abstract void UiToggleState();


        public List<GeneralUnit> UnAssignUnits(int amount, JobType job)
        {
            var list = new List<GeneralUnit>();
            foreach (var unit in mAssignedUnits.Keys)
            {
                if (unit.Job != job || amount <= 0) continue;
                mAssignedUnits.Remove(unit);
                list.Add(unit);
                amount -= 1;
            }

            return list;
        }

        // this is supposed to remove all references to this PlatformAction
        public bool Die()
        {
            mDirector.GetDistributionManager.Kill(this);
            mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
            mPlatform.Kill(this);
            mPlatform = null;

            return true;
        }

        // here a Unit want's to die (wants it's references to be removed)
        public void Kill(GeneralUnit unit)
        {
            mAssignedUnits.Remove(unit);
        }
    }
}
