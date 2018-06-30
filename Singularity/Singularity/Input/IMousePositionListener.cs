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
        /// <param name="screenX">The new x mouse coordinate</param>
        /// <param name="screenY">The new y mouse coordinate</param>
        /// <param name="worldX">The world x coordinate</param>
        /// <param name="worldY">The world y coordinate</param>
        void MousePositionChanged(float screenX, float screenY, float worldX, float worldY);
    }
}
