

namespace Singularity.Property
{
    public interface IDie
    {
        /// <summary>
        /// An integer showing when to let this die in comparision to all the other IDie implementations. Where 0 is first and the higher
        /// the int the later it dies.
        /// </summary>
        int DeathOrder { get; }

        /// <summary>
        /// Flags this object to die, being removed specified by its order at the end of the update cycle by the DeathManager.
        /// </summary>
        /// <returns></returns>
        bool FlagForDeath();

        /// <summary>
        /// Letting object 'Die', by removing all it's references from other things
        /// </summary>
        bool Die();
    }
}
