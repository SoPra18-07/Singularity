using Singularity.Property;

namespace Singularity.Screen
{
    /// <inheritdoc cref="Singularity.Property.IDraw"/>
    /// <inheritdoc cref="Singularity.Property.IUpdate"/>
    /// <remarks>
    /// Provides an abstraction for an IScreenManager. Allows for multiple different
    /// implementations if the need arises. A ScreenManager should manage all current screens
    /// added to it, always drawing and updating the "top"-Screen and possibly the screens
    /// below if they say so.
    /// </remarks>
    public interface IScreenManager : IUpdate, IDraw
    {
        /// <summary>
        /// Adds the given screen to the screen manager as the "top" screen.
        /// </summary>
        /// <param name="screen">The new "top" screen</param>
        void AddScreen(IScreen screen);

        /// <summary>
        /// Removes the "top" screen from the screen manager.
        /// </summary>
        void RemoveScreen();
    }
}
