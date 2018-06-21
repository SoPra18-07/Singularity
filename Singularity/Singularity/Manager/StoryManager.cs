using Microsoft.Xna.Framework;

namespace Singularity.Manager
{
    public class StoryManager
    {
        private int _mEnergyLevel;

        public int GetEnergyLevel()
        {
            return _mEnergyLevel;
        }

        public void AddEnergy(int energy)
        {
            _mEnergyLevel += energy;
        }

        internal void Update(GameTime gametime)
        {
            // todo this
        }
    }
}
