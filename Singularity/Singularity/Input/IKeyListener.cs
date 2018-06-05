namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses keys
    /// </summary>
    public interface IKeyListener
    {
        /// <summary>
        /// Used to set a key as typed
        /// </summary>
        /// <param name="keyEvent"></param>
        void KeyTyped(KeyEvent keyEvent);

        /// <summary>
        /// Used to set a key as pressed
        /// </summary>
        /// <param name="keyEvent"></param>
        void KeyPressed(KeyEvent keyEvent);

        /// <summary>
        /// Used to set a key as released
        /// </summary>
        /// <param name="keyEvent"></param>
        void KeyReleased(KeyEvent keyEvent);
    }
}
