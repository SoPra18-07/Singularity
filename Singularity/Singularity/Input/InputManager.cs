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
        private readonly List<IKeyListener> _mKeyListener;
        private readonly List<IMousePositionListener> _mMousePositionListener;
        private readonly List<IMouseClickListener> _mMouseClickListener;
        private readonly List<IMouseWheelListener> _mMouseWheelListener;

        private readonly Dictionary<IMouseClickListener, EClickType> _mLeftClickType;
        private readonly Dictionary<IMouseClickListener, EClickType> _mRightClickType;

        private MouseState _mCurrentMouseState;
        private MouseState _mPreviousMouseState;

        private KeyboardState _mCurrentKeyboardState;
        private KeyboardState _mPreviousKeyboardState;

        public InputManager()
        {
            _mKeyListener = new List<IKeyListener>();
            _mMousePositionListener = new List<IMousePositionListener>();
            _mMouseClickListener = new List<IMouseClickListener>();
            _mMouseWheelListener = new List<IMouseWheelListener>();

            _mLeftClickType = new Dictionary<IMouseClickListener, EClickType>();
            _mRightClickType = new Dictionary<IMouseClickListener, EClickType>();

            // get states for later
            _mPreviousMouseState = Mouse.GetState();
            _mPreviousKeyboardState = Keyboard.GetState();
        }

        public void AddKeyListener(IKeyListener iKeyListener)
        {
            _mKeyListener.Add(iKeyListener);
        }

        public bool RemoveKeyListener(IKeyListener iKeyListener)
        {
            if (!_mKeyListener.Contains(iKeyListener))
            {
                return false;
            }

            _mKeyListener.Remove(iKeyListener);
            return true;
        }

        public void AddMousePositionListener(IMousePositionListener iMouseListener)
        {
            _mMousePositionListener.Add(iMouseListener);
        }

        public bool RemoveMousePositionListener(IMousePositionListener iMouseListener)
        {
            if (!_mMousePositionListener.Contains(iMouseListener))
            {
                return false;
            }

            _mMousePositionListener.Remove(iMouseListener);
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
            _mMouseClickListener.Add(iMouseClickListener);
            _mLeftClickType.Add(iMouseClickListener, leftClickType);
            _mRightClickType.Add(iMouseClickListener, rightClickType);
        }

        public bool RemoveMouseClickListener(IMouseClickListener iMouseClickListener)
        {
            if (!_mMouseClickListener.Contains(iMouseClickListener))
            {
                return false;
            }

            _mMouseClickListener.Remove(iMouseClickListener);
            _mLeftClickType.Remove(iMouseClickListener);
            _mRightClickType.Remove(iMouseClickListener);
            return true;
        }

        public void AddMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            _mMouseWheelListener.Add(iMouseWheelListener);
        }

        public bool RemoveMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!_mMouseWheelListener.Contains(iMouseWheelListener))
            {
                return false;
            }

            _mMouseWheelListener.Remove(iMouseWheelListener);
            return true;
        }

        public void Update(GameTime gametime)
        {
            // update 'current' values
            _mCurrentMouseState = Mouse.GetState();
            _mCurrentKeyboardState = Keyboard.GetState();


            // # BEGIN - mouse wheel events


            if (_mCurrentMouseState.ScrollWheelValue < _mPreviousMouseState.ScrollWheelValue)
            // mouse wheel has been scrolled downwards -> create event 'ScrollDown'
            {
                foreach (var mouseWheelListener in _mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollDown);
                }
            }
            else if (_mCurrentMouseState.ScrollWheelValue > _mPreviousMouseState.ScrollWheelValue)
            // mouse wheel has been scrolled upwards -> create event 'ScrollUp'
            {
                foreach (var mouseWheelListener in _mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollUp);
                }
            }


            // # END - mouse wheel events


            // # BEGIN - mouse click events


            // process left mouse button changes
            switch (_mCurrentMouseState.LeftButton)
                // switch-case for left mouse button-state
            {
                case ButtonState.Pressed:
                    // left mouse button is pressed
                    if (_mPreviousMouseState.LeftButton != ButtonState.Pressed)
                    // left mouse button just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mLeftClickType[mouseListener])
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
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mLeftClickType[mouseListener])
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
                    if (_mPreviousMouseState.LeftButton == ButtonState.Pressed)
                    // left mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mLeftClickType[mouseListener])
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

            switch (_mCurrentMouseState.RightButton)
            // switch-case for right mouse button-state
            {
                case ButtonState.Pressed:
                    if (_mPreviousMouseState.RightButton != ButtonState.Pressed)
                    // right mouse button was just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mRightClickType[mouseListener])
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
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mRightClickType[mouseListener])
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
                    if (_mPreviousMouseState.RightButton == ButtonState.Pressed)
                    // right mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in _mMouseClickListener)
                        {
                            var doesIntersect = RectAtPosition(_mCurrentMouseState.X, _mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (_mRightClickType[mouseListener])
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

            if (_mCurrentMouseState.X != _mPreviousMouseState.X || _mCurrentMouseState.Y != _mPreviousMouseState.Y)
            {
                foreach (var mousePositionListener in _mMousePositionListener)
                {
                   mousePositionListener.MousePositionChanged(_mCurrentMouseState.X, _mCurrentMouseState.Y);
                }
            }

            // # END - mouse position changed events

            // # BEGIN - process key events

            var createPressed = false;

            foreach (var pressedKey in _mCurrentKeyboardState.GetPressedKeys())
            // go through all pressed keys and create events accordingly
            {
                if (!_mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in _mKeyListener)
                    {
                        keyListener.KeyTyped(new KeyEvent(_mCurrentKeyboardState.GetPressedKeys()));
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
                foreach (var keyListener in _mKeyListener)
                {
                    keyListener.KeyPressed(new KeyEvent(_mCurrentKeyboardState.GetPressedKeys()));
                }
            }

            var releasedKeys = new List<Keys>();

            foreach (var previouslyPressedKey in _mPreviousKeyboardState.GetPressedKeys())
            // go through all previously pressed keys and create events if they are no longer pressed
            {
                if (_mCurrentKeyboardState.GetPressedKeys().Contains(previouslyPressedKey))
                // the key was already pressed -> no event
                {
                    continue;
                }
                releasedKeys.Add(previouslyPressedKey);
            }

            if (releasedKeys.Count > 0)
            {

                // the key was released -> create event 'release'
                foreach (var keyListener in _mKeyListener)
                {
                    keyListener.KeyReleased(new KeyEvent(releasedKeys.ToArray()));
                }
            }

            // # END - process key events


            // update 'previous'-values
            _mPreviousMouseState = _mCurrentMouseState;
            _mPreviousKeyboardState = _mCurrentKeyboardState;

        }

        private static Rectangle RectAtPosition(float x, float y)
        {
            return new Rectangle((int) x, (int) y, 1, 1);
        }
    }
}
