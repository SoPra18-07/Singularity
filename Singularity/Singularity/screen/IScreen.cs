using Singularity.property;

namespace Singularity.screen
{
    /// <inheritdoc cref="Singularity.property.IDraw"/>
    /// <inheritdoc cref="Singularity.property.IUpdate"/>
    /// <remarks>
    /// An abstraction for "Screens" used by an IScreenManager. Possible examples
    /// for IScreens would be "Game Screens", "Main Menu Screens", etc..
    /// </remarks>
    internal interface IScreen : IUpdate, IDraw
    { 
        /// <summary>
        /// Determines whether the screen below this one in a screen manager should be updated or not.
        /// </summary>
        /// <returns>true if the lower screen should be updates, false otherwise</returns>
        bool UpdateLower();

        /// <summary>
        /// Determines whether the screen below this one in a screen manager should be drawn or not.
        /// </summary>
        /// <returns>true if the lower screen should be drawn, false otherwise</returns>
        bool DrawLower();
    }
}
