using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.DistributionManager;
using Singularity.Platform;
using Singularity.Property;
using Singularity.Resources;
using Singularity.Utils;

namespace Singularity.Units
{
    [DataContract()]
    public class GeneralUnit : IUnit, IUpdate, IDraw
    {
        [DataMember()]
        public int Id { get; }
        [DataMember()]
        private int mPositionId;
        [DataMember()]
        public string Assignment { get; set; } // TODO change to an enum type
        [DataMember()]
        public Resource Carrying { get; set; } // TODO change resource into a nullable type
        [DataMember()]
        private int? mTargetId;
        [DataMember()]
        private Stack<int> mPathQueue; // the queue of platform and edges the unit has to traverse to get to its target
        [DataMember()]
        private bool mConstructionResourceFound; // a flag to indicate that the unit has found the construction resource it was looking for
        

        //These are the assigned task and a flag, wether the unit is done with it.
        private Task mAssignedTask;

        private bool mDone;

        private DistributionManager.DistributionManager mDistrManager;

        private IPlatformAction AssignedAction;

        // TODO: also use the size for the units representation since we someday need to draw rectangles over units (bounding box)

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        internal JobType Job { get; set; } = JobType.Idle;

        public GeneralUnit(int spawnPositionId)
        {
            Id = 0; // TODO make this randomized or simply ascending
            AbsolutePosition = Vector2.Zero; // TODO figure out how to search platform by ID and get its position
            mPositionId = spawnPositionId;
            Carrying = null; // TODO change this to a nullable type or some other implementation after dist manager is implemented
            mPathQueue = null;
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
            switch (assignedTask)
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
            }
        }
        /// <summary>
        /// Calculates where the unit should move to next
        /// </summary>
        /// <param name="targetPosition">The target the unit should move towards</param>
        /// <returns></returns>
        private Vector2 Move(int targetPosition)
        {
            throw new NotImplementedException();
            // first get the target position Vector2 position
            // then move x distance in that direction or until above the coordinate of the position
            // TODO
            return Vector2.Zero;
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

        }
        public void Update(GameTime gametime)
        {
            throw new NotImplementedException();
            // I think the intention here was to do the tasks.
            switch (mAssignedTask.GetFirst())
            {
                case Task.Idle:
                    mAssignedTask = mDistrManager.RequestNewTask(Job, Optional<IPlatformAction>.Of(AssignedAction));
                    break;
                case Task.Move:
                    //Move around
                    break;
                case Task.BuildPlatform:
                    //Build();
                    break;
                case Task.MoveResource:
                    //MoveResource();
                    break;
                case Task.RepairPlatform:
                    //Repair();
                    break;
            }
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
