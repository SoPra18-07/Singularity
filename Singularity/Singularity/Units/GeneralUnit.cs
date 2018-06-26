using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Graph;
using Singularity.Graph.Paths;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Units
{
    [DataContract]
    public class GeneralUnit : ISpatial
    {
        [DataMember]
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
        
        [DataMember]
        //These are the assigned task and a flag, wether the unit is done with it.
        private Task mAssignedTask;
        [DataMember]
        private bool mDone;

        private IPlatformAction AssignedAction;

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
        /// The node the unit started from. Changes when the unit reaches its destination (to the destination).
        /// </summary>
        [DataMember]
        private INode mCurrentNode;

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

        [DataMember()]
        internal JobType Job { get; set; } = JobType.Idle;

        //If a Command center controlling this unit is destroyed or turned off, this unit will also be turned off
        [DataMember()]
        public bool Active { get; set; }


        public GeneralUnit(PlatformBlank platform, ref Director director)
        {
            mDestination = Optional<INode>.Of(null);

            CurrentNode = platform;

            AbsolutePosition = ((IRevealing) platform).Center; // TODO figure out how to search platform by ID and get its position
            mPathQueue = new Queue<Vector2>();
            mNodeQueue = new Queue<INode>();

            mIsMoving = false;
            mDirector = director;
            mDirector.GetDistributionManager.Register(this);
            mDone = true;
        }
        /// <summary>
        /// Used to pick the Task that the unit does. It assigns the unit to a job which the update method uses
        /// to figure out what to do with the unit.
        /// </summary>
        /// <param name="assignedTask">The Task that the unit should have.</param>
        /// <param name="targetId">If the Task is idle, then null. Otherwise, the id of either the resource or platform
        /// which it should do the Task on.</param>
        public void AssignedTask(Task assignedTask, int? targetId = null)
        {
            throw new NotImplementedException();
            /*switch (assignedTask)
            {
                case Task.Idle:
                    Job = JobType.Idle;
                    mTargetId = null;
                    break;

                case Task.BuildPlatform:
                    if (targetId != null)
                    {
                        // to build a platform, first get the required resources then go into logistics mode and get that resource
                        Job = JobType.Construction;
                        mTargetId = targetId;
                        // unimplementable since resource list is a dictionary whereas it should really be a list so that the
                        // resource objects can be peaked at and then popped once it has been delivered
                        mTargetId =
                            null; // once the platform implementation has been changed, this can just pick up the resource
                        // ID of the nearest resource of the type
                    }
                    else
                    {
                        Job = JobType.Idle;
                    }
                    break;

                case Task.MoveResource:
                    if (targetId != null)
                    {
                        Job = JobType.Logistics;
                        mTargetId = targetId;
                    }
                    else
                    {
                        Job = JobType.Idle;
                    }
                    break;

                case Task.RepairPlatform:
                    if (targetId != null)
                    {
                        Job = JobType.Defense;
                        mTargetId = targetId;
                    }
                    else
                    {
                        Job = JobType.Idle;
                    }
                    break;

                //The idea is to use this task to move while your jobtype is idle (since we want these units to move around)
                //case Task.Move:
                    //Move(targetId); 
            }*/
        }

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

        /// <summary>
        /// Used to change the job. Is usually only called if the player wants more/less Units working in a certain job.
        /// </summary>
        /// <param name="job">The job the unit should do.</param>
        public void ChangeJob(JobType job)
        {
            Job = job;
        }

        /// <summary>
        /// The method called by both construction and defense. Making it its own method simplifies the code.
        /// </summary>
        /// <param name="targetPlatformId">The target platform that is to be constructed or repaired.</param>
        private void Build(int? targetPlatformId)
        {
            throw new NotImplementedException();
            // pop out the required resource from the required resource list of the target platform
            // then goes and finds the nearest storage platform with that resource
            // does this by finding (using BFS) closest storage platform and querying it
            // and continues to the next closest storage platform until it finds it
            // if it doesn't find the resource, it waits but does not go idle
            // once it finds the resource, create a PathQueue using Dijkstra() to the storage platform
            // Travel to storage platform using Move()
            // pick up resource
            // travel back to original target using Move()
            // once it has arrived, drops the resource onto the platform

            /*
            if (targetPlatformId != null)
            {
                mConstructionResourceFound = false; // sets flag to false first
                //targetPlatformId.popRequiredResources() // not possible yet since it's not possible to search by platform ID

                // TODO implement BFS after Graph has been implemented
                int? storagePlatformId = Pathfinding.Bfs(mPositionId, Carrying); // Carrying should be changed later to the required resource
                                                                           // this is only as a placeholder
                if (storagePlatformId != null)
                {
                    mConstructionResourceFound = true;
                    mPathQueue = Pathfinding.Dijkstra(mPositionId, (int) storagePlatformId);
                    while (mPositionId != storagePlatformId)
                    {
                        // set currentTarget to the top most id on the queue
                        int currentTarget = mPathQueue.Pop();
                        // go to the next node in the pathQueue
                        while (mPositionId != currentTarget)
                        {
                            AbsolutePosition = Move(currentTarget);
                            // then update the positionID with whatever the unit is standing on top of
                        }
                    }
                    // pick up resource
                    mPathQueue = Pathfinding.Dijkstra(mPositionId, (int)storagePlatformId);
                    while (mPositionId != targetPlatformId)
                    {
                        // set currentTarget to the top most id on the queue
                        int currentTarget = mPathQueue.Pop();
                        // go to the next node in the pathQueue
                        while (mPositionId != currentTarget)
                        {
                            AbsolutePosition = Move(currentTarget);
                            // then update the positionID with whatever the unit is standing on top of
                        }
                    }
                    // drop the resource.
                }
                



            }
            */

        }

        /// <summary>
        /// Only contains implementation for the Idle case so far
        /// </summary>
        /// <inheritdoc cref="Singularity.Property.IUpdate"/>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (!mIsMoving && mDone)
            {
                mDone = false;
                if (Job == JobType.Idle)
                {
                    //Care!!! DO NOT UNDER ANY CIRCUMSTANCES USE THIS PLACEHOLDER
                    IPlatformAction action = new ProduceMineResource(null, null);
                    mAssignedTask = mDirector.GetDistributionManager.RequestNewTask(this, Job, Optional<IPlatformAction>.Of(action));
                    mDestination = Optional<INode>.Of(mAssignedTask.End.Get());
                }
            }

            // if this if clause is fulfilled we get a new path to move to.
            // we only do this if we're not moving, have no destination and our
            // current nodequeue is empty (the path)
            if (mDestination.IsPresent() && mNodeQueue.Count <= 0 && !mIsMoving)
            {
                mNodeQueue = mDirector.GetPathManager.GetPath(this, mDestination.Get()).GetNodePath();

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
            if (mNodeQueue.Count == 0 && Job == JobType.Idle)
            {
                mDone = true;
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
                CurrentNode = mCurrentNode;
                mDestination = Optional<INode>.Of(null);
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
