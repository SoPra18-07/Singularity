using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Libraries;

namespace Singularity.Screen
{
    class Slider: IWindowItem
    {
        // minimum and maximum position that slider can reach
        private readonly float _mMin;
        private readonly float _mMax;

        // current x position of the slider (from the middle)
        private float _mCurrentX;

        // y position of the line
        private readonly float _mPositionY;

        // bool on whether the slider is currently being moved by
        // left mouse select
        private bool _mSlave;

        // size of slider in pixels (will be represented as a square)
        private readonly int _mSliderSize;

        // custom delegate in order to be able to send out the percentage moved along slider bar
        public delegate void SliderMovingEventHandler(object source, EventArgs args, float percentMoved);
        public event SliderMovingEventHandler SliderMoving;

        // whether to add window on side that indicates value of slider postion
        private bool mWithValue;

        // font used to show slider value
        private SpriteFont mFont;

        // value as stringb of the slider position
        private String mStringValue;

        // current value of slider and previous value of slider to only send out
        // changes in slider value when slave to mouse
        private float mValueCurrent;
        private float mValuePrevious;



        /// <summary>
        /// Creates an instance of a slide where the square size of the
        /// slider can be specified in pixels. without any information on value,
        /// no page jumps
        /// </summary>
        /// <param name="postion"> position of slider (left corner)</param>
        /// <param name="length"> length of slider</param>
        /// <param name="sliderSize"> size of movable slider</param>
        public Slider(Vector2 postion, int length, int sliderSize)
        {
            _mMin = postion.X;
            _mMax = _mMin + length;
            _mPositionY = postion.Y;
            _mCurrentX = _mMin;
            _mSliderSize = sliderSize;
            mValueCurrent = 0;
            mValuePrevious = 0;
        }

        /// <summary>
        /// creates slider with a window on the right side that displays the
        /// current value of the slider as an integer
        /// </summary>
        /// <param name="postion"> position of slider (left corner)</param>
        /// <param name="length"> length of slider</param>
        /// <param name="sliderSize"> size of movable slider</param>
        /// <param name="font"> font in which to displaye slider value in</param>
        public Slider(Vector2 postion, int length, int sliderSize, SpriteFont font)
        {
            _mMin = postion.X;
            _mMax = _mMin + length;
            _mPositionY = postion.Y;
            _mCurrentX = _mMin;
            _mSliderSize = sliderSize;
            mWithValue = true;
            mValueCurrent = 0;
            mValuePrevious = 0;
            mFont = font;
            // start off value at 0
            mStringValue = 0.ToString();
        }


        /// <summary>
        /// sends out event that slider is being moved as well
        /// as the decimal of distance covered by slider
        /// </summary>
        protected virtual void OnSliderMoving()
        {
            if (SliderMoving != null)
            {
                SliderMoving(this, EventArgs.Empty, (_mCurrentX/(_mMax-_mMin)));
            }
        }

        /// <summary>
        /// Updates the position of the slider as well as if it
        /// is currently slave to the mouse or not
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            // if slider is left click them make it slave to the mouse
            if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                Mouse.GetState().X >= _mCurrentX - ((float)_mSliderSize / 2) &&
                Mouse.GetState().X <= _mCurrentX + ((float)_mSliderSize / 2) &&
                Mouse.GetState().Y >= _mPositionY - ((float)_mSliderSize / 2) &&
                Mouse.GetState().Y <= _mPositionY + ((float)_mSliderSize / 2))
            {
                _mSlave = true;
            }

            // if slider is left button released then unslave from mouse
            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                _mSlave = false;
            }

            // if button is slaved to mouse than adjust coordinates of slider
            // based on position of mouse. do not exceed min or max position of
            // slider bar however
            if (_mSlave)
            {
                if (Mouse.GetState().X < _mMin)
                {
                    _mCurrentX = _mMin;
                }
                else if (Mouse.GetState().X > _mMax)
                {
                    _mCurrentX = _mMax;
                }
                else
                {
                    _mCurrentX = Mouse.GetState().X;
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
                mStringValue = ((int)(((mCurrentX - mMin) / (mMax - mMin)) * 100)).ToString();
            }
        }

        /// <summary>
        /// Draws slider bar as well as slider
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(mMin, mPositionY, mMax, mPositionY, (Color.White * (float)0.6), 3);

            // slider based on current position
            spriteBatch.StrokedRectangle(new Vector2(mCurrentX - ((float)mSliderSize / 2), mPositionY - ((float)mSliderSize / 2)),
                new Vector2(mSliderSize, mSliderSize), Color.Gray, Color.Black, (float).5, (float)0.8);

            // add value display
            if (mWithValue)
            {
                // draws rectangle to the right side of slider
                spriteBatch.StrokedRectangle(new Vector2(mMax + mSliderSize, mPositionY - 30), new Vector2(60, 60), Color.Gray, Color.Black, 1, (float)0.8);

                // draws in value of slider in the center of display window
                spriteBatch.DrawString(mFont,
                    origin: Vector2.Zero,
                    position: new Vector2((mMax + mSliderSize + 30) - (mFont.MeasureString(mStringValue).X / 2), mPositionY - 12),
                    color: Color.White,
                    text: mStringValue.ToString(),
                    rotation: 0f,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0.2f);

            }
        }

    }
}
