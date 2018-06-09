namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses mouse wheel input
    /// </summary>
    public interface IMouseWheelListener
    {
        /// <summary>
        /// This event is fired if the mouse wheel value has been changed.
        /// </summary>
        /// <param name="mouseAction">The mouse Action, either ScrollUp or ScrollDown</param>
        void MouseWheelValueChanged(EMouseAction mouseAction);
    }
}
