using Microsoft.Xna.Framework;

namespace Singularity.Manager
{
    public class StoryManager
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

        internal void Update(GameTime gametime)
        {
            // todo this
        }
    }
}
