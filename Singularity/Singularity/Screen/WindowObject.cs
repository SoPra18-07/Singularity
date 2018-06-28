using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <summary>
    /// TODO
    /// </summary>
    internal sealed class WindowObject: IDraw, IUpdate, IMouseWheelListener, IMouseClickListener, IMousePositionListener
    {
        #region member variables declaration

        // parameters
        private readonly string mWindowName; // the window name is the windows title
        private Vector2 mPosition; // position of the window
        private readonly Vector2 mSize; // size of the window
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
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = new Color(r: 0.68f, g: 0.933f, b: 0.933f, alpha: .8f);
            mColorFill = new Color(r: 0.27f, g: 0.5f, b: 0.7f, alpha: 0.8f);
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

            inputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.InBoundsOnly, rightClickType: EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(iMouseWheelListener: this);
            inputManager.AddMousePositionListener(iMouseListener: this);
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
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
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

            inputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.InBoundsOnly, rightClickType: EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(iMouseWheelListener: this);
            inputManager.AddMousePositionListener(iMouseListener: this);
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
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
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

            inputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.InBoundsOnly, rightClickType: EClickType.InBoundsOnly);
            inputManager.AddMouseWheelListener(iMouseWheelListener: this);
            inputManager.AddMousePositionListener(iMouseListener: this);
        }

        #endregion

        /// <summary>
        /// initializes all windowObjects
        /// </summary>
        private void Initialization()
        {
            // calculate resizing by screensize
            mMinimizationSize = (int)mSize.X / 12;
            mTitleSizeY = 720 / 26;

            // position where the next item will be drawn
            mItemPosTop = new Vector2(x: mPosition.X + mBorderPadding, y: mPosition.Y + mTitleSizeY + 2 * mMinimizationSize);

            // Only input from inside the window is proccessed
            Bounds = new Rectangle(x: (int)mPosition.X, y: (int)mPosition.Y, width: (int)mSize.X, height: (int)mSize.Y);
        }

        /// <summary>
        /// Adds the given WindowItem to the Window
        /// </summary>
        /// <param name="item">IWindowItem</param>
        public void AddItem(IWindowItem item)
        {
            mItemList.Add(item: item);
        }

        /// <summary>
        /// Removes the given WindowItem from the Window
        /// </summary>
        /// <param name="item">IWindowItem</param>
        /// <returns>true, if element successfully deleted</returns>
        public bool DeleteItem(IWindowItem item)
        {
            // item is not in list -> can't be removed
            if (!mItemList.Contains(item: item))
            {
                return false;
            }

            // item in list -> remove successful
            mItemList.Remove(item: item);
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            if (!mMinimized)
            {
                // if a background was given -> draw filled rectangle
                if (mBackgroundGiven)
                {
                    spriteBatch.FillRectangle(rect: mWindowRectangle, color: mColorFill);
                }

                spriteBatch.DrawRectangle(rect: mBorderRectangle, color: mColorBorder, thickness: 2);

                // Add the scrollbar if the window is scrollable
                if (mScrollable)
                {
                    spriteBatch.DrawRectangle(rect: mScrollBarBorderRectangle, color: mColorBorder, thickness: 2);

                    // set up current scissor rectangle
                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(x: mScrollBarBorderRectangle.X + 1, y: mScrollBarBorderRectangle.Y + 1, width: mScrollBarBorderRectangle.Width - 2, height: mScrollBarBorderRectangle.Height - 2);

                    spriteBatch.FillRectangle(rect: mScrollBarRectangle, color: mColorBorder);
                }

                // set up current scissor rectangle
                spriteBatch.GraphicsDevice.ScissorRectangle = mScissorRectangle;

                // draw IWindowItems
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch: spriteBatch);
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
                    spriteBatch.FillRectangle(rect: mMinimizedWindowRectangle, color: mColorFill);
                }

                spriteBatch.DrawRectangle(rect: mMinimizedBorderRectangle, color: mColorBorder, thickness: 2);
            }

            // if the window should be minimizable -> draw tiny rectangle for minimization to click on
            if (mMinimizable)
            {
                spriteBatch.DrawRectangle(rect: mMinimizationRectangle, color: mColorBorder, thickness: 2);
                spriteBatch.DrawRectangle(rect: mMinimizationLine, color: mColorBorder, thickness: 1);
            }

            // draw window title + bar
            spriteBatch.DrawString(spriteFont: mSpriteFont, text: mWindowName, position: new Vector2(x: mPosition.X + mMinimizationSize / 2f, y: mPosition.Y + mMinimizationSize / 2f), color: new Color(r: 255, g: 255, b: 255));
            spriteBatch.DrawRectangle(rect: mTitleBarRectangle, color: mColorBorder, thickness: 1);
        }

        public void Update(GameTime gametime)
        {
            // current position to place the next item
            var localItemPos = mItemPosTop;

            // calculate the size of all windowItems + padding
            mCombinedItemsSize = 0;
            foreach (var item in mItemList)
            {
                item.Update(gametime: gametime);

                item.Position = localItemPos;

                mCombinedItemsSize += mObjectPadding + item.Size.Y;

                localItemPos = new Vector2(x: localItemPos.X, y: localItemPos.Y + item.Size.Y + mObjectPadding);
            }

            // bottom of all items combined
            mItemPosBottom = new Vector2(x: localItemPos.X, y: localItemPos.Y);

            // check if the window is overflowed with items
            mScrollable = mCombinedItemsSize > mScissorRectangle.Height;

            // set the window rectangle
            mWindowRectangle = new Rectangle(
                x: (int)(mPosition.X + 1),
                y: (int)(mPosition.Y + 2),
                width: (int)(mSize.X - 2),
                height: (int)(mSize.Y - 2)
                );
            mBorderRectangle = new Rectangle(
                x: (int)mPosition.X,
                y: (int)mPosition.Y,
                width: (int)mSize.X,
                height: (int)mSize.Y
                );

            // ScissorRectangle will cut everything drawn outside of this rectangle when set
            mScissorRectangle = new Rectangle(
                x: (int)(mPosition.X - 1),
                y: (int)(mPosition.Y + mTitleSizeY + 2 * mMinimizationSize),
                width: (int)(mSize.X + 2),
                height: (int)(mSize.Y + 2 - mTitleSizeY - 2 * mMinimizationSize - 2)
                );

            // set the rectangle for minimization in the top right corner of the window
            mMinimizationRectangle = new Rectangle(
                x: (int)(mPosition.X + mSize.X - mMinimizationSize),
                y: (int)mPosition.Y,
                width: mMinimizationSize,
                height: mMinimizationSize
                );
            mMinimizationLine = new Rectangle(
                x: (int)(mPosition.X + mSize.X - 3 * mMinimizationSize / 4f),
                y: (int)(mPosition.Y + mMinimizationSize / 2f),
                width: mMinimizationSize / 2,
                height: 1
                );

            // set the rectangle for the minimized window
            mMinimizedWindowRectangle = new Rectangle(
                x: (int)(mPosition.X + 1),
                y: (int)(mPosition.Y + 2),
                width: (int)mSize.X - 2,
                height: mTitleSizeY + mMinimizationSize
                );
            mMinimizedBorderRectangle = new Rectangle(
                x: (int)mPosition.X,
                y: (int)mPosition.Y,
                width: (int)mSize.X,
                height: mTitleSizeY + mMinimizationSize
                );

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle(
                x: (int)(mPosition.X + mSize.X - mMinimizationSize),
                y: (int)(mPosition.Y + mTitleSizeY + 2 * mMinimizationSize),
                width: mMinimizationSize,
                height: (int)(mSize.Y - mTitleSizeY - 3 * mMinimizationSize)
                );
            mScrollBarRectangle = CalcScrollbarRectangle(scissorRectangle: mScissorRectangle, combinedItemsSize: mCombinedItemsSize
            );

            // set the rectangle of the title bar
            mTitleBarRectangle = new Rectangle(
                x: (int)mPosition.X + mMinimizationSize / 2,
                y: (int)mPosition.Y + mTitleSizeY + mMinimizationSize,
                width: (int)mSize.X - 2 * mMinimizationSize,
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
            if ((mMouseX > mPosition.X) && (mMouseX < mPosition.X + mSize.X) && (mMouseY > mPosition.Y) &&
                (mMouseY < mPosition.Y + mSize.Y) && !mMinimized && mScrollable)
            {
                // scroll up or down
                switch (mouseAction)
                {
                    case EMouseAction.ScrollUp:
                        if (!(mItemPosTop.Y > mPosition.Y + mTitleSizeY + 1.5 * mMinimizationSize))
                            // stop from overflowing
                        {
                            mItemPosTop.Y += +10;
                        }
                        break;
                    case EMouseAction.ScrollDown:
                        if (!(mItemPosBottom.Y < mPosition.Y + mSize.Y))
                            // stop from overflowing
                        {
                            mItemPosTop.Y += -10;
                        }
                        break;
                }
            }

            // everything following handles if the input is given through or not
            if (!mMinimized &&
                (mMouseX > mPosition.X && mMouseX < mPosition.X + mSize.X &&
                 mMouseY > mPosition.Y && mMouseY < mPosition.Y + mSize.Y))
                // not minimized + mouse on window
            {
                return false;
            }

            // resharper wanted it this 'overseeable' way o.O
            // minimized + mouse on minimized window -> return false ... else true
            return !mMinimized || (!(mMouseX > mPosition.X) || !(mMouseX < mMinimizedBorderRectangle.X + mMinimizedBorderRectangle.Width) || 
                                   !(mMouseY > mPosition.Y) || !(mMouseY < mMinimizedBorderRectangle.Y + mMinimizedBorderRectangle.Height));
        }

        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            if (mouseAction == EMouseAction.LeftClick && withinBounds)
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
                            item.Active = false;
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
                            item.Active = true;
                        }

                        // catch window being out of screen at the bottom after maximization
                        if (mPosition.Y + mSize.Y > mCurrentScreenHeight)
                        {
                            // reset window position
                            mPosition.Y = mCurrentScreenHeight - mSize.Y;
                            // reset item position
                            mItemPosTop = new Vector2(x: mPosition.X + mBorderPadding, y: mPosition.Y + mTitleSizeY + 2 * mMinimizationSize);
                        }
                    }
                }

                #endregion

                #region window movement initiation

            if (mMouseX > mPosition.X &&
                mMouseX < mPosition.X + mPosition.X + mSize.X &&
                mMouseY > mPosition.Y &&
                mMouseY < mPosition.Y + mTitleSizeY + mMinimizationSize &&
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
                        mWindowDragPos = new Vector2(x: mMouseX - mPosition.X, y: mMouseY - mPosition.Y);

                        // new Bounds so that the window can be moved everywhere
                        Bounds = new Rectangle(x: 0, y: 0, width: mCurrentScreenWidth, height: mCurrentScreenHeight);
                    }
                }

                #endregion
            }

            // everything following handles if the input is given through or not
            if (!mMinimized &&
                (mMouseX > mPosition.X && mMouseX < mPosition.X + mSize.X &&
                 mMouseY > mPosition.Y && mMouseY < mPosition.Y + mSize.Y))
                // not minimized + mouse on window
            {
                return false;
            }

            // resharper wanted it this 'overseeable' way o.O
            // minimized + mouse on minimized window -> return false ... else true
            return !mMinimized || (!(mMouseX > mPosition.X) || !(mMouseX < mMinimizedBorderRectangle.X + mMinimizedBorderRectangle.Width) || 
                                   !(mMouseY > mPosition.Y) || !(mMouseY < mMinimizedBorderRectangle.Y + mMinimizedBorderRectangle.Height));
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            #region window movement
            if (mClickOnTitleBar)
                // enable single window movement
            {
                // backup old window position to calculate the movement
                var positionOld = mPosition;

                // update window position
                mPosition.X = mMouseX - mWindowDragPos.X;
                mPosition.Y = mMouseY - mWindowDragPos.Y;

                #region catch window moving out of screen
                // catch left / right
                if (mPosition.X < 0)
                {
                    mPosition.X = 0;
                }
                else if (mPosition.X + mSize.X > mCurrentScreenWidth)
                {
                    mPosition.X = mCurrentScreenWidth - mSize.X;
                }

                // catch top / bottom
                if (!mMinimized)
                    // full window
                {
                    if (mPosition.Y < 0)
                    {
                        mPosition.Y = 0;
                    }
                    else if (mPosition.Y + mSize.Y > mCurrentScreenHeight)
                    {
                        mPosition.Y = mCurrentScreenHeight - mSize.Y;
                    }
                }
                else
                    // minimized window
                {
                    if (mPosition.Y < 0)
                    {
                        mPosition.Y = 0;
                    }
                    else if (mPosition.Y + mMinimizedBorderRectangle.Height > mCurrentScreenHeight)
                    {
                        mPosition.Y = mCurrentScreenHeight - mSize.Y;
                    }
                }
                #endregion

                // calculate the movement
                var movementVector = new Vector2(x: mPosition.X - positionOld.X, y: mPosition.Y - positionOld.Y);

                // item movement
                mItemPosTop = new Vector2(x: mItemPosTop.X + movementVector.X, y: mItemPosTop.Y + movementVector.Y);
            }

            #endregion

            return false;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            mClickOnTitleBar = false;

            Bounds = new Rectangle(x: (int)mPosition.X, y: (int)mPosition.Y, width: (int)mSize.X, height: (int)mSize.Y);

            return false;
        }

        public void MousePositionChanged(float newX, float newY)
        {
            // update member variable with new mouse position
            mMouseX = newX;
            mMouseY = newY;
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

            return new Rectangle(x: (int)(mPosition.X + mSize.X - mMinimizationSize + 2), y: (int)positionY, width: mMinimizationSize - 4, height: (int)sizeY);
        }
    }
}
