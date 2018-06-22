using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    internal sealed class WindowObject: IDraw, IUpdate, IMouseWheelListener, IMouseClickListener, IMousePositionListener
    {
        // list of windowItems added to the window
        private List<IWindowItem> mItemList = new List<IWindowItem>();

        // basic window rectangle
        private Rectangle mWindowRectangle;
        private Rectangle mBorderRectangle;
        private Rectangle mTitleBarRectangle;

        // the rectangle to minimize the window
        private Rectangle mMinimizationRectangle;
        private Rectangle mMinimizationLine;

        // the rectangle for the minimized window
        private Rectangle mMinimizedWindowRectangle;
        private Rectangle mMinimizedBorderRectangle;

        // the rectangles needed for scrollable windows
        private Rectangle mScrollBarRectangle;
        private Rectangle mScrollBarBorderRectangle;
        private Rectangle mScissorRectangle;

        // parameters
        private readonly string mWindowName;
        private Vector2 mPosition;
        private Vector2 mSize;
        private readonly Color mColorBorder;
        private readonly Color mColorFill;
        private readonly float mBorderPadding;
        private readonly float mObjectPadding;
        private readonly bool mMinimizable;

        private bool mBackgroundGiven;
        private bool mMinimized; // = false as default value
        private bool mScrollable;

        // activates the window movement after a mouse click on the title bar
        private bool mClickOnTitleBar;
        private bool mClickOnScrollbar;

        // top and bottom positin of the window's combined items - used by scrolling
        private Vector2 mItemPosVector2Top;
        private Vector2 mItemPosVector2Bottom;

        // height of all windowItems with borderpaddings - used to implement scrollable windows if needed
        private float mCombinedItemsSize;

        // previous mouse position during window movement
        private Vector2 mWindowDragPos;

        // current mouse position
        private float mMouseX;
        private float mMouseY;

        // used by scissorrectangle
        private readonly RasterizerState mRasterizerState;

        // TODO
        private SpriteFont mTestFont;
        private int mNameSizeY;
        private int mMinimizationSize;

        // current screen size values - needed to place the windows at the correct positions at the beginning
        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        // this is the constructor which gets called if the window should have a background color
        public WindowObject(string windowName,
            Vector2 position,
            Vector2 size,
            Color colorBorder,
            Color colorFill,
            float borderPadding,
            float objectPadding,
            bool minimizable,
            SpriteFont testFontForUserI,
            InputManager inputManager,
            GraphicsDeviceManager mgraphics)
        {
            // set parameter-variables
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = colorBorder;
            mColorFill = colorFill;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;

            // TODO : REPLACE WITH PARAMETER of type Vector2
            mCurrentScreenWidth = mgraphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = mgraphics.PreferredBackBufferHeight;

            // calculate resizing by screensize
            mMinimizationSize = (int)mSize.X / 12;
            mNameSizeY = mCurrentScreenHeight / 26;

            // TODO : TESTING
            mTestFont = testFontForUserI;

            // TODO : order
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // set start values
            mBackgroundGiven = true;
            mCombinedItemsSize = 0;

            // position where the next item will be drawn
            mItemPosVector2Top = new Vector2(mPosition.X + mBorderPadding, mPosition.Y + mNameSizeY + 2 * mMinimizationSize);

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);

            // TODO: MAKE CONNECTED TO mBorderRectangle, so that if one gets changed both get changed
            // Only input from inside the window is proccessed
            Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));

            // TODO : manage input -> 'enables' minimization
            if (!mMinimizable)
            {
                return;
            }
        }


        // this is the constructor which gets called if the window should NOT have a background color
        public WindowObject(string windowName,
            Vector2 position,
            Vector2 size,
            Color colorBorder,
            float borderPadding,
            float objectPadding,
            bool minimizable,
            InputManager inputManager)
        {
            // set parameter-variables
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = colorBorder;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;

            mItemList = new List<IWindowItem>();

            mBackgroundGiven = false;

            mBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(size.X), ((int)size.Y));
        }

        /// <summary>
        /// Adds the given WindowItem to the Window
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IWindowItem item)
        {
            mItemList.Add(item);
        }

        /// <summary>
        /// Removes the given WindowItem from the Window
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true, if element successfully deleted</returns>
        public bool DeleteItem(IWindowItem item)
        {
            // item is not in list -> can't be removed
            if (!mItemList.Contains(item))
            {
                return false;
            }

            // item in list -> remove successful
            mItemList.Remove(item);
            return true;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // background color for all rectangles
            var windowTexture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            windowTexture2D.SetData(new[] {mColorFill});

            // TODO: DELETE TESTING STUFF - STRING SETTING
                
            var textaaa = "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA\n" +
                          "AAAAAAAAAA";

            // get size of string drawing
            var textaaaSize = mTestFont.MeasureString(textaaa);






            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            // TODO: DELETE TESTING STUFF - STRING DRAWING
            //spriteBatch.DrawString(mTestFont, textaaa, new Vector2(mPosition.X, mPosition.Y + 20), new Color(255, 255, 255));
            //spriteBatch.DrawString(mTestFont, DateTime.Now.ToLongTimeString(), new Vector2(mPosition.X, mPosition.Y + mNameSizeY), new Color(255, 255, 255));

            if (!mMinimized)
            {
                // Add the scrollbar if the window is scrollable
                if (mScrollable)
                {
                    spriteBatch.Draw(windowTexture2D, mScrollBarRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);
                }

                if (mBackgroundGiven)
                    // if a background was given -> draw filled rectangle
                {
                    spriteBatch.Draw(windowTexture2D, mWindowRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    //spriteBatch.FillRectangle(mWindowRectangle, mColorFill, 0f, 0.99f);
                }

                spriteBatch.DrawRectangle(mBorderRectangle, mColorBorder, 2);

                // set up current scissor rectangle
                spriteBatch.GraphicsDevice.ScissorRectangle = mScissorRectangle;

                // draw IWindowItems
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch);
                }

                // restore scissor from backup - AFTER DRAW
                spriteBatch.GraphicsDevice.ScissorRectangle = saveRect;
            }
            else
            {
                if (mBackgroundGiven)
                    // if a background was given -> draw filled rectangle
                {
                    spriteBatch.Draw(windowTexture2D, mMinimizedWindowRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    //spriteBatch.FillRectangle(mWindowRectangle, mColorFill, 0f, mOpacity);
                }

                spriteBatch.DrawRectangle(mMinimizedBorderRectangle, mColorBorder, 2);
            }

            // if the window should be minimizable -> draw tiny rectangle for minimization to click on
            if (mMinimizable)
            {
                spriteBatch.DrawRectangle(mMinimizationRectangle, mColorBorder, 2);
                spriteBatch.DrawRectangle(mMinimizationLine, mColorBorder, 1);
            }

            // draw window title + bar
            spriteBatch.DrawString(mTestFont, mWindowName, new Vector2(mPosition.X + mMinimizationSize / 2, mPosition.Y + mMinimizationSize / 2), new Color(255, 255, 255));
            spriteBatch.DrawRectangle(mTitleBarRectangle, mColorBorder, 1);

            spriteBatch.End();


        }

        public void Update(GameTime gametime)
        {
            var localItemPosVector2 = mItemPosVector2Top;

            mCombinedItemsSize = 0;

            foreach (var item in mItemList)
            {
                item.Update(gametime);

                item.Position = localItemPosVector2;

                mCombinedItemsSize += mObjectPadding + item.Size.Y;

                localItemPosVector2 = new Vector2(localItemPosVector2.X, localItemPosVector2.Y + item.Size.Y + mObjectPadding);
            }
            
            mItemPosVector2Bottom = new Vector2(localItemPosVector2.X, localItemPosVector2.Y);

            // check if the window is overflowed with items
            mScrollable = mCombinedItemsSize > mSize.Y;

            // DEFINE ALL RECTANGLES

            // drawRectangle and FillRectangle draw differently -> match position and size values
            mWindowRectangle = new Rectangle((int)(mPosition.X + 1), (int)(mPosition.Y + 2), (int)(mSize.X - 2), (int)(mSize.Y - 2));
            mBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));

            // ScissorRectangle will cut everything drawn outside of this rectangle
            mScissorRectangle = new Rectangle((int)(mPosition.X - 1), (int)(mPosition.Y + mNameSizeY + 2 * mMinimizationSize), (int)(mSize.X + 2), (int)(mSize.Y + 2 - mNameSizeY - 2 * mMinimizationSize));

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y), (int)mMinimizationSize, (int)mMinimizationSize);
            mMinimizationLine = new Rectangle((int)(mPosition.X + mSize.X - 3 * mMinimizationSize / 4), (int)(mPosition.Y + (int)(mMinimizationSize / 2)), (int)(mMinimizationSize / 2), 1);

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle((int)(mPosition.X + 1), (int)(mPosition.Y + 2), ((int)mSize.X - 2), mNameSizeY + mMinimizationSize);
            mMinimizedBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), mNameSizeY + mMinimizationSize);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y + mNameSizeY + 2 * mMinimizationSize), (int)mMinimizationSize, (int)(mSize.Y - mNameSizeY - 3 * mMinimizationSize));
            mScrollBarRectangle = CalcScrollbarRectangle(mScissorRectangle, mCombinedItemsSize);

            // title bar
            mTitleBarRectangle = new Rectangle((int)mPosition.X + mMinimizationSize / 2, (int)mPosition.Y + mNameSizeY + mMinimizationSize, (int)mSize.X - 2 * mMinimizationSize, 1);
        }


        #region InputManagement

        public Rectangle Bounds { get; set; }

        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            // enabled only if
            //  - the mouse is above the scrollable part of the window
            //  - the window is not minimized
            //  - the window is scrollable (the number of items is too big for one window)
            if (!(mMouseX > mPosition.X) || !(mMouseX < mPosition.X + mSize.X) || !(mMouseY > mPosition.Y) ||
                !(mMouseY < mPosition.Y + mSize.Y) || mMinimized || !mScrollable)
            {
                return;
            }

            // scroll up or down
            switch (mouseAction)
            {
                case EMouseAction.ScrollUp:
                    if (!(mItemPosVector2Top.Y > mPosition.Y + mNameSizeY + 1.5 * mMinimizationSize))
                        // stop from overflowing
                    {
                        mItemPosVector2Top.Y += +10;
                    }
                    break;
                case EMouseAction.ScrollDown:
                    if (!(mItemPosVector2Bottom.Y < mPosition.Y + mSize.Y))
                        // stop from overflowing
                    {
                        mItemPosVector2Top.Y += -10;
                    }
                    break;
            }
        }

        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.LeftClick && withinBounds)
            {

                #region minimization

                if (mMouseX >= mMinimizationRectangle.X &&
                    mMouseX < mMinimizationRectangle.X + mMinimizationSize &&
                    mMouseY >= mMinimizationRectangle.Y &&
                    mMouseY < mMinimizationRectangle.Y + mMinimizationSize)
                {
                    if (!mMinimized && mMinimizable)
                    // LeftClick on Minimize-Button, window IS NOT minimized and it's minimizable
                    // -> use minimized rectangles + empty itemList for draw
                    {
                        mMinimized = true;

                        // disable all items due to minimization
                        foreach (var item in mItemList)
                        {
                            item.Active = false;
                        }
                    }
                    else if (mMinimized)
                    // LeftClick on Minimize-Button, window IS minimized
                    // -> use regular rectangles + actual itemList for draw
                    {
                        mMinimized = false;

                        // disable all items due to minimization
                        foreach (var item in mItemList)
                        {
                            item.Active = true;
                        }
                    }
                }

                #endregion

                #region window movement initiation

                if ((mMouseX > mPosition.X &&
                    mMouseX < mPosition.X + mPosition.X + mSize.X &&
                    mMouseY > mPosition.Y &&
                    mMouseY < mPosition.Y + mNameSizeY + mMinimizationSize) &&
                    !mClickOnTitleBar)
                      // mouse above the title rectangle
                {
                    if (!(mMouseX >= mMinimizationRectangle.X &&
                          mMouseX < mMinimizationRectangle.X + mMinimizationSize &&
                          mMouseY >= mMinimizationRectangle.Y &&
                          mMouseY < mMinimizationRectangle.Y + mMinimizationSize))
                        // mouse not on minimization rectangle
                    {
                        mClickOnTitleBar = true;
                        mWindowDragPos = new Vector2(mMouseX - mPosition.X, mMouseY - mPosition.Y);

                        // new Bounds so that the window can be moved everywhere
                        Bounds = new Rectangle(0,0,mCurrentScreenWidth, mCurrentScreenHeight);
                    }
                }

                #endregion

                #region scrollbar movement initiation

                if ((mMouseX > mScrollBarRectangle.X &&
                     mMouseX < mScrollBarRectangle.X + mScrollBarRectangle.Width &&
                     mMouseY > mScrollBarRectangle.Y &&
                     mMouseY < mScrollBarRectangle.Y + mScrollBarRectangle.Height) &&
                    !mClickOnScrollbar)
                    // mouse on scrollbar & no other scrollbar clicked on
                {
                    mClickOnScrollbar = true;
                }

                #endregion
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {

            #region window movement
            // move the window by pressing the left mouse button above the title line TODO: add to 'controls'
            if (!mClickOnTitleBar)
            {
                return;
            }

            mPosition.X = (mMouseX - mWindowDragPos.X);
            mPosition.Y = (mMouseY - mWindowDragPos.Y);

            #region catch window moving out of screen
            if (mPosition.X < 0)
            {
                mPosition.X = 0;
            }
            else if (mPosition.X + mSize.X > mCurrentScreenWidth)
            {
                mPosition.X = mCurrentScreenWidth - mSize.X;
            }

            if (mPosition.Y < 0)
            {
                mPosition.Y = 0;
            }
            else if (mPosition.Y + mSize.Y > mCurrentScreenHeight)
            {
                mPosition.Y = mCurrentScreenHeight - mSize.Y;
            }
            #endregion

            mItemPosVector2Top = new Vector2(mPosition.X + mBorderPadding, mPosition.Y + mNameSizeY + 2 * mMinimizationSize);

            #endregion


            #region scrollbar movement

            // move the scrollbar - works intuitively
            if (!mClickOnScrollbar)
            {
                return;
            }

            mPosition.X = (mMouseX - mWindowDragPos.X);
            mPosition.Y = (mMouseY - mWindowDragPos.Y);

            #endregion
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            mClickOnTitleBar = false;
            Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));
        }

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }
        #endregion

        /// <summary>
        /// Calculate the size of the scrollbar
        /// </summary>
        /// <param name="scissorRectangle">the scissorrectangle used to cut the winodw</param>
        /// <param name="combinedItemsSize">the size of all items combined with padding</param>
        /// <returns></returns>
        private Rectangle CalcScrollbarRectangle(Rectangle scissorRectangle, float combinedItemsSize)
        {
            // scrollbar to scrollbarRectangleBorder has the same ratio as scissorRectangle to combinedItemSize
            // (here we use the height of the scissorRectangle twice, because it has got the same height as the scrollbarBorderRectangle
            var sizeY = (scissorRectangle.Height / combinedItemsSize) * mScrollBarBorderRectangle.Height; // TODO: MINUS TO CATCH OUT OF BOX...

            // the y position of the scrollbar
            var naivePositionY = scissorRectangle.Y + (scissorRectangle.Y - mItemPosVector2Top.Y);
            var positionY = naivePositionY - ( (naivePositionY - mScrollBarBorderRectangle.Y) * sizeY / mScrollBarBorderRectangle.Height );

            return new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize + 2), (int)positionY, (mMinimizationSize - 4), (int)sizeY);
        }
    }
}
