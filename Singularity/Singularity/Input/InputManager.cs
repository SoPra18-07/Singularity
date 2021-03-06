using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Input
{
    /// <inheritdoc />
    /// <summary>
    /// Manages the UserInput
    /// </summary>
    public sealed class InputManager : IUpdate
    {

        private readonly Dictionary<EScreen, List<IKeyListener>> mKeyListener;

        private readonly List<IMousePositionListener> mMousePositionListener;

        private readonly Dictionary<EScreen, List<IMouseClickListener>> mMouseClickListener;

        private readonly Dictionary<EScreen, List<IMouseWheelListener>> mMouseWheelListener;

        private readonly Dictionary<IMouseClickListener, EClickType> mLeftClickType;

        private readonly Dictionary<IMouseClickListener, EClickType> mRightClickType;

        private readonly List<EScreen> mScreensToCheck;

        private MouseState mCurrentMouseState;

        private MouseState mPreviousMouseState;

        private readonly List<IMouseClickListener> mClickListenerToRemove;

        private readonly List<IMouseWheelListener> mWheelListenerToRemove;

        private readonly List<IKeyListener> mKeyListenerToRemove;

        private readonly List<Triple<IMouseClickListener, EClickType, EClickType>> mClickListenerToAdd;

        private readonly List<IMouseWheelListener> mWheelListenerToAdd;

        private readonly List<IKeyListener> mKeyListenerToAdd;


        private KeyboardState mCurrentKeyboardState;

        private KeyboardState mPreviousKeyboardState;


        private bool mCameraMoved;

        private Matrix mCurrentTransform;

        public InputManager()
        {
            mScreensToCheck = new List<EScreen>();

            mClickListenerToRemove = new List<IMouseClickListener>();
            mWheelListenerToRemove = new List<IMouseWheelListener>();
            mKeyListenerToRemove = new List<IKeyListener>();
            mClickListenerToAdd = new List<Triple<IMouseClickListener, EClickType, EClickType>>();
            mWheelListenerToAdd = new List<IMouseWheelListener>();
            mKeyListenerToAdd = new List<IKeyListener>();
            mKeyListener = new Dictionary<EScreen, List<IKeyListener>>();
            mMousePositionListener = new List<IMousePositionListener>();
            mMouseClickListener = new Dictionary<EScreen, List<IMouseClickListener>>();
            mMouseWheelListener = new Dictionary<EScreen, List<IMouseWheelListener>>();

            mLeftClickType = new Dictionary<IMouseClickListener, EClickType>();
            mRightClickType = new Dictionary<IMouseClickListener, EClickType>();

            // get states for later
            mPreviousMouseState = Mouse.GetState();
            mPreviousKeyboardState = Keyboard.GetState();
        }

        public void FlagForRemoval(IKeyListener keyListener)
        {
            mKeyListenerToRemove.Add(keyListener);
        }

        public void FlagForRemoval(IMouseClickListener mouseClickListener)
        {
            mClickListenerToRemove.Add(mouseClickListener);
        }

        public void FlagForRemoval(IMouseWheelListener mouseWheelListener)
        {
            mWheelListenerToRemove.Add(mouseWheelListener);
        }

        public void FlagForAddition(IKeyListener keylistener)
        {
            mKeyListenerToAdd.Add(keylistener);
        }

        public void FlagForAddition(IMouseClickListener clickListener, EClickType left, EClickType right)
        {
            mClickListenerToAdd.Add(new Triple<IMouseClickListener, EClickType, EClickType>(clickListener, left, right));
        }

        public void FlagForAddition(IMouseWheelListener mouseWheelListener)
        {
            mWheelListenerToAdd.Add(mouseWheelListener);
        }

        private void AddKeyListener(IKeyListener iKeyListener)
        {
            if (!mKeyListener.ContainsKey(iKeyListener.Screen))
            {
                mKeyListener[iKeyListener.Screen] = new List<IKeyListener>();
            }

            mKeyListener[iKeyListener.Screen].Add(iKeyListener);
        }

        private bool RemoveKeyListener(IKeyListener iKeyListener)
        {
            if (!mKeyListener.ContainsKey(iKeyListener.Screen))
            {
                return false;
            }

            if (!mKeyListener[iKeyListener.Screen].Contains(iKeyListener))
            {
                return false;
            }

            mKeyListener[iKeyListener.Screen].Remove(iKeyListener);
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
        private void AddMouseClickListener(IMouseClickListener iMouseClickListener, EClickType leftClickType, EClickType rightClickType)
        {
            if (!mMouseClickListener.ContainsKey(iMouseClickListener.Screen))
            {
                mMouseClickListener[iMouseClickListener.Screen] = new List<IMouseClickListener>();
            }

            mMouseClickListener[iMouseClickListener.Screen]
                .Add(iMouseClickListener);

            if (!mLeftClickType.ContainsKey(iMouseClickListener))
            {
                mLeftClickType.Add(iMouseClickListener, leftClickType);
            }

            if (!mRightClickType.ContainsKey(iMouseClickListener))
            {
                mRightClickType.Add(iMouseClickListener, rightClickType);
            }
        }

        private bool RemoveMouseClickListener(IMouseClickListener iMouseClickListener)
        {
            if (!mMouseClickListener.ContainsKey(iMouseClickListener.Screen))
            {
                return false;
            }

            if (!mMouseClickListener[iMouseClickListener.Screen].Contains(iMouseClickListener))
            {
                return false;
            }

            mMouseClickListener[iMouseClickListener.Screen]
                .Remove(iMouseClickListener);


            mLeftClickType.Remove(iMouseClickListener);
            mRightClickType.Remove(iMouseClickListener);
            return true;
        }

        private void AddMouseWheelListener(IMouseWheelListener iMouseWheelListener)
        {
            if (!mMouseWheelListener.ContainsKey(iMouseWheelListener.Screen))
            {
                mMouseWheelListener[iMouseWheelListener.Screen] = new List<IMouseWheelListener>();
            }

            mMouseWheelListener[iMouseWheelListener.Screen].Add(iMouseWheelListener);
        }

        private bool RemoveMouseWheelListener(IMouseWheelListener iMouseWheelListener)
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
            if (mCurrentMouseState.X != mPreviousMouseState.X || mCurrentMouseState.Y != mPreviousMouseState.Y || mCameraMoved)
            {
                var worldMouse = Vector2.Transform(new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y), Matrix.Invert(mCurrentTransform));

                foreach (var mousePositionListener in mMousePositionListener)
                {
                    mousePositionListener.MousePositionChanged(mCurrentMouseState.X, mCurrentMouseState.Y, worldMouse.X, worldMouse.Y);
                }
            }
        }

        private bool CreateKeyEvents(EScreen screen)
        {
            if (!mKeyListener.ContainsKey(screen))
            {
                return true;
            }

            var createPressed = false;

            var giveThrough = true;

            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
                // go through all pressed keys and create events accordingly
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                    // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in mKeyListener[screen])
                    {
                        giveThrough = giveThrough && keyListener.KeyTyped(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
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
                foreach (var keyListener in mKeyListener[screen])
                {
                    giveThrough = giveThrough && keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
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
                foreach (var keyListener in mKeyListener[screen])
                {
                    giveThrough = giveThrough && keyListener.KeyReleased(new KeyEvent(releasedKeys.ToArray()));
                }
            }

            return giveThrough;
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void Update(GameTime gametime)
        {
            // update 'current' values
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();

            var giveWheelThrough = true;

            var giveClickThrough = true;

            var giveKeyThrough = true;

            foreach (var screen in mScreensToCheck)
            {
                if (giveWheelThrough) {
                    giveWheelThrough = giveWheelThrough && CreateMouseWheelEvents(screen);

                }

                if (giveClickThrough) {
                    giveClickThrough = giveClickThrough && CreateMouseClickEvents(screen);

                }

                if (giveKeyThrough)
                {
                    giveKeyThrough = giveKeyThrough && CreateKeyEvents(screen);
                }

            }

            CreateMousePositionEvents();

            foreach(var clickListener in mClickListenerToRemove)
            {
                RemoveMouseClickListener(clickListener);
            }

            foreach(var wheelListener in mWheelListenerToRemove)
            {
                RemoveMouseWheelListener(wheelListener);
            }

            foreach(var keyListener in mKeyListenerToRemove)
            {
                RemoveKeyListener(keyListener);
            }

            mClickListenerToRemove.Clear();
            mWheelListenerToRemove.Clear();
            mKeyListenerToRemove.Clear();

            foreach(var clickListener in mClickListenerToAdd)
            {
                AddMouseClickListener(clickListener.GetFirst(), clickListener.GetSecond(), clickListener.GetThird());
            }

            foreach(var wheelListener in mWheelListenerToAdd)
            {
                AddMouseWheelListener(wheelListener);
            }

            foreach(var keyListener in mKeyListenerToAdd)
            {
                AddKeyListener(keyListener);
            }

            mClickListenerToAdd.Clear();
            mWheelListenerToAdd.Clear();
            mKeyListenerToAdd.Clear();


            // update 'previous'-values
            mPreviousMouseState = mCurrentMouseState;
            mPreviousKeyboardState = mCurrentKeyboardState;

            mCameraMoved = false;

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

        public void CameraMoved(Matrix transform)
        {
            mCameraMoved = true;
            mCurrentTransform = transform;
        }


        public void RemoveEverythingFromInputManager()
        {
            mMouseClickListener.Clear();
            mClickListenerToRemove.Clear();
            mMouseWheelListener.Clear();
            mWheelListenerToRemove.Clear();
            mKeyListener.Clear();
            mKeyListenerToRemove.Clear();

            mMousePositionListener.Clear();

            mLeftClickType.Clear();
            mRightClickType.Clear();

            mClickListenerToAdd.Clear();
            mKeyListenerToAdd.Clear();
            mWheelListenerToAdd.Clear();
        }
    }
}
