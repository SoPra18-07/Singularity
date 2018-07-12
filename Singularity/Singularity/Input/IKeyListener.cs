using Singularity.Screen;

namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses keys
    /// </summary>
    public interface IKeyListener
    {
        EScreen Screen { get; }

        /// <summary>
        /// Used to set a key as typed
        /// </summary>
        /// <param name="keyEvent"></param>
        bool KeyTyped(KeyEvent keyEvent);

        /// <summary>
        /// Used to set a key as pressed
        /// </summary>
        /// <param name="keyEvent"></param>
        bool KeyPressed(KeyEvent keyEvent);

        /// <summary>
        /// Used to set a key as released
        /// </summary>
        /// <param name="keyEvent"></param>
        bool KeyReleased(KeyEvent keyEvent);
    }
}
