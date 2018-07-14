using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Manager;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{
    
    public class FlockingGroup : AFlocking
    {

        /// <summary>
        /// MilitaryPathfinders enables pathfinding using jump point search or line of sight.
        /// </summary>
        protected FreeMovingPathfinder mPathfinder;

        protected Vector2 mTargetPosition;

        protected Stack<Vector2> mPath;

        private List<IFlocking> mUnits;
        
        private int? mSuperiorFlockingId = null;
        public int FlockingId { get; private set; }

        public Vector2 CohesionRaw { get; private set; }
        public Vector2 SeperationRaw { get; private set; }
        public float ActualSpeed { get; private set; }

        public bool Moved { get; private set; }

        private Map.Map mMap;



        public FlockingGroup(ref Director director, Map.Map map) : base(ref director, Optional<FlockingGroup>.Of(null))
        {
            Velocity = Vector2.Zero;
            CohesionRaw = Vector2.Zero;
            SeperationRaw = Vector2.Zero;

            mMap = map;
            mPathfinder = new FreeMovingPathfinder();

            if (mGroup.IsPresent())
                mSuperiorFlockingId = mGroup.Get().FlockingId;
        }

        public override void ReloadContent(ref Director director)
        {
            base.ReloadContent(ref director);
            mPathfinder = new FreeMovingPathfinder();
        }

        public override void Move()
        {

            // calculate path to target position


            /*
            else if (mIsMoving)
            {
                if (!HasReachedWaypoint())
                {
                    MoveToTarget(mPath.Peek());
                }

                else
                {
                    mPath.Pop();
                    MoveToTarget(mPath.Peek());
                }
            }

            */


            AbsolutePosition = Vector2.Zero;
            Velocity = Vector2.Zero;
            SeperationRaw = Vector2.Zero;


            foreach (var unit in mUnits)
            {
                AbsolutePosition += unit.AbsolutePosition;
            }

            SeperationRaw = new Vector2(AbsolutePosition.X, AbsolutePosition.Y);
            AbsolutePosition = AbsolutePosition / mUnits.Count;
            CohesionRaw = AbsolutePosition;
            // ActualSpeed = mUnits.Any(u => u.Speed > ActualSpeed);


            // setting variables used from the AFlocking parts
            mUnits.ForEach(u => u.Move());

        }


        

        /// <summary>
        /// Checks whether the next waypoint has been reached.
        /// </summary>
        /// <returns></returns>
        protected bool HasReachedWaypoint()
        {
            // Debug.WriteLine("Waypoint reached.");
            // Debug.WriteLine("Next waypoint: " + mPath.Peek());

            return Geometry.Length(mPath.Peek() - AbsolutePosition) < Speed;

            // If the position is within 8 pixels of the waypoint, (i.e. it will overshoot the waypoint if it moves
            // for one more update, do the following
        }




        protected void FindPath(Vector2 target)
        {

            Moved = true;
            Debug.WriteLine("Starting path finding at: " + Center.X + ", " + Center.Y);
            Debug.WriteLine("Target: " + mTargetPosition.X + ", " + mTargetPosition.Y);

            mPath = new Stack<Vector2>();
            mPath = mPathfinder.FindPath(AbsolutePosition,
                target,
                ref mMap);

            if (GlobalVariables.DebugState)
            {
                mDebugPath = mPath.ToArray();
            }

            mZoomSnapshot = mCamera.GetZoom();
        }



        /// <summary>
        /// Calculates the direction the unit should be moving and moves it into that direction.
        /// </summary>
        /// <param name="target">The target to which to move.</param>
        protected void MoveToTarget(Vector2 target)
        {
            Velocity = new Vector2(target.X - Center.X, target.Y - Center.Y);
        }



        public void MakePartOf(FlockingGroup group)
        {
            mGroup = Optional<FlockingGroup>.Of(group);
            mSuperiorFlockingId = group.FlockingId;
        }


        public override void Update(GameTime t)
        {
            base.Update(t);
            // mUnits.ForEach(u => u.Update(t));
        }

        public void AssignUnit(IFlocking unit)
        {
            mUnits.Add(unit);
        }

        public bool RemoveUnit(IFlocking unit)
        {
            return mUnits.Remove(unit);
        }
        
        public List<IFlocking> GetUnits()
        {
            return mUnits;
        }

        public int UnitCount()
        {
            return mUnits.Count;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Flocking groups do not need to be seperately drawn ... yet.
            // mUnits.ForEach(u => u.Draw(spriteBatch));
        }

        public bool Die()
        {
            // The FlockingGroup cannot die as a whole, and is not dead if there's still units left in it.
            return mUnits.Count == 0;
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
    }
}
