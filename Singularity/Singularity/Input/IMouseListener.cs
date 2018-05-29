﻿namespace Singularity.Input
{
    /// <summary>
    /// Provides an interface for everything that uses mouse input
    /// </summary>
    internal interface IMouseListener
    {
        /// <summary>
        /// Used to set a mouse button as pressed
        /// </summary>
        /// <param name="mouseEvent"></param>
        void MousePressed(MouseEvent mouseEvent);

        /// <summary>
        /// Used to set a mouse button as released
        /// </summary>
        /// <param name="mouseEvent"></param>
        void MouseReleased(MouseEvent mouseEvent);
    }
}