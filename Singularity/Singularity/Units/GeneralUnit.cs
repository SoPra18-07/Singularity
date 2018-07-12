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
        private bool mFinishTask;
        [DataMember]
        private Queue<Vector2> mPathQueue; // the queue of platform center locations
        [DataMember]
        private Queue<INode> mNodeQueue;

        [DataMember]
        private bool mConstructionResourceFound; // a flag to indicate that the unit has found the construction resource it was looking for


        //These are the assigned task and a flag, wether the unit is done with it.
        [DataMember]
        private Task mTask;

        //Represent the current workstate.
        [DataMember]
        private bool mDone;
        [DataMember]
        private bool mAssigned;

        [DataMember]
        private IPlatformAction mAssignedAction;

        /// <summary>
        /// The node the unit is currently targeting.
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

        [DataMember]
        public int TargetGraphid { get; set; }

        [DataMember]
        public int Graphid { get; set; }

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

        public GeneralUnit(PlatformBlank platform, ref Director director, int graphid)
        {
            platform.AddGeneralUnit(this);
            Graphid = graphid;
            Id = IdGenerator.NextiD();
            mDestination = Optional<INode>.Of(null);

            CurrentNode = platform;
            Carrying = Optional<Resource>.Of(null);

            AbsolutePosition = ((IRevealing) platform).Center;
            mPathQueue = new Queue<Vector2>();
            mNodeQueue = new Queue<INode>();

            mIsMoving = false;
            mDirector = director;
            mDirector.GetDistributionDirector.GetManager(Graphid).Register(this);
            mDone = true;
            mFinishTask = false;
        }

        /// <summary>
        /// Used to change the job. Is usually only called if the player wants more/less Units working in a certain job.
        /// </summary>
        /// <param name="job">The job the unit should do.</param>
        public void ChangeJob(JobType job)
        {
            //If its moving it cannot be assigned, since the unit only assigns itself when it reached the target (and stopped moving)
            //That also means, that the CurrentNode is the Producing platform, so we call that UnAssign method.
            if ((Job == JobType.Production || Job == JobType.Defense) && mTask.End.IsPresent())
            {
                mTask.End.Get().UnAssignUnits(this, Job);
                mAssigned = false;
            }

            //Finish what you started.
            if (Job == JobType.Logistics || Job == JobType.Construction)
            {
                if (Carrying.IsPresent())
                {
                    mFinishTask = true;
                }
                else
                {
                    //Put the task back in the Queue.
                    var isbuilding = Job == JobType.Construction;
                    if (mTask.Action.IsPresent())
                    {
                        if (mTask.GetResource != null)
                        {
                            mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, mTask.Action.Get(), isbuilding);
                        }
                    }
                    else
                    {
                        if (mTask.GetResource != null)
                        {
                            mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, null, isbuilding);
                        }
                    }
                }
            }
            Job = job;
        }

        /// <summary>
        /// Used to Abort the current Task, when the unit notices it cannot reach its destination.
        /// </summary>
        private void AbortTask()
        {
            //TODO: This. Also dont forget to put the task back in the task queue, but not sure if this is necessary. Talk with felix about that.
        }


        /// <summary>
        /// Is called if this Units Job is changed to Production or Defense. BUT NOT BY THE UNIT ITSELF. Should only be called by the DistrManager.
        /// Can also be used to just change the "home" of the unit. In that case just give it the job it already has (in the task).
        /// </summary>
        /// <param name="task">The new task for the unit</param>
        internal void AssignTask(Task task)
        {
            mDone = false;
            mTask = task;
            ChangeJob(mTask.Job);
            //Check whether there is a Destination. (it should)
            if (mTask.End.IsPresent())
            {
                //This only tells the platform that the unit is on the way! Use ShowedUp to tell the platform that the unit has arrived.
                mTask.End.Get().AssignUnits(this, Job);
                mDestination = Optional<INode>.Of(mTask.End.Get());
                TargetGraphid = mTask.End.Get().GetGraphIndex();
            }

            if (mTask.Action.IsPresent())
            {
                mAssignedAction = mTask.Action.Get();
            }
            else
            {
                mAssignedAction = null;
            }
            //This is needed so the unit will not first go to the end of their previous task
            mNodeQueue = new Queue<INode>();
        }


        /// <summary>
        /// In the Idle case the unit will just get a Target to move to and do so.
        /// In the Production case the unit will go to the producing Platform and call its produce method.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            //If true, this unit has still a task to finish and shall not act like the job it has now until the task is finished.
            //This should only occur when Logistic or Constructing units have only just picked up a Resource and their job changed after that.
            if (mFinishTask)
            {
                RegulateMovement();
                if (Carrying.IsPresent())
                        {
                            Carrying.Get().Follow(this);
                        }
                //This means we arrived at the point we want to leave the Resource and consider our work done
                if (mTask.End.IsPresent() && CurrentNode.Equals(mTask.End.Get()) &&
                    ReachedTarget(mTask.End.Get().Center))
                {
                    if (Carrying.IsPresent())
                    {
                        var res = Carrying.Get();
                        res.UnFollow();
                        ((PlatformBlank)CurrentNode).StoreResource(res);
                        Carrying = Optional<Resource>.Of(null);
                    }

                    mDone = true;
                    //We can now do the job we were assigned to.
                    mFinishTask = false;
                }
            }
            else
            {
                switch (Job)
                {
                    case JobType.Idle:
                        if (!mIsMoving && mDone)
                        {
                            mDone = false;

                            mTask = mDirector.GetDistributionDirector.GetManager(Graphid).RequestNewTask(unit: this, job: Job, assignedAction: Optional<IPlatformAction>.Of(null));
                            //Check if the given destination is null (it shouldnt)
                            if (mTask.End.IsPresent())
                            {
                                mDestination = Optional<INode>.Of(mTask.End.Get());
                                TargetGraphid = mTask.End.Get().GetGraphIndex();
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
                        if (!mIsMoving && !mDone && CurrentNode.Equals(mTask.End.Get()))
                        {
                            if (!mAssigned)
                            {
                                mTask.End.Get().ShowedUp(this, Job);
                                mAssigned = true;
                            }
                        }
                        RegulateMovement();
                        break;

                    case JobType.Defense:
                        //You arrived at your destination and you now want to work.
                        if (!mIsMoving && !mDone && CurrentNode.Equals(mTask.End.Get()))
                        {
                            if (!mAssigned)
                            {
                                mTask.End.Get().ShowedUp(this, Job);
                                mAssigned = true;
                            }
                        }
                        RegulateMovement();
                        break;

                    case JobType.Logistics:

                        HandleTransport();
                        RegulateMovement();

                        if (Carrying.IsPresent())
                        {
                            Carrying.Get().Follow(this);
                        }
                        break;

                    case JobType.Construction:
                        HandleTransport();
                        RegulateMovement();

                        if (Carrying.IsPresent())
                        {
                            Carrying.Get().Follow(this);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Logistics and Construction resemble each other very much, so this is the method to handle both.
        /// </summary>
        private void HandleTransport()
        {
            if (!mIsMoving && mDone)
            {
                mDone = false;

                mTask = mDirector.GetDistributionDirector.GetManager(Graphid).RequestNewTask(unit: this, job: Job, assignedAction: Optional<IPlatformAction>.Of(null));
                //First go to the location where you want to get your Resource from
                //Check if the given destination is null (it shouldnt).
                if (mTask.Begin.IsPresent())
                {
                    mDestination = Optional<INode>.Of(mTask.Begin.Get());
                    TargetGraphid = mTask.Begin.Get().GetGraphIndex();
                }
                else
                {
                    //In this case the DistributionManager has given you no valid task. That means there is no work in this job to be done. Ask in the next cycle.
                    mDone = true;
                }
            }

            //This means we arrived at the point we want to pick up a Resource
            if (mTask.Begin.IsPresent() && CurrentNode.Equals(mTask.Begin.Get()) &&
                ReachedTarget(mTask.Begin.Get().Center))
            {
                if (!Carrying.IsPresent())
                {
                    if (mTask.GetResource != null)
                    {
                        PickUp((EResourceType)mTask.GetResource);
                    }

                    //Failed to get resource, because no Resources were present. Tell the DistributionManager and consider your work done.
                    if (!Carrying.IsPresent())
                    {
                        if (mTask.Job == JobType.Logistics)
                        {
                            if (mTask.Action.IsPresent())
                            {
                                if (mTask.GetResource != null)
                                {
                                    mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, mTask.Action.Get());
                                }
                            }
                            else
                            {
                                if (mTask.GetResource != null)
                                {
                                    mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, null);
                                }
                            }
                        }
                        if (mTask.Job == JobType.Construction)
                        {
                            if (mTask.Action.IsPresent())
                            {
                                if (mTask.GetResource != null)
                                {
                                    mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, mTask.Action.Get(), true);
                                }
                            }
                            else
                            {
                                if (mTask.GetResource != null)
                                {
                                    mDirector.GetDistributionDirector.GetManager(Graphid).RequestResource(mTask.End.Get(), (EResourceType)mTask.GetResource, null, true);
                                }
                            }
                        }
                        mDone = true;
                    }
                }

                //Everything went fine with picking up, so now move on to your final destination
                if (mTask.End.IsPresent() && !mDone)
                {
                    mDestination = Optional<INode>.Of(mTask.End.Get());
                    TargetGraphid = mTask.End.Get().GetGraphIndex();
                }
            }

            //This means we arrived at the point we want to leave the Resource and consider our work done
            if (mTask.End.IsPresent() && CurrentNode.Equals(mTask.End.Get()) &&
                ReachedTarget(mTask.End.Get().Center))
            {
                if (Carrying.IsPresent())
                {
                    var res = Carrying.Get();
                    res.UnFollow();
                    ((PlatformBlank)CurrentNode).StoreResource(res);
                    Carrying = Optional<Resource>.Of(null);
                }

                mDone = true;
            }
        }

        private void PickUp(EResourceType resource)
        {
            if (mTask.Begin.Get().GetPlatformResources().Count > 0)
            {
                var res = ((PlatformBlank) CurrentNode).GetResource(resource);
                if (res.IsPresent())
                {
                    Carrying = res;
                }
            }
        }

        #region Movement

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
                ((PlatformBlank)CurrentNode).RemoveGeneralUnit(this);

                mNodeQueue = mDirector.GetPathManager.GetPath(this, mDestination.Get(), ((PlatformBlank)mDestination.Get()).GetGraphIndex()).GetNodePath();

                CurrentNode = mNodeQueue.Dequeue();

                ((PlatformBlank) CurrentNode)?.AddGeneralUnit(this);
            }

            if (CurrentNode == null)
            {
                return;
            }

            // update the current node to move to after the last one got reached.
            if (ReachedTarget(((PlatformBlank)CurrentNode).Center) && mNodeQueue.Count > 0)
            {
                ((PlatformBlank)CurrentNode).RemoveGeneralUnit(this);

                CurrentNode = mNodeQueue.Dequeue();

                ((PlatformBlank)CurrentNode).AddGeneralUnit(this);
            }

            // finally move to the current node.
            if (mTask.End.IsPresent() && (!ReachedTarget(((PlatformBlank)CurrentNode).Center) || !mTask.End.Get().Equals(CurrentNode)))
            {
                Move(((PlatformBlank)CurrentNode).Center);
            }

            // check whether we have reached the target after our move call.
            ReachedTarget(((PlatformBlank)CurrentNode).Center);
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
                mIsMoving = false;
                return true;
            }

            return false;
        }
#endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.StrokedCircle(AbsolutePosition, 10, Color.AntiqueWhite, Color.Black, LayerConstants.GeneralUnitLayer);
            if (Carrying.IsPresent())
            {
                Carrying.Get().Draw(spriteBatch);
            }
        }

        public bool Die()
        {
            mTask = new Task(Job, Optional<PlatformBlank>.Of(null), null, Optional<IPlatformAction>.Of(null));
            if (Carrying.IsPresent())
            {
                Carrying.Get().Die();
                Carrying = Optional<Resource>.Of(null);
            }

            mDirector.GetDistributionDirector.GetManager(Graphid).Kill(this);
            mAssignedAction.Kill(this);

            return true;
        }

        public void Kill(int id)
        {
            if (mTask.Contains(id))
            {
                mTask = mDirector.GetDistributionDirector.GetManager(Graphid).RequestNewTask(this, Job, null);
                // also the mAssignedTask-platformaction is included in this.
            }
        }

        public Task Task
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public JobType JobType
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
    }
}
