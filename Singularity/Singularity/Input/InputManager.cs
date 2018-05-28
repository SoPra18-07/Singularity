using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;

namespace Singularity.Input
{
    /// <inheritdoc />
    /// <summary>
    /// Manages the UserInput
    /// </summary>
    internal class InputManager : IUpdate
    {
        private readonly List<IKeyListener> mKeyListener;
        private readonly List<IMouseListener> mMouseListeners;

        private MouseState mCurrentMouseState;
        private MouseState mPreviousMouseState;

        public InputManager()
        {
            mKeyListener = new List<IKeyListener>();
            mMouseListeners = new List<IMouseListener>();

            mPreviousMouseState = Mouse.GetState();
        }

        private void AddKeyListener(IKeyListener iKeyListener)
        {
            mKeyListener.Add(iKeyListener);
        }

        private bool RemoveKeyListener(IKeyListener iKeyListener)
        {
            if (!mKeyListener.Contains(iKeyListener))
            {
                return false;
            }

            mKeyListener.Remove(iKeyListener);
            return true;
        }

        private void AddMouseListener(IMouseListener iMouseListener)
        {
            mMouseListeners.Add(iMouseListener);
        }

        private bool RemoveMouseListener(IMouseListener iMouseListener)
        {
            if (!mMouseListeners.Contains(iMouseListener))
            {
                return false;
            }

            mMouseListeners.Remove(iMouseListener);
            return true;
        }

        public void Update(GameTime gametime)
        {
            mCurrentMouseState = Mouse.GetState();

            if (mCurrentMouseState != mPreviousMouseState)
            {
                switch (mCurrentMouseState.LeftButton)
                {
                    case ButtonState.Pressed:
                        foreach (var mouseListener in mMouseListeners)
                        {
                            mouseListener.MousePressed(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }

                        break;
                    case ButtonState.Released:
                        foreach (var mouseListener in mMouseListeners)
                        {
                            mouseListener.MouseReleased(new MouseEvent(EMouseAction.LeftClick,
                                new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                        }

                        break;
                }

                switch (mCurrentMouseState.RightButton)
                 {
                     case ButtonState.Pressed:
                         foreach (var mouseListener in mMouseListeners)
                         {
                             mouseListener.MousePressed(new MouseEvent(EMouseAction.RightClick,
                                 new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                         }

                         break;
                     case ButtonState.Released:
                         foreach (var mouseListener in mMouseListeners)
                         {
                             mouseListener.MouseReleased(new MouseEvent(EMouseAction.RightClick,
                                 new Vector2(mCurrentMouseState.X, mCurrentMouseState.Y)));
                         }

                         break;
                 }
            }

            mPreviousMouseState = mCurrentMouseState;
        }
    }
}
