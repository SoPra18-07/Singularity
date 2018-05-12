using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Units
{
    class MilitaryUnit : IUnit
    {
        private Vector2 mPosition;
        private Vector2 mTargetPosition;
        private int mRotation;

        public MilitaryUnit(Vector2 position)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            mPosition = position;
        }

        /// <summary>
        /// The property which defines the health of the unit
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// This method allows a target to be selected but may eventually be redundant.
        /// </summary>
        /// <param name="target">The vector of the target position.</param>
        public void Move(Vector2 target)
        {
            //TODO
            mTargetPosition = target;
            mPosition = mTargetPosition;

            // figure out last position compared to next position
            // and figure out the angle between the two positions
            mRotation = 5; //TODO
        }

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
        /// The required Draw method. This is the method called during the Draw method of the main game class.
        /// It works by taking the sprite batch, extracting the sprite sheet for the unit, and
        /// drawing a rectangle around the required area.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            // the sprite sheet is 900 x 1500 px, 6 x 20 sprites
            // meaning each sprite is 150 x 75 px
            var spriteNumber = mRotation / 3;
            var x = spriteNumber % 6;
            var y = (int) Math.Ceiling(spriteNumber / 6d);
            spriteBatch.Draw(texture, mPosition, new Rectangle(x, y, 150, 75), Color.White);
        }

        /// <summary>
        /// The required Update method. This is the method called during the Update method of the main class.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Damages the unit by a certain amount.
        /// </summary>
        /// <param name="damage"></param>
        public void MakeDamage(int damage)
        {
             Health -= damage; //TODO
        } 

    }
}
