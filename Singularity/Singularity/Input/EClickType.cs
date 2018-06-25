namespace Singularity.Input
{
    public enum EClickType
    {
        /// <summary>
        /// Represents whether only in bounds click events should be fired.
        /// </summary>
        InBoundsOnly,

        /// <summary>
        /// Represents whether only out of bounds click events should be fired.
        /// </summary>
        OutOfBoundsOnly,

        /// <summary>
        /// Represents that both, in bounds and out of bounds click events should be fired.
        /// </summary>
        Both
    }
}
