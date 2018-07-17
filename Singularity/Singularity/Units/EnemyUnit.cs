using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Sound;
using Singularity.Utils;

namespace Singularity.Units
{
    /// <inheritdoc cref="MilitaryUnit"/>
    [DataContract]
    public class EnemyUnit : MilitaryUnit
    {
        /// <summary>
        /// Enemy units controlled by AI and opposed to the player; standard type.
        /// </summary>
        /// <param name="position">Where the unit should be spawned.</param>
        /// <param name="camera">Game camera being used.</param>
        /// <param name="director">Reference to the game director.</param>
        public EnemyUnit(Vector2 position, Camera camera, ref Director director)
            : base(position, camera, ref director, false)
        {
            mColor = Color.Maroon;
            mShootColor = Color.Red;
            ColliderGrid = new bool[,] {};
        }

        public ICollider GetShootingTarget()
        {
            return mShootingTarget;
        }

        public override void Move()
        {
            base.Move();
            Debug.WriteLine("Enemy Unit trying to move.");
        }

        public void SetMovementTarget(Vector2 goal)
        {
            // let the flocker take care of it
            // throw new NotImplementedException();
            if (!mGroup.IsPresent())
            {
                Debug.WriteLine("Enemy Unit now gets assigned to new Flock ... " + Id);
                mGroup = Optional<FlockingGroup>.Of(mDirector.GetMilitaryManager.GetNewFlock());
                mGroup.Get().AssignUnit(this);
            }
            mGroup.Get().FindPath(goal);
            // mGroup.Get().mTargetPosition = goal;
        }
    }
}
