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
        /// <param name="newX">The new x mouse coordinate</param>
        /// <param name="newY">The new y mouse coordinate</param>
        void MousePositionChanged(float screenX, float screenY, float worldX, float worldY);
    }
}
