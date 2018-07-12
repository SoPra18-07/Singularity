using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Screen
{
    /// <summary>
    /// InfoBoxes are small boxes which can be used to quickly show a notice to the player. (With or without border)
    /// These boxes can contain IWindowItems and will be shown when the mouse is above a specific rectangle that needs to be set.
    /// </summary>
    sealed class InfoBoxWindow : IDraw, IUpdate, IMousePositionListener
    {
        #region member variables 

        // list of items to put in info box
        private readonly List<IWindowItem> mItemList;

        // size of info box
        private Vector2 mSize;

        // colors for the info box rectangle
        private readonly Color mBorderColor;
        private readonly Color mCenterColor;

        // mouse Position
        private Vector2 mMouse;

        // bool to enable or disable the rectangle around the infoBox
        private readonly bool mBoxed;

        // counter to prevent the infoBox showing up at the wrong position by updating the position first before drawing
        private int mCounter;

        #endregion

        /// <summary>
        /// Creates a info box which is displayed above the mouse position
        /// </summary>
        /// <param name="itemList">list of items for infoBox</param>
        /// <param name="size">size of infobox</param>
        /// <param name="borderColor">bordercolor of infoBox</param>
        /// <param name="centerColor">fillcolor of infoBox</param>
        /// <param name="boxed">true, if window should have a border</param>
        /// <param name="director">the director</param>
        public InfoBoxWindow(List<IWindowItem> itemList, Vector2 size, Color borderColor, Color centerColor, bool boxed, Director director)
        {
            // set members
            mItemList = itemList;
            mSize = new Vector2(size.X + 10, size.Y + 10);
            mBorderColor = borderColor;
            mCenterColor = centerColor;
            mBoxed = boxed;

            // window only active if mouse on Bound Rectangle
            director.GetInputManager.AddMousePositionListener(this);

            Active = false;
            mCounter = 0;
        }

        /// <summary>
        /// standard draw method
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active && mCounter > 10)
            {
                if (mBoxed)
                {
                    // infoBox Rectangle
                    spriteBatch.StrokedRectangle(new Vector2(Position.X, Position.Y), new Vector2(mSize.X, mSize.Y), mBorderColor, mCenterColor, 1f, 0.8f);
                }

                // draw all items of infoBox
                foreach (var item in mItemList)
                {
                    item.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// standard update method
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (Active)
            {
                // update position to mouse position
                Position = new Vector2(mMouse.X - mSize.X, mMouse.Y - mSize.Y);

                // shifts the items from the top left corner to their position
                var yShift = 2;
<<<<<<< HEAD

                // set starting values, maxWidth is the maximum width of all items added to the infoBox, maxHeight ~ same just with height
=======
                //
>>>>>>> master
                float maxWidth = 0;
                float maxHeight = 0;

                // update the position of all items from inside the infoBox
                foreach (var item in mItemList)
                {
                    item.Update(gametime);
                    item.Position = new Vector2(Position.X + 5, Position.Y + yShift);

                    yShift = (int)item.Size.Y + 5;

                    // update width values so that the rectangle matches the items width
                    if (item.Size.X > maxWidth)
                    {
                        maxWidth = item.Size.X;
                    }

                    // update height values so that the rectangle matches the items height
                    maxHeight = maxHeight + yShift;
                }

                // update size
                mSize = new Vector2(maxWidth + 10, maxHeight);

                ++mCounter;
            }
            else
            {
                mCounter = 0;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="screenX"></param>
        /// <param name="screenY"></param>
        /// <param name="worldX"></param>
        /// <param name="worldY"></param>
        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            if (Active)
            {
                // update mouse position
                mMouse = new Vector2(screenX, screenY);
            }
        }

        // top left corner of infoBox
        private Vector2 Position { get; set; }

        // true if the infoBox is active
        public bool Active { get; set; }
    }
}
