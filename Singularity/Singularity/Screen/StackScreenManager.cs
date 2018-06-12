using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Screen
{
    /// <inheritdoc/>
    /// <remarks>
    /// An implementation for the ScreenManager as shown in the lecture. Utilizes a stack
    /// for the screens to maintain a stacked view of all current screens, allowing to cascade
    /// through all of them if needed.
    /// </remarks>
    internal sealed class StackScreenManager : IScreenManager
    {
        /// <summary>
        /// The stack holding all the currently added screens.
        /// </summary>
        private readonly Stack<IScreen> mScreenStack;

        public StackScreenManager()
        {
            mScreenStack = new Stack<IScreen>();

        }

        public void AddScreen(IScreen screen)
        {
            mScreenStack.Push(screen);
        }

        public void RemoveScreen()
        {
            mScreenStack.Pop();
        }

        public void Update(GameTime gameTime)
        {
            // Note that this already "reverses" the stack, since the implementation calls
            // LinkedList.addLast(mScreenStack.pop()) for every element in the stack, thus the
            // formerly "top" item in the stack is now the first item in the linked list. This
            // removes the "reverse-list" effort.
            var screens = new LinkedList<IScreen>(mScreenStack);

            foreach (var currentScreen in screens)
            {
                currentScreen.Update(gameTime);

                if (!currentScreen.UpdateLower())
                {
                    break;

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Same as the comment in Update.
            var screens = new LinkedList<IScreen>(mScreenStack);

            foreach (var currentScreen in screens)
            {
                currentScreen.Draw(spriteBatch);

                if (!currentScreen.DrawLower())
                {
                    break;

                }
            }
        }
    }
}
