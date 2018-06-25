using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;

namespace Singularity.Input
{
    /// <inheritdoc />
    /// <summary>
    /// Manages the UserInput
    /// </summary>
    public sealed class InputManager : IUpdate
    {
        private readonly List<IKeyListener> mKeyListener;
        private readonly List<IMousePositionListener> mMousePositionListener;
        private readonly List<IMouseClickListener> mMouseClickListener;
        private readonly List<IMouseWheelListener> mMouseWheelListener;

        private readonly Dictionary<IMouseClickListener, EClickType> mLeftClickType;
        private readonly Dictionary<IMouseClickListener, EClickType> mRightClickType;

        private MouseState mCurrentMouseState;
        private MouseState mPreviousMouseState;

        private KeyboardState mCurrentKeyboardState;
        private KeyboardState mPreviousKeyboardState;

        public InputManager()
        {
            mKeyListener = new List<IKeyListener>();
            mMousePositionListener = new List<IMousePositionListener>();
            mMouseClickListener = new List<IMouseClickListener>();
            mMouseWheelListener = new List<IMouseWheelListener>();

            mLeftClickType = new Dictionary<IMouseClickListener, EClickType>();
            mRightClickType = new Dictionary<IMouseClickListener, EClickType>();

            // get states for later
            mPreviousMouseState = Mouse.GetState();
            mPreviousKeyboardState = Keyboard.GetState();
        }

        public void AddKeyListener(IKeyListener iKeyListener)
        {
            mKeyListener.Add(iKeyListener);
        }

        public bool RemoveKeyListener(IKeyListener iKeyListener)
        {
            if (!mKeyListener.Contains(iKeyListener))
            {
                return false;
            }

            mKeyListener.Remove(iKeyListener);
            return true;
        }

        public void AddMousePositionListener(IMousePositionListener iMouseListener)
        {
            mMousePositionListener.Add(iMouseListener);
        }

        public bool RemoveMousePositionListener(IMousePositionListener iMouseListener)
        {
            if (!mMousePositionListener.Contains(iMouseListener))
            {
                return false;
            }

            mMousePositionListener.Remove(iMouseListener);
            return true;
        }

        /// <summary>
        /// Adds the given object to the objects that receive mouse click events.
        /// EClickType is inBounds, outOfBounds or both.
        /// </summary>
        /// <param name="iMouseClickListener">The object which should receive events</param>
        /// <param name="leftClickType">The LeftClickType</param>
        /// <param name="rightClickType">The RightClickType</param>
        public void AddMouseClickListener(IMouseClickListener iMouseClickListener, EClickType leftClickType, EClickType rightClickType)
        {
            mMouseClickListener.Add(iMouseClickListener);
            mLeftClickType.Add(iMouseClickListener, leftClickType);
            mRightClickType.Add(iMouseClickListener, rightClickType);
        }

        public bool RemoveMouseClickListener(IMouseClickListener iMouseClickListener)
        {
            if (!mMouseClickListener.Contains(iMouseClickListener))
            {
                return false;
            }

            mMouseClickListener.Remove(iMouseClickListener);
            mLeftClickType.Remove(iMouseClickListener);
            mRightClickType.Remove(iMouseClickListener);
            return true;
        }

        public void AddMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            mMouseWheelListener.Add(iMouseWheelListener);
        }

        public bool RemoveMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.Contains(iMouseWheelListener))
            {
                return false;
            }

            mMouseWheelListener.Remove(iMouseWheelListener);
            return true;
        }

        public void Update(GameTime gametime)
        {
            // update 'current' values
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();


            // # BEGIN - mouse wheel events


            if (mCurrentMouseState.ScrollWheelValue < mPreviousMouseState.ScrollWheelValue)
            // mouse wheel has been scrolled downwards -> create event 'ScrollDown'
            {
                foreach (var mouseWheelListener in mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollDown);
                }
            }
            else if (mCurrentMouseState.ScrollWheelValue > mPreviousMouseState.ScrollWheelValue)
            // mouse wheel has been scrolled upwards -> create event 'ScrollUp'
            {
                foreach (var mouseWheelListener in mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollUp);
                }
            }


            // # END - mouse wheel events


            // # BEGIN - mouse click events


            // process left mouse button changes
            switch (mCurrentMouseState.LeftButton)
                // switch-case for left mouse button-state
            {
                case ButtonState.Pressed:
                    // left mouse button is pressed
                    if (mPreviousMouseState.LeftButton != ButtonState.Pressed)
                    // left mouse button just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonClicked(EMouseAction.LeftClick, true);
                                        mouseListener.MouseButtonPressed(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonClicked(EMouseAction.LeftClick, false);
                                        mouseListener.MouseButtonPressed(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonClicked(EMouseAction.LeftClick, doesIntersect);
                                    mouseListener.MouseButtonPressed(EMouseAction.LeftClick, doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // left mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonPressed(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonPressed(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonPressed(EMouseAction.LeftClick, doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.LeftButton == ButtonState.Pressed)
                    // left mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonReleased(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonReleased(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonReleased(EMouseAction.LeftClick, doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
            }

            switch (mCurrentMouseState.RightButton)
            // switch-case for right mouse button-state
            {
                case ButtonState.Pressed:
                    if (mPreviousMouseState.RightButton != ButtonState.Pressed)
                    // right mouse button was just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonClicked(EMouseAction.RightClick, true);
                                        mouseListener.MouseButtonPressed(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonClicked(EMouseAction.RightClick, false);
                                        mouseListener.MouseButtonPressed(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonClicked(EMouseAction.RightClick, doesIntersect);
                                    mouseListener.MouseButtonPressed(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // right mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonPressed(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonPressed(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonPressed(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }

                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.RightButton == ButtonState.Pressed)
                    // right mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        mouseListener.MouseButtonReleased(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        mouseListener.MouseButtonReleased(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    mouseListener.MouseButtonReleased(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
            }

            // # END - mouse click events

            // # BEGIN - mouse position changed events

            if (mCurrentMouseState.X != mPreviousMouseState.X || mCurrentMouseState.Y != mPreviousMouseState.Y)
            {
                foreach (var mousePositionListener in mMousePositionListener)
                {
                   mousePositionListener.MousePositionChanged(mCurrentMouseState.X, mCurrentMouseState.Y);
                }
            }

            // # END - mouse position changed events

            // # BEGIN - process key events

            var createPressed = false;

            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
            // go through all pressed keys and create events accordingly
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyTyped(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                    }
                }
                else
                // key was already pressed -> create event 'pressed'
                {
                    createPressed = true;
                }
            }

            if (createPressed)
            {
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                }
            }

            var releasedKeys = new List<Keys>();

            foreach (var previouslyPressedKey in mPreviousKeyboardState.GetPressedKeys())
            // go through all previously pressed keys and create events if they are no longer pressed
            {
                if (mCurrentKeyboardState.GetPressedKeys().Contains(previouslyPressedKey))
                // the key was already pressed -> no event
                {
                    continue;
                }
                releasedKeys.Add(previouslyPressedKey);
            }

            if (releasedKeys.Count > 0)
            {

                // the key was released -> create event 'release'
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyReleased(new KeyEvent(releasedKeys.ToArray()));
                }
            }

            // # END - process key events


            // update 'previous'-values
            mPreviousMouseState = mCurrentMouseState;
            mPreviousKeyboardState = mCurrentKeyboardState;

        }

        private static Rectangle RectAtPosition(float x, float y)
        {
            return new Rectangle((int) x, (int) y, 1, 1);
        }
    }
}
