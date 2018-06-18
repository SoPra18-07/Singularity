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
        private List<IWindowItem> mItemList = new List<IWindowItem>();

        // basic window rectangle
        private Rectangle mWindowRectangle;
        private Rectangle mBorderRectangle;
        private Rectangle mTitleBarRectangle;

        // minimization rectangle
        private Rectangle mMinimizationRectangle;
        private Rectangle mMinimizationLine;
        private Rectangle mMinimizedWindowRectangle;
        private Rectangle mMinimizedBorderRectangle;
        private Rectangle mScrollBarRectangle;
        private Rectangle mScrollBarBorderRectangle;
        private Rectangle mScissorRectangle;

        // parameters
        private string mWindowName;
        private Vector2 mPosition;
        private Vector2 mSize;
        private Color mColorBorder;
        private Color mColorFill;
        private float mOpacity;
        private float mBorderPadding;
        private float mObjectPadding;
        private bool mMinimizable;

        private bool mBackgroundGiven;
        private bool mMinimized; // = false as default value

        private bool mScrollable;

        // TODO: TESTING
        private bool mClicked;
        private Vector2 mItemPosVector2Top;
        private Vector2 mItemPosVector2Bottom;
        private float mCombinedItemsSize;

        private float mMouseX;
        private float mMouseY;

        private float mMouseStartX;
        private float mMouseStartY;

        private TimeSpan mGameTime;

        private RasterizerState mRasterizerState;

        private SpriteFont mTestFont;
        private int mNameSizeY;
        private int mMinimizationSize;

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
            InputManager inputManager)
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
            mCurrentScreenWidth = 1080;
            mCurrentScreenHeight = 720;

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

            #region defineRectangles commented
            // DEFINE ALL RECTANGLES
            /*
            // drawRectangle and FillRectangle draw differently -> match position and size values
            mWindowRectangle = new Rectangle((int)(this.mPosition.X + 1), (int)(this.mPosition.Y + 2), (int)(mSize.X - 2), (int)(mSize.Y - 2));
            mBorderRectangle = new Rectangle((int)this.mPosition.X, (int)this.mPosition.Y, (int)(mSize.X), ((int)mSize.Y));
            // ScissorRectangle will cut everything drawn outside of this rectangle
            mScissorRectangle = new Rectangle((int)(this.mPosition.X - 1), (int)(mPosition.Y), (int)(mSize.X + 2), (int)(mSize.Y + 2));

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y), (int)mMinimizationSize, (int)mMinimizationSize);
            mMinimizationLine = new Rectangle((int)(mPosition.X + mSize.X - 3 * mMinimizationSize / 4), (int)(mPosition.Y + (int)(mMinimizationSize / 2)), (int)(mMinimizationSize / 2), 1);

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle((int)(mPosition.X + 1), (int)(mPosition.Y + 2), ((int)mSize.X - 2), mNameSizeY + mMinimizationSize);
            mMinimizedBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), mNameSizeY + mMinimizationSize);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y + mNameSizeY + mMinimizationSize), (int)mMinimizationSize, (int)(mSize.Y - mNameSizeY - 2 * mMinimizationSize));
            mScrollBarRectangle = new Rectangle();

            // title bar
            mTitleBarRectangle = new Rectangle((int)this.mPosition.X + mMinimizationSize / 2, (int)this.mPosition.Y + mNameSizeY + mMinimizationSize, (int)mSize.X - 2 * mMinimizationSize, 1);
            */
            #endregion

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);

            // TODO: MAKE CONNECTED TO mBorderRectangle, so that if one gets changed both get changed
            Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));

            // manage input -> 'enables' minimization
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


        #region Update + Draw


        public void Draw(SpriteBatch spriteBatch)
        {
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
                // TODO: DELETE TEST STUFF - testing the scissor rectangle
                if (textaaaSize.Y > mBorderRectangle.Height)
                {
                    spriteBatch.Draw(windowTexture2D, mScrollBarRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    //spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);
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

            if (mMinimizable)
                // if the window should be minimizable -> draw tiny rectangle for minimization to click on
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
            mScrollable = false;
            if (mCombinedItemsSize > mSize.Y)
            {
                mScrollable = true;
            }


            // TODO: GET FROM GAME1.CS ?
            mGameTime = gametime.TotalGameTime;

            // DEFINE ALL RECTANGLES

            // drawRectangle and FillRectangle draw differently -> match position and size values
            mWindowRectangle = new Rectangle((int)(mPosition.X + 1), (int)(mPosition.Y + 2), (int)(mSize.X - 2), (int)(mSize.Y - 2));
            mBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));
            // ScissorRectangle will cut everything drawn outside of this rectangle
            mScissorRectangle = new Rectangle((int)(mPosition.X - 1), (int)(mPosition.Y + mNameSizeY + 2 * mMinimizationSize), (int)(mSize.X + 2), (int)(mSize.Y + 2));

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y), (int)mMinimizationSize, (int)mMinimizationSize);
            mMinimizationLine = new Rectangle((int)(mPosition.X + mSize.X - 3 * mMinimizationSize / 4), (int)(mPosition.Y + (int)(mMinimizationSize / 2)), (int)(mMinimizationSize / 2), 1);

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle((int)(mPosition.X + 1), (int)(mPosition.Y + 2), ((int)mSize.X - 2), mNameSizeY + mMinimizationSize);
            mMinimizedBorderRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), mNameSizeY + mMinimizationSize);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle((int)(mPosition.X + mSize.X - mMinimizationSize), (int)(mPosition.Y + mNameSizeY + mMinimizationSize), (int)mMinimizationSize, (int)(mSize.Y - mNameSizeY - 2 * mMinimizationSize));
            mScrollBarRectangle = new Rectangle();

            // title bar
            mTitleBarRectangle = new Rectangle((int)mPosition.X + mMinimizationSize / 2, (int)mPosition.Y + mNameSizeY + mMinimizationSize, (int)mSize.X - 2 * mMinimizationSize, 1);
        }


        #endregion


        #region InputManagement


        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            if (!(mMouseX > mPosition.X) || !(mMouseX < mPosition.X + mSize.X) || !(mMouseY > mPosition.Y) ||
                !(mMouseY < mPosition.Y + mSize.Y))
            {
                return;
            }

            if (!mScrollable)
            {
                return;
            }

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
                    {
                        mItemPosVector2Top.Y += -10;
                    }
                    break;
            }
        }

        public Rectangle Bounds { get; set; }


        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.LeftClick && withinBounds)
            {
                mMouseStartX = mMouseX;
                mMouseStartY = mMouseY;

                // mouse on minimization-rectangle
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
                    }
                    else if (mMinimized)
                    // LeftClick on Minimize-Button, window IS minimized
                    // -> use regular rectangles + actual itemList for draw
                    {
                        mMinimized = false;
                    }
                }

                // set 'clicked' for moving to prevent other windows from being dragged along this moving window when moving above their 'bounds'
                if ((mMouseX > mPosition.X &&
                    mMouseX < mPosition.X + mPosition.X + mSize.X &&
                    mMouseY > mPosition.Y &&
                    mMouseY < mPosition.Y + mNameSizeY + mMinimizationSize))
                {
                    if (!(mMouseX >= mMinimizationRectangle.X &&
                          mMouseX < mMinimizationRectangle.X + mMinimizationSize &&
                          mMouseY >= mMinimizationRectangle.Y &&
                          mMouseY < mMinimizationRectangle.Y + mMinimizationSize))
                        // mouse not on minimization rectangle
                    mClicked = true;
                }
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            // move the window by pressing the left mouse button above the title line TODO: add to 'controls'

            if (mMouseX > mPosition.X &&
                mMouseX < mPosition.X + mSize.X &&
                mMouseY > mPosition.Y &&
                mMouseY < mPosition.Y + mNameSizeY + mMinimizationSize &&
                mClicked)
            // mouse in bounds of title rectangle
            {
                if (mouseAction == EMouseAction.LeftClick)
                {
                    if (!(mPosition.X < 0 && mPosition.X + mSize.X > mCurrentScreenWidth && mPosition.Y < 0 &&
                          mPosition.Y + mSize.Y > mCurrentScreenHeight))
                    {
                        mPosition.X += mMouseX - mMouseStartX;
                        mPosition.Y += mMouseY - mMouseStartY;

                        Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(mSize.X), ((int)mSize.Y));
                        mItemPosVector2Top = new Vector2(mPosition.X + mBorderPadding, mPosition.Y + mNameSizeY + 2 * mMinimizationSize);
                    }
                    mMouseStartX = mMouseX;
                    mMouseStartY = mMouseY;
                }
            }
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            mClicked = false;
        }

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }
        #endregion

        private Rectangle CalculateScrollbarRectangle()
        {
            return new Rectangle();
        }
    }
}
