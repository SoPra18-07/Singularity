using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.StoryManager
{
    class StoryManager
    {
        private int mEnergyLevel;

        public int GetEnergyLevel()
        {
            return mEnergyLevel;
        }

        public void AddEnergy(int energy)
        {
            mEnergyLevel += energy;
        }
    }
}
