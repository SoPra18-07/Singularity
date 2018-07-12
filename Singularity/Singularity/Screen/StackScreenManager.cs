using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;

namespace Singularity.Screen
{
    /// <inheritdoc/>
    /// <remarks>
    /// An implementation for the ScreenManager as shown in the lecture. Utilizes a stack
    /// for the screens to maintain a stacked view of all current screens, allowing to cascade
    /// through all of them if needed.
    /// </remarks>
    public sealed class StackScreenManager : IScreenManager
    {
        /// <summary>
        /// The stack holding all the currently added screens.
        /// </summary>
        private readonly Stack<IScreen> mScreenStack;

        private readonly LinkedList<IScreen> mScreensToAdd;

        private int mScreenRemovalCounter;

        private readonly ContentManager mContentManager;

        private readonly InputManager mInputManager;

        public StackScreenManager(ContentManager contentManager, InputManager inputManager)
        {
            mInputManager = inputManager;
            mContentManager = contentManager;

            //these are used to savely add new screens without changing the stack size while iterating.
            mScreensToAdd = new LinkedList<IScreen>();
            mScreenRemovalCounter = 0;
            mScreenStack = new Stack<IScreen>();

        }

        internal ScreenClasses.MainMenuManagerScreen MainMenuManagerScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public ScreenClasses.GameScreen GameScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal ScreenClasses.SaveGameScreen SaveGameScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public ScreenClasses.DebugScreen DebugScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public ScreenClasses.UserInterfaceScreen UserInterfaceScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal ScreenClasses.GamePauseScreen GamePauseScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        internal ScreenClasses.StatisticsScreen StatisticsScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public EScreen EScreen
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public void AddScreen(IScreen screen)
        {
            mScreensToAdd.AddLast(screen);
        }

        public void RemoveScreen()
        {
            mScreenRemovalCounter++;
        }

        public void Update(GameTime gameTime)
        {

            // Note that this already "reverses" the stack, since the implementation calls
            // LinkedList.addLast(mScreenStack.pop()) for every element in the stack, thus the
            // formerly "top" item in the stack is now the first item in the linked list. This
            // removes the "reverse-list" effort.
            var screens = new LinkedList<IScreen>(mScreenStack);

            /*
             * Ok, so basically our stack is from bottom to top. The list screens
             * contains all the elements from a top to bottom view, so we can easily ask, whether DrawLower/UpdateLower.
             * The problem is that we have to update our logic and drawing code the other way around, so we need
             * to reverse again. This is what this is for. This contains all the screens we actually look at
             * considering drawlower/updatelower and gets later reversed for logic updates in correct order.
             */
            var testScreens = new LinkedList<IScreen>();

            foreach (var currentScreen in screens)
            {

                if (!currentScreen.Loaded)
                {
                    //make sure to load the content as soon as we want to do something with the screen
                    currentScreen.LoadContent(mContentManager);
                    currentScreen.Loaded = true;
                }

                mInputManager.AddScreen(currentScreen.Screen);

                testScreens.AddLast(currentScreen);

                if (!currentScreen.UpdateLower())
                {
                    break;

                }
            }

            var reversed = testScreens.Reverse();

            foreach (var currentScreen in reversed)
            {
                currentScreen.Update(gameTime);
            }

            for (var i = 0; i < mScreenRemovalCounter; i++)
            {
                mScreenStack.Pop();
            }

            mScreenRemovalCounter = 0;


            foreach (var screen in mScreensToAdd)
            {
                mScreenStack.Push(screen);
            }

            mScreensToAdd.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Same as the comment in Update.
            var screens = new LinkedList<IScreen>(mScreenStack);

            var testScreens = new LinkedList<IScreen>();

            foreach (var currentScreen in screens)
            {

                testScreens.AddLast(currentScreen);

                if (!currentScreen.DrawLower())
                {
                    break;

                }
            }

            var reversed = testScreens.Reverse();

            foreach (var currentScreen in reversed)
            {
                if (!currentScreen.Loaded)
                {
                    //draw loading screen
                    break;
                }

                currentScreen.Draw(spriteBatch);
            }
        }

        public IScreen Peek()
        {
            return mScreenStack.Peek();
        }
    }
}
