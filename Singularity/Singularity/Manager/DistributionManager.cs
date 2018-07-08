using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.PlatformActions;
using Singularity.Platforms;
using Singularity.Resources;
using Singularity.Screen;
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
        private List<Pair<int, int>> mKilled;

        [DataMember]
        private Random mRandom;

        [DataMember]
        private SliderHandler mHandler;

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

            //Other stuff -- ???
            mBlueprintBuilds = new List<BuildBluePrint>();
            mPlatformActions = new List<IPlatformAction>();
            mProdPlatforms = new List<Pair<PlatformBlank, int>>();
            mDefPlatforms = new List<Pair<PlatformBlank, int>>();
            mRandom = new Random();
            mKilled = new List<Pair<int, int>>();
        }

        /// <summary>
        /// Is called by the unit when it is created.
        /// </summary>
        /// <param name="unit">the unit that has been created</param>
        public void Register(GeneralUnit unit)
        {
            mIdle.Add(item: unit);
        }

        /// <summary>
        /// Gets the total amount of Units INDIRECTLY assigned. So this will not include directly assigned units.
        /// </summary>
        public int GetUnitTotal()
        {
            var total = 0;
            total += mIdle.Count;
            total += mProduction.Count;
            total += mConstruction.Count;
            total += mDefense.Count;
            total += mLogistics.Count;
            return total;
        }

        /// <summary>
        /// Get the total amount of Units INDIRECTLY assigned in a Job.
        /// </summary>
        /// <returns></returns>
        public int GetJobCount(JobType job)
        {
            switch (job)
            {
                case JobType.Idle:
                    return mIdle.Count;
                case JobType.Construction:
                    return mConstruction.Count;
                case JobType.Defense:
                    return mDefense.Count;
                case JobType.Logistics:
                    return mLogistics.Count;
                case JobType.Production:
                    return mProduction.Count;
                default:
                    throw new InvalidGenericArgumentException(
                        message: "There are no other Jobs! Or at least there weren't any when this was coded...");
            }
        }

        /// <summary>
        /// This will be called from the SliderHandler when its created.
        /// It just registers its reference, so the DistributionManager can communicate with it.
        /// </summary>
        /// <param name="handler"></param>
        internal void Register(SliderHandler handler)
        {
            mHandler = handler;
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
                NewlyDistribute(platform: platform, isDefense: true);
            }
            else
            {
                //Make sure the new platform gets some units
                NewlyDistribute(platform: platform, isDefense: false);
            }
        }

        public void TestAttributes()
        {
            Console.Out.WriteLine(value: mProduction.Count);
            Console.Out.WriteLine(value: mIdle.Count);
            Console.Out.WriteLine(value: mProdPlatforms[index: 1].GetFirst().mType + " " + mProdPlatforms[index: 1].GetSecond());
            Console.Out.WriteLine(value: mProdPlatforms[index: 0].GetFirst().mType + " " + mProdPlatforms[index: 0].GetSecond());
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
                    throw new InvalidGenericArgumentException(message: "You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");

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
                    throw new InvalidGenericArgumentException(message: "You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }

            //Production and Defense have to be distributed differently than the other jobs because we want to assure fairness
            if (oldj == JobType.Production || oldj == JobType.Defense)
            {
                List<GeneralUnit> list;
                //Get the units to change jobs here
                if (oldj == JobType.Production)
                {
                    list = GetUnitsFairly(amount: amount, list: mProdPlatforms, isDefense: false);
                }
                else
                {
                    list = GetUnitsFairly(amount: amount, list: mDefPlatforms, isDefense: false);
                }

                //Now actually change their Jobs.
                if (newj == JobType.Production)
                {
                    AssignUnitsFairly(toassign: list, isDefense: false);
                }else if (newj == JobType.Defense)
                {
                    AssignUnitsFairly(toassign: list, isDefense: true);
                }
                else
                {
                    foreach (var unit in list)
                    {
                        newlist.Add(item: unit);
                        unit.ChangeJob(job: newj);
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
                    var rand = mRandom.Next(minValue: 0, maxValue: oldlist.Count);
                    var unassigned = oldlist.ElementAt(index: rand);
                    unassigned.ChangeJob(job: JobType.Idle);
                    oldlist.Remove(item: unassigned);
                    list.Add(item: unassigned);
                }

                if (newj == JobType.Production)
                {
                    AssignUnitsFairly(toassign: list, isDefense: false);
                }
                else if (newj == JobType.Defense)
                {
                    AssignUnitsFairly(toassign: list, isDefense: true);
                }
                else
                {
                    foreach (var unit in list)
                    {
                        newlist.Add(item: unit);
                        unit.ChangeJob(job: newj);
                    }
                }
            }

            TestAttributes();
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
                if (lowassign >= list[index: i].GetSecond())
                {
                    lowassign = list[index: i].GetSecond();
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
                if (list[index: startindex].GetSecond() == 0)
                {
                    return units;
                }

                var platUnits = list[index: startindex].GetFirst().GetAssignedUnits();

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
                var transferunit = platUnits[key: job].First().GetFirst();
                units.Add(item: transferunit);
                joblist.Remove(item: transferunit);
                var number = list[index: startindex].GetSecond() - 1;
                list[index: startindex] = new Pair<PlatformBlank, int>(firstValue: list[index: startindex].GetFirst(), secondValue: number);
                transferunit.ChangeJob(job: JobType.Idle);

                //Decrement index or begin at the right end again
                if (startindex == 0)
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
                if (highassign <= list[index: i].GetSecond())
                {
                    highassign = list[index: i].GetSecond();
                }
                else
                {
                    //Found the place to increment units
                    highassign = list[index: i].GetSecond();
                    search = false;
                    startindex = i;
                }
            }

            //ADD THE GIVEN UNITS/GIVE THEM THE TASKS
            foreach (var unit in toassign)
            {
                var goTo = list[index: startindex].GetFirst();
                if (job == JobType.Defense)
                {
                    mDefense.Add(item: unit);
                }
                else
                {
                    mProduction.Add(item: unit);
                }
                //The unit will change its job when calling this
                unit.AssignTask(task: new Task(job: job, end: Optional<PlatformBlank>.Of(value: goTo), res: null, action: Optional<IPlatformAction>.Of(value: null)));
                list[index: startindex] = new Pair<PlatformBlank, int>(firstValue: goTo, secondValue: list[index: startindex].GetSecond() + 1);
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
        {
            if (isDefense)
            {
                // + 1 because we didnt add the new platform yet
                var times = mDefense.Count / (mDefPlatforms.Count + 1);
                var list = GetUnitsFairly(amount: times, list: mDefPlatforms, isDefense: false);

                foreach (var unit in list)
                {
                    //We have to re-add the units to the job list because GetUnitsFairly did unassign them
                    mDefense.Add(item: unit);
                    //Also unassigns the unit.
                    unit.AssignTask(task: new Task(job: JobType.Defense, end: Optional<PlatformBlank>.Of(value: platform), res: null, action: Optional<IPlatformAction>.Of(value: null)));
                }

                mDefPlatforms.Add(item: new Pair<PlatformBlank, int>(firstValue: platform, secondValue: list.Count));
            }
            else
            {
                // + 1 because we didnt add the new platform yet
                var times = mProduction.Count / (mProdPlatforms.Count + 1);
                var list = GetUnitsFairly(amount: times, list: mProdPlatforms, isDefense: false);

                foreach (var unit in list)
                {
                    //We have to re-add the units to the job list because GetUnitsFairly did unassign them
                    mProduction.Add(item: unit);
                    //Also unassigns the unit.
                    unit.AssignTask(task: new Task(job: JobType.Production, end: Optional<PlatformBlank>.Of(value: platform), res: null, action: Optional<IPlatformAction>.Of(value: null)));
                }

                mProdPlatforms.Add(item: new Pair<PlatformBlank, int>(firstValue: platform, secondValue: list.Count));
            }
        }

        /// <summary>
        /// Manually Assign Units to a certain PlatformAction.
        /// </summary>
        /// <param name="amount">The amount of units to be assigned</param>
        /// <param name="action">The action to which the units shall be assigned</param>
        /// <param name="job">The Job the units had/are supposed to have.</param>
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
                    oldlist = mProduction;
                    break;
                case JobType.Defense:
                    oldlist = mDefense;
                    break;
                case JobType.Logistics:
                    oldlist = mLogistics;
                    break;
                default:
                    throw new InvalidGenericArgumentException(message: "You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }

            //COLLECT THE UNITS
            List<GeneralUnit> list;
            if (job == JobType.Defense)
            {
                list = GetUnitsFairly(amount: amount, list: mDefPlatforms, isDefense: true);
            }else if (job == JobType.Production)
            {
                list = GetUnitsFairly(amount: amount, list: mProdPlatforms, isDefense: false);
            }
            else
            {
                list = new List<GeneralUnit>();
                for (var i = amount; i >= 0; i--)
                {
                    //Change the job of a random unit
                    var rand = mRandom.Next(minValue: 0, maxValue: oldlist.Count);
                    var removeit = oldlist.ElementAt(index: rand);
                    list.Add(item: removeit);
                }
            }

            foreach (var unit in list)
            {
                mManual.Add(item: unit);
                action.AssignUnit(unit: unit, job: job);
                if (job == JobType.Production)
                {

                } else if (job == JobType.Defense)
                {

                }
            }

            if (mHandler != null)
            {
                mHandler.Refresh();
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
            var list = action.UnAssignUnits(amount: amount, job: job);
            foreach (var unit in list)
            {
                mManual.Remove(item: unit);
                unit.ChangeJob(job: JobType.Idle);
                mIdle.Add(item: unit);
            }

            if (mHandler != null)
            {
                mHandler.Refresh();
            }
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
            //TODO: Create Action references, when interfaces were created.
            if (isbuilding)
            {
                mBuildingResources.Enqueue(item: new Task(job: JobType.Construction, end: Optional<PlatformBlank>.Of(value: platform), res: resource, action: Optional<IPlatformAction>.Of(value: action)));
            }
            else
            {
                mRefiningOrStoringResources.Enqueue(item: new Task(job: JobType.Logistics, end: Optional<PlatformBlank>.Of(value: platform), res: resource, action: Optional<IPlatformAction>.Of(value: action)));
            }
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
                        nodes.Add(item: edge.GetParent());
                    }
                    foreach (var edge in unit.CurrentNode.GetOutwardsEdges())
                    {
                        nodes.Add(item: edge.GetChild());
                    }

                    if (nodes.Count == 0)
                    {
                        //Could be very inefficient, since the Units will bombard the DistributionManager with asks for tasks when there is only one platform
                        //connected to theirs
                        nodes.Add(item: unit.CurrentNode);
                    }
                    var rndnmbr = mRandom.Next(minValue: 0, maxValue: nodes.Count);
                    //Just give them the inside of the Optional action witchout checking because
                    //it doesnt matter anyway if its null if the unit is idle.
                    return new Task(job: job, end: Optional<PlatformBlank>.Of(value: (PlatformBlank) nodes.ElementAt(index: rndnmbr)), res: null, action: assignedAction);

                case JobType.Production:
                    throw new InvalidGenericArgumentException(message: "You shouldnt ask for Production tasks, you just assign units to production.");

                case JobType.Defense:
                    throw new InvalidGenericArgumentException(message: "You shouldnt ask for Defense tasks, you just assign units to defense.");

                case JobType.Construction:
                    if (mBuildingResources.Count == 0)
                    {
                        var nulltask = new Task(job: JobType.Logistics,
                            end: Optional<PlatformBlank>.Of(value: null),
                            res: null,
                            action: Optional<IPlatformAction>.Of(value: null));
                        nulltask.Begin = Optional<PlatformBlank>.Of(value: null);
                        return nulltask;
                    }
                    task = mBuildingResources.Dequeue();
                    if (task.End.IsPresent() && task.GetResource != null)
                    {
                        var begin = FindBegin(destination: task.End.Get(), res: (EResourceType)task.GetResource);
                        //Use BFS to find the place you want to get your resources from
                        if (begin != null)
                        {
                            task.Begin = Optional<PlatformBlank>.Of(value: begin);
                        }
                        else
                        {
                            //TODO: Talk with felix about how this could affect the killing thing
                            mBuildingResources.Enqueue(item: task);
                            //This means the unit will identify this task as "do nothing" and ask again.
                            task.Begin = null;
                        }
                    }
                    else
                    {
                        throw new InvalidGenericArgumentException(message: "There is a task in your queue that is faulty. Check the RequestResource method!!!");
                    }
                    break;

                case JobType.Logistics:
                    if (mRefiningOrStoringResources.Count == 0)
                    {
                        var nulltask = new Task(job: JobType.Logistics,
                            end: Optional<PlatformBlank>.Of(value: null),
                            res: null,
                            action: Optional<IPlatformAction>.Of(value: null));
                        nulltask.Begin = Optional<PlatformBlank>.Of(value: null);
                        return nulltask;
                    }
                    task = mRefiningOrStoringResources.Dequeue();
                    if (task.End.IsPresent() && task.GetResource != null)
                    {
                        var begin = FindBegin(destination: task.End.Get(), res: (EResourceType)task.GetResource);
                        //Use BFS to find the place you want to get your resources from
                        if (begin != null)
                        {
                            task.Begin = Optional<PlatformBlank>.Of(value: begin);
                        }
                        else
                        {
                            //TODO: Talk with felix about how this could affect the killing thing
                            mRefiningOrStoringResources.Enqueue(item: task);
                            //This means the unit will identify this task as "do nothing" and ask again.
                            task.Begin = null;
                        }
                    }
                    else
                    {
                        throw new InvalidGenericArgumentException(message: "There is a task in your queue that is faulty. Check the RequestResource method!!!");
                    }
                    break;

                default:
                    throw new InvalidGenericArgumentException(message: "Your requested JobType does not exist.");
            }

            mKilled = mKilled.Select(selector: p => new Pair<int, int>(firstValue: p.GetFirst(), secondValue: p.GetSecond() - 1)).ToList();
            if (mKilled != null)
            {
                mKilled.RemoveAll(match: p => p.GetSecond() < 0);
                return mKilled.TrueForAll(match: p => !task.Contains(id: p.GetFirst())) ? task : RequestNewTask(unit: unit, job: job, assignedAction: assignedAction);
            }

            return task;
        }

        /// <summary>
        /// Uses BFS to find the nearest Platform holding a resource you want to get. Might be expensive...
        /// </summary>
        /// <param name="destination">The platform you start the bfs from.</param>
        /// <param name="res">The resourcetype to look for.</param>
        /// <returns>The platform with the desired resource, if it exists, null if not</returns>
        private PlatformBlank FindBegin(PlatformBlank destination, EResourceType res)
        {
            //We need these lists, because we are operating on a undirected graph. That means we have to ensure we dont go back in our BFS
            //this list contains platforms that cannot be visited next run.
            var previouslevel = new List<PlatformBlank>();
            //This list contains platforms that have been visited this run.
            var nextpreviouslevel = new List<PlatformBlank>();

            var currentlevel = new List<PlatformBlank>();
            currentlevel.Add(item: destination);

            var nextlevel = new List<PlatformBlank>();


            while (currentlevel.Count > 0)
            {
                //Create the next level of BFS. While doing this, check if any platform has the resource you want. If yes return it.
                foreach (PlatformBlank platform in currentlevel)
                {

                    foreach (var edge in platform.GetInwardsEdges())
                    {
                        var candidatePlatform = (PlatformBlank)edge.GetParent();
                        //If true, we have already visited this platform
                        if (previouslevel.Contains(item: platform) || nextpreviouslevel.Contains(item: platform))
                        {
                            continue;
                        }
                        //Check for the resource
                        foreach (var resource in candidatePlatform.GetPlatformResources())
                        {
                            if (resource.Type != res)
                            {
                                continue;
                            }
                            return candidatePlatform;
                        }

                        nextlevel.Add(item: candidatePlatform);
                    }

                    foreach (var edge in platform.GetOutwardsEdges())
                    {
                        var candidatePlatform = (PlatformBlank)edge.GetChild();
                        //If true, we have already visited this platform
                        if (previouslevel.Contains(item: platform) || nextpreviouslevel.Contains(item: platform))
                        {
                            continue;
                        }
                        //Check for the resource
                        foreach (var resource in candidatePlatform.GetPlatformResources())
                        {
                            if (resource.Type != res)
                            {
                                continue;
                            }
                            return candidatePlatform;
                        }
                        nextlevel.Add(item: candidatePlatform);
                    }
                    //mark that you have visited this platform now
                    nextpreviouslevel.Add(item: platform);
                }

                //Update levels
                previouslevel = nextpreviouslevel;
                nextpreviouslevel = new List<PlatformBlank>();
                currentlevel = nextlevel;
                nextlevel = new List<PlatformBlank>();
            }
            return null;
        }

        public void PausePlatformAction(IPlatformAction action)
        {
            throw new NotImplementedException();
            // Actions need a sleep method
            // No, they're just being removed from occurences in the DistributionManager. As soon as they unpause, they'll send requests for Resources and units again.
            // Ah ok I got that part
        }


        #region Killing
        public void Kill(PlatformBlank platform)
        {
            // Strong assumption that a platform is only listed at most once in either of these.
            mProdPlatforms.Remove(item: mProdPlatforms.Find(match: p => p.GetFirst().Equals(other: platform)));
            mDefPlatforms.Remove(item: mDefPlatforms.Find(match: p => p.GetFirst().Equals(other: platform)));
            var lists = new List<List<GeneralUnit>> { mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual };
            lists.ForEach(action: l => l.ForEach(action: u => u.Kill(id: platform.Id)));
            // the first in the pair is the id, the second is the TTL
            mKilled.Add(item: new Pair<int, int>(firstValue: platform.Id, secondValue: Math.Max(val1: mBuildingResources.Count, val2: mRefiningOrStoringResources.Count)));
        }

        public void Kill(IPlatformAction action)
        {
            // Strong assumption that a PlatformAction is only listed at most once here.
            if (action is BuildBluePrint)
            {
                mBlueprintBuilds.Remove(item: mBlueprintBuilds.Find(match: b => b.Equals(obj: action)));
            }
            else
            {
                mPlatformActions.Remove(item: mPlatformActions.Find(match: p => p.Equals(obj: action)));
            }

            var lists = new List<List<GeneralUnit>> { mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual };
            lists.ForEach(action: l => l.ForEach(action: u => u.Kill(id: action.Id)));
            // the first in the pair is the id, the second is the TTL
            mKilled.Add(item: new Pair<int, int>(firstValue: action.Id, secondValue: Math.Max(val1: mBuildingResources.Count, val2: mRefiningOrStoringResources.Count)));
        }

        public void Kill(GeneralUnit unit)
        {
            unit.Die();
            var lists = new List<List<GeneralUnit>> {mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual};
            lists.ForEach(action: l => l.Remove(item: unit));
        }
#endregion
    }
}
