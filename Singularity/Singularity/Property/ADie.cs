using System.Runtime.Serialization;
using Singularity.Manager;

namespace Singularity.Property
{
    [DataContract]
    public abstract class ADie : IDie
    {
        protected Director mDirector;

        protected ADie(ref Director director)
        {
            mDirector = director;
        }

        [DataMember]
        public int DeathOrder { get; protected set; } = 0;

        public virtual void ReloadContent(ref Director dir)
        {
            mDirector = dir;
        }
        public abstract bool Die();

        public bool FlagForDeath()
        {
            mDirector.GetDeathManager.AddToKill(this);
            return true;
        }
    }
}
