using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Input
{
    static class InputManager2
    {
        private static MouseState _sPrevMouseState;
        private static MouseState _sCurrentMouseState;

        private static KeyboardState _sPrevKeyboardState;
        private static KeyboardState _sCurrentKeyboardState;

        #region Left Button

        /// <summary>
        /// Returns a bool that says whether or not a the left mouse button has been
        /// clicked over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool LeftClicked(Rectangle bounds)
        {
            if (_sPrevMouseState.LeftButton == ButtonState.Released
                && _sCurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        /// <summary>
        /// Returns a bool that says whether or not the left mouse button has been
        /// released over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool LeftReleased(Rectangle bounds)
        {
            if (_sPrevMouseState.LeftButton == ButtonState.Pressed
                && _sCurrentMouseState.LeftButton == ButtonState.Released)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        #endregion

        #region Right Button

        /// <summary>
        /// Returns a bool that says whether or not the right mouse button has been
        /// clicked over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool RightClicked(Rectangle bounds)
        {
            if (_sPrevMouseState.RightButton == ButtonState.Released
                && _sCurrentMouseState.RightButton == ButtonState.Pressed)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        /// <summary>
        /// Returns a bool that says whether or not the right mouse button has been
        /// released over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool RightReleased(Rectangle bounds)
        {
            if (_sPrevMouseState.RightButton == ButtonState.Pressed
                && _sCurrentMouseState.RightButton == ButtonState.Released)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        #endregion

        #region Middle Button

        /// <summary>
        /// Returns a bool that says whether or not the middle mouse button has been
        /// clicked over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool MiddleClicked(Rectangle bounds)
        {
            if (_sPrevMouseState.MiddleButton == ButtonState.Released
                && _sCurrentMouseState.MiddleButton == ButtonState.Pressed)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        /// <summary>
        /// Returns a bool that says whether or not the right mouse button has been
        /// released over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool MiddleReleased(Rectangle bounds)
        {
            if (_sPrevMouseState.MiddleButton == ButtonState.Pressed
                && _sCurrentMouseState.MiddleButton == ButtonState.Released)
            {
                // if the rectangle that is the cursor intersects the bounds of the region,
                // then it can be considered to have clicked the region.
                return MakeRectangleAtCursor().Intersects(bounds);
            }

            return false;
        }

        #endregion

        #region HelperMethods

        private static Rectangle MakeRectangleAtCursor()
        {
            // create a rectangle around the center of the mouse button
            return new Rectangle(_sCurrentMouseState.Position, new Point(1, 1));
        }

        #endregion


        /// <summary>
        /// Updates the state of the values inside this class
        /// </summary>
        /// <param name="gameTime">Gives the current gametime to the class</param>
        public static void Update(GameTime gameTime)
        {
            _sPrevMouseState = _sCurrentMouseState;
            _sCurrentMouseState = Mouse.GetState();

            _sPrevKeyboardState = _sCurrentKeyboardState;
            _sCurrentKeyboardState = Keyboard.GetState();

        }
    }
}
