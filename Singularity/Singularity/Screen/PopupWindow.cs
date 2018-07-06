using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Property;

namespace Singularity.Screen
{
    internal sealed class PopupWindow : IDraw, IUpdate, IMouseWheelListener, IMousePositionListener
    {
        #region member variables declaration
        // parameters
        private readonly string mWindowName;
        private readonly Button mButton;
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
            Position = position;
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
            var buttonSize = new Vector2(x: button.Size.X + 10, y: button.Size.Y + 10);

            // position where the next item will be drawn
            mItemPosTop = new Vector2(x: Position.X + 10, y: Position.Y + titleSizeY + 30); // TODO

            // set the window rectangle
            mWindowRectangle = new Rectangle(
                x: (int)(Position.X + 1),
                y: (int)(Position.Y + 2),
                width: (int)(mSize.X - 2),
                height: (int)(mSize.Y - 2)
                );
            mBorderRectangle = new Rectangle(
                x: (int)Position.X,
                y: (int)Position.Y,
                width: (int)mSize.X,
                height: (int)mSize.Y
                );

            // ScissorRectangle will cut everything drawn outside of this rectangle when set
            mScissorRectangle = new Rectangle(
                x: (int)(Position.X + 10),
                y: (int)(Position.Y + titleSizeY + 30),
                width: mWindowRectangle.Width - 20,
                height: (int)(mSize.Y - titleSizeY - 3 * 10 - buttonSize.Y)
                );

            // set the rectangle of the title bar
            mTitleBarRectangle = new Rectangle(
                x: (int)Position.X + 10,
                y: (int)Position.Y + titleSizeY + 20,
                width: (int)mSize.X - 40,
                height: 1
                );

            // set the rectangle of the button
            mButtonBorderRectangle = new Rectangle(
                x: (int)(Position.X + mSize.X / 2 - buttonSize.X / 2),
                y: (int)(Position.Y + mSize.Y - buttonSize.Y + 1),
                width: (int)buttonSize.X,
                height: (int)buttonSize.Y - 2);

            // set the rectangle for scrolling
            mScrollBarBorderRectangle = new Rectangle(
                x: (int)(Position.X + mSize.X - 20),
                y: mScissorRectangle.Y,
                width: 20,
                height: mScissorRectangle.Height
            );

            // set button position
            mButton.Position = new Vector2(x: mButtonBorderRectangle.X + 5, y: mButtonBorderRectangle.Y + 5);

            // Initialize scissor window
            mRasterizerState = new RasterizerState { ScissorTestEnable = true };

            inputManager.AddMouseWheelListener(iMouseWheelListener: this);
            inputManager.AddMousePositionListener(iMouseListener: this);
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
            // TODO: what is there still to be done?

            // draw window
            /*spriteBatch.FillRectangle(mWindowRectangle, mColorFill);
            spriteBatch.DrawRectangle(mBorderRectangle, mColorBorder, 2);*/
            spriteBatch.StrokedRectangle(location: new Vector2(x: mWindowRectangle.X, y: mWindowRectangle.Y), size: new Vector2(x: mWindowRectangle.Width, y: mWindowRectangle.Height), colorBorder: mColorBorder, colorCenter: mColorFill, opacityBorder: 1f, opacityCenter: 0.8f );

            // draw window title + bar
            spriteBatch.DrawString(spriteFont: mSpriteFontTitle, text: mWindowName, position: new Vector2(x: Position.X + 10, y: Position.Y + 10), color: new Color(r: 255, g: 255, b: 255));
            //spriteBatch.DrawRectangle(mTitleBarRectangle, mColorBorder, 1);
            spriteBatch.StrokedRectangle(location: new Vector2(x: mTitleBarRectangle.X, y: mTitleBarRectangle.Y), size: new Vector2(x: mTitleBarRectangle.Width, y: mTitleBarRectangle.Height), colorBorder: mColorBorder, colorCenter: mColorFill, opacityBorder: 1f, opacityCenter: 0.8f );

            // draw 'button'
            //spriteBatch.DrawRectangle(mButtonBorderRectangle, mColorBorder, 2);
            spriteBatch.StrokedRectangle(location: new Vector2(x: mButtonBorderRectangle.X, y: mButtonBorderRectangle.Y), size: new Vector2(x: mButtonBorderRectangle.Width, y: mButtonBorderRectangle.Height), colorBorder: mColorBorder, colorCenter: mColorFill, opacityBorder: 1f, opacityCenter: 0.8f );

            // backup current scissor so we can restore later
            var saveRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            // Add the scrollbar if the window is scrollable
            if (mScrollable)
            {
                //spriteBatch.DrawRectangle(mScrollBarBorderRectangle, mColorBorder, 2);
                spriteBatch.StrokedRectangle(location: new Vector2(x: mScrollBarBorderRectangle.X - 1, y: mScrollBarBorderRectangle.Y), size: new Vector2(x: mScrollBarBorderRectangle.Width, y: mScrollBarBorderRectangle.Height), colorBorder: mColorBorder, colorCenter: mColorFill, opacityBorder: 1f, opacityCenter: 0.8f );

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

            // draw button
            mButton.Draw(spriteBatch: spriteBatch);
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

                mCombinedItemsSize += 10 + item.Size.Y;

                localItemPos = new Vector2(x: localItemPos.X, y: localItemPos.Y + item.Size.Y + 10);
            }

            // bottom of all items combined
            mItemPosBottom = new Vector2(x: localItemPos.X, y: localItemPos.Y);

            // check if the window is overflowed with items
            mScrollable = mCombinedItemsSize > mScissorRectangle.Height;

            // update the scrollbar
            mScrollBarRectangle = CalcScrollbarRectangle(scissorRectangle: mScissorRectangle, combinedItemsSize: mCombinedItemsSize);

            mButton.Update(gametime: gametime);
        }

        public EScreen Screen { get; } = EScreen.UserInterfaceScreen;

        public bool MouseWheelValueChanged(EMouseAction mouseAction)
        {
            // enabled only if
            //  - the mouse is above the scrollable part of the window
            //  - the window is scrollable (the number of items is too big for one window)
            if (!(mMouseX > Position.X) || !(mMouseX < Position.X + mSize.X) || !(mMouseY > Position.Y) ||
                !(mMouseY < Position.Y + mSize.Y) || !mScrollable)
            {
                return true;
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

            return false;
        }

        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            // update member variable with new mouse position
            mMouseX = screenX;
            mMouseY = screenY;
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
            var sizeY = scissorRectangle.Height / combinedItemsSize * mScrollBarBorderRectangle.Height;

            // number of possible steps rounded up
            var numberOfSteps = (combinedItemsSize - scissorRectangle.Height) / 10;
            // number of times scrolled down
            var numberOfStepsTaken = (scissorRectangle.Y - mItemPosTop.Y) / 10;
            // step size for the scrollbar
            var stepSize = (mScrollBarBorderRectangle.Height - sizeY) / numberOfSteps;
            // calculate new position
            var positionY = mScrollBarBorderRectangle.Y + numberOfStepsTaken * stepSize + 3;

            return new Rectangle(x: (int)(Position.X + mSize.X - 20 + 2), y: (int)positionY, width: 20 - 4, height: (int)sizeY);
        }

        // position of the window
        public Vector2 Position { get; set; }
    }
}
