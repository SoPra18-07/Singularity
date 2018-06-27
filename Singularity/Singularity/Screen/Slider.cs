﻿using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;

namespace Singularity.Screen
{
    class Slider : IWindowItem, IMouseClickListener, IMousePositionListener
    {
        #region Declaration

        // x position of start of slider bar (min) and end of slider bar (max)
        private float mMin;
        private float mMax;

        // current x position of slider
        private float mCurrentX;

        // is the slider slave to the mouse
        private bool mSlave;

        // dimensions of slider and bar in pixels
        private readonly int mSliderSize;
        private readonly int mBarLength;

        // with value box on right side, bar with pages (notches)
        private readonly bool mWithValue;
        private readonly bool mWithPages;

        private int mCurrentPage;
        private int mLastPage;

        // size of a page relative to the lenght of the slider bar
        private float mPageSize;

        // font for writing the value string of the slider 
        private readonly SpriteFont mFont;
        private String mStringValue;

        // previous position of slider
        private float mValuePrevious;

        private Director mDirector;

        // event handler for sending out event of percent of bar covered by slider
        public delegate void SliderMovingEventHandler(object source, EventArgs args, float percentMoved);
        public event SliderMovingEventHandler SliderMoving;

        // event handler for sending out event of page slider is located on 
        public delegate void PageMovingEventHandler(object source, EventArgs args, int currentPage);
        public event PageMovingEventHandler PageMoving;
        #endregion

        public Slider(Vector2 position, int length,int sliderSize, SpriteFont font, ref Director director, bool withValueBox = true, bool withPages = false, int pages = 0)
        {
            Position = position;
            mMin = Position.X;
            mMax = Position.X + length;
            Size = new Vector2(length, sliderSize);
            mCurrentX = mMin;
            mSliderSize = sliderSize;
            mBarLength = length;
            mValuePrevious = mMin;
            mWithValue = withValueBox;
            mWithPages = withPages;
            mFont = font;
            Active = true;
            Pages = pages;
            mDirector = director;
            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);

            // if value box requested, initiate string value to 0
            if (mWithValue)
            {
                mStringValue = 0.ToString();
            }

            // set initial max increment of the slider to the max amount of pages
            if (mWithPages)
            {
                mCurrentPage = 1;
                mLastPage = 1;
                MaxIncrement = pages;
                mPageSize = mBarLength / (Pages-1);
            }
        }

        protected virtual void OnSliderMoving()
        {
            if (SliderMoving != null && Active)
            {
                SliderMoving(this, EventArgs.Empty, (mCurrentX / (mMax - mMin)));
            }
        }

        protected virtual void OnPageMoving()
        {
            if (PageMoving != null && Active)
            {
                PageMoving(this, EventArgs.Empty, mCurrentPage);
            }
        }


        public void Update(GameTime gametime)
        {
            // if slider should be shown 
            if (Active)
            {
                mMin = Position.X;
                mMax = Position.X + mBarLength;
                mPageSize = mBarLength / (Pages - 1);

                // if slave to the mouse then move according to the limits of the slider bar
                if (mSlave && !mWithPages)
                {
                    if (Mouse.GetState().X < mMin)
                    {
                        mCurrentX = mMin;
                    }
                    else if (Mouse.GetState().X > mMax)
                    {
                        mCurrentX = mMax;
                    }
                    else
                    {
                        mCurrentX = Mouse.GetState().X;
                    }

                    // if the bar hasn't increased by a full percent then dont send out an event
                    if (Math.Abs(mCurrentX - mValuePrevious) > 0.009)
                    {
                        OnSliderMoving();
                        mValuePrevious = mCurrentX;
                    }
                    // calculate value of slide and convert to string
                    if (mWithValue)
                    {
                        mStringValue = ((int)(((mCurrentX - mMin) / (mMax - mMin)) * 100)).ToString();
                    }
                }

                else if (mSlave && mWithPages)
                {
                    if (Pages == MaxIncrement)
                    {
                        // set current page to zero if mouse past bar min
                        if (Mouse.GetState().X < mMin)
                        {
                            mCurrentX = mMin;
                            mCurrentPage = 1;
                        }

                        // set current page to max page if mouse past bar min
                        else if (Mouse.GetState().X > mMax)
                        {
                            mCurrentX = mMax;
                            mCurrentPage = Pages;
                        }

                        else
                        {
                            // move one page over if more than half way to next page covered by mouse
                            float distanceCovered = Mouse.GetState().X - mMin;
                            if ((distanceCovered - ((mCurrentPage-1) * mPageSize)) > (.5 * mPageSize))
                            { 
                                mCurrentPage += 1;
                                mCurrentX += mPageSize;
                            }
                        }
                    }

                    else
                    {
                        if (Mouse.GetState().X < mMin)
                        {
                            mCurrentX = mMin; mCurrentPage = 1;
                        }

                        // set max movement of mouse to max increment
                        else if (Mouse.GetState().X > ((MaxIncrement-1) * mPageSize + mMin))
                        {
                            mCurrentX = ((MaxIncrement-1) * mPageSize) + mMin;
                            mCurrentPage = MaxIncrement;
                        }

                        else
                        {
                            float distanceCovered = Mouse.GetState().X - mMin;
                            if ((distanceCovered - ((mCurrentPage-1) * mPageSize)) > (.5 * mPageSize))
                            {
                                mCurrentX += mPageSize;
                                mCurrentPage += 1;
                            }
                        }
                    }

                    // double check to make sure it doesn't exceed MaxIncrement, otherwise set back
                    if (mCurrentPage > MaxIncrement)
                    {
                        mCurrentPage = MaxIncrement;
                        mCurrentX = (MaxIncrement - 1) * mPageSize;
                    }

                    // if page has changed : send out event with new current page
                    if (mLastPage != mCurrentPage)
                    {
                        OnPageMoving();
                        mLastPage = mCurrentPage;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // if slider should be shown
            if (Active)
            {
                // draws slider bar
                spriteBatch.DrawLine(Position.X, Position.Y, mMax, Position.Y, (Color.White * (float) 0.6), 3);

                // draws slider
                spriteBatch.StrokedRectangle(
                    new Vector2(mCurrentX - ((float) mSliderSize / 2), Position.Y - ((float) mSliderSize / 2)),
                    new Vector2(mSliderSize, mSliderSize),
                    Color.Gray,
                    Color.Black,
                    (float) .5,
                    (float) 0.8);

                // add value display
                if (mWithValue)
                {
                    // draws rectangle to the right side of slider
                    spriteBatch.StrokedRectangle(
                        new Vector2(
                            (mMax + mSliderSize + 30) -
                            (mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X / 2),
                            Position.Y - 12 - mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).Y / 4),
                        new Vector2(mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X,
                            mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X),
                        Color.Gray,
                        Color.Black,
                        1,
                        (float) 0.8);

                    // draws in value of slider in the center of display window
                    spriteBatch.DrawString(mFont,
                        origin: Vector2.Zero,
                        position: new Vector2((mMax + mSliderSize + 30) - (mFont.MeasureString(mStringValue).X / 2),
                            Position.Y - 12),
                        color: Color.White,
                        text: mStringValue.ToString(),
                        rotation: 0f,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: 0.2f);
                }
            }
        }

        #region MouseActions

        // TODO not woriking, wont print debug message
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // when left key is pressed and mouse within slider bounds then make slider slave to mouse
                case EMouseAction.LeftClick:
                    Debug.Write("Hello");
                    if(Mouse.GetState().LeftButton == ButtonState.Pressed &&
                        Mouse.GetState().X >= mCurrentX - ((float)mSliderSize / 2) &&
                        Mouse.GetState().X <= mCurrentX + ((float)mSliderSize / 2) &&
                        Mouse.GetState().Y >= Position.Y - ((float)mSliderSize / 2) &&
                        Mouse.GetState().Y <= Position.Y + ((float)mSliderSize / 2))
                    {
                        mSlave = true;

                        return false;
                    }
                    break;
            }
            return false;
        }


        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // once left button is released, release slider as slave from mouse
                case EMouseAction.LeftClick:
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        mSlave = false;
                    }

                    return false;
            }

            return true;
        }

        #endregion

        #region Properties
        public Vector2 Position { get; set; }

        public Vector2 Size { get; }

        public bool Active { get; set; }

        public int Pages { get; set; }

        public int MaxIncrement { get; set; }
        #endregion

        #region NotUsed

        public EScreen Screen { get; }

        public Rectangle Bounds { get; }
        public void MousePositionChanged(float newX, float newY)
        {
        }
        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }
        #endregion
    }
}
