﻿namespace Singularity.Property
{
    public interface IDamageable
    {
        /// <summary>
        /// Stores an object's health
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Determines if a unit is friendly or not.
        /// </summary>
        bool Friendly { get; }

        /// <summary>
        /// Heal the platform or inflict damage on it.
        /// </summary>
        /// <param name="damage">Amount of damage to take. Negative values mean healing</param>
        void MakeDamage(int damage);
    }
}
