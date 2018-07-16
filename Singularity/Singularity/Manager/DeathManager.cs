using System.Collections.Generic;
using System.Runtime.Serialization;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Manager
{
    [DataContract]
    public sealed class DeathManager
    {
        [DataMember]
        private readonly SortedList<int, List<IDie>> mToKill;

        public DeathManager()
        {
            mToKill = new SortedList<int, List<IDie>>(new AscendingIntegerComparer());
        }

        public void AddToKill(IDie objectToKill)
        {
            if (!mToKill.ContainsKey(objectToKill.DeathOrder))
            {
                mToKill[objectToKill.DeathOrder] = new List<IDie>();
            }

            // we want don't want to add the same object twice if somewhere FlagForDeath was called twice by accident.
            // killing the object twice even though it should only die once could lead in massive null pointer problems and stuff.
            if (mToKill[objectToKill.DeathOrder].Contains(objectToKill))
            {
                return;
            }
            mToKill[objectToKill.DeathOrder].Add(objectToKill);
        }

        public void KillAddedObjects()
        {
            if (mToKill.Count <= 0)
            {
                return;
            }

            foreach (var objectsToKill in mToKill.Values)
            {
                foreach (var killable in objectsToKill)
                {
                    killable.Die();
                }
                objectsToKill.Clear();

            }
            mToKill.Clear();
        }
    }
}
