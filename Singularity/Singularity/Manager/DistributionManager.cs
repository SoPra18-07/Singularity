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

        [DataMember]
        private int mGraphId;


        public DistributionManager(int graphid)
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

            //Actionlists
            mBlueprintBuilds = new List<BuildBluePrint>();
            mPlatformActions = new List<IPlatformAction>();

            //Lists for observing unit counts on platforms
            mProdPlatforms = new List<Pair<PlatformBlank, int>>();
            mDefPlatforms = new List<Pair<PlatformBlank, int>>();

            mRandom = new Random();
            mKilled = new List<Pair<int, int>>();
            mGraphId = graphid;
        }

        public void TestAttributes()
        {
            Console.Out.WriteLine(mProduction.Count);
            Console.Out.WriteLine(mIdle.Count);
            Console.Out.WriteLine(mProdPlatforms[1].GetFirst().mType + " " + mProdPlatforms[1].GetSecond());
            Console.Out.WriteLine(mProdPlatforms[0].GetFirst().mType + " " + mProdPlatforms[0].GetSecond());
        }

        /// <summary>
        /// Deactivates tasks requested by a certain action.
        /// </summary>
        /// <param name="removeaction">The Action I was talking about</param>
        private void DeactivateAction(IPlatformAction removeaction)
        {
            mPlatformActions.Remove(removeaction);
        }

        #region Looking for Resources

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
            currentlevel.Add(destination);

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
                        if (previouslevel.Contains(platform) || nextpreviouslevel.Contains(platform))
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

                        nextlevel.Add(candidatePlatform);
                    }

                    foreach (var edge in platform.GetOutwardsEdges())
                    {
                        var candidatePlatform = (PlatformBlank)edge.GetChild();
                        //If true, we have already visited this platform
                        if (previouslevel.Contains(platform) || nextpreviouslevel.Contains(platform))
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
                        nextlevel.Add(candidatePlatform);
                    }
                    //mark that you have visited this platform now
                    nextpreviouslevel.Add(platform);
                }

                //Update levels
                previouslevel = nextpreviouslevel;
                nextpreviouslevel = new List<PlatformBlank>();
                currentlevel = nextlevel;
                nextlevel = new List<PlatformBlank>();
            }
            return null;
        }

        #endregion

        #region Getters and Setters
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
        /// Gets the Lists of unit in that Job.
        /// </summary>
        /// <param name="job">The job I was talking about</param>
        /// <returns>The list I was talking about</returns>
        internal List<GeneralUnit> GetJobUnits(JobType job)
        {
            switch (job)
            {
                case JobType.Idle:
                    return mIdle;
                case JobType.Production:
                    return mProduction;
                case JobType.Construction:
                    return mConstruction;
                case JobType.Defense:
                    return mDefense;
                case JobType.Logistics:
                    return mLogistics;
                case JobType.Manual:
                    return mManual;
                default:
                    throw new InvalidGenericArgumentException("There is no such Job!");
            }
        }

        /// <summary>
        /// Sets the Joblists of this DistributionManager
        /// </summary>
        /// <param name="joblist">The list to set</param>
        /// <param name="job">The job</param>
        internal void SetJobUnits(List<GeneralUnit> joblist, JobType job)
        {
            switch (job)
            {
                case JobType.Idle:
                    mIdle = joblist;
                    break;
                case JobType.Production:
                    mProduction = joblist;
                    break;
                case JobType.Construction:
                    mConstruction = joblist;
                    break;
                case JobType.Defense:
                    mDefense = joblist;
                    break;
                case JobType.Logistics:
                    mLogistics = joblist;
                    break;
                case JobType.Manual:
                    mManual = joblist;
                    break;
                default:
                    throw new InvalidGenericArgumentException("There is no such Job!");
            }
        }

        /// <summary>
        /// Sets the Tasks of this DM
        /// </summary>
        /// <param name="taskqueue">The tasks to set</param>
        /// <param name="isBuilding">True if you want to set the Buildingtasks, false if you want to set the Transporttasks</param>
        internal void SetTasks(Queue<Task> taskqueue, bool isBuilding)
        {
            if (isBuilding)
            {
                mBuildingResources = taskqueue;
            }

            mRefiningOrStoringResources = taskqueue;

        }

        /// <summary>
        /// Return the Tasks this DistributionManager received and saved.
        /// </summary>
        /// <param name="isBuilding">True if you want the Build-Tasks, false otherwise</param>
        /// <returns>The Build-Tasks or the Transport-Tasks</returns>
        internal Queue<Task> GetTasks(bool isBuilding)
        {
            if (isBuilding)
            {
                return mBuildingResources;
            }

            return mRefiningOrStoringResources;
        }

        /// <summary>
        /// Get the Platforms this DistributionManager is observing.
        /// </summary>
        /// <param name="isDef">True if you want to get the Defending platforms, false if you want to get the Producing platforms</param>
        /// <returns>A list containing the platforms you requested paired with how many units work there</returns>
        internal List<Pair<PlatformBlank, int>> GetPlatforms(bool isDef)
        {
            if (isDef)
            {
                return mDefPlatforms;
            }

            return mProdPlatforms;
        }

        /// <summary>
        /// Set the Platforms this DistributionManager is observing
        /// </summary>
        /// <param name="platformlist">The list you want to set it to. Has to contain pairs with how many units work on the corresponding platform</param>
        /// <param name="isDef">True if you want to set the Defending platforms, false if you want to set the Producing platforms</param>
        internal void SetPlatforms(List<Pair<PlatformBlank, int>> platformlist, bool isDef)
        {
            if (isDef)
            {
                mDefPlatforms = platformlist;
            }

            mProdPlatforms = platformlist;
        }

        /// <summary>
        /// Used to determine whether there are no defending or producing platforms.
        /// </summary>
        /// <param name="isDefense">A bool to determine whether you are asking for defending or producing platforms.</param>
        /// <returns>False if there are such platforms, true otherwise</returns>
        public bool GetRestrictions(bool isDefense)
        {
            if (isDefense)
            {
                return mDefPlatforms.Count == 0;
            }

            return mProdPlatforms.Count == 0;
        }

        /// <summary>
        /// Get the total amount of Units INDIRECTLY assigned in a Job.
        /// </summary>
        /// <param name="job">The job I was talking about.</param>
        /// <returns>The total amount of Units Inridectly assigned in the given job, as an int.</returns>
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
                        "There are no other Jobs! Or at least there weren't any when this was coded...");
            }
        }
        #endregion

        #region Requesting Tasks and Resources

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
                mBuildingResources.Enqueue(new Task(JobType.Construction, Optional<PlatformBlank>.Of(platform), resource, Optional<IPlatformAction>.Of(action)));
            }
            else
            {
                mRefiningOrStoringResources.Enqueue(new Task(JobType.Logistics, Optional<PlatformBlank>.Of(platform), resource, Optional<IPlatformAction>.Of(action)));
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
                    return new Task(job, Optional<PlatformBlank>.Of((PlatformBlank)nodes.ElementAt(rndnmbr)), null, assignedAction);

                case JobType.Production:
                    throw new InvalidGenericArgumentException("You shouldnt ask for Production tasks, you just assign units to production.");

                case JobType.Defense:
                    throw new InvalidGenericArgumentException("You shouldnt ask for Defense tasks, you just assign units to defense.");

                case JobType.Construction:
                    if (mBuildingResources.Count == 0)
                    {
                        var nulltask = new Task(JobType.Logistics,
                            Optional<PlatformBlank>.Of(null),
                            null,
                            Optional<IPlatformAction>.Of(null));
                        nulltask.Begin = Optional<PlatformBlank>.Of(null);
                        return nulltask;
                    }
                    task = mBuildingResources.Dequeue();
                    //This means that the Action is paused.
                    if (task.Action.IsPresent() && !mPlatformActions.Contains(task.Action.Get()))
                    {
                        mBuildingResources.Enqueue(task);
                        task = RequestNewTask(unit, job, assignedAction);
                    }
                    if (task.End.IsPresent() && task.GetResource != null)
                    {
                        var begin = FindBegin(task.End.Get(), (EResourceType)task.GetResource);
                        //Use BFS to find the place you want to get your resources from
                        if (begin != null)
                        {
                            task.Begin = Optional<PlatformBlank>.Of(begin);
                        }
                        else
                        {
                            //TODO: Talk with felix about how this could affect the killing thing
                            mBuildingResources.Enqueue(task);
                            //This means the unit will identify this task as "do nothing" and ask again.
                            task.Begin = null;
                        }
                    }
                    else
                    {
                        throw new InvalidGenericArgumentException("There is a task in your queue that is faulty. Check the RequestResource method!!!");
                    }
                    break;

                case JobType.Logistics:
                    if (mRefiningOrStoringResources.Count == 0)
                    {
                        var nulltask = new Task(JobType.Logistics,
                            Optional<PlatformBlank>.Of(null),
                            null,
                            Optional<IPlatformAction>.Of(null));
                        nulltask.Begin = Optional<PlatformBlank>.Of(null);
                        return nulltask;
                    }
                    task = mRefiningOrStoringResources.Dequeue();
                    //This means that the Action is paused.
                    if (task.Action.IsPresent() && !mPlatformActions.Contains(task.Action.Get()))
                    {
                        mRefiningOrStoringResources.Enqueue(task);
                        task = RequestNewTask(unit, job, assignedAction);
                    }
                    if (task.End.IsPresent() && task.GetResource != null)
                    {
                        var begin = FindBegin(task.End.Get(), (EResourceType)task.GetResource);
                        //Use BFS to find the place you want to get your resources from
                        if (begin != null)
                        {
                            task.Begin = Optional<PlatformBlank>.Of(begin);
                        }
                        else
                        {
                            //TODO: Talk with felix about how this could affect the killing thing
                            mRefiningOrStoringResources.Enqueue(task);
                            //This means the unit will identify this task as "do nothing" and ask again.
                            task.Begin = null;
                        }
                    }
                    else
                    {
                        throw new InvalidGenericArgumentException("There is a task in your queue that is faulty. Check the RequestResource method!!!");
                    }
                    break;

                default:
                    throw new InvalidGenericArgumentException("Your requested JobType does not exist.");
            }

            mKilled = mKilled.Select(p => new Pair<int, int>(p.GetFirst(), p.GetSecond() - 1)).ToList();
            if (mKilled != null)
            {
                mKilled.RemoveAll(p => p.GetSecond() < 0);
                return mKilled.TrueForAll(p => !task.Contains(p.GetFirst())) ? task : RequestNewTask(unit, job, assignedAction);
            }

            return task;
        }



        #endregion

        #region Unit Distribution and Assigning Units to Jobs

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
        /// If there are no platforms for production or defense, the job will stay Idle afterwards
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

            if (list.Count == 0)
            {
                //Dont let them change the job to production or defense when there are no platforms to work on.
                foreach (var unit in toassign)
                {
                    mIdle.Add(unit);
                }
                return;
            }

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
        /// A method called by the DistributionManager in case a new platform is registered, then it will redistribute all the units.
        /// BE SURE ALL THE UNITS THAT ARE ON THE PLATFORM ARE REGISTERED IN THE DISTRIBUTIONMANAGER OTHERWISE THIS WILL FAIL!
        /// </summary>
        /// <param name="platform">The newly registered platform</param>
        /// <param name="isDefense">Is true if its a DefensePlatform, false otherwise</param>
        /// <param name="alreadyonplatform">The amount of units already working on the platform</param>
        private void NewlyDistribute(PlatformBlank platform, bool isDefense, int alreadyonplatform)
        {
            var job = isDefense ? JobType.Defense : JobType.Production;
            var joblist = isDefense ? mDefense : mProduction;
            if (isDefense)
            {
                // + 1 because we didnt add the new platform yet
                var times = mDefense.Count / (mDefPlatforms.Count + 1);
                if (times > alreadyonplatform)
                {
                    times = times - alreadyonplatform;

                    var list = GetUnitsFairly(times, mDefPlatforms, true);

                    foreach (var unit in list)
                    {
                        //We have to re-add the units to the job list because GetUnitsFairly did unassign them
                        joblist.Add(unit);
                        //Also unassigns the unit.
                        unit.AssignTask(new Task(JobType.Defense, Optional<PlatformBlank>.Of(platform), null, Optional<IPlatformAction>.Of(null)));
                    }

                    mDefPlatforms.Add(new Pair<PlatformBlank, int>(platform, list.Count));
                }
                else if (times < alreadyonplatform)
                {
                    times = alreadyonplatform - times;
                    var units = new List<GeneralUnit>();
                    GeneralUnit transferunit;
                    for (var i = times; i > 0; i--)
                    {
                        transferunit = platform.GetAssignedUnits()[job].First().GetFirst();
                        units.Add(transferunit);
                        mIdle.Add(transferunit);
                        joblist.Remove(transferunit);
                        transferunit.ChangeJob(JobType.Idle);
                    }
                    AssignUnitsFairly(units, true);
                }
                else
                {
                    mDefPlatforms.Add(new Pair<PlatformBlank, int>(platform, alreadyonplatform));
                }
            }
            else
            {
                // + 1 because we didnt add the new platform yet
                var times = mProduction.Count / (mProdPlatforms.Count + 1);
                if (times > alreadyonplatform)
                {
                    times = times - alreadyonplatform;

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
                else if (times < alreadyonplatform)
                {
                    times = alreadyonplatform - times;
                    var units = new List<GeneralUnit>();
                    GeneralUnit transferunit;
                    for (var i = times; i > 0; i--)
                    {
                        transferunit = platform.GetAssignedUnits()[job].First().GetFirst();
                        units.Add(transferunit);
                        mIdle.Add(transferunit);
                        joblist.Remove(transferunit);
                        transferunit.ChangeJob(JobType.Idle);
                    }
                    AssignUnitsFairly(units, false);
                }
                else
                {
                    mProdPlatforms.Add(new Pair<PlatformBlank, int>(platform, alreadyonplatform));
                }
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
                    throw new InvalidGenericArgumentException("You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }

            //COLLECT THE UNITS
            List<GeneralUnit> list;
            if (job == JobType.Defense)
            {
                list = GetUnitsFairly(amount, mDefPlatforms, true);
            }
            else if (job == JobType.Production)
            {
                list = GetUnitsFairly(amount, mProdPlatforms, false);
            }
            else
            {
                list = new List<GeneralUnit>();
                for (var i = amount; i >= 0; i--)
                {
                    //Change the job of a random unit
                    var rand = mRandom.Next(0, oldlist.Count);
                    var removeit = oldlist.ElementAt(rand);
                    list.Add(removeit);
                }
            }

            foreach (var unit in list)
            {
                mManual.Add(unit);
                action.AssignUnit(unit, job);
                if (job == JobType.Production)
                {

                }
                else if (job == JobType.Defense)
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
            var list = action.UnAssignUnits(amount, job);
            foreach (var unit in list)
            {
                mManual.Remove(unit);
                unit.ChangeJob(JobType.Idle);
                mIdle.Add(unit);
            }

            if (mHandler != null)
            {
                mHandler.Refresh();
            }
        }

        #endregion

        #region Register or Unregister

        /// <summary>
        /// Is called by the unit when it is created.
        /// </summary>
        /// <param name="unit">the unit that has been created</param>
        public void Register(GeneralUnit unit)
        {
            mIdle.Add(unit);
        }

        /// <summary>
        /// Is called to add the unit with a certain job.
        /// </summary>
        /// <param name="unit">the unit that has been mentioned</param>
        /// <param name="job">the job of the unit.</param>
        public void Register(GeneralUnit unit, JobType job)
        {
            GetJobUnits(job).Add(unit);
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
        /// Is called by producing and defending Platforms when they are created or added to the distributionmanager.
        /// </summary>
        /// <param name="platform">The platform itself</param>
        /// <param name="isDef">Is true, when the platform is a defending platform, false otherwise (only producing platforms should register themselves besides defending ones)</param>
        public void Register(PlatformBlank platform, bool isDef)
        {
            var job = isDef ? JobType.Defense : JobType.Production;
            var joblist = isDef ? mDefense : mProduction;
            var alreadyonplatform = 0;
            if (platform.GetAssignedUnits()[job].Count > 0)
            {
                alreadyonplatform = platform.GetAssignedUnits()[job].Count;
            }

            //Make sure the units are added to their joblist if they arent already.
            foreach (var unitbool in platform.GetAssignedUnits()[job])
            {
                if (!joblist.Contains(unitbool.GetFirst()))
                {
                    joblist.Add(unitbool.GetFirst());
                }
            }
            if (isDef)
            {
                //Make sure the new platform gets some units
                NewlyDistribute(platform, true, alreadyonplatform);
            }
            else
            {
                //Make sure the new platform gets some units
                NewlyDistribute(platform, false, alreadyonplatform);
            }
        }

        /// <summary>
        /// Unregister platforms from the DistributionManager.
        /// This changes the graph the distributionmanager knows and is only needed for Producing or Defending platforms.
        /// </summary>
        /// <param name="platforms">The platforms you want to unregister.</param>
        /// <param name="isDef">True if they are defending platforms, false if they are producing platforms</param>
        /// /// <param name="inactivate">True if the platform is not truly removed from the graph but inactivated, false if it is removed from the graph</param>
        public void Unregister(List<PlatformBlank> platforms, bool isDef, bool inactivate)
        {
            var list = isDef ? mDefPlatforms : mProdPlatforms;
            var job = isDef ? JobType.Defense : JobType.Production;
            var joblist = isDef ? mDefense : mProduction;
            var unitstodistribute = new List<GeneralUnit>();
            Pair<PlatformBlank, int> pair;

            //REMOVE PLATFORMS, COLLECT THEIR UNITS
            foreach (var platform in platforms)
            {
                //No longer observe platform
                pair = list.Find(x => x.GetFirst().Equals(platform));
                list.Remove(pair);

                var units = platform.GetAssignedUnits()[job];
                //In this case just collect every unit on the platform and redistribute it (redistribution later in code).
                if (inactivate)
                {
                    foreach (var unitbool in new List<Pair<GeneralUnit, bool>>(units))
                    {
                        unitstodistribute.Add(unitbool.GetFirst());
                        joblist.Remove(unitbool.GetFirst());
                        //Unassigns itself from its platform
                        unitbool.GetFirst().ChangeJob(JobType.Idle);
                    }
                }
                //In this case look if every assigned unit is on the same graph as the platform and handle it
                else
                {
                    foreach (var unitbool in units)
                    {
                        //In this case they are on the platform
                        if (unitbool.GetSecond())
                        {
                            //Remove unit from this joblist to no longer influence this unit.
                            joblist.Remove(unitbool.GetFirst());
                        }
                        else
                        {
                            //It will arrive, so just handle it as if it had already arrived at the platform
                            if (unitbool.GetFirst().Graphid == platform.GetGraphIndex())
                            {
                                joblist.Remove(unitbool.GetFirst());
                            }
                            //It will not arrive, so it has to be redistributed!
                            else
                            {
                                unitstodistribute.Add(unitbool.GetFirst());
                                //Unassigns itself from its platform
                                unitbool.GetFirst().ChangeJob(JobType.Idle);
                            }
                        }
                    }

                    // the first in the pair is the id, the second is the TTL
                    // we tell the manager we killed the platform, but we didnt really kill it.
                    mKilled.Add(new Pair<int, int>(platform.Id, Math.Max(mBuildingResources.Count, mRefiningOrStoringResources.Count)));
                }
            }
            //If there are any units to redistribute do it now
            AssignUnitsFairly(unitstodistribute, isDef);
            mHandler.ForceSliderPages();

        }
        #endregion

        public void PausePlatformAction(IPlatformAction action)
        {
            // throw new NotImplementedException();
            // Actions need a sleep method
            // No, they're just being removed from occurences in the DistributionManager. As soon as they unpause, they'll send requests for Resources and units again.
            // Ah ok I got that part
        }


        #region Killing
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
            if (action is BuildBluePrint)
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
            var lists = new List<List<GeneralUnit>> {mIdle, mLogistics, mConstruction, mProduction, mDefense, mManual};
            lists.ForEach(l => l.Remove(unit));
        }
#endregion
    }
}
