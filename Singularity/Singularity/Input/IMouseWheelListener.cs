namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses mouse wheel input
    /// </summary>
    internal interface IMouseWheelListener
    {
        /// <summary>
        /// Used to set the mouse wheel as changed
        /// </summary>
        /// <param name="mouseEvent"></param>
        void MouseWheelValueChanged(MouseEvent mouseEvent);
    }
}
