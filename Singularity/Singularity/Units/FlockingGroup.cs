using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    
    [DataContract]
    public class FlockingGroup : AFlocking
    {

        /// <summary>
        /// MilitaryPathfinders enables pathfinding using jump point search or line of sight.
        /// </summary>
        protected static FreeMovingPathfinder mPathfinder = new FreeMovingPathfinder();

        [DataMember]
        public Vector2 mTargetPosition;

        [DataMember]
        protected Vector2 mUltimateTarget;

        /// <summary>
        /// Path the unit must take to get to the target position without colliding with obstacles.
        /// </summary>
        [DataMember]
        protected Stack<Vector2> mPath = new Stack<Vector2>();

        [DataMember]
        private List<IFlocking> mUnits;
        
        [DataMember]
        private int? mSuperiorFlockingId = null;
        [DataMember]
        public int FlockingId { get; private set; }


        public Vector2 CohesionRaw { get; private set; }
        public Vector2 SeperationRaw { get; set; }
        public float ActualSpeed { get; private set; }

        [DataMember]
        public bool Moved { get; private set; }

        public Map.Map Map { get; private set; }
        
        [DataMember]
        public int Counter { get; private set; } // todo: use counter in FlockingGroup to look for other units 

        /// <summary>
        /// Stores the path the unit is taking so that it can be drawn for debugging.
        /// </summary>
        [DataMember]
        protected Vector2[] mDebugPath;


        public FlockingGroup(ref Director director, ref Map.Map map) : base(ref director, Optional<FlockingGroup>.Of(null))
        {
            mUnits = new List<IFlocking>();
            Velocity = Vector2.Zero;
            CohesionRaw = Vector2.Zero;
            SeperationRaw = Vector2.Zero;
            
            // mMap = mDirector.GetStoryManager.Level.Map;
            Debug.WriteLineIf(map == null, "Map is null for some reason.");
            Map = map;

            FlockingId = mDirector.GetIdGenerator.NextId();

            if (mGroup.IsPresent())
                mSuperiorFlockingId = mGroup.Get().FlockingId;
        }

        public override void ReloadContent(ref Director director)
        {
            base.ReloadContent(ref director);
            Map = mDirector.GetStoryManager.Level.Map;
            foreach (var u in mUnits) u.ReloadContent(ref director);
        }

        public override void Move()
        {

            // todo: now get a velocity to the current target.
            // also: todo: actively let units avoid obstacles. (in progress)
            // (lookup at precomputed map velocities).
            
            // if we don't need to move, why bother recalculating all the values?
            if (!Moved) return;
            
            SeperationRaw = Vector2.Zero;
            

            foreach (var unit in mUnits)
            {
                SeperationRaw += unit.AbsolutePosition;
            }
            
            AbsolutePosition = SeperationRaw / mUnits.Count;
            CohesionRaw = AbsolutePosition;
            // ActualSpeed = mUnits.Any(u => u.Speed > ActualSpeed);


            if (Geometry.Length(mTargetPosition - AbsolutePosition) < mUnits.Count * Speed)
            {
                Moved = false;
            }

            var diff = mTargetPosition - AbsolutePosition;
            var dist = (float) Geometry.Length(diff);

            ActualSpeed = (dist > 100 ? 1 : dist / 100) * Speed;

            Velocity = Vector2.Normalize(diff) * ActualSpeed;

            if (dist < 50 && mPath.Count > 0)
            {
                mTargetPosition = mPath.Pop();
            }


            Debug.WriteLine("vel:" + Velocity + ", pos: " + AbsolutePosition + ", Coh: " + CohesionRaw + ", Sep: " + SeperationRaw);

            // setting variables used from the AFlocking parts
            mUnits.ForEach(u => u.Move());

            /*   todo: implement / fix

            Counter++;
            if (Counter % 20 != 0) return;
            Counter = 0;

            var colliders = mDirector.GetMilitaryManager.GetAdjecentUnits(AbsolutePosition);
            mUnits.ForEach(u => u.TakeCareof(colliders)); // todo: implement differently ... (precomputing time-velocity-based-lookup table)

            // */
        }


        internal void FindPath(Vector2 target)
        {

            if (target == mUltimateTarget) return;


            SeperationRaw = Vector2.Zero;

            if (mUnits.Count == 1)
            {
                AbsolutePosition = mUnits[0].AbsolutePosition;
                Debug.WriteLine("pos2: " + mUnits[0].AbsolutePosition);
            }
            else
            {
                foreach (var unit in mUnits)
                {
                    SeperationRaw += unit.AbsolutePosition;
                }
                AbsolutePosition = SeperationRaw / mUnits.Count;
                Debug.WriteLine("pos2: " + AbsolutePosition + ", count: " + mUnits.Count);
            }

            Moved = true;
            mUltimateTarget = target;
            Debug.WriteLine("Starting path finding at: " + AbsolutePosition + " for " + FlockingId);
            Debug.WriteLine("Target: " + target.X + ", " + target.Y);

            var map = Map;
            mPath = new Stack<Vector2>();
            mPath = mPathfinder.FindPath(AbsolutePosition,
                target,
                ref map);
            
            mDebugPath = mPath.ToArray();

            Debug.WriteLine("Path is " + mPath.Count + " long.");

            mTargetPosition = mPath.Pop(); // directly getting first goal-part.
        }


        /*
        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move.</param>
        protected void MoveToTarget(Vector2 target)
        {
            Velocity = new Vector2(target.X - AbsolutePosition.X, target.Y - AbsolutePosition.Y);
        }
        */

        
        public void MakePartOf(FlockingGroup group)
        {
            mGroup = Optional<FlockingGroup>.Of(group);
            mSuperiorFlockingId = group.FlockingId;
        }


        public override void Update(GameTime t)
        {
            base.Update(t); // this effectively only calls 'Move()'.
            // mUnits.ForEach(u => u.Update(t));
        }

        internal void AssignUnit(IFlocking unit)
        {
            Debug.WriteLine("Unit got added to " + FlockingId);
            mUnits.Add(unit);
            unit.AddGroup(this);
            if (Speed == 0)
            {
                Speed = unit.Speed;
            }
            else
            {
                if (unit.Speed >= Speed) return;
                Speed = unit.Speed;
            }
        }

        public bool RemoveUnit(IFlocking unit)
        {
            return mUnits.Remove(unit);
        }
        
        public List<IFlocking> GetUnits()
        {
            return mUnits;
        }

        public int Count => mUnits.Count;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Flocking groups do not need to be seperately drawn ... yet.
            // mUnits.ForEach(u => u.Draw(spriteBatch));


            if (!GlobalVariables.DebugState || mDebugPath == null) return;
            for (var i = 0; i < mDebugPath.Length - 1; i++)
            {
                spriteBatch.DrawLine(mDebugPath[i], mDebugPath[i + 1], Color.Orange);
            }
        }

        public override bool Die()
        {
            // The FlockingGroup cannot die as a whole, and is not dead if there's still units left in it.
            return mUnits.Count == 0 && mDirector.GetMilitaryManager.Kill(this);
        }


        public void Merge(FlockingGroup other)
        {
            // todo: merge two FlockingGroups
        }

        public FlockingGroup Split()
        {
            // todo split this FlockingGroup based on Distance / speed.
            return null;
        }

        public void AdJust()
        {
            // todo: Tree-like splitting / merging if sensible.
        }

        internal void Reset()
        {
            mUnits = new List<IFlocking>();
        }

        public bool NearTarget()
        {
            return mPath.Count == 0 && !Moved;
        }

        public bool Kill(IFlocking unit)
        {
            return mUnits.Remove(unit);
        }
    }
}
