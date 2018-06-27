using System;
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
        private float mMin;
        private float mMax;
        private float mCurrentX;
        private bool mSlave;
        private readonly int mSliderSize;
        private readonly int mBarLength;
        private bool mWithValue;
        private SpriteFont mFont;
        private String mStringValue;
        private float mValueCurrent;
        private float mValuePrevious;
        private Director mDirector;

        public delegate void SliderMovingEventHandler(object source, EventArgs args, float percentMoved);
        public event SliderMovingEventHandler SliderMoving;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="sliderSize"></param>
        /// <param name="font"></param>
        /// <param name="valueBox"></param>
        public Slider(Vector2 position, int length, int sliderSize, SpriteFont font, bool valueBox, ref Director director)
        {
            Position = position;
            mMin = Position.X;
            mMax = Position.X + length;
            Size = new Vector2(length, sliderSize);
            mCurrentX = mMin;
            mSliderSize = sliderSize;
            mBarLength = length;
            mValueCurrent = 0;
            mValuePrevious = 0;
            mWithValue = valueBox;
            mDirector = director;
            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);
            if (mWithValue)
            {
                mStringValue = 0.ToString();
            }

            mFont = font;
            Active = true;
        }


        /// <summary>
        /// sends out event that slider is being moved as well
        /// as the decimal of distance covered by slider
        /// </summary>
        protected virtual void OnSliderMoving()
        {
            if (SliderMoving != null && Active)
            {
                SliderMoving(this, EventArgs.Empty, (mCurrentX / (mMax - mMin)));
            }
        }


        public void Update(GameTime gametime)
        {
            if (Active)
            {
                mMin = Position.X;
                mMax = Position.X + mBarLength;
                mCurrentX = Position.X;

                if (mSlave)
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

                    mValuePrevious = mValueCurrent;
                    mValueCurrent = mCurrentX;

                    if (Math.Abs(mValueCurrent - mValuePrevious) > 0.009)
                    {
                        OnSliderMoving();
                    }
                }

                // calculate int value of slider and convert to string
                if (mWithValue)
                {
                    mStringValue = ((int) (((mCurrentX - mMin) / (mMax - mMin)) * 100)).ToString();
                }
            }
        }

        /// <summary>
        /// Draws slider bar as well as slider
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.DrawLine(Position.X, Position.Y, mMax, Position.Y, (Color.White * (float) 0.6), 3);

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

        public Vector2 Position { get; set; }

        public Vector2 Size { get; }

        public bool Active { get; set; }


        // TODO not woriking, wont print debug message
        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            switch (mouseAction)
            {
                case EMouseAction.LeftClick:
                    Debug.Write("Hello");
                    if (Mouse.GetState().X >= mCurrentX - ((float)mSliderSize / 2) &&
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
                case EMouseAction.LeftClick:
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        mSlave = false;
                    }

                    return false;
            }

            return true;
        }


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
