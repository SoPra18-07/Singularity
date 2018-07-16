using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Manager;

namespace Singularity.Property
{
    public abstract class ADie : IDie
    {
        private Director mDirector;

        protected ADie(ref Director director)
        {
            mDirector = director;
        }

        public int DeathOrder { get; protected set; } = 0;

        public abstract bool Die();

        public bool FlagForDeath()
        {
            mDirector.GetDeathManager.AddToKill(this);
            return true;
        }
    }
}
