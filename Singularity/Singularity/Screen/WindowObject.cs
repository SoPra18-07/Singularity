using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    internal sealed class WindowObject: IDraw, IUpdate, IMouseWheelListener, IMouseClickListener
    {

        private string mWindowName;
        private List<IWindowItem> mItemList;
        private List<IWindowItem> mItemListBackup;

        private Rectangle mWindowRectangle;
        private Rectangle mBorderRectangle;
        private Rectangle mMinimizationRectangle;
        private Rectangle mMinimizationLine;
        private Rectangle mMinimizedWindowRectangle;
        private Rectangle mMinimizedBorderRectangle;
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
        private TimeSpan mGameTime;

        private RasterizerState mRasterizerState;

        private SpriteFont mTestFont;


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
            mMinimizationRectangle = new Rectangle((int) (position.X + size.X - 20), (int) (position.Y), 20, 20);
            mMinimizationLine = new Rectangle((int)(position.X + size.X - 15), (int)(position.Y + 9), 10, 1);

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle((int)(position.X + 1), (int)(position.Y + 2), ((int)size.X - 2), 20);
            mMinimizedBorderRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(size.X), 22);

            // manage input -> 'enables' minimization
            if (!mMinimizable)
            {
                return;
            }
            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
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
            if (!mMinimized)
                // window is not minimized -> the windowObject currently uses it's standard list
            {
                mItemList.Add(item);
            }
            else
                // window is minimized -> the windowObject currently uses an empty list -> add new elements to the backupList
            {
                mItemListBackup.Add(item);
            }
        }

        /// <summary>
        /// Removes the given WindowItem from the Window
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true, if element successfully deleted</returns>
        public bool DeleteItem(IWindowItem item)
        {
            if (!mMinimized)
                // the window is maximized -> delete elements from standard item-list
            {
                // item is not in list -> can't be removed
                if (!mItemList.Contains(item))
                {
                    return false;
                }

                // item in list -> remove will be successful
                mItemList.Remove(item);
                return true;
            }


            // the window is minimized -> delete elements from backup-list
            if (!mItemListBackup.Contains(item))
                // item is not in list -> can't be removed
            {
                return false;
            }

            // item in list -> remove will be successful
            mItemListBackup.Remove(item);
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
            System.Diagnostics.Debug.WriteLine(textaaaSize);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            // set up current scissor rectangle
            spriteBatch.GraphicsDevice.ScissorRectangle = mDrawBorderRectangle;

            if (mBackgroundGiven)
                // if a background was given -> draw filled rectangle
            {
                spriteBatch.Draw(windowTexture2D, mDrawWindowRectangle, null, mColorFill, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
                //spriteBatch.FillRectangle(mWindowRectangle, mColorFill, 0f, mOpacity);
            }

            // spriteBatch.DrawRectangle(mDrawBorderRectangle, mColorBorder, 2);
            spriteBatch.DrawRectangle(mDrawBorderRectangle, mColorBorder, 2);


            if (textaaaSize.Y > mDrawBorderRectangle.Height)
            {
                spriteBatch.DrawRectangle(new Rectangle(mMinimizationRectangle.X, (int)(mMinimizationRectangle.Y + 3 * mMinimizationRectangle.Height), mMinimizationRectangle.Width, (int)(mSize.Y - 3 * mMinimizationRectangle.Height - 20)), mColorBorder, 2);
                spriteBatch.Draw(windowTexture2D, new Rectangle(mMinimizationRectangle.X + 1, (int)(mMinimizationRectangle.Y + 3 * mMinimizationRectangle.Height + 1), mMinimizationRectangle.Width - 2, (int)(mSize.Y - 3 * mMinimizationRectangle.Height - 20 - 2)), null, new Color(255,0,0, 100), 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            }

            if (mMinimizable)
                // if the window should be minimizable -> draw tiny rectangle for minimization to click on
            {
                spriteBatch.DrawRectangle(mMinimizationRectangle, mColorBorder, 2);
                spriteBatch.DrawRectangle(mMinimizationLine, mColorBorder, 1);
            }

            // TEST DRAW STRING
            spriteBatch.DrawString(mTestFont, textaaa, new Vector2(mPosition.X, mPosition.Y + 20), new Color(255, 255, 255));
            spriteBatch.DrawString(mTestFont, DateTime.Now.ToLongTimeString(), new Vector2(mPosition.X, mPosition.Y), new Color(255, 255, 255));

            // restore scissor from backup - needed for other draws
            spriteBatch.GraphicsDevice.ScissorRectangle = saveRect;

            spriteBatch.End();
        }

        public void Update(GameTime gametime)
        {
            if (!mMinimized)
            {
                foreach (var item in mItemList)
                {
                    item.Update(gametime);
                }
            }
            else
            {
                foreach (var item in mItemListBackup)
                {
                    item.Update(gametime);
                }
            }

            // TODO: GET FROM GAME1.CS ?
            mGameTime = gametime.TotalGameTime;
        }


        #endregion


        #region InputManagemend


        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            throw new System.NotImplementedException();
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
                mItemListBackup = mItemList;
                mItemList = new List<IWindowItem>();

                mMinimized = true;
            }
            else if (mouseAction == EMouseAction.LeftClick && withinBounds && mMinimized)
                // LeftClick on Minimize-Button, window IS minimized
                // -> use regular rectangles + actual itemList for draw
            {
                mDrawWindowRectangle = mWindowRectangle;
                mDrawBorderRectangle = mBorderRectangle;
                mItemList = mItemListBackup;

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
    }
}
