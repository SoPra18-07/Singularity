using System;
using System.Collections.Generic;
using Singularity.Manager;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;

namespace Singularity.PlatformActions
{
    internal sealed class Shoot : APlatformAction
    {
        private new DefenseBase mPlatform;
        public Shoot(DefenseBase platform, ref Director director) : base(platform, ref director)
        {
            mPlatform = platform;
        }

        public override void Execute()
        {
            var enemyPosition = mPlatform.EnemyPosition;
            throw new NotImplementedException();
        }

        public override void UiToggleState()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<EResourceType, int> GetRequiredResources()
        {
            throw new NotImplementedException();
        }

        public override List<JobType> UnitsRequired { get; }
    }
}
