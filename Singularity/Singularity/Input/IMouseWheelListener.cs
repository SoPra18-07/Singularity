namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses mouse wheel input
    /// </summary>
    internal interface IMouseWheelListener
    {
        /// <summary>
        /// Used to set the mouse wheel as scrolled upwards
        /// </summary>
        /// <param name="mouseEvent"></param>
        void ScrollUp(MouseEvent mouseEvent);

        /// <summary>
        /// Used  to set the mouse wheel as scrolled downwards
        /// </summary>
        /// <param name="mouseEvent"></param>
        void ScrollDown(MouseEvent mouseEvent);
    }
}
