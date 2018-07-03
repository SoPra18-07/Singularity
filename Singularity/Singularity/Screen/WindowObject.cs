﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <summary>
    /// WindowObject is a modular window which holds IWindowItems. The IWindowItems get placed one below the other in the window.
    /// If the IWindowItems are too big for the window, the window automatically becomes scrollable.
    /// The WindowObject is moveable and can be made minimizable
    /// </summary>
    internal sealed class WindowObject: IDraw, IUpdate, IMouseWheelListener, IMouseClickListener, IMousePositionListener
    {
        #region member variables declaration

        // parameters
        private readonly Color mColorBorder; // color of the windowborder
        private readonly Color mColorFill; // color of the window background
        private readonly float mBorderPadding; // gap between items and the left window border
        private readonly float mObjectPadding; // gat between items
        private readonly bool mMinimizable; // weather the window should be minimizable

        // list of windowItems added to the window
        private readonly List<IWindowItem> mItemList = new List<IWindowItem>();

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

        // true when the windowObject was given a backgroundColor in the constructor
        private readonly bool mBackgroundGiven;

        // true when the windowObject is minimized, false else
        private bool mMinimized; // = false as default value

        // size of the rectangle in the top right corner of the window which minimizes the window when clicked on
        private int mMinimizationSize;

        // true when the window's windowItems and the padding between them
        private bool mScrollable; // = false as default value

        // activates the window movement after a mouse click on the title bar
        private bool mClickOnTitleBar;

        // top and bottom positin of the window's combined items - used by scrolling
        private Vector2 mItemPosTop;
        private Vector2 mItemPosBottom;
        private int mItemScrolledValue;

        // height of all windowItems with borderpaddings - used to implement scrollable windows if needed
        private float mCombinedItemsSize;

        // previous mouse position during window movement
        private Vector2 mWindowDragPos;

        // current mouse position
        private float mMouseX;
        private float mMouseY;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        // title font
        private readonly SpriteFont mSpriteFont;

        // size of the window title
        private int mTitleSizeY;

        // current screen size values
        private readonly int mCurrentScreenWidth;
        private readonly int mCurrentScreenHeight;

        #endregion

        #region constructors

        /// <summary>
        /// STANDARD CONSTRUCTOR - window with standard border / background / padding
        /// </summary>
        /// <param name="windowName">window title</param>
        /// <param name="position">top left corner of the window</param>
        /// <param name="size">window size</param>
        /// <param name="minimizable">window minimizable</param>
        /// <param name="spriteFont">title font</param>
        /// <param name="inputManager">standard inputManager</param>
        /// <param name="graphics">standard graphicManager</param>
        public WindowObject(
            string windowName,
            Vector2 position,
            Vector2 size,
            bool minimizable,
            SpriteFont spriteFont,
            InputManager inputManager,
            GraphicsDeviceManager graphics)
        {
            // use parameter-variables
            WindowName = windowName;
            Position = position;
            Size = size;
            mColorBorder = new Color(0.68f, 0.933f, 0.933f, .8f);
            mColorFill = new Color(0.27f, 0.5f, 0.7f, 0.8f);
            mBorderPadding = 10f;
            mObjectPadding = 10f;
            mMinimizable = minimizable;
            mSpriteFont = spriteFont;

            // screen size - needed for input management
            mCurrentScreenWidth = graphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = graphics.PreferredBackBufferHeight;

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            Initialization();

            mBackgroundGiven = true;

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);
        }


        /// <summary>
        /// +BACKGROUND CONSTRUCTOR - window with given border / background / padding
        /// </summary>
        /// <param name="windowName">window title</param>
        /// <param name="position">top left corner of the window</param>
        /// <param name="size">window size</param>
        /// <param name="colorBorder">color windowBorder</param>
        /// <param name="colorFill">color windowBackground</param>
        /// <param name="borderPadding">gap between item & border</param>
        /// <param name="objectPadding">gap between items</param>
        /// <param name="minimizable">window minimizable</param>
        /// <param name="spriteFont">title font</param>
        /// <param name="inputManager">standard inputManager</param>
        /// <param name="graphics">standard graphicManager</param>
        public WindowObject(
            string windowName,
            Vector2 position,
            Vector2 size,
            Color colorBorder,
            Color colorFill,
            float borderPadding,
            float objectPadding,
            bool minimizable,
            SpriteFont spriteFont,
            InputManager inputManager,
            GraphicsDeviceManager graphics)
        {
            // set parameter-variables
            WindowName = windowName;
            Position = position;
            Size = size;
            mColorBorder = colorBorder;
            mColorFill = colorFill;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;
            mSpriteFont = spriteFont;

            // screen size - needed for input management
            mCurrentScreenWidth = graphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = graphics.PreferredBackBufferHeight;

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            Initialization();

            // set start values
            mBackgroundGiven = true;

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);
        }


        /// <summary>
        /// -BACKGROUND CONSTRUCTOR - window with given border / padding
        /// </summary>
        /// <param name="windowName">window title</param>
        /// <param name="position">top left corner of the window</param>
        /// <param name="size">window size</param>
        /// <param name="colorBorder">color windowBorder</param>
        /// <param name="borderPadding">gap between item & border</param>
        /// <param name="objectPadding">gap between items</param>
        /// <param name="minimizable">window minimizable</param>
        /// <param name="spriteFont">title font</param>
        /// <param name="inputManager">standard inputManager</param>
        /// <param name="graphics">standard graphicManager</param>
        public WindowObject(
            string windowName,
            Vector2 position,
            Vector2 size,
            Color colorBorder,
            float borderPadding,
            float objectPadding,
            bool minimizable,
            SpriteFont spriteFont,
            InputManager inputManager,
            GraphicsDeviceManager graphics)
        {
            // set parameter-variables
            WindowName = windowName;
            Position = position;
            Size = size;
            mColorBorder = colorBorder;
            mColorFill = colorBorder;
            mBorderPadding = borderPadding;
            mObjectPadding = objectPadding;
            mMinimizable = minimizable;
            mSpriteFont = spriteFont;

            // screen size - needed for input management
            mCurrentScreenWidth = graphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = graphics.PreferredBackBufferHeight;

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            Initialization();

            // set start values
            mBackgroundGiven = false;

            inputManager.AddMouseClickListener(this, EClickType.InBoundsOnly, EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);
        }

        #endregion

        /// <summary>
        /// initializes all windowObjects
        /// </summary>
        private void Initialization()
        {
            // calculate resizing by screensize
            mMinimizationSize = 20;
            mTitleSizeY = 720 / 26;

            // int to save scroll values
            mItemScrolledValue = 0;

            // activate window
            Active = true;
        }

        /// <summary>
        /// Adds the given WindowItem to the Window
        /// </summary>
        /// <param name="item">IWindowItem</param>
        public void AddItem(IWindowItem item)
        {
            mItemList.Add(item);
        }

        /// <summary>
        /// Removes the given WindowItem from the Window
        /// </summary>
        /// <param name="item">IWindowItem</param>
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
            if (!Active)
                // window is deactivated
            {
                return;
            }

            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            if (!mMinimized)
            {
                // if a background was given -> draw filled rectangle
                if (mBackgroundGiven)
                {
                    spriteBatch.FillRectangle(mWindowRectangle, mColorFill);
                }

                spriteBatch.DrawRectangle(mBorderRectangle, mColorBorder, 2);

                // Add the scrollbar if the window is scrollable
                if (mScrollable)
                {
                    spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);

                    // set up current scissor rectangle
                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(mScrollBarBorderRectangle.X + 1, mScrollBarBorderRectangle.Y + 1, mScrollBarBorderRectangle.Width - 2, mScrollBarBorderRectangle.Height - 2);

                    spriteBatch.FillRectangle(mScrollBarRectangle, mColorBorder);
                }

                // set up current scissor rectangle
                spriteBatch.GraphicsDevice.ScissorRectangle = mScissorRectangle;

                // draw IWindowItems
                foreach (var item in mItemList)
                {
                    if (item.ActiveWindow)
                    {
                        item.Draw(spriteBatch);
                    }
                }

                // restore scissor from backup
                spriteBatch.GraphicsDevice.ScissorRectangle = saveRect;
            }
            else
                // window is minimized
            {
                if (mBackgroundGiven)
                    // if a background was given -> draw filled rectangle
                {
                    spriteBatch.FillRectangle(mMinimizedWindowRectangle, mColorFill);
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
            spriteBatch.DrawString(mSpriteFont, WindowName, new Vector2(Position.X + mMinimizationSize / 2f, Position.Y + mMinimizationSize / 2f), new Color(255, 255, 255));
            spriteBatch.DrawRectangle(mTitleBarRectangle, mColorBorder, 1);
        }

        public void Update(GameTime gametime)
        {
            if (!Active)
                // window is deactivated
            {
                return;
            }

            // position where the next item will be drawn
            mItemPosTop = new Vector2(Position.X + mBorderPadding, Position.Y + mTitleSizeY + 2 * mMinimizationSize + mItemScrolledValue);

            // update Bounds
            if (mClickOnTitleBar)
            {
                // Bounds include the entire screen, so that the window can be moved everywhere
                Bounds = new Rectangle(0, 0, mCurrentScreenWidth, mCurrentScreenHeight);
            }
            else if (!mMinimized)
            {
                // input from inside the window is proccessed
                Bounds = new Rectangle((int)Position.X, (int)Position.Y, (int)(Size.X), ((int)Size.Y));
            }
            else
            {
                // input from inside the minimized window is proccessed
                Bounds = new Rectangle(mMinimizedBorderRectangle.X, mMinimizedBorderRectangle.Y, mMinimizedBorderRectangle.Width, mMinimizedBorderRectangle.Height);
            }

            // current position to place the next item
            var localItemPos = mItemPosTop;

            // calculate the size of all windowItems + padding
            mCombinedItemsSize = 0;
            foreach (var item in mItemList)
            {
                if (item.ActiveWindow)
                {
                    item.Update(gametime);

                    item.Position = localItemPos;

                    mCombinedItemsSize += mObjectPadding + item.Size.Y;

                    localItemPos = new Vector2(localItemPos.X, localItemPos.Y + item.Size.Y + mObjectPadding);
                }
            }

            // bottom of all items combined
            mItemPosBottom = new Vector2(localItemPos.X, localItemPos.Y);

            // check if the window is overflowed with items
            mScrollable = mCombinedItemsSize > mScissorRectangle.Height;

            // set the window rectangle
            mWindowRectangle = new Rectangle(
                x: (int)(Position.X + 1),
                y: (int)(Position.Y + 2),
                width: (int)(Size.X - 2),
                height: (int)(Size.Y - 2)
                );
            mBorderRectangle = new Rectangle(
                x: (int)Position.X,
                y: (int)Position.Y,
                width: (int)Size.X,
                height: (int)Size.Y
                );

            // ScissorRectangle will cut everything drawn outside of this rectangle when set
            mScissorRectangle = new Rectangle(
                x: (int)(Position.X - 1),
                y: (int)(Position.Y + mTitleSizeY + 2 * mMinimizationSize),
                width: (int)(Size.X + 2),
                height: (int)(Size.Y + 2 - mTitleSizeY - 2 * mMinimizationSize - 2)
                );

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle(
                x: (int)(Position.X + Size.X - mMinimizationSize),
                y: (int)Position.Y,
                width: mMinimizationSize,
                height: mMinimizationSize
                );
            mMinimizationLine = new Rectangle(
                x: (int)(Position.X + Size.X - 3 * mMinimizationSize / 4f),
                y: (int)(Position.Y + mMinimizationSize / 2f),
                width: mMinimizationSize / 2,
                height: 1
                );

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle(
                x: (int)(Position.X + 1),
                y: (int)(Position.Y + 2),
                width: (int)Size.X - 2,
                height: mTitleSizeY + mMinimizationSize
                );
            mMinimizedBorderRectangle = new Rectangle(
                x: (int)Position.X,
                y: (int)Position.Y,
                width: (int)Size.X,
                height: mTitleSizeY + mMinimizationSize
                );

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle(
                x: (int)(Position.X + Size.X - mMinimizationSize),
                y: (int)(Position.Y + mTitleSizeY + 2 * mMinimizationSize),
                width: mMinimizationSize,
                height: (int)(Size.Y - mTitleSizeY - 3 * mMinimizationSize)
                );
            mScrollBarRectangle = CalcScrollbarRectangle(mScissorRectangle, mCombinedItemsSize
            );

            // set the rectangle of the title bar
            mTitleBarRectangle = new Rectangle(
                x: (int)Position.X + mMinimizationSize / 2,
                y: (int)Position.Y + mTitleSizeY + mMinimizationSize,
                width: (int)Size.X - 2 * mMinimizationSize,
                height: 1
                );
        }

        #region InputManagement

        public Rectangle Bounds { get; private set; }

        public EScreen Screen { get; } = EScreen.UserInterfaceScreen;

        public bool MouseWheelValueChanged(EMouseAction mouseAction)
        {
            // enabled only if
            //  - the mouse is above the scrollable part of the window
            //  - the window is not minimized
            //  - the window is scrollable (the number of items is too big for one window)
            //  - the window is active
            if (mMouseX > Position.X && mMouseX < Position.X + Size.X && mMouseY > Position.Y &&
                mMouseY < Position.Y + Size.Y && !mMinimized && mScrollable && Active)
            {
                // scroll up or down
                switch (mouseAction)
                {
                    case EMouseAction.ScrollUp:
                        if (!(mItemPosTop.Y > Position.Y + mTitleSizeY + 1.5 * mMinimizationSize))
                            // stop from overflowing
                        {
                            mItemScrolledValue += +10;
                        }
                        break;
                    case EMouseAction.ScrollDown:
                        if (!(mItemPosBottom.Y < Position.Y + Size.Y))
                            // stop from overflowing
                        {
                            mItemScrolledValue += -10;
                        }
                        break;
                }
            }

            // everything following handles if the input is given through or not
            if (!mMinimized &&
                (mMouseX > Position.X && mMouseX < Position.X + Size.X &&
                 mMouseY > Position.Y && mMouseY < Position.Y + Size.Y))
                // not minimized + mouse on window
            {
                return false;
            }

            // resharper wanted it this 'overseeable' way o.O
            // minimized + mouse on minimized window -> return false ... else true
            return !mMinimized || (!(mMouseX > Position.X) || !(mMouseX < mMinimizedBorderRectangle.X + mMinimizedBorderRectangle.Width) ||
                                   !(mMouseY > Position.Y) || !(mMouseY < mMinimizedBorderRectangle.Y + mMinimizedBorderRectangle.Height));
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.LeftClick && withinBounds && Active)
            {
                #region minimization

                if (mMouseX >= mMinimizationRectangle.X &&
                    mMouseX < mMinimizationRectangle.X + mMinimizationSize &&
                    mMouseY >= mMinimizationRectangle.Y &&
                    mMouseY < mMinimizationRectangle.Y + mMinimizationSize)
                // mouse on minimization rectangle
                {
                    if (!mMinimized && mMinimizable)
                    // window IS NOT minimized and it's minimizable
                    // -> use minimized rectangles
                    {
                        mMinimized = true;

                        // disable all items due to minimization
                        foreach (var item in mItemList)
                        {
                            item.ActiveWindow = false;
                        }
                    }
                else if (mMinimized)
                    // LeftClick on Minimize-Button, window IS minimized
                    // -> use regular rectangles + move window back in screen if outside
                    {
                        mMinimized = false;

                        // enable all items due to maximization
                        foreach (var item in mItemList)
                        {
                            item.ActiveWindow = true;
                        }

                        // catch window being out of screen at the bottom after maximization
                        if (Position.Y + Size.Y > mCurrentScreenHeight)
                        {
                            // reset window position
                            Position = new Vector2(Position.X, mCurrentScreenHeight - Size.Y);
                        }
                    }
                }

                #endregion

                #region window movement initiation

            if (mMouseX > Position.X &&
                mMouseX < Position.X + Position.X + Size.X &&
                mMouseY > Position.Y &&
                mMouseY < Position.Y + mTitleSizeY + mMinimizationSize &&
                !mClickOnTitleBar)
                // mouse above the title rectangle
                {
                    if (!(mMouseX >= mMinimizationRectangle.X &&
                          mMouseX < mMinimizationRectangle.X + mMinimizationSize &&
                          mMouseY >= mMinimizationRectangle.Y &&
                          mMouseY < mMinimizationRectangle.Y + mMinimizationSize))
                    // mouse not on minimization rectangle (no movement when pressing the minimization rectangle)
                    {
                        mClickOnTitleBar = true;

                        // set 'previous mouse position'
                        mWindowDragPos = new Vector2(mMouseX - Position.X, mMouseY - Position.Y);
                    }
                }

                #endregion
            }


            // everything following handles if the input is given through or not

            if (!mMinimized &&
                (mMouseX > Position.X && mMouseX < Position.X + Size.X &&
                 mMouseY > Position.Y && mMouseY < Position.Y + Size.Y))
                // not minimized + mouse on window
            {
                return false;
            }

            // resharper wanted it this 'overseeable' way o.O
            // minimized + mouse on minimized window -> return false ... else true
            return !(mMinimized && mMouseX > Position.X && mMouseX < mMinimizedBorderRectangle.X + mMinimizedBorderRectangle.Width &&
                                   mMouseY > Position.Y && mMouseY < mMinimizedBorderRectangle.Y + mMinimizedBorderRectangle.Height);
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            #region window movement

            if (mClickOnTitleBar && Active)
                // enable single window movement + no reaction when deactivated
            {
                // backup old window position to calculate the movement
                var positionOld = Position;

                // update window position
                Position = new Vector2(mMouseX - mWindowDragPos.X, mMouseY - mWindowDragPos.Y);

                #region catch window moving out of screen
                // catch left / right
                if (Position.X < 0)
                {
                    Position = new Vector2(0, Position.Y);
                }
                else if (Position.X + Size.X > mCurrentScreenWidth)
                {
                    Position = new Vector2(mCurrentScreenWidth - Size.X, Position.Y);
                }

                // catch top / bottom
                if (!mMinimized)
                    // full window
                {
                    if (Position.Y < 0)
                    {
                        Position = new Vector2(Position.X, 0);
                    }
                    else if (Position.Y + Size.Y > mCurrentScreenHeight)
                    {
                        Position = new Vector2(Position.X, mCurrentScreenHeight - Size.Y);
                    }
                }
                else
                    // minimized window
                {
                    if (Position.Y < 0)
                    {
                        Position = new Vector2(Position.X, 0);
                    }
                    else if (Position.Y + mMinimizedBorderRectangle.Height > mCurrentScreenHeight)
                    {
                        Position = new Vector2(Position.X, mCurrentScreenHeight - Size.Y);
                    }
                }
                #endregion

                // calculate the movement
                var movementVector = new Vector2(Position.X - positionOld.X, Position.Y - positionOld.Y);
            }

            #endregion

            return false;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            if (!Active)
                // window deactivated
            {
                return true;
            }

            mClickOnTitleBar = false;

            return false;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            if (!Active)
                // window is deactivated
            {
                return;
            }

            // update member variable with new mouse position
            mMouseX = screenX;
            mMouseY = screenY;
        }
        #endregion

        /// <summary>
        /// Calculate the scrollbar
        /// </summary>
        /// <param name="scissorRectangle">the scissorrectangle used to cut the winodw</param>
        /// <param name="combinedItemsSize">the size of all items combined with padding</param>
        /// <returns></returns>
        private Rectangle CalcScrollbarRectangle(Rectangle scissorRectangle, float combinedItemsSize)
        {
            // scrollbar to scrollbarRectangleBorder has the same ratio as scissorRectangle to combinedItemSize
            var sizeY = scissorRectangle.Height / combinedItemsSize * mScrollBarBorderRectangle.Height;

            // number of possible steps
            var numberOfSteps = (combinedItemsSize - scissorRectangle.Height) / 10;
            // number of times scrolled down
            var numberOfStepsTaken = (scissorRectangle.Y - mItemPosTop.Y) / 10;
            // step size for the scrollbar
            var stepSize = (mScrollBarBorderRectangle.Height - sizeY) / numberOfSteps;
            // calculate new position
            var positionY = mScrollBarBorderRectangle.Y + numberOfStepsTaken * stepSize + 3;

            return new Rectangle((int)(Position.X + Size.X - mMinimizationSize + 2), (int)positionY, mMinimizationSize - 4, (int)sizeY);
        }

        // true if window is active (window + items in window will be drawn/updated) or inactive (not drawn/updated)
        internal bool Active { get; set; }

        // position of the window
        public Vector2 Position { private get; set; }

        // size of the window
        public Vector2 Size { get; }

        // the window name is the windows title
        public string WindowName { private get; set; }
    }
}
