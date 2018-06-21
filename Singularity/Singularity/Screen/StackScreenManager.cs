﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private readonly Stack<IScreen> _mScreenStack;

        private readonly LinkedList<IScreen> mScreensToAdd;

        private int mScreenRemovalCounter;

        private readonly ContentManager mContentManager;

        public StackScreenManager(ContentManager contentManager)
        {
            mContentManager = contentManager;

            //these are used to savely add new screens without changing the stack size while iterating.
            mScreensToAdd = new LinkedList<IScreen>();
            mScreenRemovalCounter = 0;
            _mScreenStack = new Stack<IScreen>();

        }

        public void AddScreen(IScreen screen)
        {
            _mScreenStack.AddLast(screen);
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
            var screens = new LinkedList<IScreen>(_mScreenStack);

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
            var screens = new LinkedList<IScreen>(_mScreenStack);

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
    }
}
