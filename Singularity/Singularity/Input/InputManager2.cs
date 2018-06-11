using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;

namespace Singularity.Input
{
    static class InputManager2
    {
        private static MouseState sPrevMouseState;
        private static MouseState sCurrentMouseState;

        private static KeyboardState sPrevKeyboardState;
        private static KeyboardState sCurrentKeyboardState;

        #region Left Button

        /// <summary>
        /// Returns a bool that says whether or not a the left mouse button has been
        /// clicked over a rectangular region.
        /// </summary>
        /// <param name="bounds">A rectangle representing the bounds of the button</param>
        /// <returns></returns>
        public static bool LeftClicked(Rectangle bounds)
        {
            if (sPrevMouseState.LeftButton == ButtonState.Released
                && sCurrentMouseState.LeftButton == ButtonState.Pressed)
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
            if (sPrevMouseState.LeftButton == ButtonState.Pressed
                && sCurrentMouseState.LeftButton == ButtonState.Released)
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
            if (sPrevMouseState.RightButton == ButtonState.Released
                && sCurrentMouseState.RightButton == ButtonState.Pressed)
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
            if (sPrevMouseState.RightButton == ButtonState.Pressed
                && sCurrentMouseState.RightButton == ButtonState.Released)
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
            if (sPrevMouseState.MiddleButton == ButtonState.Released
                && sCurrentMouseState.MiddleButton == ButtonState.Pressed)
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
            if (sPrevMouseState.MiddleButton == ButtonState.Pressed
                && sCurrentMouseState.MiddleButton == ButtonState.Released)
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
            return new Rectangle(sCurrentMouseState.Position, new Point(1, 1));
        }

        #endregion


        /// <summary>
        /// Updates the state of the values inside this class
        /// </summary>
        /// <param name="gameTime">Gives the current gametime to the class</param>
        public static void Update(GameTime gameTime)
        {
            sPrevMouseState = sCurrentMouseState;
            sCurrentMouseState = Mouse.GetState();

            sPrevKeyboardState = sCurrentKeyboardState;
            sCurrentKeyboardState = Keyboard.GetState();

        }
    }
}
