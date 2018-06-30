using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Units
{
    [DataContract]
    public sealed class GeneralUnit : ISpatial
    {

        public int Id { get; }
        [DataMember]
        private int mPositionId;
        [DataMember]
        public Optional<Resource> Carrying { get; set; }
        [DataMember]
        private int? mTargetId;
        [DataMember]
        private Queue<Vector2> mPathQueue; // the queue of platform center locations
        [DataMember]
        private Queue<INode> mNodeQueue;

        [DataMember]
        private bool mConstructionResourceFound; // a flag to indicate that the unit has found the construction resource it was looking for


        //These are the assigned task and a flag, wether the unit is done with it.
        [DataMember]
        private Task mAssignedTask;

        //Represent the current workstate.
        [DataMember]
        private bool mDone;
        [DataMember]
        private bool mAssigned;

        [DataMember]
        private IPlatformAction mAssignedAction;

        /// <summary>
        /// The node the unit started from. Changes when the unit reaches its destination (to the destination).
        /// </summary>
        [DataMember]
        public INode CurrentNode { get; private set; }

        // TODO: also use the size for the units representation since we someday need to draw rectangles over units (bounding box)

        [DataMember]
        public Vector2 AbsolutePosition { get; set; }
        [DataMember]
        public Vector2 AbsoluteSize { get; set; }
        [DataMember]
        public Vector2 RelativePosition { get; set; }
        [DataMember]
        public Vector2 RelativeSize { get; set; }
        [DataMember]
        private readonly Director mDirector;

        /// <summary>
        /// whether the unit is moving or currently standing still,
        /// this is used so the unit can ask for a new path if it
        /// doesn't move
        /// </summary>
        [DataMember]
        private bool mIsMoving;
        
        /// <summary>
        /// The node the unit moves to. Null if the unit doesn't move anywhere
        /// </summary>
        [DataMember]
        private Optional<INode> mDestination;

        /// <summary>
        /// The speed the unit moves at.
        /// </summary>
        [DataMember]
        private const float Speed = 3f;

        [DataMember]
        internal JobType Job { get; set; } = JobType.Idle;

        //If a Command center controlling this unit is destroyed or turned off, this unit will also be turned off
        [DataMember]
        public bool Active { get; set; }

        public GeneralUnit(PlatformBlank platform, ref Director director)
        {
            mDestination = Optional<INode>.Of(null);

            CurrentNode = platform;
            Carrying = Optional<Resource>.Of(null);

            AbsolutePosition = ((IRevealing) platform).Center;
            mPathQueue = new Queue<Vector2>();
            mNodeQueue = new Queue<INode>();

            mIsMoving = false;
            mDirector = director;
            mDirector.GetDistributionManager.Register(this);
            mDone = true;
        }

        /// <summary>
        /// Used to change the job. Is usually only called if the player wants more/less Units working in a certain job.
        /// </summary>
        /// <param name="job">The job the unit should do.</param>
        public void ChangeJob(JobType job)
        {
            //If its moving it cannot be assigned, since the unit only assigns itself when it reached the target (and stopped moving)
            //That also means, that the CurrentNode is the Producing platform, so we call that UnAssign method.
            if (Job == JobType.Production && mAssigned && !mIsMoving)
            {
                ((PlatformBlank)CurrentNode).UnAssignUnits(this, Job);
                mAssigned = false;
            }
            Job = job;
        }

        /// <summary>
        /// Is called if this Units Job is changed to Production or Defense. BUT NOT BY THE UNIT ITSELF. Should only be called by the DistrManager.
        /// Can also be used to just change the "home" of the unit. In that case just give it the job it already has (in the task).
        /// </summary>
        /// <param name="task">The new task for the unit</param>
        public void AssignTask(Task task)
        {
            mDone = false;
            mAssignedTask = task;
            ChangeJob(mAssignedTask.Job);
            //Check whether there is a Destination. (it should)
            if (mAssignedTask.End.IsPresent())
            {
                mDestination = Optional<INode>.Of(mAssignedTask.End.Get());
            }

            if (mAssignedTask.Action.IsPresent())
            {
                mAssignedAction = mAssignedTask.Action.Get();
            }
            else
            {
                mAssignedAction = null;
            }
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
                        // IPlatformAction action = new ProduceMineResource(platform: null, resourceMap: null, director: ref mDirector);
                        // WHY THE FUCK GOT A PLATFORMACTION CREATED HERE ?!?!?

                        mAssignedTask = mDirector.GetDistributionManager.RequestNewTask(unit: this, job: Job, assignedAction: Optional<IPlatformAction>.Of(null));
                        //Check if the given destination is null (it shouldnt)
                        if (mAssignedTask.End.IsPresent())
                        {
                            mDestination = Optional<INode>.Of(mAssignedTask.End.Get());
                        }
                    }

                    RegulateMovement();

                    //We arrived at the target, so its now time to get another job
                    if (mNodeQueue.Count == 0 && Job == JobType.Idle)
                    {
                        mDone = true;
                    }
                    break;

                case JobType.Production:
                    //You arrived at your destination and you now want to work.
                    if(!mIsMoving && !mDone && CurrentNode.Equals(mAssignedTask.End.Get()))
                    {
                        if (!mAssigned)
                        {
                            mAssignedTask.End.Get().AssignUnits(this, Job);
                            mAssigned = true;
                        }
                    }
                    RegulateMovement();
                    break;
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
            if (mDestination.IsPresent() && mNodeQueue.Count <= 0 && !mIsMoving)
            {
                mNodeQueue = mDirector.GetPathManager.GetPath(this, mDestination.Get()).GetNodePath();

                CurrentNode = mNodeQueue.Dequeue();
            }

            if (CurrentNode == null)
            {
                return;
            }

            // update the current node to move to after the last one got reached.
            if (ReachedTarget(((PlatformBlank)CurrentNode).Center) && mNodeQueue.Count > 0)
            {
                CurrentNode = mNodeQueue.Dequeue();
            }

            // finally move to the current node.
            if (!ReachedTarget(((PlatformBlank)CurrentNode).Center) || !mAssignedTask.End.Get().Equals(CurrentNode))
            {
                Move(((PlatformBlank)CurrentNode).Center);
            }

            // check whether we have reached the target after our move call.
            ReachedTarget(((PlatformBlank)CurrentNode).Center);


            if (((PlatformBlank) CurrentNode).GetPlatformResources().Count > 0)
            {
                // todo: fix
                var res = ((PlatformBlank) CurrentNode).GetResource(EResourceType.Oil);
                if (res.IsPresent())
                {
                    Carrying = res;
                }
                else if (!Carrying.IsPresent())
                {
                    res = ((PlatformBlank) CurrentNode).GetResource(EResourceType.Trash);
                    if (res.IsPresent())
                    {
                        Carrying = res;
                    }
                }
            }

            if (Carrying.IsPresent())
            {
                Carrying.Get().Follow(this);
            }

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
                if (mDestination.IsPresent())
                {
                    CurrentNode = mDestination.Get();
                }
                mDestination = Optional<INode>.Of(null);
                mIsMoving = false;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(AbsolutePosition, 10, 20, Color.DarkOliveGreen, 10, LayerConstants.GeneralUnitLayer);

            if (Carrying.IsPresent())
            {
                Carrying.Get().Draw(spriteBatch);
            }
        }
    }
}
