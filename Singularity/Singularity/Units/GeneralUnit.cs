using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Property;
using Singularity.Resources;

namespace Singularity.Units
{
    public class GeneralUnit : IUnit, IUpdate, IDraw
    {
        public int Id { get; }
        private int mPositionId;
        public string Assignment { get; set; } // TODO change to an enum type
        public EResourceType Carrying { get; set; } // TODO change resource into a nullable type
        private int? mTargetId;
        private Stack<int> mPathQueue; // the queue of platform and edges the unit has to traverse to get to its target
        private bool mConstructionResourceFound; // a flag to indicate that the unit has found the construction resource it was looking for

        // TODO: also use the size for the units representation since we someday need to draw rectangles over units (bounding box)

        public Vector2 AbsolutePosition { get; set; }

        public Vector2 AbsoluteSize { get; set; }

        public Vector2 RelativePosition { get; set; }

        public Vector2 RelativeSize { get; set; }

        internal JobType Job { get; set; } = JobType.Idle;

        //If a Command center controlling this unit is destroyed or turned off, this unit will also be turned off
        public bool Active { get; set; }

        public GeneralUnit(int spawnPositionId)
        {
            Id = 0; // TODO make this randomized or simply ascending
            AbsolutePosition = Vector2.Zero; // TODO figure out how to search platform by ID and get its position
            mPositionId = spawnPositionId;
            Carrying = EResourceType.Trash; // TODO change this to a nullable type or some other implementation after dist manager is implemented
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
            }
        }
        /// <summary>
        /// Calculates where the unit should move to next
        /// </summary>
        /// <param name="targetPosition">The target the unit should move towards</param>
        /// <returns></returns>
        private Vector2 Move(int targetPosition)
        {
            // first get the target position Vector2 position
            // then move x distance in that direction or until above the coordinate of the position
            // TODO
            return Vector2.Zero;
        }

        /// <summary>
        /// The method called by both construction and defense. Making it its own method simplifies the code.
        /// </summary>
        /// <param name="targetPlatformId">The target platform that is to be constructed or repaired.</param>
        private void Build(int? targetPlatformId)
        {
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
            // use switch to change between jobs
            switch (Job)
            {
                case JobType.Idle:
                    // does nothing
                    break;
                case JobType.Construction:
                    // runs build()
                    Build(mTargetId);
                    break;
                case JobType.Logistics:
                    // TODO unclear how this should be implmented
                    break;
                case JobType.Defense:
                    // basically the same as the construction one
                    Build(mTargetId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
