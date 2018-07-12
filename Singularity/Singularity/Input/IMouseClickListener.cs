using Microsoft.Xna.Framework;
using Singularity.Screen;

namespace Singularity.Input
{
    /// <summary>
    /// An interface which provides mouse click events.
    /// </summary>
    public interface IMouseClickListener
    {
        /// <summary>
        /// The screen this object is on
        /// </summary>
        EScreen Screen { get; }

        /// <summary>
        /// The bounds of this object. Simplifies click events.
        /// </summary>
        Rectangle Bounds { get; }
        EMouseAction EMouseAction { get; set; }
        EClickType EClickType { get; set; }

        /// <summary>
        /// This will only be fired ONCE per mouse press, even if the mouse button is held down.
        /// Return true if the input should be passed further (is still unused).
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds);

        /// <summary>
        /// This will be fired the whole entirety the mouse button is held down.
        /// Return true if the input should be passed further (is still unused).
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds);

        /// <summary>
        /// This will be fired once when the mouse button was released.
        /// Return true if the input should be passed further (is still unused).
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds);
    }
}
