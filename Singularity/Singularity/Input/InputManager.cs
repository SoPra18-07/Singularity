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
            // update 'current' values
            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();


            // # BEGIN - mouse wheel events


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


            // # END - mouse wheel events


            // # BEGIN - mouse button events


            // process left mouse button changes
            switch (mCurrentMouseState.LeftButton)
                // switch-case for left mouse button-state
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
                    if (mPreviousMouseState.LeftButton == ButtonState.Pressed)
                    // left mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MouseReleased(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
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
                    if (mPreviousMouseState.RightButton == ButtonState.Pressed)
                    // right mouse button was released -> create event 'released'
                    {
                        foreach (var mouseListener in mMouseListener)
                        {
                            mouseListener.MouseReleased(new MouseEvent(EMouseAction.RightClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }
                    }

                    break;
            }


            // # END - mouse button events


            // # BEGIN - process key events


            foreach (var pressedKey in mCurrentKeyboardState.GetPressedKeys())
            // go through all pressed keys and create events accordingly
            {
                if (!mPreviousKeyboardState.GetPressedKeys().Contains(pressedKey))
                // new key was pressed -> create events 'typed' + 'pressed'
                {
                    foreach (var keyListener in mKeyListener)
                    {
                        keyListener.KeyPressed(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
                        keyListener.KeyTyped(new KeyEvent(mCurrentKeyboardState.GetPressedKeys()));
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

            foreach (var previouslyPressedKey in mPreviousKeyboardState.GetPressedKeys())
            // go through all previously pressed keys and create events if they are no longer pressed
            {
                if (mCurrentKeyboardState.GetPressedKeys().Contains(previouslyPressedKey))
                // the key was already pressed -> no event
                {
                    continue;
                }

                // the key was released -> create event 'release'
                foreach (var keyListener in mKeyListener)
                {
                    keyListener.KeyReleased(new KeyEvent(mPreviousKeyboardState.GetPressedKeys()));
                }
            }


            // # END - process key events


            // update 'previous'-values
            mPreviousMouseState = mCurrentMouseState;
            mPreviousKeyboardState = mCurrentKeyboardState;

        }
    }
}
