using Singularity.property;

namespace Singularity.screen
{
    internal interface IScreenManager : IUpdate, IDraw
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
