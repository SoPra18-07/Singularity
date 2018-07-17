using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{


    [DataContract]
    public abstract class APlatformAction : IPlatformAction
    {
        [DataMember]
        protected Dictionary<GeneralUnit, JobType> mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
        [DataMember]
        protected PlatformBlank mPlatform;
        protected Director mDirector;

        [DataMember]
        public int Id { get; private set; }

        protected APlatformAction(PlatformBlank platform, ref Director director)
        {
            mPlatform = platform;
            mDirector = director;
            Id = director.GetIdGenerator.NextId();
        }

        public void ReloadContent(ref Director dir)
        {
            mDirector = dir;
        }

        [DataMember]
        public PlatformActionState State { get; protected set; } = PlatformActionState.Active;
        [DataMember]
        public abstract List<JobType> UnitsRequired { get; set; }
        [DataMember]
        PlatformBlank IPlatformAction.Platform
        {
            get { return mPlatform; }
            set { mPlatform = value; }
        }
        [DataMember]
        Dictionary<GeneralUnit, JobType> IPlatformAction.AssignedUnits
        {
            get { return mAssignedUnits; }
            set { mAssignedUnits = value; }
        }

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

        public bool Die()
        {
            if (mPlatform.Friendly)
            {
                mDirector.GetDistributionDirector.GetManager(mPlatform.GetGraphIndex()).Kill(this);
            }
            mAssignedUnits = new Dictionary<GeneralUnit, JobType>();
            mPlatform.Kill(this);
            mPlatform = null;

            return true;
        }

        public void Kill(GeneralUnit unit)
        {
            mAssignedUnits.Remove(unit);
        }

        public abstract void Update(GameTime t);
    }
}
