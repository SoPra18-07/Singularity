using Singularity.property;

namespace Singularity.screen
{
    internal interface IScreen : IUpdate, IDraw
    { 
        /// <summary>
        /// Determines whether the screen below this one in a screen manager should be updated or not.
        /// </summary>
        /// <returns></returns>
        bool UpdateLower();

        /// <summary>
        /// Determines whether the screen below this one in a screen manager should be drawn or not.
        /// </summary>
        /// <returns></returns>
        bool DrawLower();
    }
}
