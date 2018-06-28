﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;

namespace Singularity.Screen.ScreenClasses
{
    /// <summary>
    /// InfoBoxes are small boxes which can be used to quickly show a notice to the player. (With or without border)
    /// These boxes can contain IWindowItems and will be shown when the mouse is above a specific rectangle that needs to be set.
    /// </summary>
    class InfoBoxWindow : IDraw, IUpdate, IMousePositionListener
    {
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

        /// <summary>
        /// Creates a info box which is displayed above the mouse position
        /// </summary>
        /// <param name="itemList">list of items for infoBox</param>
        /// <param name="size">size of infobox</param>
        /// <param name="borderColor">bordercolor of infoBox</param>
        /// <param name="centerColor">fillcolor of infoBox</param>
        /// <param name="boundsRectangle">rectangle in which the windowBox is active</param>
        /// <param name="boxed">true, if window should have a border</param>
        /// <param name="director">the director</param>
        public InfoBoxWindow(List<IWindowItem> itemList, Vector2 size, Color borderColor, Color centerColor, Rectangle boundsRectangle, bool boxed, Director director)
        {
            // set members
            mItemList = itemList;
            mSize = new Vector2(size.X + 10, size.Y + 10);
            mBorderColor = borderColor;
            mCenterColor = centerColor;
            BoundRectangle = boundsRectangle;
            mBoxed = boxed;

            // window only active if mouse on Bound Rectangle
            director.GetInputManager.AddMousePositionListener(this);

            //BoundRectangle = boundsRectangle;
            OnRectangle = true;
            Active = true;
        }

        /// <summary>
        /// standard draw method
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active && OnRectangle)
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
            if (Active && OnRectangle)
            {
                // update position to mouse position
                Position = new Vector2(mMouse.X - mSize.X, mMouse.Y - mSize.Y);

                // shifts the items from the top left corner to their position
                var yShift = 5;
                // 
                float maxWidth = 0;
                float maxHeight = 0;

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

                mSize = new Vector2(maxWidth + 10, maxHeight);
            }
        }

        // mousePosition updates the onRectangle bool
        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {
            if (Active)
            {
                // update mouse position
                mMouse = new Vector2(screenX, screenY);

                // infoBox is active if mouse on BoundRectangle (basically the button)
                if (screenX >= BoundRectangle.X &&
                    screenX <= BoundRectangle.X + BoundRectangle.Width &&
                    screenY >= BoundRectangle.Y &&
                    screenY <= BoundRectangle.Y + BoundRectangle.Height)
                {
                    OnRectangle = true;
                }
                else
                {
                    OnRectangle = false;
                }
            }
        }

        // top left corner of infoBox
        private Vector2 Position { get; set; }

        // true if the mouse is above the BoundRectangle
        private bool OnRectangle { get; set; }

        // true if the infoBox is active
        public bool Active { get; set; }

        // Rectangle in which the mouse has to be for the InfoBox to be active
        public Rectangle BoundRectangle { private get; set; }
    }
}