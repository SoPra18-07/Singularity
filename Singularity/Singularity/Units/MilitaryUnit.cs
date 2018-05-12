using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Units
{
    class MilitaryUnit : IUnit
    {
        private Vector2 _position;
        private Vector2 _targetPosition;

        public MilitaryUnit(Vector2 position)
        {
            Id = 0; // TODO this will later use a random number generator to create a unique
                    // id for the specific unit.
            Health = 10; //TODO
            _position = position;
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
            _targetPosition = target;
            _position = _targetPosition;
        }

        /// <summary>
        /// The unique ID of the unit.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The current position of the unit.
        /// </summary>
        public Vector2 Position => _position;

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
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
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
