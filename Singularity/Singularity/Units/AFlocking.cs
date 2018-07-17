using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    public interface IFlocking : ICollider
    {

        /// <summary>
        /// This is simply the velocity the FlockingUnit has.
        /// </summary>
        [DataMember]
        Vector2 Velocity { get; set; }

        [DataMember]
        int Speed { get; }
        
        [DataMember]
        int FlockingId { get; }

        void Move();
        void ReloadContent(ref Director director);
        void AddGroup(FlockingGroup group);
    }



    [DataContract]
    public abstract class AFlocking : ADie, IFlocking
    {
        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public Vector2 Velocity { get; set; }
        [DataMember]
        public int Speed { get; set; }

        [DataMember]
        public bool Moved { get; set; }

        [DataMember]
        protected Optional<FlockingGroup> mGroup;
        [DataMember]
        public int FlockingId { get; set; }


        private List<IFlocking> mOtherUnits = new List<IFlocking>();

        protected AFlocking(ref Director director, Optional<FlockingGroup> group) : base(ref director)
        {
            Id = director.GetIdGenerator.NextId(); // id for the specific unit.
            mGroup = group;
            FlockingId = -1;
            // FlockingId = mGroup.IsPresent() ? mGroup.Get().FlockingId : mDirector.GetIdGenerator.NextId();
        }

        public abstract void SetAbsBounds();

        public virtual void Move()
        {
            /*
            AbsolutePosition += Velocity;
            SetAbsBounds();
            if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
            {
                AbsolutePosition -= Velocity;
                SetAbsBounds();
            } // */
            // weird shit happening. Try fully another day.
            AbsolutePosition += Velocity;
            SetAbsBounds();
            if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
            {
                Debug.WriteLine("Could not move1 " + Id);
                AbsolutePosition -= new Vector2(Velocity.X, 0);
                SetAbsBounds();
                if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
                {
                    Debug.WriteLine("Could not move2 " + Id);
                    AbsolutePosition += new Vector2(Velocity.X, -Velocity.Y);
                    SetAbsBounds();
                    if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
                    {
                        Debug.WriteLine("Could not move at all. " + Id);
                        AbsolutePosition -= new Vector2(0, Velocity.Y);
                        SetAbsBounds();
                    }
                }
            }
            // */
            
            if (mGroup.Get().Count == 1)
            {
                Velocity = mGroup.Get().Velocity;
                Debug.WriteLine("unit: " + AbsolutePosition + ", vel: " + Velocity);
                return;
            }


            var diff = mGroup.Get().mTargetPosition - AbsolutePosition;
            var dist = (float)Geometry.Length(diff);
            var actualSpeed = (dist > 100 ? 1 : dist / 100) * Speed;


            // calculate the forces:
            // - Alignment
            //     The Velocity of the FlockingGroup. This is part for the PathFinding.
            // - Cohesion
            //     Steering towards the Center of mass relative to other members of the FlockingGroup (has a Center/AbsolutePosition for that matter)
            // - Seperation
            //     The steering away from other members of the FlockingGroup. this is also precomputed there.
            var align = Vector2.Normalize(mGroup.Get().Velocity);
            var cohes = Vector2.Normalize(mGroup.Get().CohesionRaw - AbsolutePosition);
            // var seper = Vector2.Normalize(mGroup.Get().SeperationRaw - AbsolutePosition * mGroup.Get().UnitCount()) * -1;
            // var seper = Vector2.Normalize(new Vector2(
                // mGroup.Get().GetUnits().Sum(u => u.AbsolutePosition.X - AbsolutePosition.X),
                // mGroup.Get().GetUnits().Sum(u => u.AbsolutePosition.Y - AbsolutePosition.Y))) * -1;
            var close = mGroup.Get()
                .GetUnits()
                .FindAll(u => Geometry.Length(u.AbsolutePosition - AbsolutePosition) < 90 && !u.Equals(this))
                .Select(f => f.AbsolutePosition).ToList();
                // .Aggregate((a, b) => a + b);
            var seper = close.Count > 0 ? Vector2.Normalize(close.Aggregate((a, b) => a + b) - AbsolutePosition * close.Count) * -1 : Vector2.Zero;
            
            var goal = Vector2.Normalize(diff);

            Velocity = Vector2.Normalize(goal * 0.2f + Velocity * 2 + align + cohes * 0.75f + seper * 1.5f) * actualSpeed;
        
            // Debug.WriteLine("Freemoving: " + mGroup.Get().Velocity + ", " + mGroup.Get().CohesionRaw);
            // Debug.WriteLine("unit: " + goal + ", " + align + ", " + cohes + ", " + seper + ", " + Velocity);
            
            // the new velocity will actually be used only next Update in the beginning.
        
        }
        
        

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Update(GameTime gametime)
        {
            Move();
        }

        public void AddGroup(FlockingGroup group)
        {
            if (mGroup.IsPresent())
            {
                mGroup.Get().RemoveUnit(this);
            }
            mGroup = Optional<FlockingGroup>.Of(group);
        }



        #region ICollider-stuff.

        


        public Vector2 RelativePosition { get; set; }
        public Vector2 RelativeSize { get; set; }

        [DataMember]
        public bool[,] ColliderGrid { get; protected set; }
        [DataMember]
        public Rectangle AbsBounds { get; protected set; }
        [DataMember]
        public Vector2 Center { get; protected set; }

        [DataMember]
        public int Health { get; protected set; }
        [DataMember]
        public bool Friendly { get; protected set; }
        public abstract void MakeDamage(int damage);
        [DataMember]
        public int Id { get; }


        #endregion
    }



    // more Links:
    // http://www.kfish.org/boids/pseudocode.html
    // https://3carrotsonastick.wordpress.com/2012/11/01/boids-in-c-xna/
}
