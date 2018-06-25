using Singularity.Screen;

namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses mouse wheel input
    /// </summary>
    public interface IMouseWheelListener
    {

        /// <summary>
        /// The screen this object is on
        /// </summary>
        EScreen Screen { get; }

        /// <summary>
        /// This event is fired if the mouse wheel value has been changed.
        /// </summary>
        /// <param name="mouseAction">The mouse Action, either ScrollUp or ScrollDown</param>
        bool MouseWheelValueChanged(EMouseAction mouseAction);
    }
}
