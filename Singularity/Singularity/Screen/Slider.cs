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


        /// <summary>
        /// Creates an instance of a slide where the square size of the
        /// slider can be specified in pixels 
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="length"></param>
        /// <param name="sliderSize"></param>
        public Slider(Vector2 postion, int length, int sliderSize)
        {
            _mMin = postion.X;
            _mMax = _mMin + length;
            _mPositionY = postion.Y;
            _mCurrentX = _mMin;
            _mSliderSize = sliderSize;
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
                OnSliderMoving();
            }
        }

        /// <summary>
        /// Draws slider bar as well as slider 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.DrawLine(_mMin, _mPositionY, _mMax, _mPositionY, Color.Gray);
           
           // slider based on current position
           spriteBatch.DrawRectangle(new Vector2(_mCurrentX - ((float)_mSliderSize/2), _mPositionY - ((float)_mSliderSize / 2)), 
               new Vector2(_mSliderSize, _mSliderSize), Color.Black);
        }
    }
}
