using Microsoft.Xna.Framework.Content;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <inheritdoc cref="Singularity.Property.IDraw"/>
    /// <inheritdoc cref="Singularity.Property.IUpdate"/>
    /// <remarks>
    /// An abstraction for "Screens" used by an IScreenManager. Possible examples
    /// for IScreens would be "Game Screens", "Main Menu Screens", etc..
    /// </remarks>
    public interface IScreen : IUpdate, IDraw
    {
        EScreen Screen { get; }

        /// <summary>
        /// Loads any content required by a screen.
        /// </summary>
        void LoadContent(ContentManager content);

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

        /// <summary>
        /// Determines whether the screen is already loaded or not
        /// </summary>
        /// <returns>true if the screen has once been loaded, false otherwise</returns>
        bool Loaded { get; set; }
    }
}
