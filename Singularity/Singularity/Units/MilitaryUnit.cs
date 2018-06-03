﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Singularity.Property;

namespace Singularity.Units
{
    class MilitaryUnit : IUnit, IDraw, IUpdate
    {
        private Vector2 mPosition;
        private Vector2 mTargetPosition;
        private int mRotation;
        readonly Texture2D mMilSheet;
        private double mXstep;
        private double mYstep;
        private bool mSelected;
        private bool mTargetReached;

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            mPosition = position;
            mTargetReached = true;
            mMilSheet = spriteSheet;
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination
        /// </summary>
        /// <param name="target"></param>
        public void Rotate(Vector2 target)
        {
            // form a triangle from unit location to mouse location
            // adjust to be at center of sprite 150x75
            double x = (target.X - (mPosition.X + 75));
            double y = (target.Y - (mPosition.Y + 37.5));
            double hypot = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            // calculate degree between formed triangle
            double degree;
            if (Math.Abs(hypot) < 0.01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(y / hypot) * (180.0 / Math.PI);
            }

            // calculate rotation with increased degrees going counterclockwise
            if (x >= 0)
            {
                mRotation = (int) (Math.Round(270 - degree, MidpointRounding.AwayFromZero));
            }
            else
            {
                mRotation = (int) (Math.Round(90 + degree, MidpointRounding.AwayFromZero));
            }

            // add 42 degrees since sprite sheet starts at sprite -42d not 0
            mRotation = (mRotation + 42) % 360;

        }

        /// <summary>
        /// The property which defines the health of the unit
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The current position of the unit.
        /// </summary>
        public Vector2 Position => mPosition;

        /// <summary>
        /// Used to set and get assignment. Assignment will potentially be implemented as a new type which
        /// can be used to determine what assignment a unit has.
        /// </summary>
        public string Assignment
        {
            get; set; //TODO
        }


        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
             Health -= damage; //TODO
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // calculate correct sprite on spritesheet to use based on angle 
            // direction that unit is meant to be faceing 
            int rowNumber = (mRotation / 18);
            int columnNumber = ((mRotation - (rowNumber * 18)) / 3);

            // darken color if unit is selected 
            Color color;
            if (!mSelected) { color = Color.White; }
            else { color = Color.Gainsboro; }

            spriteBatch.Draw(mMilSheet, mPosition, new Rectangle((150 * columnNumber), (75 * rowNumber), 150, 75), color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerConstants.MilitaryUnitLayer);

        }


        public void Update(GameTime gameTime)
        {
            Selected();

            // rotate to face mouse
            if (mSelected && mTargetReached)
            {
                Rotate(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            }

            // calculate path to target position
            if (mSelected && Mouse.GetState().LeftButton == ButtonState.Pressed &&
                ((Math.Abs((mPosition.X + 75) - Mouse.GetState().X) > 60) ||
                 (Math.Abs((mPosition.Y + 37.5) - Mouse.GetState().Y) > 45)))
            {
                Steps(new Vector2(Mouse.GetState().X, (Mouse.GetState().Y)));
            }

            // move unit until target reached
            if (!mTargetReached)
            {
                Move();
            }
        }


        /// <summary>
        /// determines if the the military unit is currently selected by user
        /// left click within area is select
        /// right click anywhere else is deselect 
        /// </summary>
        private void Selected()
        {
            // if left click within unit area : selected
            if ((Math.Abs((mPosition.X + 75) - Mouse.GetState().X) < 60) &&
                (Math.Abs((mPosition.Y + 37.5) - Mouse.GetState().Y) < 45) &&
                Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mSelected = true;
            }

            // right click deselects unit
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                mSelected = false;
            }
        }

        /// <summary>
        /// determine x and y movement of unit in order to
        /// reach target destination. This will then be used
        /// by the Move method 
        /// </summary>
        /// <param name="target"></param>
        private void Steps(Vector2 target)
        {
            // set new unit target, and face target position
            mTargetPosition = target;
            Rotate(mTargetPosition);
            mTargetReached = false;
            

            // travel along the hypotenuse of triangle formed by start and target position
            double hypot = (Math.Sqrt(Math.Pow((mPosition.X - mTargetPosition.X + 75), 2) +
                                      Math.Pow((mPosition.Y - mTargetPosition.Y + 37.5), 2)));

            // calculate the x distance and y distance needed to get to target
            if (Math.Abs(hypot) < 0.01)
            {
                mXstep = Math.Abs(mPosition.X - mTargetPosition.X + 75);
                mYstep = Math.Abs(mPosition.Y - mTargetPosition.Y + 37.5);
            }
            else
            {
                mXstep = Math.Abs((mPosition.X - mTargetPosition.X + 75) / hypot);
                mYstep = Math.Abs((mPosition.Y - mTargetPosition.Y + 37.5) / hypot);
            }

            // determine correct direction of x/y movement
            if (mPosition.X - mTargetPosition.X + 75 < 0)
            {
                mXstep = -mXstep;
            }

            if (mPosition.Y - mTargetPosition.Y +37.5 < 0)
            {
                mYstep = -mYstep;
            }

            // adjusts speed of unit movement 
            mXstep = mXstep * 3;
            mYstep = mYstep * 3;     
        }

        /// <summary>
        /// Updates position of unit in order to reach
        /// target position
        /// </summary>
        private void Move()
        {
            // move position of unit over by x/y step to reach target
            mPosition.X -= (float)mXstep;
            mPosition.Y -= (float)mYstep;

            // check if target position has been reached
            if (Math.Abs((mPosition.X) - mTargetPosition.X + 75) < 8 && Math.Abs((mPosition.Y) - mTargetPosition.Y + 37.5) < 8)
            {
                mTargetReached = true;
            }
        }
    }
}
