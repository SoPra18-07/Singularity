using Microsoft.Xna.Framework;

namespace Singularity.Screen
{
    /// <inheritdoc cref = "IScreen" />
    /// <summary>
    /// Any menu screen that has transitions should implement this interface
    /// </summary>
    internal interface ITransitionableMenu : IScreen
    {
        /// <summary>
        /// Gives the current state of the transition of this screen
        /// </summary>
        bool TransitionRunning { get; }

        /// <summary>
        /// Method called to tell a screen to transition and which screen it should transition to.
        /// </summary>
        /// <param name="eScreen">Target screen for the transition</param>
        /// <param name="gameTime">GameTime when the transition is called indicating transition start time</param>
        void TransitionTo(EScreen eScreen, GameTime gameTime);
    }
}
