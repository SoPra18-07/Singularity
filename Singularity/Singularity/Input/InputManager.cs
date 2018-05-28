using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

        public InputManager()
        {
            mKeyListener = new List<IKeyListener>();
            mMouseListeners = new List<IMouseListener>();

            var mouseEvent = new MouseEvent();
            var keyEvent = new KeyEvent();
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
            throw new NotImplementedException();
        }
    }
}
