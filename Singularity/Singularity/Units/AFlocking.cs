using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Map.Properties;
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
        int Speed { get; set; }

        [DataMember]
        int FlockingId { get; set; }

        [DataMember]
        bool Selected { get; set; }

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

        protected Optional<FlockingGroup> mGroup;
        [DataMember]
        public int FlockingId { get; set; }

        public bool Selected { get; set; }


        protected AFlocking(ref Director director, Optional<FlockingGroup> group) : base(ref director)
        {
            Id = director.GetIdGenerator.NextId(); // id for the specific unit.
            mGroup = group;
            // FlockingId = -1;
            Velocity = Vector2.Zero;
            FlockingId = mGroup.IsPresent() ? mGroup.Get().FlockingId : -1;
        }

        public abstract void SetAbsBounds();

        public virtual void Move()
        {
            if (!mGroup.IsPresent() || mGroup.Get().GetUnits().Count == 0 || !Moved)
            {
                return;
            }

            AbsolutePosition += Velocity; // todo: fix
            /*
            AbsolutePosition += Velocity;
            SetAbsBounds();
            if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
            {
                AbsolutePosition -= Velocity;
                SetAbsBounds();
            } // */
            // weird shit happening. Try fully another day.

            /* inefficient. todo: improve
            // Debug.WriteLine("Abs before setting: " + AbsolutePosition + ", vel: " + Velocity);
            AbsolutePosition += Velocity;
            // Debug.WriteLine("Abs after setting: " + AbsolutePosition);
            SetAbsBounds();
            if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
            {
                // Debug.WriteLine("Could not move1 " + Id);
                AbsolutePosition -= new Vector2(Velocity.X, 0);
                SetAbsBounds();
                if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
                {
                    // Debug.WriteLine("Could not move2 " + Id);
                    AbsolutePosition += new Vector2(Velocity.X, -Velocity.Y);
                    SetAbsBounds();
                    if (!mDirector.GetStoryManager.Level.Map.GetCollisionMap().CanPlaceCollider(this))
                    {
                        // Debug.WriteLine("Could not move at all. " + Id);
                        AbsolutePosition -= new Vector2(0, Velocity.Y);
                        SetAbsBounds();
                    }
                }
            }
            // */

            if (mGroup.Get().Count == 1)
            {
                Velocity = mGroup.Get().Velocity;
                // Debug.WriteLine("unit: " + AbsolutePosition + ", vel: " + Velocity);
                return;
            }


            var diff = mGroup.Get().mTargetPosition - AbsolutePosition;
            var dist = (float)Geometry.Length(diff);
            var actualSpeed = (dist > 100 ? 1 : (dist / 100)) * Speed;


            // calculate the forces:
            // - Alignment
            //     The Velocity of the FlockingGroup. This is part for the PathFinding.
            // - Cohesion
            //     Steering towards the Center of mass relative to other members of the FlockingGroup (has a Center/AbsolutePosition for that matter)
            // - Seperation
            //     The steering away from other members of the FlockingGroup. this is also precomputed there.

            var x = AbsBounds.X / MapConstants.GridWidth;
            var y = AbsBounds.Y / MapConstants.GridHeight;
            var align = mGroup.Get().HeatMap.IsPresent() ? mGroup.Get().HeatMap.Get()[x, y] : Vector2.Normalize(mGroup.Get().Velocity);
            if (align.Length() > 1)
            {
                align = Vector2.Normalize(align);
            }
            var cohes = (mGroup.Get().CohesionRaw - AbsolutePosition).Length() > 0.5 ? Vector2.Normalize(mGroup.Get().CohesionRaw - AbsolutePosition) : Vector2.Zero;
            // var seper = Vector2.Normalize(mGroup.Get().SeperationRaw - AbsolutePosition * mGroup.Get().UnitCount()) * -1;
            // var seper = Vector2.Normalize(new Vector2(
            // mGroup.Get().GetUnits().Sum(u => u.AbsolutePosition.X - AbsolutePosition.X),
            // mGroup.Get().GetUnits().Sum(u => u.AbsolutePosition.Y - AbsolutePosition.Y))) * -1;
            if (mDirector == null)
            {
                return;
            }
            var close = mDirector.GetMilitaryManager.GetAdjecentUnits(AbsolutePosition)
                                 .FindAll(u => Geometry.Length(u.AbsolutePosition - AbsolutePosition) < 30 + u.AbsoluteSize.X * 1.5f && !u.Equals(this))
                                 .ToList();
                // .Aggregate((a, b) => a + b);
            var seper = close.Count > 0 ? (close.Select(f => f.AbsolutePosition).Aggregate((a, b) => a + b) - AbsolutePosition * close.Count) * -1 : Vector2.Zero;

            if (seper.Length() > 1)
            {
                seper = Vector2.Normalize(seper);
            }

            var goal = Vector2.Normalize(diff);

            Velocity = Vector2.Normalize(goal * 0.5f + Velocity * 2 + align + cohes * 0.75f + seper * 2f) * actualSpeed;

            var stoppedFlocking = close.Where(f => !f.Moved && f is FreeMovingUnit && (f as FreeMovingUnit).FlockingId == FlockingId).ToList();


            if (stoppedFlocking.Count > 0 && stoppedFlocking.Select(f => (f.AbsolutePosition - AbsolutePosition).Length()).Min() < 70 || dist < 50)
            {
                Moved = false;
            }

            // Debug.WriteLine("Group: " + mGroup.Get().Velocity + ", " + mGroup.Get().CohesionRaw);
            // Debug.WriteLine("unit: " + AbsolutePosition + ", " + align + ", " + cohes + ", " + seper + ", " + Velocity);

            // the new velocity will actually be used only next Update in the beginning.
        }



        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Update(GameTime gametime)
        {
            Move();
        }

        public void AddGroup(FlockingGroup group)
        {
            if (mGroup == null)
            {
                mGroup = Optional<FlockingGroup>.Of(null);
            }
            if (mGroup.IsPresent())
            {
                mGroup.Get().RemoveUnit(this);
            }
            mGroup = Optional<FlockingGroup>.Of(group);
            FlockingId = group.FlockingId;
        }



        #region ICollider-stuff.




        public Vector2 RelativePosition { get; set; }
        public Vector2 RelativeSize { get; set; }

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
        public int Id { get; protected set; }


        #endregion
    }



    // more Links:
    // http://www.kfish.org/boids/pseudocode.html
    // https://3carrotsonastick.wordpress.com/2012/11/01/boids-in-c-xna/
}
