using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map.Properties;
using Singularity.Property;
using Singularity.Utils;

namespace Singularity.Units
{

    public class FlockingGroup : AFlocking
    {

        /// <summary>
        /// MilitaryPathfinders enables pathfinding using jump point search or line of sight.
        /// </summary>
        protected static FreeMovingPathfinder mPathfinder = new FreeMovingPathfinder();

        public Vector2 mTargetPosition;

        protected Vector2 mUltimateTarget;

        /// <summary>
        /// Path the unit must take to get to the target position without colliding with obstacles.
        /// </summary>
        protected Stack<Vector2> mPath = new Stack<Vector2>();

        private List<IFlocking> mUnits;

        private int? mSuperiorFlockingId;


        public Vector2 CohesionRaw { get; private set; }
        public Vector2 SeperationRaw { get; set; }
        public float ActualSpeed { get; private set; }

        public int Count => mUnits.Count;
        public Optional<Vector2[,]> HeatMap { get; protected set; }


        public Map.Map Map { get; private set; }

        public int Counter { get; private set; } // todo: use counter in FlockingGroup to look for other units

        /// <summary>
        /// Stores the path the unit is taking so that it can be drawn for debugging.
        /// </summary>
        protected Vector2[] mDebugPath;


        public FlockingGroup(ref Director director, ref Map.Map map) : base(ref director, Optional<FlockingGroup>.Of(null))
        {
            mUnits = new List<IFlocking>();
            Velocity = Vector2.Zero;
            CohesionRaw = Vector2.Zero;
            SeperationRaw = Vector2.Zero;

            // mMap = mDirector.GetStoryManager.Level.Map;
            Map = map;

            FlockingId = mDirector.GetIdGenerator.NextId();

            if (mGroup.IsPresent())
            {
                mSuperiorFlockingId = mGroup.Get().FlockingId;
            }

            HeatMap = Optional<Vector2[,]>.Of(null);
        }

        internal void Circle()
        {
            mTargetPosition = new Vector2(MapConstants.MapHeight, MapConstants.MapWidth) * 0.5f;
            CreateCircleHeatMap();
            mUnits.ForEach(u => u.Moved = true);
            Moved = true;
        }


        public override void ReloadContent(ref Director director)
        {
            mDirector = director;
            base.ReloadContent(ref director);
            Map = mDirector.GetStoryManager.Level.Map;
            foreach (var u in mUnits)
            {
                u.ReloadContent(ref director);
            }
        }

        // public override Vector2 RelativePosition { get; set; }   // does not really make sense but AFloking : ICollider ...
        // public override Vector2 RelativeSize { get; set; }       // does not really make sense but AFloking : ICollider ...

        // public override bool[,] ColliderGrid { get; } // does not really make sense but AFloking : ICollider ...
        // public override Rectangle AbsBounds { get; }  // does not really make sense but AFloking : ICollider ...
        // public override Vector2 Center { get; }       // does not really make sense but AFloking : ICollider ...

        public override void SetAbsBounds()
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {

            // todo: now get a velocity to the current target.
            // also: todo: actively let units avoid obstacles. (in progress)
            // (lookup at precomputed map velocities).

            // if we don't need to move, why bother recalculating all the values?
            if (!Moved)
            {
                return;
            }

            SeperationRaw = Vector2.Zero;


            foreach (var unit in mUnits)
            {
                SeperationRaw += unit.AbsolutePosition;
            }

            AbsolutePosition = SeperationRaw / mUnits.Count;
            CohesionRaw = AbsolutePosition;
            // ActualSpeed = mUnits.Any(u => u.Speed > ActualSpeed);

            var diff = mTargetPosition - AbsolutePosition;
            var dist = (float) Geometry.Length(diff);

            ActualSpeed = (dist > 100 ? 1 : dist / 100) * Speed;

            Velocity = Vector2.Normalize(diff) * ActualSpeed;

            if (dist < 50 && mPath.Count > 0)
            {
                mTargetPosition = mPath.Pop();
            }

            // setting variables used from the AFlocking parts
            mUnits.ForEach(u => u.Move());

            Moved = mUnits.Exists(u => u.Moved);

            /*

            if (mGoalCounter > 3)
            {
                // Moved = false;
                // mUnits.ForEach(u => u.Moved = false);
                mGoalCounter = 0;
            }
            // */

            /*   todo: implement / fix

            Counter++;
            if (Counter % 20 != 0) return;
            Counter = 0;

            var colliders = mDirector.GetMilitaryManager.GetAdjecentUnits(AbsolutePosition);
            mUnits.ForEach(u => u.TakeCareof(colliders)); // todo: implement differently ... (precomputing time-velocity-based-lookup table)

            // */
        }


        private void CreateCircleHeatMap()
        {

            var temp = new Vector2[mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0), mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1)];

            // new Vector2(MapConstants.MapHeight, MapConstants.MapWidth) * 0.5f;
            Vector2 center = new Vector2(mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0) / 2, mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1) / 2);

            for (var i = 0; i < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0); i++)
            {
                for (var j = 0; j < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1); j++)
                {
                    var centerDir = center - new Vector2(i, j);
                    var dist = centerDir.Length();
                    var correction = Vector2.Zero;

                    if (dist < 500 || dist > 900)
                    {
                        correction = Vector2.Normalize(centerDir * (dist - 700));
                    }

                    // new ~ old
                    // |      |
                    // v      v
                    //
                    // x   =  y
                    // y   = -x
                    //
                    // exactly -90° Rotation

                    temp[i, j] = Vector2.Normalize(Vector2.Normalize(new Vector2(centerDir.Y, - centerDir.X)) * 0.4f + correction * 0.6f);
                }
            }
            HeatMap = Optional<Vector2[,]>.Of(temp);
        }


        private void CreateHeatmap(Pair<int, int> node, Queue<Pair<int, int>> queue)
        {
            int[,] heatMap = new int[mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0), mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1)];

            for (var i = 0; i < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0); i++)
            {
                for (var j = 0; j < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1); j++)
                {
                    heatMap[i, j] = int.MaxValue;
                }
            }

            while (true)
            {

                List<Pair<int, int>> neighbours = new List<Pair<int, int>>
                {
                    new Pair<int, int>(node.GetFirst() + 1, node.GetSecond()),
                    new Pair<int, int>(node.GetFirst(), node.GetSecond() + 1),
                    new Pair<int, int>(node.GetFirst() - 1, node.GetSecond()),
                    new Pair<int, int>(node.GetFirst(), node.GetSecond() - 1)
                };
                var val = heatMap[node.GetFirst(), node.GetSecond()];
                foreach (Pair<int, int> n in neighbours)
                {
                    if (n.GetFirst() >= mDirector.GetStoryManager.Level.Map.GetCollisionMap()
                            .GetCollisionMap()
                            .GetLength(0) ||
                        n.GetSecond() >= mDirector.GetStoryManager.Level.Map.GetCollisionMap()
                            .GetCollisionMap()
                            .GetLength(1) ||
                        n.GetFirst() < 0 || n.GetSecond() < 0)
                    {
                        continue;
                    }
                    if (!(heatMap[n.GetFirst(), n.GetSecond()] >= int.MaxValue - 2) || !mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetWalkabilityGrid().IsWalkableAt(n.GetFirst(), n.GetSecond()))
                    {
                        continue;
                    }

                    heatMap[n.GetFirst(), n.GetSecond()] = val + 1;
                    queue.Enqueue(n);
                }
                if (queue.Count == 0)
                {
                    break;
                }

                var next = queue.Dequeue();
                node = next;
            }

            var temp = new Vector2[mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0), mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1)];


            for (var i = 0; i < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0); i++)
            {
                for (var j = 0; j < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1); j++)
                {
                    int x = 0, z = 0;
                    if (i + 1 < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(0) &&
                        i - 1 > 0)
                    {
                        x =  heatMap[i - 1, j] - heatMap[i + 1, j];
                    }

                    if (j + 1 < mDirector.GetStoryManager.Level.Map.GetCollisionMap().GetCollisionMap().GetLength(1) &&
                        j - 1 > 0)
                    {
                        z = heatMap[i, j - 1] - heatMap[i, j + 1];
                    }

                    temp[i, j] = new Vector2(x, z);
                }
            }
            HeatMap = Optional<Vector2[,]>.Of(temp);
        }


        internal void FindPath(Vector2 target)
        {

            if (target == mUltimateTarget || mUnits.Count == 0)
            {
                return;
            }

            mUltimateTarget = target;

            if (mUnits.Count > 30)
            {
                // find path with heatmap.
                var x = (int) target.X / MapConstants.GridWidth;
                var y = (int) target.Y / MapConstants.GridHeight;
                CreateHeatmap(new Pair<int, int>(x, y), new Queue<Pair<int, int>>());
                mTargetPosition = target;
                mDirector.GetMilitaryManager.EnsureIncluded(this);
                Moved = true;
                mUnits.ForEach(u => u.Moved = true);
                return;
            }

            SeperationRaw = Vector2.Zero;

            if (mUnits.Count == 1)
            {
                AbsolutePosition = mUnits[0].AbsolutePosition;
            }
            else
            {
                foreach (var unit in mUnits)
                {
                    SeperationRaw += unit.AbsolutePosition;
                }
                AbsolutePosition = SeperationRaw / mUnits.Count;
            }


            var map = Map;
            mPath = new Stack<Vector2>();
            mPath = mPathfinder.FindPath(AbsolutePosition,
                target,
                ref map);

            // test if the Path is valid to begin with
            if (mPath.Count == 0)
            {
                return;
            }

            Moved = true;
            mUnits.ForEach(u => u.Moved = true);

            mDebugPath = mPath.ToArray();

            mTargetPosition = mPath.Pop(); // directly getting first goal-part.
            mDirector.GetMilitaryManager.EnsureIncluded(this);
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

        // public override int Health { get; }           // does not really make sense but AFloking : ICollider ...
        // public override bool Friendly { get; }        // does not really make sense but AFloking : ICollider ...
        public override void MakeDamage(int damage)   // does not really make sense but AFloking : ICollider ...
        {
            throw new NotImplementedException();
        }

        internal void AssignUnit(IFlocking unit)
        {
            mUnits.Add(unit);
            unit.AddGroup(this);
            if (Speed == 0)
            {
                Speed = unit.Speed;
            }
            else
            {
                if (unit.Speed >= Speed)
                {
                    return;
                }

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Flocking groups do not need to be seperately drawn ... yet.
            // mUnits.ForEach(u => u.Draw(spriteBatch));


            if (!GlobalVariables.DebugState || mDebugPath == null)
            {
                return;
            }

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

        public bool AllSelected()
        {
            return mUnits.TrueForAll(u => u.Selected);
        }
    }
}
