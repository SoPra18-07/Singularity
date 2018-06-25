using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen.ScreenClasses
{
    internal sealed class PopupWindow : IDraw, IUpdate, IMouseWheelListener, IMousePositionListener
    {
        #region member variables declaration
        // parameters
        private readonly string mWindowName;
        private readonly Button mButton;
        private readonly Vector2 mPosition;
        private readonly Vector2 mSize;
        private readonly Color mColorBorder;
        private readonly Color mColorFill;
        private readonly SpriteFont mSpriteFontTitle;

        // list of windowItems added to the window
        private readonly List<IWindowItem> mItemList = new List<IWindowItem>();

        // basic window rectangle
        private readonly Rectangle mWindowRectangle;
        private readonly Rectangle mBorderRectangle;
        private readonly Rectangle mTitleBarRectangle;

        // the rectangles needed for scrollable windows
        private Rectangle mScrollBarRectangle;
        private Rectangle mScrollBarBorderRectangle;
        private readonly Rectangle mScissorRectangle;

        // rectangles needed for the next button
        private readonly Rectangle mButtonBorderRectangle;

        // true when the window's windowItems and the padding between them 
        private bool mScrollable; // = false as default value

        // current screen size values
        private int mCurrentScreenWidth;
        private int mCurrentScreenHeight;

        // used by scissorrectangle to create a scrollable window by cutting everything outside specific bounds
        private readonly RasterizerState mRasterizerState;

        // top and bottom positin of the window's combined items - used by scrolling
        private Vector2 mItemPosTop;
        private Vector2 mItemPosBottom;

        // height of all windowItems with borderpaddings - used to implement scrollable windows if needed
        private float mCombinedItemsSize;

        // current mouse position
        private float mMouseX;
        private float mMouseY;

        #endregion

        public PopupWindow(
            string windowName, 
            Button button, 
            Vector2 position, 
            Vector2 size, 
            Color colorBorder, 
            Color colorFill, 
            SpriteFont spriteFontTitle,
            InputManager inputManager, 
            GraphicsDeviceManager graphics)
        {
            mWindowName = windowName;
            mPosition = position;
            mSize = size;
            mColorBorder = colorBorder;
            mColorFill = colorFill;
            mButton = button;
            mSpriteFontTitle = spriteFontTitle;

            mCurrentScreenWidth = graphics.PreferredBackBufferWidth;
            mCurrentScreenHeight = graphics.PreferredBackBufferHeight;

            // size of the title
            const int titleSizeY = 720 / 26;

            // calculate buttonRectangle size
            var buttonSize = new Vector2(button.Size.X + 10, button.Size.Y + 10);

            // position where the next item will be drawn
            mItemPosTop = new Vector2(mPosition.X + 10, mPosition.Y + titleSizeY + 30); // TODO

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
                width: (int)(mSize.X), 
                height: ((int)mSize.Y)
                );

            // ScissorRectangle will cut everything drawn outside of this rectangle when set
            mScissorRectangle = new Rectangle(
                x: (int)(mPosition.X + 10),
                y: (int)(mPosition.Y + titleSizeY + 30),
                width: (mWindowRectangle.Width - 20), 
                height: (int)(mSize.Y - titleSizeY - 3 * 10 - buttonSize.Y)
                );

            // set the rectangle of the title bar
            mTitleBarRectangle = new Rectangle(
                x: (int)mPosition.X + 10,
                y: (int)mPosition.Y + titleSizeY + 20, 
                width: (int)mSize.X - 40,
                height: 1
                );

            // set the rectangle of the button
            mButtonBorderRectangle = new Rectangle(
                x: (int)(mPosition.X + mSize.X / 2 - buttonSize.X / 2),
                y: (int)(mPosition.Y + mSize.Y - buttonSize.Y + 1),
                width: (int)buttonSize.X,
                height: (int)buttonSize.Y - 2);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle(
                x: (int)(mPosition.X + mSize.X - 20),
                y: mScissorRectangle.Y,
                width: 20,
                height: mScissorRectangle.Height
            );

            // set button position
            mButton.Position = new Vector2(mButtonBorderRectangle.X + 5, mButtonBorderRectangle.Y + 5);

            // Initialize scissor window
            mRasterizerState = new RasterizerState() { ScissorTestEnable = true };

            inputManager.AddMouseWheelListener(this);
            inputManager.AddMousePositionListener(this);
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, mRasterizerState);

            // draw window
            spriteBatch.FillRectangle(mWindowRectangle, mColorFill);
            spriteBatch.DrawRectangle(mBorderRectangle, mColorBorder, 2);

            // draw window title + bar
            spriteBatch.DrawString(mSpriteFontTitle, mWindowName, new Vector2(mPosition.X + 10, mPosition.Y + 10), new Color(255, 255, 255));
            spriteBatch.DrawRectangle(mTitleBarRectangle, mColorBorder, 1);

            // draw 'button' + buttonText
            spriteBatch.DrawRectangle(mButtonBorderRectangle, mColorBorder, 2);
            // .DrawString(mSpriteFontButton, mButtonText, new Vector2(mButtonBorderRectangle.X + 5, mButtonBorderRectangle.Y + 5), new Color(0,0,0));

            // Add the scrollbar if the window is scrollable
            if (mScrollable)
            {
                spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);
                spriteBatch.FillRectangle(mScrollBarRectangle, mColorBorder);
            }

            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            // set up current scissor rectangle
            spriteBatch.GraphicsDevice.ScissorRectangle = mScissorRectangle;

            // draw IWindowItems
            foreach (var item in mItemList)
            {
                item.Draw(spriteBatch);
            }

            // restore scissor from backup
            spriteBatch.GraphicsDevice.ScissorRectangle = saveRect;

            // draw button
            mButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void Update(GameTime gametime)
        {
            // current position to place the next item
            var localItemPos = mItemPosTop;

            // calculate the size of all windowItems + padding
            mCombinedItemsSize = 0;

            foreach (var item in mItemList)
            {
                item.Update(gametime);

                item.Position = localItemPos;

                mCombinedItemsSize += 10 + item.Size.Y;

                localItemPos = new Vector2(localItemPos.X, localItemPos.Y + item.Size.Y + 10);
            }

            // bottom of all items combined
            mItemPosBottom = new Vector2(localItemPos.X, localItemPos.Y);

            // check if the window is overflowed with items
            mScrollable = mCombinedItemsSize > mScissorRectangle.Height;

            // update the scrollbar
            mScrollBarRectangle = CalcScrollbarRectangle(mScissorRectangle, mCombinedItemsSize);

            mButton.Update(gametime);
        }

        public void MouseWheelValueChanged(EMouseAction mouseAction)
        {
            // enabled only if
            //  - the mouse is above the scrollable part of the window
            //  - the window is scrollable (the number of items is too big for one window)
            if (!(mMouseX > mPosition.X) || !(mMouseX < mPosition.X + mSize.X) || !(mMouseY > mPosition.Y) ||
                !(mMouseY < mPosition.Y + mSize.Y) || !mScrollable)
            {
                return;
            }

            // scroll up or down
            switch (mouseAction)
            {
                case EMouseAction.ScrollUp:
                    if (!(mItemPosTop.Y > mScissorRectangle.Y - 10))
                        // stop from overflowing
                    {
                        mItemPosTop.Y += +10;
                    }
                    break;
                case EMouseAction.ScrollDown:
                    if (!(mItemPosBottom.Y < mScissorRectangle.Y + mScissorRectangle.Height + 10))
                        // stop from overflowing
                    {
                        mItemPosTop.Y += -10;
                    }
                    break;
            }
        }

        public void MousePositionChanged(float newX, float newY)
        {
            // update member variable with new mouse position
            mMouseX = newX;
            mMouseY = newY;
        }

        /// <summary>
        /// Calculate the scrollbar
        /// </summary>
        /// <param name="scissorRectangle">the scissorrectangle used to cut the winodw</param>
        /// <param name="combinedItemsSize">the size of all items combined with padding</param>
        /// <returns></returns>
        private Rectangle CalcScrollbarRectangle(Rectangle scissorRectangle, float combinedItemsSize)
        {
            // scrollbar to scrollbarRectangleBorder has the same ratio as scissorRectangle to combinedItemSize
            var sizeY = (scissorRectangle.Height / combinedItemsSize) * mScrollBarBorderRectangle.Height;

            // number of possible steps rounded up
            var numberOfSteps = (combinedItemsSize - scissorRectangle.Height + 10 - 1) / 10;
            // number of times scrolled down
            var numberOfStepsTaken = ((scissorRectangle.Y - mItemPosTop.Y) / 10);
            // step size for the scrollbar
            var stepSize = (mScrollBarBorderRectangle.Height - sizeY) / numberOfSteps;
            // calculate new position
            var positionY = mScrollBarBorderRectangle.Y + numberOfStepsTaken * stepSize + 3;

            return new Rectangle((int)(mPosition.X + mSize.X - 20 + 2), (int)positionY, (20 - 4), (int)sizeY);
        }
    }
}
