using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// This will only be fired ONCE per mouse press, even if the mouse button is held down.
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds);

        /// <summary>
        /// This will be fired the whole entirety the mouse button is held down.
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds);

        /// <summary>
        /// This will be fired once when the mouse button was released.
        /// </summary>
        /// <param name="mouseAction">The mouse action, either LeftClick or RightClick</param>
        /// <param name="withinBounds">Whether the event was fired within the objects bounds or not</param>
        bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds);
    }
}
