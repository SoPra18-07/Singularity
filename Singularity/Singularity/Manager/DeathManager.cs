using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Manager
{
    public sealed class DeathManager
    {
        private readonly SortedList<int, IDie> mToKill;

        public DeathManager()
        {
            mToKill = new SortedList<int, IDie>(new AscendingIntegerComparer());
        }

        public void AddToKill(IDie objectToKill)
        {
            mToKill.Add(objectToKill.DeathOrder, objectToKill);
        }

        public void KillAddedObjects()
        {
            if (mToKill.Count <= 0)
            {
                return;
            }

            foreach (var objectToKill in mToKill.Values)
            {
                objectToKill.Die();
            }
            mToKill.Clear();
        }
    }
}
