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
    internal sealed class InputManager : IUpdate
    {
        private readonly List<IKeyListener> mKeyListener;
        private readonly List<IMouseListener> mMouseListener;
        private readonly List<IMouseWheelListener> mMouseWheelListener;

        private MouseState mCurrentMouseState;
        private MouseState mPreviousMouseState;

        private KeyboardState mCurrentKeyboardState;
        private KeyboardState mPreviousKeyboardState;

        public InputManager()
        {
            mKeyListener = new List<IKeyListener>();
            mMouseListener = new List<IMouseListener>();
            mMouseWheelListener = new List<IMouseWheelListener>();

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

        public void AddMouseListener(IMouseListener iMouseListener)
        {
            mMouseListener.Add(iMouseListener);
        }

        public bool RemoveMouseListener(IMouseListener iMouseListener)
        {
            if (!mMouseListener.Contains(iMouseListener))
            {
                return false;
            }

            mMouseListener.Remove(iMouseListener);
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
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();

            // check for changes in the mouse state
            if (mCurrentMouseState != mPreviousMouseState)
            {
                // process mouse wheel changes
                if (mCurrentMouseState.ScrollWheelValue < mPreviousMouseState.ScrollWheelValue)
                {
                    foreach (var mouseWheelListener in mMouseWheelListener)
                    {
                        mouseWheelListener.ScrollDown(new MouseEvent(EMouseAction.ScrollDown, new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                    }
                }
                else if (mCurrentMouseState.ScrollWheelValue > mPreviousMouseState.ScrollWheelValue)
                {
                    foreach (var mouseWheelListener in mMouseWheelListener)
                    {
                        mouseWheelListener.ScrollUp(new MouseEvent(EMouseAction.ScrollUp, new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                    }
                }

                // process left mouse button changes
                if (mCurrentMouseState.LeftButton != mPreviousMouseState.LeftButton)
                {
                    switch (mCurrentMouseState.LeftButton)
                    {
                        case ButtonState.Pressed:
                            foreach (var mouseListener in mMouseListener)
                            {
                                mouseListener.MousePressed(new MouseEvent(EMouseAction.LeftClick,
                                    new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            }

                            break;
                        case ButtonState.Released:
                            foreach (var mouseListener in mMouseListener)
                            {
                                mouseListener.MouseReleased(new MouseEvent(EMouseAction.LeftClick,
                                    new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            }

                            break;
                    }
                }

                // process right mouse button changes
                if (mCurrentMouseState.RightButton != mPreviousMouseState.RightButton)
                {
                    switch (mCurrentMouseState.RightButton)
                    {
                        case ButtonState.Pressed:
                            foreach (var mouseListener in mMouseListener)
                            {
                                mouseListener.MousePressed(new MouseEvent(EMouseAction.RightClick,
                                    new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            }

                            break;
                        case ButtonState.Released:
                            foreach (var mouseListener in mMouseListener)
                            {
                                mouseListener.MouseReleased(new MouseEvent(EMouseAction.RightClick,
                                    new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            }

                            break;
                    }
                }
                mPreviousMouseState = mCurrentMouseState;
            }

            // check for changes in the keyboard state
            if (mCurrentKeyboardState == mPreviousKeyboardState)
            {
                return;
            }

            // check all pressed keys
            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                    // new key has been pressed -> key is in 'typed-state'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyTyped(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                    }
                }
                else // key is still in 'pressed-state'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                    }
                }
            }

            // check if keys have been released
            foreach (var previouslyPressedKey in mPreviousKeyboardState.GetPressedKeys())
            {
                if (mCurrentKeyboardState.GetPressedKeys().Contains(previouslyPressedKey))
                    // the key is still pressed
                {
                    continue;
                }

                // the key has been released since the last update
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyReleased(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                }
            }
            mPreviousKeyboardState = mCurrentKeyboardState;
        }
    }
}
