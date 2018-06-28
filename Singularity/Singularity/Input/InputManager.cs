using System.Collections.Generic;
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
            mKeyListener.Add(item: iKeyListener);
        }

        public bool RemoveKeyListener(IKeyListener iKeyListener)
        {
            if (!mKeyListener.Contains(item: iKeyListener))
            {
                return false;
            }

            mKeyListener.Remove(item: iKeyListener);
            return true;
        }

        public void AddMousePositionListener(IMousePositionListener iMouseListener)
        {
            mMousePositionListener.Add(item: iMouseListener);
        }

        public bool RemoveMousePositionListener(IMousePositionListener iMouseListener)
        {
            if (!mMousePositionListener.Contains(item: iMouseListener))
            {
                return false;
            }

            mMousePositionListener.Remove(item: iMouseListener);
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
            if (!mMouseClickListener.ContainsKey(key: iMouseClickListener.Screen))
            {
                mMouseClickListener[key: iMouseClickListener.Screen] = new List<IMouseClickListener>();
            }

            mMouseClickListener[key: iMouseClickListener.Screen].Add(item: iMouseClickListener);

            mLeftClickType.Add(key: iMouseClickListener, value: leftClickType);
            mRightClickType.Add(key: iMouseClickListener, value: rightClickType);
        }

        public bool RemoveMouseClickListener(IMouseClickListener iMouseClickListener)
        {
            if (!mMouseClickListener.ContainsKey(key: iMouseClickListener.Screen))
            {
                return false;
            }

            if (!mMouseClickListener[key: iMouseClickListener.Screen].Contains(item: iMouseClickListener))
            {
                return false;
            }

            mMouseClickListener[key: iMouseClickListener.Screen].Remove(item: iMouseClickListener);


            mLeftClickType.Remove(key: iMouseClickListener);
            mRightClickType.Remove(key: iMouseClickListener);
            return true;
        }

        public void AddMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.ContainsKey(key: iMouseWheelListener.Screen))
            {
                mMouseWheelListener[key: iMouseWheelListener.Screen] = new List<IMouseWheelListener>();
            }

            mMouseWheelListener[key: iMouseWheelListener.Screen].Add(item: iMouseWheelListener);
        }

        public bool RemoveMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.ContainsKey(key: iMouseWheelListener.Screen))
            {
                return false;
            }

            if (!mMouseWheelListener[key: iMouseWheelListener.Screen].Contains(item: iMouseWheelListener))
            {
                return false;
            }

            mMouseWheelListener[key: iMouseWheelListener.Screen].Remove(item: iMouseWheelListener);
            return true;
        }


        private bool CreateMouseWheelEvents(EScreen screen)
        {
            if (!mMouseWheelListener.ContainsKey(key: screen))
            {
                return true;
            }

            var giveThrough = true;

            if (mCurrentMouseState.ScrollWheelValue < mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled downwards -> create event 'ScrollDown'
            {
                foreach (var mouseWheelListener in mMouseWheelListener[key: screen])
                {
                    giveThrough = giveThrough && mouseWheelListener.MouseWheelValueChanged(mouseAction: EMouseAction.ScrollDown);
                }
            }
            else if (mCurrentMouseState.ScrollWheelValue > mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled upwards -> create event 'ScrollUp'
            {
                foreach (var mouseWheelListener in mMouseWheelListener[key: screen])
                {
                    giveThrough = giveThrough && mouseWheelListener.MouseWheelValueChanged(mouseAction: EMouseAction.ScrollUp);
                }
            }

            return giveThrough;
        }


        private bool CreateMouseClickEvents(EScreen screen)
        {
            if (!mMouseClickListener.ContainsKey(key: screen))
            {
                return true;
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
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mLeftClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.LeftClick, withinBounds: true);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.LeftClick, withinBounds: false);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.LeftClick, withinBounds: doesIntersect);
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // left mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mLeftClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.LeftClick, withinBounds: doesIntersect);
                                    break;
                            }
                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.LeftButton == ButtonState.Pressed)
                    // left mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mLeftClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.LeftClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.LeftClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.LeftClick, withinBounds: doesIntersect);
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
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mRightClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.RightClick, withinBounds: true);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.RightClick, withinBounds: false);
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonClicked(mouseAction: EMouseAction.RightClick, withinBounds: doesIntersect);
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: doesIntersect);
                                    break;
                            }
                        }
                    }
                    else
                    // right mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mRightClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonPressed(mouseAction: EMouseAction.RightClick, withinBounds: doesIntersect);
                                    break;
                            }

                        }
                    }

                    break;
                case ButtonState.Released:
                    if (mPreviousMouseState.RightButton == ButtonState.Pressed)
                    // right mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseClickListener[key: screen])
                        {
                            var doesIntersect = RectAtPosition(x: mCurrentMouseState.X, y: mCurrentMouseState.Y)
                                .Intersects(value: mouseListener.Bounds);

                            switch (mRightClickType[key: mouseListener])
                            {
                                case EClickType.InBoundsOnly:
                                    if (doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.RightClick, withinBounds: true);
                                    }
                                    break;

                                case EClickType.OutOfBoundsOnly:
                                    if (!doesIntersect)
                                    {
                                        giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.RightClick, withinBounds: false);
                                    }
                                    break;

                                case EClickType.Both:
                                    giveThrough = giveThrough && mouseListener.MouseButtonReleased(mouseAction: EMouseAction.RightClick, withinBounds: doesIntersect);
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
                    mousePositionListener.MousePositionChanged(newX: mCurrentMouseState.X, newY: mCurrentMouseState.Y);
                }
            }
        }

        private void CreateKeyEvents()
        {
            var createPressed = false;

            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
                // go through all pressed keys and create events accordingly
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(value: pressedKey))
                    // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyTyped(keyEvent: new KeyEvent(currentKeys: mCurrentKeyboardState.GetPressedKeys()));
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
                    keyListener.KeyPressed(keyEvent: new KeyEvent(currentKeys: mCurrentKeyboardState.GetPressedKeys()));
                }
            }

            var releasedKeys = new List<Keys>();

            foreach (var previouslyPressedKey in mPreviousKeyboardState.GetPressedKeys())
                // go through all previously pressed keys and create events if they are no longer pressed
            {
                if (mCurrentKeyboardState.GetPressedKeys().Contains(value: previouslyPressedKey))
                    // the key was already pressed -> no event
                {
                    continue;
                }
                releasedKeys.Add(item: previouslyPressedKey);
            }

            if (releasedKeys.Count > 0)
            {

                // the key was released -> create event 'release'
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyReleased(keyEvent: new KeyEvent(currentKeys: releasedKeys.ToArray()));
                }
            }
        }

        public void Update(GameTime gametime)
        {
            // update 'current' values
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();

            var giveWheelThrough = true;

            var giveClickThrough = true;

            foreach (var screen in mScreensToCheck)
            {
                if (giveWheelThrough) {
                    giveWheelThrough = giveWheelThrough && CreateMouseWheelEvents(screen: screen);

                }

                if (giveClickThrough) {
                    giveClickThrough = giveClickThrough && CreateMouseClickEvents(screen: screen);

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
            mScreensToCheck.Add(item: screen);
        }

        private static Rectangle RectAtPosition(float x, float y)
        {
            return new Rectangle(x: (int) x, y: (int) y, width: 1, height: 1);
        }
    }
}
