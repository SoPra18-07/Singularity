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

        private string mWindowName;
        private List<IWindowItem> mItemList;

        private Rectangle mWindowRectangle;
        private Rectangle mBorderRectangle;
        private Rectangle mMinimizationRectangle;
        private Rectangle mMinimizationLine;
        private Rectangle mMinimizedWindowRectangle;
        private Rectangle mMinimizedBorderRectangle;
        private Rectangle mScrollBarRectangle;
        private Rectangle mScrollBarBorderRectangle;
        private Rectangle mTitleBarRectangle;

        private Rectangle mDrawWindowRectangle;
        private Rectangle mDrawBorderRectangle;

        private Vector2 mPosition;
        private Vector2 mSize;
        private Color mColorBorder;
        private Color mColorFill;
        private float mOpacity;
        private float mBorderPadding;
        private float mObjectPadding;
        private bool mBackgroundGiven;
        private bool mMinimizable;
        private bool mMinimized;
        private bool mScrollableInWindow;

        private float mMouseX;
        private float mMouseY;

        private TimeSpan mGameTime;

        private RasterizerState mRasterizerState;

        private SpriteFont mTestFont;
        private int mNameSize;
        private int mMinimizationSize;


        // this is the constructor which gets called if the window should have a background color
        public WindowObject(string windowName,
            Vector2 position,
            Vector2 size,
            Color colorBorder,
            Color colorFill,
            float opacity,
            float borderPadding,
            float objectPadding,
            bool minimizable,
            SpriteFont testFontForUserI,
            InputManager inputManager)
        {
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = colorBorder;
            mColorFill = colorFill;
            mOpacity = opacity;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;

            mItemList = new List<IWindowItem>();

            mMinimized = false;
            mBackgroundGiven = true;

            var currentScreenWidth = 1080;
            var currentScreenHeight = 720;

            mMinimizationSize = (int)mSize.X / 12; //20;
            mNameSize = currentScreenHeight / 26; // 26

            // TESTING
            mTestFont = testFontForUserI;
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // drawRectangle and FillRectangle draw differently -> match position and size values
            mWindowRectangle = new Rectangle((int)(position.X + 1), (int)(position.Y + 2), (int)(size.X - 2), (int)(size.Y - 2));
            mBorderRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(size.X), ((int)size.Y));

            // set the rectangles which should be drawn -> these can change later on when minimized
            mDrawWindowRectangle = mWindowRectangle;
            mDrawBorderRectangle = mBorderRectangle;

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle((int) (position.X + size.X - mMinimizationSize), (int) (position.Y), (int)mMinimizationSize, (int)mMinimizationSize);
            mMinimizationLine = new Rectangle((int)(position.X + size.X - 3 * mMinimizationSize / 4), (int)(position.Y + (int)(mMinimizationSize / 2)), (int)(mMinimizationSize / 2), 1);

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle((int)(position.X + 1), (int)(position.Y + 2), ((int)size.X - 2), mNameSize + mMinimizationSize);
            mMinimizedBorderRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(mSize.X), mNameSize + mMinimizationSize);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle((int)(position.X + size.X - mMinimizationSize), (int)(position.Y + mNameSize + mMinimizationSize), (int)mMinimizationSize, (int)(mSize.Y - mNameSize - 2 * mMinimizationSize));
            //mScrollBarRectangle

            // title bar
            mTitleBarRectangle = new Rectangle((int)mPosition.X + mMinimizationSize / 2, (int)mPosition.Y + mNameSize + mMinimizationSize, (int)mSize.X - 2 * mMinimizationSize, 1);

            // manage input -> 'enables' minimization
            if (!mMinimizable)
            {
                return;
            }

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);
            Bounds = mMinimizationRectangle;
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
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = colorBorder;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;

            mItemList = new List<IWindowItem>();

            mMinimized = false;
            mBackgroundGiven = false;

            mBorderRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(size.X), ((int)size.Y));

            mDrawBorderRectangle = mBorderRectangle;
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

            // TESTING
                
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

            // set up current scissor rectangle
            spriteBatch.GraphicsDevice.ScissorRectangle = mBorderRectangle;

            // TEST DRAW STRING
            //spriteBatch.DrawString(mTestFont, textaaa, new Vector2(mPosition.X, mPosition.Y + 20), new Color(255, 255, 255));
            //spriteBatch.DrawString(mTestFont, DateTime.Now.ToLongTimeString(), new Vector2(mPosition.X, mPosition.Y + mNameSize), new Color(255, 255, 255));

            if (!mMinimized)
            {
                // draw items
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch);
                }

                // scrollable
                if (textaaaSize.Y > mBorderRectangle.Height)
                {
                    //spriteBatch.Draw(windowTexture2D, mScrollBarRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);
                }

                if (mBackgroundGiven)
                    // if a background was given -> draw filled rectangle
                {
                    spriteBatch.Draw(windowTexture2D, mWindowRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                    //spriteBatch.FillRectangle(mWindowRectangle, mColorFill, 0f, mOpacity);
                }

                spriteBatch.DrawRectangle(mBorderRectangle, mColorBorder, 2);
            }
            else
            {

            }

            // restore scissor from backup - needed for other draws
            spriteBatch.GraphicsDevice.ScissorRectangle = saveRect;

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
            foreach (var item in mItemList)
            {
                item.Update(gametime);
            }

            // TODO: GET FROM GAME1.CS ?
            mGameTime = gametime.TotalGameTime;
        }


        #endregion


        #region InputManagemend


        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            if (mMouseX > mPosition.X && 
                mMouseX < mPosition.X + mSize.X && 
                mMouseY > mPosition.Y &&
                mMouseY < mPosition.Y + mSize.Y)
                // in bound of 'top'
            {
                if (mouseAction == EMouseAction.LeftClick)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public Rectangle Bounds { get; }

        public void MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.LeftClick && withinBounds && !mMinimized && mMinimizable)
                // LeftClick on Minimize-Button, window IS NOT minimized and it's minimizable
                // -> use minimized rectangles + empty itemList for draw
            {
                mDrawWindowRectangle = mMinimizedWindowRectangle;
                mDrawBorderRectangle = mMinimizedBorderRectangle;

                mMinimized = true;
            }
            else if (mouseAction == EMouseAction.LeftClick && withinBounds && mMinimized)
                // LeftClick on Minimize-Button, window IS minimized
                // -> use regular rectangles + actual itemList for draw
            {
                mDrawWindowRectangle = mWindowRectangle;
                mDrawBorderRectangle = mBorderRectangle;

                mMinimized = false;
            }
        }

        public void MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            // no effect
        }

        public void MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            // no effect
        }

        #endregion

        public void MousePositionChanged(float newX, float newY)
        {
            mMouseX = newX;
            mMouseY = newY;
        }
    }
}
