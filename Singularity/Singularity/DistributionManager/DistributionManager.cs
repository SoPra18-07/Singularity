using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Singularity.Exceptions;
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
        [DataMember()]
        private List<GeneralUnit> mIdle;
        [DataMember()]
        private List<GeneralUnit> mLogistics;
        [DataMember()]
        private List<GeneralUnit> mConstruction;
        [DataMember()]
        private List<GeneralUnit> mProduction;
        [DataMember()]
        private List<GeneralUnit> mDefense;
        [DataMember()]
        private List<GeneralUnit> mManual;

        [DataMember()]
        private StructureMap mStructure;

        [DataMember()]
        private Queue<Task> mBuildingResources;
        [DataMember()]
        private Queue<Task> mRefiningOrStoringResources;
        [DataMember()]
        private Queue<Task> mRequestedUnitsProduce;
        [DataMember()]
        private Queue<Task> mRequestedUnitsDefense;

        [DataMember()]
        private Random mRandom;
        // An Felix: Vielleicht BuildBluePrint nicht in "ProduceResourceAction.cs" reinhauen (da hätte ich nicht danach gesucht) - muss ich eh nochmal refactorn mit PlatformBlank und jz dem hier
        [DataMember()]
        private List<BuildBluePrint> mBlueprintBuilds;

        // L:An der Stelle mit Felix reden, PlatformActionProduce als abstrakte Klasse würde helfen?
        // F:Mhm weiß nicht ob das wirklich notwendig ist ... ich mach mir mal gedanken
        // L:Zumindest ein interface würde benötigt, ich denke nicht dass der sinn hinter der sache ist für jede produzierende plattform ne extra liste mit
        // List<PlatformnamehiereinsetzenActionProduce> zu erstellen. Das gleiche mit mDefensivePlatforms
        [DataMember()]
        private List<IPlatformAction> mPlatformActions;

        // Alternativ könnte man auch bei den beiden Listen direkt die Platformen einsetzen?
        // Momentan ja, aber wenn du ne plattform haben willst die (rein theoretisch) verteidigen und Produzieren gleichzeitig kann? Oder gleichzeitig KineticDefense und LaserDefense ist?
        // Aber wollen wir das? also entweder so, oder halt wie oben vorgeschlagen.
        public DistributionManager(StructureMap structure)
        {
            mIdle = new List<GeneralUnit>();
            mLogistics = new List<GeneralUnit>();
            mConstruction = new List<GeneralUnit>();
            mProduction = new List<GeneralUnit>();
            mDefense = new List<GeneralUnit>();
            mManual = new List<GeneralUnit>();

            mStructure = structure;

            mBuildingResources = new Queue<Task>();
            mRefiningOrStoringResources = new Queue<Task>();
            mRequestedUnitsProduce = new Queue<Task>();
            mRequestedUnitsDefense = new Queue<Task>();
            mBlueprintBuilds = new List<BuildBluePrint>();
            mPlatformActions = new List<IPlatformAction>();
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
                    oldlist = mDefense;
                    break;
                case JobType.Defense:
                    oldlist = mProduction;
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
                    newlist = mDefense;
                    break;
                case JobType.Defense:
                    newlist = mProduction;
                    break;
                default:
                    throw new InvalidGenericArgumentException("You have to use a JobType of Idle, Production, Logistics, Construction or Defense.");
            }
            for (var i = amount; i >= 0; i--)
            {
                if (oldlist.Count == 0)
                {
                    break;
                }
                var unassigned = oldlist.First();
                unassigned.ChangeJob(newj);
                newlist.Add(unassigned);
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
                var removeit = oldlist.First();
                oldlist.Remove(removeit);
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
            //TODO: Create Action references, when interfaces were created.
            EResourceType? resource = null;
            if (isdefending)
            {
                mRequestedUnitsDefense.Enqueue(new Task(JobType.Construction, platform, resource, action));
            }
            else
            {
                mRequestedUnitsProduce.Enqueue(new Task(JobType.Logistics, platform, resource, action));
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
        public Task RequestNewTask(JobType job, Optional<IPlatformAction> assignedAction)
        {
            switch(job)
            //TODO: Implement other Job cases.
            {
                case JobType.Idle:
                    //TODO:
                    //Find a more efficient way to determine the random platform to go to. It has to be Platformreferences (Julian requested it)
                    //but the List of Platforms currently used is a linkedlist which is not very efficient with ELementAt();
                    //Maybe use the Graph somehow in the future
                    var plist = mStructure.GetPlatformList();
                    var rndnmbr = mRandom.Next(1, plist.Count - 1);
                    //Just give them the inside of the Optional action witchout checking because
                    //it doesnt matter anyway if its null if the unit is idle.
                    return new Task(job, plist.ElementAt(rndnmbr), null, assignedAction.Get());
            }
            //TODO: Make this disappear when the rest is implemented, since its only a placeholder
            return new Task(job, mStructure.GetPlatformList().ElementAt(0), null, assignedAction.Get());
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
