using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Content;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.Manager
{
    [DataContract]
    public sealed class DistributionManager
    {
        [DataMember]
        private List<GeneralUnit> mIdle;
        [DataMember]
        private List<GeneralUnit> mLogistics;
        [DataMember]
        private List<GeneralUnit> mConstruction;
        [DataMember]
        private List<GeneralUnit> mProduction;
        [DataMember]
        private List<GeneralUnit> mDefense;
        [DataMember]
        private List<GeneralUnit> mManual;

        [DataMember]
        private Queue<Task> mBuildingResources;
        [DataMember]
        private Queue<Task> mRefiningOrStoringResources;
        [DataMember]
        private Queue<Task> mRequestedUnitsProduce;
        [DataMember]
        private Queue<Task> mRequestedUnitsDefense;

        [DataMember]
        private List<Pair<int, int>> mKilled;

        [DataMember]
        private Random mRandom;

        [DataMember]
        private List<BuildBluePrint> mBlueprintBuilds;

        [DataMember]
        private List<IPlatformAction> mPlatformActions;

        [DataMember]
        private List<Pair<PlatformBlank, int>> mProdPlatforms;
        [DataMember]
        private List<Pair<PlatformBlank, int>> mDefPlatforms;


        public DistributionManager()
        {
            //Lists of Units of different Jobtypes
            mIdle = new List<GeneralUnit>();
            mLogistics = new List<GeneralUnit>();
            mConstruction = new List<GeneralUnit>();
            mProduction = new List<GeneralUnit>();
            mDefense = new List<GeneralUnit>();
            mManual = new List<GeneralUnit>();

            //Lists for Tasks to do
            mBuildingResources = new Queue<Task>();
            mRefiningOrStoringResources = new Queue<Task>();
            mRequestedUnitsProduce = new Queue<Task>();
            mRequestedUnitsDefense = new Queue<Task>();

            //Other stuff -- ???
            mBlueprintBuilds = new List<BuildBluePrint>();
            mPlatformActions = new List<IPlatformAction>();
            mProdPlatforms = new List<Pair<PlatformBlank, int>>();
            mDefPlatforms = new List<Pair<PlatformBlank, int>>();
            mRandom = new Random();
        }

        /// <summary>
        /// Is called by the unit when it is created.
        /// </summary>
        /// <param name="unit">the unit that has been created</param>
        public void Register(GeneralUnit unit)
        {
            mIdle.Add(unit);
        }

        /// <summary>
        /// Is called by producing and defending Platforms when they are created.
        /// </summary>
        /// <param name="platform">The platform itself</param>
        /// <param name="isDef">Is true, when the platform is a defending platform, false otherwise (only producing platforms should register themselves besides defending ones)</param>
        public void Register(PlatformBlank platform, bool isDef)
        {
            if (isDef)
            {
                //Make sure the new platform gets some units
                NewlyDistribute(platform, true);
            }
            else
            {
                //Make sure the new platform gets some units
                NewlyDistribute(platform, false);
            }
        }

        public void TestAttributes()
        {
            Console.Out.WriteLine(mProduction.Count);
            Console.Out.WriteLine(mIdle.Count);
            Console.Out.WriteLine(mProdPlatforms[1].GetFirst().mType + " " + mProdPlatforms[1].GetSecond());
            Console.Out.WriteLine(mProdPlatforms[0].GetFirst().mType + " " + mProdPlatforms[0].GetSecond());
        }

        /// <summary>
        /// This is called by the player, when he wants to distribute the units to certain jobs.
        /// </summary>
        /// <param name="oldj">The old job of the units</param>
        /// <param name="newj">The new job of the units</param>
        /// <param name="amount">The amount of units to be transferred</param>
        public void DistributeJobs(JobType oldj, JobType newj, int amount)
        {
            List<GeneralUnit> oldlist;
            switch (oldj)
            {
                case JobType.Construction:
                    oldlist = mConstruction;
                    break;
                case JobType.Logistics:
                    oldlist = mLogistics;
                    break;
                case JobType.Idle:
                    oldlist = mIdle;
                    break;
                case JobType.Production:
                    oldlist = mProduction;
                    break;
                case JobType.Defense:
                    oldlist = mDefense;
                    break;
                default:
                    throw new InvalidGenericArgumentException("You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");

            }
            List<GeneralUnit> newlist;
            switch (newj)
            {
                case JobType.Construction:
                    newlist = mConstruction;
                    break;
                case JobType.Idle:
                    newlist = mIdle;
                    break;
                case JobType.Logistics:
                    newlist = mLogistics;
                    break;
                case JobType.Production:
                    newlist = mProduction;
                    break;
                case JobType.Defense:
                    newlist = mDefense;
                    break;
                default:
                    throw new InvalidGenericArgumentException("You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }

            //Production and Defense have to be distributed differently than the other jobs because we want to assure fairness
            if (oldj == JobType.Production || oldj == JobType.Defense)
            {
                List<GeneralUnit> list;
                //Get the units to change jobs here
                if (oldj == JobType.Production)
                {
                    list = GetUnitsFairly(amount, mProdPlatforms, false);
                }
                else
                {
                    list = GetUnitsFairly(amount, mDefPlatforms, false);
                }

                //Now actually change their Jobs.
                if (newj == JobType.Production)
                {
                    AssignUnitsFairly(list, false);
                }else if (newj == JobType.Defense)
                {
                    AssignUnitsFairly(list, true);
                }
                else
                {
                    foreach (var unit in list)
                    {
                        newlist.Add(unit);
                        unit.ChangeJob(newj);
                    }
                }
            }
            else
            {
                var list = new List<GeneralUnit>();
                for (var i = amount; i > 0; i--)
                {
                    if (oldlist.Count == 0)
                    {
                        break;
                    }

                    //Pick a random Unit and change its job
                    var rand = mRandom.Next(0, oldlist.Count);
                    var unassigned = oldlist.ElementAt(rand);
                    unassigned.ChangeJob(JobType.Idle);
                    oldlist.Remove(unassigned);
                    list.Add(unassigned);
                }

                if (newj == JobType.Production)
                {
                    AssignUnitsFairly(list, false);
                }
                else if (newj == JobType.Defense)
                {
                    AssignUnitsFairly(list, true);
                }
                else
                {
                    foreach (var unit in list)
                    {
                        newlist.Add(unit);
                        unit.ChangeJob(newj);
                    }
                }
            }
        }

        /// <summary>
        /// Collect the Units from mProdPlatforms or mDefPlatforms and put them in a list. Change their job to idle.
        /// </summary>
        /// <param name="amount">the amount of units to be collected</param>
        /// <param name="list">the list to collect from. Has to be mDefPlatforms or mProdPlatforms</param>
        /// <param name="isDefense">Determines if its the left or the right one</param>
        /// <returns>A list containing the collected units</returns>
        private List<GeneralUnit> GetUnitsFairly(int amount, List<Pair<PlatformBlank, int>> list, bool isDefense)
        {
            var search = true;
            var startindex = list.Count - 1;
            var lowassign = int.MaxValue;

            if (list.Count == 0)
            {
                return new List<GeneralUnit>();
            }

            //this will always be overridden this way
            //The idea is to search the list backwards and try to find the platform with more units than the previous platform
            //Given that the units have been distributed fairly, we can now decrement units from there.
            //If we reach the end that way, we have to continue decrementing the units from the end.
            for (var i = startindex; search; i--)
            {
                if (i == 0)
                {
                    search = false;
                    startindex = list.Count - 1;
                }

                //Relys on fairness
                if (lowassign >= list[i].GetSecond())
                {
                    lowassign = list[i].GetSecond();
                }
                else
                {
                    //Found the place to decrement units
                    search = false;
                    startindex = i;
                }
            }

            var units = new List<GeneralUnit>();


            //NOW SUBSTRACT THE REQUESTED AMOUNT OF UNITS
            for (var i = amount; i > 0; i--)
            {

                //This means there are no Units to subtract
                if (list[startindex].GetSecond() == 0)
                {
                    return units;
                }

                var platUnits = list[startindex].GetFirst().GetAssignedUnits();

                JobType job;
                List<GeneralUnit> joblist;
                if (isDefense)
                {
                    job = JobType.Defense;
                    joblist = mDefense;
                }
                else
                {
                    job = JobType.Production;
                    joblist = mProduction;
                }

                //Remove the first unit in the AssignedUnitList. The unit will unassign itself. Then add the unit to our unitslist.
                //Also dont forget to decrement the number in the tuple, and to delete the unit from the joblist.
                var transferunit = platUnits[job].First().GetFirst();
                units.Add(transferunit);
                joblist.Remove(transferunit);
                var number = list[startindex].GetSecond() - 1;
                list[startindex] = new Pair<PlatformBlank, int>(list[startindex].GetFirst(), number);
                transferunit.ChangeJob(JobType.Idle);

                //Decrement index or begin at the right end again
                if (startindex - 1 == 0)
                {
                    startindex = list.Count - 1;
                }
                else
                {
                    startindex--;
                }
            }

            return units;
        }

        /// <summary>
        /// Is called internally by the Distributionmanager to Assign Units to the production or the defense, without destroying the balance of the numbers
        /// of units at the platforms. The units will have the corresponding job afterwards.
        /// </summary>
        /// <param name="toassign">The units to assign</param>
        /// <param name="isDefense"></param>
        private void AssignUnitsFairly(List<GeneralUnit> toassign, bool isDefense)
        {
            List<Pair<PlatformBlank, int>> list;
            JobType job;
            if (isDefense)
            {
                list = mDefPlatforms;
                job = JobType.Defense;
            }
            else
            {
                list = mProdPlatforms;
                job = JobType.Production;
            }

            var startindex = 0;
            var highassign = int.MinValue;
            var search = true;
            //SEARCH INDEX
            //iterate through the list, from the left side this time (see GetUnitsFairly) and search the index of the platforms with less units
            for (var i = 0; search; i++)
            {
                if (i == list.Count - 1)
                {
                    search = false;
                    startindex = 0;
                }
                if (highassign <= list[i].GetSecond())
                {
                    highassign = list[i].GetSecond();
                }
                else
                {
                    //Found the place to increment units
                    highassign = list[i].GetSecond();
                    search = false;
                    startindex = i;
                }
            }

            //ADD THE GIVEN UNITS/GIVE THEM THE TASKS
            foreach (var unit in toassign)
            {
                var goTo = list[startindex].GetFirst();
                if (job == JobType.Defense)
                {
                    mDefense.Add(unit);
                }
                else
                {
                    mProduction.Add(unit);
                }
                //The unit will change its job when calling this
                unit.AssignTask(new Task(job, Optional<PlatformBlank>.Of(goTo), null, Optional<IPlatformAction>.Of(null)));
                list[startindex] = new Pair<PlatformBlank, int>(goTo, list[startindex].GetSecond() + 1);
                if (startindex == list.Count - 1)
                {
                    startindex = 0;
                }
                else
                {
                    startindex++;
                }
            }
        }

        /// <summary>
        /// A method called by the DistributionManager in case a new platform is registered, then it will distribute units to it.
        /// </summary>
        /// <param name="platform">The newly registered platform</param>
        /// <param name="isDefense">Is true if its a DefensePlatform, false otherwise</param>
        private void NewlyDistribute(PlatformBlank platform, bool isDefense)
        { //TODO: Change this to use The written helpermethods
            if (isDefense)
            {
                // + 1 because we didnt add the new platform yet
                var times = mDefense.Count / (mDefPlatforms.Count + 1);
                var list = GetUnitsFairly(times, mDefPlatforms, false);

                foreach (var unit in list)
                {
                    //We have to re-add the units to the job list because GetUnitsFairly did unassign them
                    mDefense.Add(unit);
                    //Also unassigns the unit.
                    unit.AssignTask(new Task(JobType.Defense, Optional<PlatformBlank>.Of(platform), null, Optional<IPlatformAction>.Of(null)));
                }

                mDefPlatforms.Add(new Pair<PlatformBlank, int>(platform, list.Count));
            }
            else
            {
                // + 1 because we didnt add the new platform yet
                var times = mProduction.Count / (mProdPlatforms.Count + 1);
                var list = GetUnitsFairly(times, mProdPlatforms, false);

                foreach (var unit in list)
                {
                    //We have to re-add the units to the job list because GetUnitsFairly did unassign them
                    mProduction.Add(unit);
                    //Also unassigns the unit.
                    unit.AssignTask(new Task(JobType.Production, Optional<PlatformBlank>.Of(platform), null, Optional<IPlatformAction>.Of(null)));
                }

                mProdPlatforms.Add(new Pair<PlatformBlank, int>(platform, list.Count));
            }
        }

        /// <summary>
        /// Manually Assign Units to a certain PlatformAction.
        /// </summary>
        /// <param name="amount">The amount of units to be assigned</param>
        /// <param name="action">The action to which the units shall be assigned</param>
        /// <param name="job">The Job the units are supposed to have.</param>
        public void ManualAssign(int amount, IPlatformAction action, JobType job)
        {
            List<GeneralUnit> oldlist;
            switch (job)
            {
                case JobType.Construction:
                    oldlist = mConstruction;
                    break;
                case JobType.Idle:
                    oldlist = mIdle;
                    break;
                case JobType.Production:
                    oldlist = mDefense;
                    break;
                case JobType.Defense:
                    oldlist = mProduction;
                    break;
                case JobType.Logistics:
                    oldlist = mLogistics;
                    break;
                default:
                    throw new InvalidGenericArgumentException("You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }

            for (var i = amount; i >= 0; i--)
            {
                //Change the job of a random unit
                var rand = mRandom.Next(0, oldlist.Count);
                var removeit = oldlist.ElementAt(rand);
                mManual.Add(removeit);
                action.AssignUnit(removeit, job);
            }
        }

        /// <summary>
        /// Manually Unassign some units of a Platformaction.
        /// </summary>
        /// <param name="job">The Job they are having</param>
        /// <param name="amount">The amount of units to be unassigned</param>
        /// <param name="action">The platformaction of which they shall be unassigned</param>
        public void ManualUnassign(JobType job, int amount, IPlatformAction action)
        {
            action.UnAssignUnits(amount, job);
        }

        /// <summary>
        /// A method for Platforms/their actions to request resources.
        /// </summary>
        /// <param name="platform">The platform making the request</param>
        /// <param name="resource">The wanted resource</param>
        /// <param name="action">The corresponding platformaction</param>
        /// <param name="isbuilding">True if the resources are for building, false otherwise</param>
        public void RequestResource(PlatformBlank platform, EResourceType resource, IPlatformAction action, bool isbuilding = false)
        {
            // Will repair request ressources or units? And what unit will be used?
            // We do not have repair yet or anytime soon.
            // In that case I guess Ill ignore it for now.
            //TODO: Create Action references, when interfaces were created.
            if (isbuilding)
            {
                mBuildingResources.Enqueue(new Task(JobType.Construction, Optional<PlatformBlank>.Of(platform), resource, Optional<IPlatformAction>.Of(action)));
            }
            else
            {
                mRefiningOrStoringResources.Enqueue(new Task(JobType.Logistics, Optional<PlatformBlank>.Of(platform), resource, Optional<IPlatformAction>.Of(action)));
            }
        }
        
        //TODO: Think about if we still need this
        public void RequestUnits(PlatformBlank platform, JobType job, IPlatformAction action, bool isdefending = false)
        {
            if (isdefending)
            {
                //Assure fairness
                if (platform.GetAssignedUnits().Count <= mDefense.Count / mDefPlatforms.Count)
                {
                    mRequestedUnitsDefense.Enqueue(new Task(JobType.Construction, Optional<PlatformBlank>.Of(platform), null, Optional<IPlatformAction>.Of(action)));
                }
            }
            else
            {
                //Assure fairness
                if (platform.GetAssignedUnits().Count <= mProduction.Count / mProdPlatforms.Count)
                {
                    mRequestedUnitsProduce.Enqueue(new Task(JobType.Production, Optional<PlatformBlank>.Of(platform), null, Optional<IPlatformAction>.Of(action)));
                }
            }
        }

        // Do we even need that? I think the units should do that - huh? no this was supposed to be from platformId to Resources on that platform, primarily for internal use when searching resources ... if you have actual platform-references all the better (you could probably get them from the producing (and factory) PlatformActions ...)
        public List<EResourceType> PlatformRequests(PlatformBlank platform)
        {
            throw new NotImplementedException();
            //return platform.GetPlatformResources();
        }

        /// <summary>
        /// A method for the GeneralUnits to ask for a task to do.
        /// </summary>
        /// <param name="unit">The GeneralUnit asking</param>
        /// <param name="job">Its Job</param>
        /// <param name="assignedAction">The PlatformAction the unit is eventually assigned to</param>
        /// <returns></returns>
        public Task RequestNewTask(GeneralUnit unit, JobType job, Optional<IPlatformAction> assignedAction)
        {
            var nodes = new List<INode>();
            Task task;
            switch (job)
            {
                case JobType.Idle:
                    //It looks inefficient but I think its okay, the
                    //Platforms got not that much connections (or at least they are supposed to have not that much connections).
                    //That way the unit will only travel one node per task, but that makes it more reactive.
                    foreach (var edge in unit.CurrentNode.GetInwardsEdges())
                    {
                        nodes.Add(edge.GetParent());
                    }
                    foreach (var edge in unit.CurrentNode.GetOutwardsEdges())
                    {
                        nodes.Add(edge.GetChild());
                    }

                    if (nodes.Count == 0)
                    {
                        //Could be very inefficient, since the Units will bombard the DistributionManager with asks for tasks when there is only one platform
                        //connected to theirs
                        nodes.Add(unit.CurrentNode);
                    }
                    var rndnmbr = mRandom.Next(0, nodes.Count);
                    //Just give them the inside of the Optional action witchout checking because
                    //it doesnt matter anyway if its null if the unit is idle.
                    return new Task(job, Optional<PlatformBlank>.Of((PlatformBlank) nodes.ElementAt(rndnmbr)), null, assignedAction);

                case JobType.Production:
                    throw new InvalidGenericArgumentException("You shouldnt ask for Production tasks, you just assign units to production.");

                case JobType.Defense:
                    throw new InvalidGenericArgumentException("You shouldnt ask for Defense tasks, you just assign units to defense.");

                case JobType.Construction:
                    task = mBuildingResources.Dequeue();
                    break;

                case JobType.Logistics:
                    task = mRefiningOrStoringResources.Dequeue();
                    break;

                default:
                    throw new InvalidGenericArgumentException("Your requested JobType does not exist.");
            }
            
            mKilled = mKilled.Select(p => new Pair<int, int>(p.GetFirst(), p.GetSecond() - 1)).ToList();
            mKilled.RemoveAll(p => p.GetSecond() < 0);

            return mKilled.TrueForAll(p => !task.Contains(p.GetFirst())) ? task : RequestNewTask(unit, job, assignedAction);
        }

        public void PausePlatformAction(IPlatformAction action)
        {
            throw new NotImplementedException();
            // Actions need a sleep method
            // No, they're just being removed from occurences in the DistributionManager. As soon as they unpause, they'll send requests for Resources and units again.
            // Ah ok I got that part
        }

        public void Kill(PlatformBlank platform)
        {
            // Strong assumption that a platform is only listed at most once in either of these.
            mProdPlatforms.Remove(mProdPlatforms.Find(p => p.GetFirst().Equals(platform)));
            mDefPlatforms.Remove(mDefPlatforms.Find(p => p.GetFirst().Equals(platform)));
            var lists = new List<List<GeneralUnit>> { mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual };
            lists.ForEach(l => l.ForEach(u => u.Kill(platform.Id)));
            // the first in the pair is the id, the second is the TTL
            mKilled.Add(new Pair<int, int>(platform.Id, Math.Max(mBuildingResources.Count, mRefiningOrStoringResources.Count)));
        }

        public void Kill(IPlatformAction action)
        {
            // Strong assumption that a PlatformAction is only listed at most once here.
            if (action.GetType() == typeof(BuildBluePrint))
            {
                mBlueprintBuilds.Remove(mBlueprintBuilds.Find(b => b.Equals(action)));
            }
            else
            {
                mPlatformActions.Remove(mPlatformActions.Find(p => p.Equals(action)));
            }

            var lists = new List<List<GeneralUnit>> { mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual };
            lists.ForEach(l => l.ForEach(u => u.Kill(action.Id)));
            // the first in the pair is the id, the second is the TTL
            mKilled.Add(new Pair<int, int>(action.Id, Math.Max(mBuildingResources.Count, mRefiningOrStoringResources.Count)));
        }

        public void Kill(GeneralUnit unit)
        {
            unit.Die();
            var lists = new List<List<GeneralUnit>> {mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual};
            lists.ForEach(l => l.Remove(unit));
        }
}
}
