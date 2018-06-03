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

            // get states for later use
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

            // process mouse wheel changes
            if (mCurrentMouseState.ScrollWheelValue < mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled downwards -> create event 'ScrollDown'
            {
                foreach (var mouseWheelListener in mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(new MouseEvent(EMouseAction.ScrollDown, new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                }
            }
            else if (mCurrentMouseState.ScrollWheelValue > mPreviousMouseState.ScrollWheelValue)
                // mouse wheel has been scrolled upwards -> create event 'ScrollUp'
            {
                foreach (var mouseWheelListener in mMouseWheelListener)
                {
                    mouseWheelListener.MouseWheelValueChanged(new MouseEvent(EMouseAction.ScrollUp, new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                }
            }

            // process left mouse button changes
            switch (mCurrentMouseState.LeftButton)
            {
                case ButtonState.Pressed:
                    // left mouse button is pressed
                    if (mPreviousMouseState.LeftButton != ButtonState.Pressed)
                        // left mouse button just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MousePressed(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            mouseListener.MouseTyped(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }
                    }
                    else
                        // left mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MousePressed(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }
                    }

                    break;
                case ButtonState.Released:
                    // left mouse button was released -> create event 'released'
                    foreach (var mouseListener in mMouseListener)
                    {
                        mouseListener.MouseReleased(new MouseEvent(EMouseAction.LeftClick,
                            new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                    }

                    break;
            }

            // process right mouse button changes
            switch (mCurrentMouseState.RightButton)
            {
                case ButtonState.Pressed:
                    // right mouse button is pressed
                    if (mPreviousMouseState.RightButton != ButtonState.Pressed)
                        // right mouse button just pressed -> create events 'typed' + 'pressed'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MousePressed(new MouseEvent(EMouseAction.RightClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                            mouseListener.MouseTyped(new MouseEvent(EMouseAction.RightClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }
                    }
                    else
                        // right mouse button was already pressed -> create event 'pressed'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MousePressed(new MouseEvent(EMouseAction.RightClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }
                    }

                    break;
                case ButtonState.Released:
                    // right mouse button was released -> create event 'released'
                    foreach (var mouseListener in mMouseListener)
                    {
                        mouseListener.MouseReleased(new MouseEvent(EMouseAction.RightClick,
                            new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                    }

                    break;
            }

            // go through all pressed keys and create events accordingly
            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                    // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyTyped(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                        keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                    }
                }
                else 
                    // key was already pressed -> create event 'pressed'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                    }
                }
            }

            // go through all previously pressed keys and create events if they are no longer pressed
            foreach (var previouslyPressedKey in mPreviousKeyboardState.GetPressedKeys())
            {
                if (mCurrentKeyboardState.GetPressedKeys().Contains(previouslyPressedKey))
                    // the key was already pressed -> no event
                {
                    continue;
                }

                // the key has been released since the last update -> create event 'release'
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyReleased(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                }
            }

            mPreviousMouseState = mCurrentMouseState;
            mPreviousKeyboardState = mCurrentKeyboardState;

        }
    }
}
