using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    public interface IFlocking : IUpdate
    {
        [DataMember]
        Vector2 AbsolutePosition { get; set; }
        [DataMember]
        Vector2 AbsoluteSize { get; set; }

        /// <summary>
        /// This is simply the velocity the FlockingUnit has.
        /// </summary>
        [DataMember]
        Vector2 Velocity { get; set; }

        [DataMember]
        int Speed { get; }

        void Move();

        [DataMember]
        int FlockingId { get; }

    }



    [DataContract]
    public abstract class AFlocking : IFlocking
    {
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public Vector2 Velocity { get; set; }
        [DataMember]
        public int Speed { get; set; }

        protected Director mDirector;
        [DataMember]
        protected Optional<FlockingGroup> mGroup;
        [DataMember]
        public int FlockingId { get; set; }

        protected AFlocking(ref Director director, Optional<FlockingGroup> group)
        {
            mDirector = director;
            mGroup = group;
            FlockingId = group.IsPresent() ? group.Get().FlockingId : mDirector.GetIdGenerator.NextId();
        }


        public virtual void Move()
        {
            // calculate the forces:
            // - Alignment
            //     The Velocity of the FlockingGroup. This is part for the PathFinding.
            // - Cohesion
            //     Steering towards the Center of mass relative to other members of the FlockingGroup (has a Center/AbsolutePosition for that matter)
            // - Seperation
            //     The steering away from other members of the FlockingGroup. this is also precomputed there.
            var align = mGroup.Get().Velocity;
            var cohes = mGroup.Get().CohesionRaw - AbsolutePosition;
            var seper = Vector2.Multiply(mGroup.Get().SeperationRaw - AbsolutePosition * mGroup.Get().UnitCount(), -1f);

            Velocity = Vector2.Normalize(align + cohes + seper) * mGroup.Get().ActualSpeed;

            AbsolutePosition += Velocity;
        }





        public virtual void ReloadContent(ref Director director)
        {
            mDirector = director;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Update(GameTime gametime)
        {
            Move();
        }
    }
}
