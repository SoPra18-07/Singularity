using System;
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
        private Texture2D mMilSheet;
        private double xstep;
        private double ystep;
        private bool selected;
        private bool targetReached;

        public MilitaryUnit(Vector2 position, Texture2D spriteSheet)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            mPosition = position;
            targetReached = true;
            mMilSheet = spriteSheet;
        }


        /// <summary>
        /// Rotates unit in order when selected in order to face
        /// user mouse and eventually target destination
        /// </summary>
        /// <param name="target"></param>
        public void Rotate(Vector2 target)
        {
            double x = (target.X - (mPosition.X + 75));
            double y = (target.Y - (mPosition.Y + 37.5));
            double hypot = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            double degree;
            if (Math.Abs(hypot) < .01)
            {
                degree = 0;
            }
            else
            {
                degree = Math.Asin(y / hypot) * (180.0 / Math.PI);
            }

            if (x >= 0)
            {
                mRotation = (int) (Math.Round(270 - degree, MidpointRounding.AwayFromZero));
            }
            else
            {
                mRotation = (int) (Math.Round(90 + degree, MidpointRounding.AwayFromZero));
            }

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
            if (!selected) { color = Color.White; }
            else { color = Color.Gainsboro; }
            spriteBatch.Draw(mMilSheet, mPosition, new Rectangle((150 * columnNumber), (75 * rowNumber), 150, 75),color);

        }


        public void Update(GameTime gameTime)
        {
            this.Selected();

            if (selected && targetReached)
            {
                this.Rotate(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            }

            if (selected && Mouse.GetState().LeftButton == ButtonState.Pressed &&
                ((Math.Abs((mPosition.X + 75) - Mouse.GetState().X) > 60) ||
                 (Math.Abs((mPosition.Y + 37.5) - Mouse.GetState().Y) > 45)))
            {
                this.Steps(new Vector2(Mouse.GetState().X-75, (float)(Mouse.GetState().Y-37.5)));
            }

            if (!targetReached)
            {
                this.Move();
            }
        }


        /// <summary>
        /// determines if the the military unit is currently selected by user
        /// left click within area is select
        /// right click anywhere else is deselect 
        /// </summary>
        private void Selected()
        {
            if ((Math.Abs((mPosition.X + 75) - Mouse.GetState().X) < 60) &&
                (Math.Abs((mPosition.Y + 37.5) - Mouse.GetState().Y) < 45) &&
                Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                selected = true;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                selected = false;
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
            mTargetPosition = target;
            targetReached = false;
            this.Rotate(mTargetPosition);

            // travel along the hypotenuse of triangle formed by start and target position
            double hypot = (Math.Sqrt(Math.Pow((mPosition.X - mTargetPosition.X), 2) +
                                      Math.Pow((mPosition.Y - mTargetPosition.Y), 2)));
            if (Math.Abs(hypot) < 0.01)
            {
                xstep = Math.Abs((mPosition.X - mTargetPosition.X));
                ystep = Math.Abs(mPosition.Y - mTargetPosition.Y);
            }
            else
            {
                xstep = Math.Abs((mPosition.X - mTargetPosition.X) / hypot);
                ystep = Math.Abs((mPosition.Y - mTargetPosition.Y) / hypot);
            }

            // determine correct direction of x/y movement
            if (mPosition.X - mTargetPosition.X < 0)
            {
                xstep = -xstep;
            }

            if (mPosition.Y - mTargetPosition.Y < 0)
            {
                ystep = -ystep;
            }

            // adjusts speed of unit movement 
            xstep = xstep * 10;
            ystep = ystep * 10;     
        }

        /// <summary>
        /// Updates position of unit in order to reach
        /// target position
        /// </summary>
        private void Move()
        {
            mPosition.X -= (float)xstep;
            mPosition.Y -= (float)ystep;

            // check if target position has been reached
            if (Math.Abs((mPosition.X) - mTargetPosition.X) < 8 && Math.Abs((mPosition.Y) - mTargetPosition.Y) < 8)
            {
                targetReached = true;
            }
        }
    }
}
