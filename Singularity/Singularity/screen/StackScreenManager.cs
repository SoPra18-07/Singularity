using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.screen
{
    class StackScreenManager : IScreenManager
    {
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
