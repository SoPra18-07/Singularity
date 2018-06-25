﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.Exceptions;
using Singularity.Graph;
using Singularity.Graph.Paths;
using Singularity.Map;
using Singularity.Platform;
using Singularity.Resources;
using Singularity.Units;
using Singularity.Utils;

namespace Singularity.DistributionManager
{
    [DataContract()]
    public class DistributionManager
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
        private Random mRandom;


        [DataMember]
        private List<BuildBluePrint> mBlueprintBuilds;



        [DataMember]
        private List<IPlatformAction> mPlatformActions;

        [DataMember]
        private List<Pair<PlatformBlank, int>> mProdPlatforms;
        [DataMember]
        private List<Pair<PlatformBlank, int>> mDefPlatforms;

        // Alternativ könnte man auch bei den beiden Listen direkt die Platformen einsetzen?
        // Momentan ja, aber wenn du ne plattform haben willst die (rein theoretisch) verteidigen und Produzieren gleichzeitig kann? Oder gleichzeitig KineticDefense und LaserDefense ist?
        // Aber wollen wir das? also entweder so, oder halt wie oben vorgeschlagen.
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

            //Other stuff
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
                mDefPlatforms.Add(new Pair<PlatformBlank, int>(platform, 0));
                var times = mDefense.Count / mDefPlatforms.Count;
                //Make sure the new platform gets some units
                NewlyDistribute(platform, true);
            }
            else
            {
                mProdPlatforms.Add(new Pair<PlatformBlank, int>(platform, 0));
                var times = mProduction.Count / mProdPlatforms.Count;
                //Make sure the new platform gets some units
                NewlyDistribute(platform, false);
            }
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
                //Only implement the substract and add function.
            }
            else
            {
                for (var i = amount; i >= 0; i--)
                {
                    if (oldlist.Count == 0)
                    {
                        break;
                    }

                    //Pick a random Unit and change its job
                    var rand = mRandom.Next(0, oldlist.Count);
                    var unassigned = oldlist.ElementAt(rand);
                    unassigned.ChangeJob(newj);
                    newlist.Add(unassigned);
                }
            }
        }

        public void NewlyDistribute(PlatformBlank platform, bool isDefense)
        {
            if (isDefense)
            {
                var search = true;
                var reachend = false;
                int startindex = 0;
                var lowassign = mDefense.Count + 1;
                //this will always be overridden this way                                        
                //The idea is to search the list backwards and try to find the platform with more units than the previous platform                 
                //Given that the units have been distributed fairly, we can now decrement units from there.    
                //If we reach the end that way, we have to continue decrementing the units from the end.
                for (var i = mDefPlatforms.Count - 2; i >= 0 && search; i--)
                {
                    if (i == 0)
                    {
                        search = false;
                        startindex = mDefPlatforms.Count - 1;
                    }

                    //Relys on fairness
                    if (lowassign <= mDefPlatforms[i].GetSecond())
                    {
                        lowassign = mDefPlatforms[i].GetSecond();
                    }
                    else
                    {
                        //Found the place to decrement units
                        lowassign = mDefPlatforms[i].GetSecond();
                        search = false;
                        startindex = i + 1;
                    }
                }


                int amount = mDefense.Count / mDefPlatforms.Count;

                //The transfer itself starts here
                for (var i = amount; i > 0; i--)
                {
                    //This means there are no Units to distribute
                    if (mDefPlatforms[startindex].GetSecond() == 0)
                    {
                        return;
                    }

                    var units = mDefPlatforms[startindex].GetFirst().GetAssignedUnits();
                    var transferunit = units[JobType.Defense].First();
                    transferunit.ChangeHome(new Task(JobType.Defense, platform, null, null));
                }
            }
            else
            {
                var search = true;
                var reachend = false;
                int startindex = 0;
                var lowassign = mProduction.Count + 1;
                //this will always be overridden this way                                        
                //The idea is to search the list backwards and try to find the platform with more units than the previous platform                 
                //Given that the units have been distributed fairly, we can now decrement units from there.    
                //If we reach the end that way, we have to continue decrementing the units from the end.
                for (var i = mProdPlatforms.Count - 2; i >= 0 && search; i--)
                {
                    if (i == 0)
                    {
                        search = false;
                        startindex = mProdPlatforms.Count - 1;
                    }

                    //Relys on fairness
                    if (lowassign <= mProdPlatforms[i].GetSecond())
                    {
                        lowassign = mProdPlatforms[i].GetSecond();
                    }
                    else
                    {
                        //Found the place to decrement units
                        lowassign = mProdPlatforms[i].GetSecond();
                        search = false;
                        startindex = i + 1;
                    }
                }


                int amount = mProduction.Count / mProdPlatforms.Count;

                //The transfer itself starts here
                for (var i = amount; i > 0; i--)
                {
                    //This means there are no Units to distribute
                    if (mProdPlatforms[startindex].GetSecond() == 0)
                    {
                        return;
                    }

                    var units = mProdPlatforms[startindex].GetFirst().GetAssignedUnits();
                    if (startindex - 1 == 0)
                    {
                        startindex = mProdPlatforms.Count - 2;
                    }
                    else
                    {
                        startindex--;
                    }
                    var transferunit = units[JobType.Production].First();
                    transferunit.ChangeHome(new Task(JobType.Production, platform, null, null));
                }
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

        // Okay yes you're right. We want a PlatformAction here instead of a platform.
        public void RequestResource(PlatformBlank platform, EResourceType resource, IPlatformAction action, bool isbuilding = false) 
        {
            // Will repair request ressources or units? And what unit will be used?
            // We do not have repair yet or anytime soon.
            // In that case I guess Ill ignore it for now.
            //TODO: Create Action references, when interfaces were created.
            if (isbuilding)
            {
                mBuildingResources.Enqueue(new Task(JobType.Construction, platform, resource, action));
            }
            else
            {
                mRefiningOrStoringResources.Enqueue(new Task(JobType.Logistics, platform, resource, action));
            }
        }

        public void RequestUnits(PlatformBlank platform, JobType job, IPlatformAction action, bool isdefending = false)
        {
            if (isdefending)
            {
                //Assure fairness
                if (platform.GetAssignedUnits().Count <= mDefense.Count / mDefPlatforms.Count)
                {
                    mRequestedUnitsDefense.Enqueue(new Task(JobType.Construction, platform, null, action));
                }
            }
            else
            {
                //Assure fairness
                if (platform.GetAssignedUnits().Count <= mProduction.Count / mProdPlatforms.Count)
                {
                    mRequestedUnitsProduce.Enqueue(new Task(JobType.Production, platform, null, action));
                }
            }
        }

        // Do we even need that? I think the units should do that - huh? no this was supposed to be from platformId to Resources on that platform, primarily for internal use when searching resources ... if you have actual platform-references all the better (you could probably get them from the producing (and factory) PlatformActions ...)
        public List<EResourceType> PlatformRequests(PlatformBlank platform)
        {
            throw new NotImplementedException();
            //return platform.GetPlatformResources();
        }

        // Why does this have to return a Task? It should only take it into the queue
        // and thats it, shouldnt it? Furthermore the platformaction shouldnt be optional. This is regarding the architecture.
        // The unit should be optional tho, you give the unit only if there are assigned units for the platform.
        //
        // Ah, I can see where your confusion is coming from. So, the version you're thinking about is absolutely valid but needs more (and better) coding. This would be the only 'bigger' function either way. Oh, and it'd absolutely need a different structure ...
        //
        // Okay, so how was it supposed to work (in my version, if you want to implement it is for you to decide):
        // - The units (with nothing to do (idle, but not 'JobType: Idle') ask for new Tasks here. So what is needed is ... actually yes, unit is not required. So the JobType is required, to return a Task of that JobType. Also, if this unit is assigned to some specific PlatformAction (like building a Blueprint, Logistics for a certain Factory, ...), it is supposed to only get Tasks involving this PlatformAction. However, if a unit is not manually assigned somewhere, what action do you want to get here? 
        public Task RequestNewTask(GeneralUnit unit, JobType job, Optional<IPlatformAction> assignedAction)
        {
            var nodes = new List<INode>();
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
                    var rndnmbr = mRandom.Next(0, nodes.Count);
                    //Just give them the inside of the Optional action witchout checking because
                    //it doesnt matter anyway if its null if the unit is idle.
                    return new Task(job, (PlatformBlank) nodes.ElementAt(rndnmbr), null, assignedAction.Get());

                case JobType.Production:
                    return mRequestedUnitsProduce.Dequeue();

                case JobType.Defense:
                    return mRequestedUnitsDefense.Dequeue();

                case JobType.Construction:
                    return mBuildingResources.Dequeue();

                case JobType.Logistics:
                    return mRefiningOrStoringResources.Dequeue();

                default:
                    throw new InvalidGenericArgumentException("Your requested JobType does not exist.");
            }
        }

        public void PausePlatformAction(IPlatformAction action)
        {
            throw new NotImplementedException();
            // Actions need a sleep method
            // No, they're just being removed from occurences in the DistributionManager. As soon as they unpause, they'll send requests for Resources and units again.
            // Ah ok I got that part
        }
    }
}
