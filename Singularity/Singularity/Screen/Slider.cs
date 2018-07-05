using System;
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

        private Vector2 mLastPosition;

        // current x position of slider
        private float mCurrentX;

        // is the slider slave to the mouse
        private bool mSlave;

        // with value box on right side, bar with pages (notches)
        private readonly bool mWithValue;
        private readonly bool mWithPages;

        // current page the user is on, and last page the user was on
        private int mCurrentPage;
        private int mLastPage;

        // total amount of pages previously had (to update slider when pages are added)
        private int mLastPagesCount;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="position"> position of the slider bar left hand corner</param>
        /// <param name="length"> length of the slider bar</param>
        /// <param name="sliderSize"> size of the slider (square)</param>
        /// <param name="font"> font used for value display</param>
        /// <param name="director"></param>
        /// <param name="withValueBox"> specify if you want the value box : default yet</param>
        /// <param name="withPages"> specify if you want pages: default no</param>
        /// <param name="pages"> amout of pages you want</param>
        public Slider(Vector2 position,
            int length,
            int sliderSize,
            SpriteFont font,
            ref Director director,
            bool withValueBox = true,
            bool withPages = false,
            int pages = 0, EScreen screen = EScreen.UserInterfaceScreen)
        {
            Position = position;
            mLastPosition = position;
            mMin = Position.X;
            mMax = Position.X + length;
            Size = new Vector2(length, sliderSize);
            mCurrentX = mMin;
            mValuePrevious = mMin;
            mWithValue = withValueBox;
            mWithPages = withPages;
            mFont = font;
            ActiveWindow = true;
            Pages = pages;
            mLastPagesCount = Pages;
            mDirector = director;
            mDirector.GetInputManager.AddMouseClickListener(iMouseClickListener: this, leftClickType: EClickType.Both, rightClickType: EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(iMouseListener: this);

            // if value box requested, initiate string value to 0
            if (mWithValue)
            {
                mStringValue = 0.ToString();
            }

            // set initial max increment of the slider to the max amount of pages
            if (mWithPages)
            {
                mCurrentPage = 0;
                mLastPage = 0;
                MaxIncrement = Pages;
                mPageSize = Size.X / Pages;
            }
        }

        protected virtual void OnSliderMoving()
        {
            if (SliderMoving != null && ActiveWindow)
            {
                SliderMoving(this, EventArgs.Empty, (mCurrentX / (mMax - mMin)));
            }
        }

        protected virtual void OnPageMoving()
        {
            if (PageMoving != null && ActiveWindow)
            {
                PageMoving(this, EventArgs.Empty, mCurrentPage);
            }
        }


        /// <summary>
        /// Update the values and placement of slider bar
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // if slider should be shown
            if (ActiveWindow)
            {
                mMin = Position.X;
                mMax = Position.X + Size.X;
                mPageSize = Size.X / Pages;

                // if Position has changed, update slider bar position based on change
                if (!Position.Equals(mLastPosition))
                {
                    mCurrentX += (Position.X - mLastPosition.X);
                    mLastPosition = Position;
                }

                // if page increase or decrease, update position
                if (!Pages.Equals(mLastPagesCount))
                {
                    mCurrentX = (mMin + mCurrentPage * mPageSize);
                    mLastPagesCount = Pages;
                }

                // if slider is left click them make it slave to the mouse
                if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                    Mouse.GetState().X >= mCurrentX - (Size.Y / 2) &&
                    Mouse.GetState().X <= mCurrentX + (Size.Y / 2) &&
                    Mouse.GetState().Y >= Position.Y - (Size.Y / 2) &&
                    Mouse.GetState().Y <= Position.Y + (Size.Y / 2))
                {
                    mSlave = true;
                }

                // if slider is left button released then unslave from mouse
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    mSlave = false;
                }

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
                        mStringValue = ((int) (((mCurrentX - mMin) / (mMax - mMin)) * 100)).ToString();
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
                            mCurrentPage = 0;
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
                            if (Math.Abs((distanceCovered - (mCurrentPage * mPageSize))) > (.5 * mPageSize))
                            {
                                // move one page forward or backward depending on position relative to mouse
                                if ((distanceCovered - (mCurrentPage * mPageSize)) > 0)
                                {
                                    mCurrentPage += 1;
                                    mCurrentX += mPageSize;
                                }
                                else
                                {
                                    mCurrentPage -= 1;
                                    mCurrentX -= mPageSize;
                                }
                            }
                        }
                    }

                    else
                    {
                        if (Mouse.GetState().X < mMin)
                        {
                            mCurrentX = mMin;
                            mCurrentPage = 0;
                        }

                        // set max movement of mouse to max increment
                        else if (Mouse.GetState().X > (MaxIncrement * mPageSize + mMin))
                        {
                            mCurrentX = (MaxIncrement * mPageSize) + mMin;
                            mCurrentPage = MaxIncrement;
                        }

                        else
                        {
                            float distanceCovered = Mouse.GetState().X - mMin;
                            if (Math.Abs((distanceCovered - (mCurrentPage * mPageSize))) > (.5 * mPageSize))
                            {
                                // move one page forward or backward depending on position relative to mouse
                                if ((distanceCovered - (mCurrentPage * mPageSize)) > 0)
                                {
                                    mCurrentX += mPageSize;
                                    mCurrentPage += 1;
                                }
                                else
                                {
                                    mCurrentX -= mPageSize;
                                    mCurrentPage -= 1;

                                }
                            }
                        }
                    }

                    // double check to make sure it doesn't exceed MaxIncrement, otherwise set back
                    if (mCurrentPage > MaxIncrement)
                    {
                        mCurrentPage = MaxIncrement;
                        mCurrentX = MaxIncrement * mPageSize;
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

        /// <summary>
        /// Draws the slider bar and slider and possible page notches if specified and value box
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // if slider should be shown
            if (ActiveWindow)
            {

                // add value display
                if (mWithValue)
                {
                    // draws rectangle to the right side of slider
                    spriteBatch.StrokedRectangle(
                        new Vector2(
                            (mMax + Size.Y + 30) -
                            (mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X / 2),
                            Position.Y - 12 - mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).Y / 4),
                        new Vector2(mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X,
                            mFont.MeasureString(mMax.ToString(CultureInfo.InvariantCulture)).X),
                        Color.Gray,
                        Color.Black,
                        1,
                        (float) 0.8);

                    if (!mWithPages)
                    {
                        // draws in value of slider in the center of display window
                        spriteBatch.DrawString(mFont,
                            origin: Vector2.Zero,
                            position: new Vector2((mMax + Size.Y + 30) - (mFont.MeasureString(mStringValue).X / 2),
                                Position.Y - 12),
                            color: Color.White,
                            text: mStringValue.ToString(),
                            rotation: 0f,
                            scale: 1f,
                            effects: SpriteEffects.None,
                            layerDepth: 0.2f);
                    }
                    else
                    {
                        if (mPageSize > 3)
                        {
                            // draw page notches if they arent to close to another on bar
                            for (int i = 0; i < (Pages + 1); i++)
                            {
                                spriteBatch.DrawLine(new Vector2((Position.X + (i * (Size.X / Pages))), Position.Y - 2),
                                    4,
                                    1.57f,
                                    Color.White * .8f,
                                    1);
                            }
                        }

                        // draws in page number of slider in the center of display window
                        spriteBatch.DrawString(mFont,
                            origin: Vector2.Zero,
                            position: new Vector2(
                                (mMax + Size.Y + 30) - (mFont.MeasureString(mCurrentPage.ToString()).X / 2),
                                Position.Y - 12),
                            color: Color.White,
                            text: mCurrentPage.ToString(),
                            rotation: 0f,
                            scale: 1f,
                            effects: SpriteEffects.None,
                            layerDepth: 0.2f);

                    }

                    // draws slider bar
                    spriteBatch.DrawLine(Position, Size.X, 0f, Color.White * 0.6f, 2);

                    // draws slider
                    spriteBatch.StrokedRectangle(
                        new Vector2(mCurrentX - (Size.Y / 2), Position.Y - (Size.Y / 2)),
                        new Vector2(Size.Y, Size.Y),
                        Color.Gray,
                        Color.Black,
                        (float) .5,
                        (float) 0.8);

                }
            }
        }

        #region MouseActionsWhichDontWork

        // TODO not woriking, wont print debug message
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // when left key is pressed and mouse within slider bounds then make slider slave to mouse
                case EMouseAction.LeftClick:

                    if (Mouse.GetState().X >= mCurrentX - (Size.Y / 2) &&
                        Mouse.GetState().X <= mCurrentX + (Size.Y / 2) &&
                        Mouse.GetState().Y >= Position.Y - (Size.Y / 2) &&
                        Mouse.GetState().Y <= Position.Y + (Size.Y / 2))
                    {
                        mSlave = true;
                        return false;
                    }

                    break;
            }

            return true;
        }


        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                // once left button is released, release slider as slave from mouse
                case EMouseAction.LeftClick:
                    mSlave = false;
                    return false;
            }

            return true;
        }

        #endregion

        #region Properties

        // changes the location of slider bar
        public Vector2 Position { get; set; }

        //(length of bar, size of slider)
        public Vector2 Size { get; }

        // can make slider not active (not drawn and nothing happens)
        public bool ActiveWindow { get; set; }

        // can change the amount of pages available on slider bar
        public int Pages { get; set; }

        // can change the max amount the slider bar can go instead of to end of bar for pages
        // CAREFUL this does NOT update with changes to PAGES
        public int MaxIncrement { get; set; }

        public int CurrentPage() {return mCurrentPage;}

        public EScreen Screen { get;}

        public Rectangle Bounds { get; }
        public void MousePositionChanged(float screenX, float screenY, float worldX, float worldY)
        {

        }
        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }
        #endregion
    }
}
