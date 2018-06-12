using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Input
{
    /// <summary>
    /// An interface providing mouse position values.
    /// </summary>
    public interface IMousePositionListener
    {
        /// <summary>
        /// Gets fired if and only if the mouse position gets changed.
        /// </summary>
        /// <param name="relX">The relative x coordinate of the mouse</param>
        /// <param name="relY">The relative y coordinate of the mouse</param>
        /// <param name="absX">The absolute x coordinate of the mouse</param>
        /// <param name="absY">The absolute y coordinate of the mouse</param>
        void MousePositionChanged(float relX, float relY, float absX, float absY);
    }
}
