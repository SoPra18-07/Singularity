﻿namespace Singularity.StoryManager
{
    class StoryManager
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
    }
}
