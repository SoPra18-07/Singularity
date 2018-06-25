using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;
using Singularity.Screen;

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
        private readonly Dictionary<EScreen, List<IMouseClickListener>> mMouseClickListener;
        private readonly Dictionary<EScreen, List<IMouseWheelListener>> mMouseWheelListener;

        private readonly Dictionary<IMouseClickListener, EClickType> mLeftClickType;
        private readonly Dictionary<IMouseClickListener, EClickType> mRightClickType;

        private List<EScreen> mScreensToCheck;

        private MouseState mCurrentMouseState;
        private MouseState mPreviousMouseState;

        private KeyboardState mCurrentKeyboardState;
        private KeyboardState mPreviousKeyboardState;

        public InputManager()
        {
            mScreensToCheck = new List<EScreen>();

            mKeyListener = new List<IKeyListener>();
            mMousePositionListener = new List<IMousePositionListener>();
            mMouseClickListener = new Dictionary<EScreen, List<IMouseClickListener>>();
            mMouseWheelListener = new Dictionary<EScreen, List<IMouseWheelListener>>();

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
        /// </summary>
        /// <param name="iMouseClickListener">The object which should receive events</param>
        public void AddMouseClickListener(IMouseClickListener iMouseClickListener, EClickType leftClickType, EClickType rightClickType)
        {
            if (!mMouseClickListener.ContainsKey(iMouseClickListener.Screen))
            {
                mMouseClickListener[iMouseClickListener.Screen] = new List<IMouseClickListener>();
            }

            mMouseClickListener[iMouseClickListener.Screen].Add(iMouseClickListener);

            mLeftClickType.Add(iMouseClickListener, leftClickType);
            mRightClickType.Add(iMouseClickListener, rightClickType);
        }

        public bool RemoveMouseClickListener(IMouseClickListener iMouseClickListener)
        {
            if (!mMouseClickListener.ContainsKey(iMouseClickListener.Screen))
            {
                return false;
            }

            if (!mMouseClickListener[iMouseClickListener.Screen].Contains(iMouseClickListener))
            {
                return false;
            }

            mMouseClickListener[iMouseClickListener.Screen].Remove(iMouseClickListener);


            mLeftClickType.Remove(iMouseClickListener);
            mRightClickType.Remove(iMouseClickListener);
            return true;
        }

        public void AddMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.ContainsKey(iMouseWheelListener.Screen))
            {
                mMouseWheelListener[iMouseWheelListener.Screen] = new List<IMouseWheelListener>();
            }

            mMouseWheelListener[iMouseWheelListener.Screen].Add(iMouseWheelListener);
        }

        public bool RemoveMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.ContainsKey(iMouseWheelListener.Screen))
            {
                return false;
            }

            if (!mMouseWheelListener[iMouseWheelListener.Screen].Contains(iMouseWheelListener))
            {
                return false;
            }

            mMouseWheelListener[iMouseWheelListener.Screen].Remove(iMouseWheelListener);
            return true;
        }


        private bool CreateMouseWheelEvents(EScreen screen)
        {
            if (!mMouseWheelListener.ContainsKey(screen))
            {
                return true;
            }

            var giveThrough = true;

            if (mCurrentMouseState.ScrollWheelValue < mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled downwards -> create event 'ScrollDown'
            {
                foreach (var mouseWheelListener in mMouseWheelListener[screen])
                {
                    giveThrough = giveThrough && mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollDown);
                }
            }
            else if (mCurrentMouseState.ScrollWheelValue > mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled upwards -> create event 'ScrollUp'
            {
                foreach (var mouseWheelListener in mMouseWheelListener[screen])
                {
                    giveThrough = giveThrough && mouseWheelListener.MouseWheelValueChanged(EMouseAction.ScrollUp);
                }
            }

            return giveThrough;
        }


        private bool CreateMouseClickEvents(EScreen screen)
        {
            if (!mMouseClickListener.ContainsKey(screen))
            {
                return false;
            }

            var giveThrough = true;

            switch (mCurrentMouseState.LeftButton)
            // switch-case for left mouse button-state
            {
                case ButtonState.Pressed:
                    // left mouse button is pressed
                    if (mPreviousMouseState.LeftButton != ButtonState.Pressed)
                    // left mouse button just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.LeftClick, true);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.LeftClick, false);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.LeftClick, doesIntersect);
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // left mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.LeftClick, doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.LeftButton == ButtonState.Pressed)
                    // left mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mLeftClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.LeftClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.LeftClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.LeftClick, doesIntersect);
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
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.RightClick, true);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.RightClick, false);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonClicked(EMouseAction.RightClick, doesIntersect);
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // right mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }

                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.RightButton == ButtonState.Pressed)
                    // right mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener[screen])
                        {
                            var doesIntersect = RectAtPosition(mCurrentMouseState.X, mCurrentMouseState.Y)
                                .Intersects(mouseListener.Bounds);

                            switch (mRightClickType[mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.RightClick, true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.RightClick, false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonReleased(EMouseAction.RightClick, doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
            }

            return giveThrough;
        }


        private void CreateMousePositionEvents()
        {
            if (mCurrentMouseState.X != mPreviousMouseState.X || mCurrentMouseState.Y != mPreviousMouseState.Y)
            {
                foreach (var mousePositionListener in mMousePositionListener)
                {
                    mousePositionListener.MousePositionChanged(mCurrentMouseState.X, mCurrentMouseState.Y);
                }
            }
        }

        private void CreateKeyEvents()
        {
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
        }

        public void Update(GameTime gametime)
        {
            // update 'current' values
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();

            Debug.WriteLine(mScreensToCheck.Count);

            var giveWheelThrough = true;

            var giveClickThrough = true;

            foreach (var screen in mScreensToCheck)
            {
                if (giveWheelThrough) { 
                    giveWheelThrough = giveWheelThrough && CreateMouseWheelEvents(screen);

                }

                if (giveClickThrough) { 
                    giveClickThrough = giveClickThrough && CreateMouseClickEvents(screen);

                }

            }

            CreateMousePositionEvents();

            CreateKeyEvents();


            // update 'previous'-values
            mPreviousMouseState = mCurrentMouseState;
            mPreviousKeyboardState = mCurrentKeyboardState;

            mScreensToCheck.Clear();

        }

        internal void AddScreen(EScreen screen)
        {
            mScreensToCheck.Add(screen);
        }

        private static Rectangle RectAtPosition(float x, float y)
        {
            return new Rectangle((int) x, (int) y, 1, 1);
        }
    }
}
