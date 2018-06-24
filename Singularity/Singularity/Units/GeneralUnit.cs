using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.DistributionManager;
using Singularity.Platform;
using Singularity.Graph;
using Singularity.Graph.Paths;
using Singularity.Libraries;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;
using Action = System.Action;

namespace Singularity.Units
{
    [DataContract()]
    public class GeneralUnit : IUnit, IUpdate, IDraw, ISpatial
    {
        [DataMember()]
        public int Id { get; }
        [DataMember()]
        private DistributionManager.DistributionManager mDistrManager;
        [DataMember()]
        public Optional<Resource> Carrying { get; set; }
        [DataMember()]
        private Queue<Vector2> mPathQueue; // the queue of platform center locations
        [DataMember()]
        private Queue<INode> mNodeQueue;
        [DataMember()]
        internal JobType Job { get; set; } = JobType.Idle;

        //These are the assigned task and a flag, wether the unit is done with it.
        [DataMember()]
        private Task mAssignedTask;

        //Represent the current workstate.
        [DataMember()]
        private bool mDone;
        [DataMember()]
        private bool mAssigned;

        [DataMember()]
        private IPlatformAction mAssignedAction;

        [DataMember()]
        public INode CurrentNode { get; private set; }

        // TODO: also use the size for the units representation since we someday need to draw rectangles over units (bounding box)

        [DataMember()]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember()]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember()]
        public Vector2 RelativePosition { get; set; }
        [DataMember()]
        public Vector2 RelativeSize { get; set; }
        [DataMember()]
        private readonly PathManager mPathManager;

        /// <summary>
        /// whether the unit is moving or currently standing still,
        /// this is used so the unit can ask for a new path if it
        /// doesn't move
        /// </summary>
        [DataMember()]
        private bool mIsMoving;

        /// <summary>
        /// The node the unit started from. Changes when the unit reaches its destination (to the destination).
        /// </summary>
        [DataMember()]
        private INode mCurrentNode;

        /// <summary>
        /// The node the unit moves to. Null if the unit doesn't move anywhere
        /// </summary>
        [DataMember()]
        private INode mDestination;

        /// <summary>
        /// The speed the unit moves at.
        /// </summary>
        [DataMember()]
        private const float Speed = 3f;

        public GeneralUnit(PlatformBlank platform, PathManager pathManager, DistributionManager.DistributionManager distrManager)
        {
            mDestination = null;

            CurrentNode = platform;

            AbsolutePosition = ((IRevealing) platform).Center; // TODO figure out how to search platform by ID and get its position
            mPathQueue = new Queue<Vector2>();
            mNodeQueue = new Queue<INode>();

            mIsMoving = false;
            mPathManager = pathManager;
            mDistrManager = distrManager;
            distrManager.Register(this);
            mDone = true;
        }

        /// <summary>
        /// Used to change the job. Is usually only called if the player wants more/less Units working in a certain job.
        /// </summary>
        /// <param name="job">The job the unit should do.</param>
        public void ChangeJob(JobType job)
        {
            if (Job == JobType.Production && mAssigned)
            {
                ((PlatformBlank)mDestination).UnAssignUnits(this);
                mAssigned = false;
            }
            Job = job;
        }

        /// <summary>
        /// In the Idle case the unit will just get a Target to move to and do so.
        /// In the Production case the unit will go to the producing Platform and call its produce method.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            switch (Job)
            {
                case JobType.Idle:
                    if (!mIsMoving && mDone)
                    {
                        mDone = false;
                        //Care!!! DO NOT UNDER ANY CIRCUMSTANCES USE THIS PLACEHOLDER
                        IPlatformAction action = new ProduceMineResource(null, null);
                        mAssignedTask = mDistrManager.RequestNewTask(this, Job, Optional<IPlatformAction>.Of(action));
                        mDestination = mAssignedTask.End;
                        
                    }
                    break;

                case JobType.Production:
                    //Get a Task
                    if (!mIsMoving && mDone)
                    {
                        mDone = false;
                        //Care!!! DO NOT UNDER ANY CIRCUMSTANCES USE THIS PLACEHOLDER
                        IPlatformAction action = new ProduceMineResource(null, null);
                        mAssignedTask = mDistrManager.RequestNewTask(this, Job, Optional<IPlatformAction>.Of(action));
                        mDestination = mAssignedTask.End;
                        mAssignedAction = mAssignedTask.Action;
                    }
                    //You arrived at your destination and you now want to work.
                    else if(!mIsMoving && !mDone && mCurrentNode == mDestination)
                    {
                        if (!mAssigned)
                        {
                            ((PlatformBlank)mDestination).AssignUnits(this, Job);
                            mAssigned = true;
                        }
                        
                        //Check the passed time
                    }
                    break;
            }

            //The movement and everything revolving around it happens here!
            RegulateMovement();

            //We arrived at the target, so its now time to get another job
            if (mNodeQueue.Count == 0 && Job == JobType.Idle)
            {
                mDone = true;
            }

        }

        /*========================================================================================================================
        ====================================Everything revolving around Movement is down here=====================================
        ==========================================================================================================================*/

        /// <summary>
        /// Moves the unit to the target position by its given speed.
        /// </summary>
        /// <param name="targetPosition">The target the unit should move towards</param>
        /// <returns></returns>
        private void Move(Vector2 targetPosition)
        {

            mIsMoving = true;

            var movementVector = Geometry.NormalizeVector(new Vector2(targetPosition.X - AbsolutePosition.X, targetPosition.Y - AbsolutePosition.Y));

            AbsolutePosition = new Vector2(AbsolutePosition.X + movementVector.X * Speed, AbsolutePosition.Y + movementVector.Y * Speed);
        }

        private void RegulateMovement()
        {
            // if this if clause is fulfilled we get a new path to move to.
            // we only do this if we're not moving, have no destination and our
            // current nodequeue is empty (the path)
            if (mDestination != null && mNodeQueue.Count <= 0 && !mIsMoving)
            {
                mNodeQueue = mPathManager.GetPath(this, mDestination).GetNodePath();

                mCurrentNode = mNodeQueue.Dequeue();
            }

            if (mCurrentNode == null)
            {
                return;
            }

            // update the current node to move to after the last one got reached.
            if (ReachedTarget(((PlatformBlank)mCurrentNode).Center) && mNodeQueue.Count > 0)
            {
                mCurrentNode = mNodeQueue.Dequeue();
            }
            // finally move to the current node.
            Move(((PlatformBlank)mCurrentNode).Center);

            // check whether we have reached the target after our move call.
            ReachedTarget(((PlatformBlank)mCurrentNode).Center);
        }

        /// <summary>
        /// Checks whether the target has been reached.
        /// </summary>
        /// <param name="target">The target which is checked against</param>
        /// <returns></returns>
        private bool ReachedTarget(Vector2 target)
        {

            //since we're operating with float values we just want the distance to be smaller than 2 pixels.
            if (Vector2.Distance(AbsolutePosition, target) < 2)
            {
                CurrentNode = mCurrentNode;
                mDestination = null;
                mIsMoving = false;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(AbsolutePosition, 10, 20, Color.Green, 10, LayerConstants.GeneralUnitLayer);
        }
    }
}
